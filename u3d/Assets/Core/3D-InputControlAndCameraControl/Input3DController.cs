using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Input3DController : MonoBehaviour
{
	private const int MOVE_LAYER = 0;
	private const int SCREEN_WIDTH = 960;
	private const int SCREEN_HEIGHT = 640;
	public Camera mCurCamera;
	public CameraControl bCamera;
	public float iPhoneZoomDeltaValue = 10.0f;

	private bool bCameraEnable = true;
		
	private Vector3 downPosition = Vector3.zero;
	private Vector3 releasePosition = Vector3.zero;
	private Vector3 currentPosition = Vector3.zero;
	
	// private Vector3 originHitPoint;
	private Vector3 downPositionInWorldSpace;
    // private Vector3 downPositionInWorldSpaceInMiniGame;
	// private Vector3 upPositionInWorldSpace;
	// private Vector3 upPositionInWorldSpaceInMiniGame;

	// public bool hideUpPositionInWorldCheck = false;

	private float MOVE_DELTA = 20;
	
	private float deltaX, deltaZ;
	private bool touchMoveAway;

    private void setTouchMoveAway(bool value)
    {
        touchMoveAway = value;
        if (touchMoveAway) {
            wasDragged = true;
        }
    }

    // once touchMoveAway is set to true,
    // so is this, until cleared by a "released" action
    private bool wasDragged = false; 
		
	private bool isDown;
	private bool isPressed;
	private bool isReleased;
	private bool touch2Finger;
	private bool touch2FingerReleased;
	
#if UNITY_EDITOR || UNITY_PLAYER || UNITY_STANDALONE_WIN || UNITY_METRO
	private Vector3 downPosition_mouse = Vector3.zero;
	private Vector3 releasePosition_mouse = Vector3.zero;
	private Vector3 currentPosition_mouse = Vector3.zero;
	
	private float deltaX_mouse, deltaY_mouse, deltaZ_mouse;
	private bool touchMoveAway_mouse;
	private bool touchMoveAway2_mouse;
	
	private bool isDown_mouse;
	private bool isPressed_mouse;
	private bool isReleased_mouse;
	private bool touch2Finger_mouse;
	private bool touch2FingerReleased_mouse;
		
	private float touch2FingerDeltaX_mouse;
	private float touch2FingerDeltaY_mouse;
		
	private Vector3 downPosition2_mouse = Vector3.zero;
	private Vector3 currentPosition2_mouse = Vector3.zero;
#endif
	
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_METRO
	private Vector3 downPosition_touch = Vector3.zero;
	private Vector3 releasePosition_touch = Vector3.zero;
	private Vector3 currentPosition_touch = Vector3.zero;
	
	private float deltaX_touch, deltaY_touch, deltaZ_touch;
	private bool touchMoveAway_touch;
	private bool touchMoveAway2_touch;
	
	private bool isDown_touch;
	private bool isPressed_touch;
	private bool isReleased_touch;
	private bool touch2Finger_touch;
	private bool touch2FingerReleased_touch;
			
	private float touch2FingerDeltaX_touch;
	private float touch2FingerDeltaY_touch;
		
	private Vector3 downPosition2_touch = Vector3.zero;
	private Vector3 currentPosition2_touch = Vector3.zero;
	
	private float oldDistance_touch = 0;
	private float newDistance_touch = 0;
	
	private readonly float FINGER_2_JUDGE_TIME = 0.1f;
	private readonly float FINGER_2_JUDGE_ZOOM_ANGLE_CHANGE = 10; //means changed for 10 C will be judge as rotate
	private readonly float FINGER_2_JUDGE_ZOOM_DISTANCE_CHANGE = 0.1f; //means changed for 10 C will be judge as rotate
	private readonly float FINGER_2_ROTATE_SPEED_PER_SECOND = 90; //means changed for 10 C will be judge as rotate
	
	private float touch2StartTime;
	private bool touch2TypeDecided = false;//whether touch2 type decided
	private bool touch2Rotating = false;//whether touch2 rotate
	private bool touch2Zooming = false;//whether touch2 zoomd
	//private Vector3 touch2RotateMidPoint = Vector3.zero;


	private bool touch3TypeDecided = false;
	private bool touch3Rotating = false;
	private bool touch3Pitching = false;

	private int touch3Rotating_finger1 = 0;
	private int touch3Rotating_finger2 = 0;

	private float touch2FingerDeltaAngle;
	//private float touch3FingerDeltaAngle;
	
	private bool touchMoveAway3;
	private bool startTouch3Finger = false;
	private bool touch3Finger = false;
	private bool touch3FingerReleased;
	
	private float touch3FingerDeltaX;
	private float touch3FingerDeltaY;
	private Vector3 downPosition3 = Vector3.zero;
	private Vector3 currentPosition3 = Vector3.zero;
#endif

	// Lock the camera state on one gesture
	enum CAMERA_STATE
	{
		eIdle,
		eMoving,
		eZooming,
		eRotating,
		eAngle,
		ePressingBuilding,
	}
	private CAMERA_STATE mCameraState = CAMERA_STATE.eIdle;
	
	// Drag building move camera parameters
	public float dragBuildingBorderX = 0.2f;
	public float dragBuildingBorderY = 0.2f;
	public Vector2 dragBuildingSpeed = new Vector2(12, 12);
	private int DragCameraX = 0;
	private int DragCameraY = 0;

	// Time
	private float mDeltaTime = 0, mTimeAtLastFrame = 0;
	
	// Use this for initialization
	void Awake () 
	{
		MOVE_DELTA = SCREEN_WIDTH * 0.02f;
	}

	void OnDestroy()
	{
		//
	}
	
	// Update is called once per frame
	// all control in building part
	// include:
	//            camera control
	//            building control
	//            farmer control
	//            if input is in UI state, prevent all controls from above

	void Update () 
	{
		float _currTime = Time.realtimeSinceStartup;
		mDeltaTime = _currTime - mTimeAtLastFrame;
		mTimeAtLastFrame = _currTime;
		
		deltaX = deltaZ = 0;
		isDown = isPressed = isReleased = touch2FingerReleased = false;
		
#if UNITY_EDITOR || UNITY_PLAYER || UNITY_STANDALONE_WIN || UNITY_METRO
		deltaX_mouse = deltaY_mouse = deltaZ_mouse = 0;
		touch2FingerDeltaX_mouse = touch2FingerDeltaY_mouse = 0;
		isDown_mouse = isPressed_mouse = isReleased_mouse = touch2FingerReleased_mouse = false;
#endif		
				
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_METRO	
		deltaX_touch = deltaY_touch = deltaZ_touch = 0;
		touch2FingerDeltaX_touch = touch2FingerDeltaY_touch = 0;
		isDown_touch = isPressed_touch = isReleased_touch = touch2FingerReleased_touch = false;
		
		touch3FingerDeltaX = 0;
		touch3FingerDeltaY = 0;
		touch2FingerDeltaAngle = 0;
#endif

#if UNITY_EDITOR || UNITY_PLAYER || UNITY_STANDALONE_WIN || UNITY_METRO
		// zoom in/out
		deltaY_mouse = Input.GetAxis("Mouse ScrollWheel");

		if (Input.GetMouseButtonDown(0))
		{
			currentPosition_mouse = Input.mousePosition;
			downPosition_mouse = Input.mousePosition;
			touchMoveAway_mouse = false;
            this.setTouchMoveAway(false);
			isDown_mouse = true; 
			//PandaUI.Instance.allowUserInput = false;
		}
		else if (Input.GetMouseButton(0))
		{
			Vector3 deltaMouse = Input.mousePosition - currentPosition_mouse;
			deltaX_mouse = -deltaMouse.x;
			deltaZ_mouse = -deltaMouse.y;
			currentPosition_mouse = Input.mousePosition;
			
			if (Mathf.Abs(downPosition_mouse.x - currentPosition_mouse.x) > MOVE_DELTA || 
			    Mathf.Abs(downPosition_mouse.y - currentPosition_mouse.y) > MOVE_DELTA )
			{
				touchMoveAway_mouse = true;
				this.setTouchMoveAway(true);
			}
			isPressed_mouse = true;
		}
		else if (Input.GetMouseButtonUp(0))
		{ 
			isReleased_mouse = true;
			releasePosition_mouse = Input.mousePosition; 
		}
		else if (Input.GetMouseButtonDown(1))
		{ 
			currentPosition2_mouse = Input.mousePosition;
			downPosition2_mouse = Input.mousePosition;
			touchMoveAway2_mouse = false;
			touch2Finger_mouse = true;
			//PandaUI.Instance.allowUserInput = false;
		}
		else if (Input.GetMouseButton(1))
		{
			Vector3 deltaMouse = Input.mousePosition - currentPosition2_mouse;
			touch2FingerDeltaX_mouse = deltaMouse.x;
			touch2FingerDeltaY_mouse = deltaMouse.y;
			currentPosition2_mouse = Input.mousePosition;
			
			if (Mathf.Abs(downPosition2_mouse.x - currentPosition2_mouse.x) > MOVE_DELTA || 
			    Mathf.Abs(downPosition2_mouse.y - currentPosition2_mouse.y) > MOVE_DELTA )
			{
				touchMoveAway2_mouse = true;
			}
		}
		else if (Input.GetMouseButtonUp(1))
		{ 
			if (!touchMoveAway2_mouse)
			{
				touch2FingerReleased_mouse = true;
			}
			touch2Finger_mouse = false;
		}
		else
		{
			currentPosition_mouse = Vector3.zero;
			mCameraState = CAMERA_STATE.eIdle;
			touch2Finger_mouse = false;
			touchMoveAway_mouse = false;
			//PandaUI.Instance.allowUserInput = true;
		}
#endif
		
#if UNITY_EDITOR || UNITY_PLAYER || UNITY_STANDALONE_WIN
		deltaX = deltaX_mouse;
		deltaZ = deltaZ_mouse;
		
		isDown = isDown_mouse;
		isPressed = isPressed_mouse;
		isReleased = isReleased_mouse;
		touch2FingerReleased = touch2FingerReleased_mouse;
		
		downPosition = downPosition_mouse;
		currentPosition = currentPosition_mouse;
		releasePosition = releasePosition_mouse;
		touch2Finger = touch2Finger_mouse;
#endif

#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_METRO) && !UNITY_EDITOR
		int fingerCount = Input.touchCount;
		// camera move
		if (fingerCount == 1)
		{
			oldDistance_touch = 0;
			startTouch3Finger = false;
			
			if (!touch2Finger_touch && !touch3Finger)
			{
				isPressed_touch = true;
				if (Input.GetTouch(0).phase == TouchPhase.Began)
				{
					touchMoveAway_touch = false;
					this.setTouchMoveAway(false);
					downPosition_touch = Input.GetTouch(0).position;
					currentPosition_touch = downPosition_touch;
					isDown_touch = true;
					isPressed_touch = false;
					if(ReposHandlerX && ReposHandlerX.ReposNow){ReposHandlerX.Btn_ReposBuilding_release();}
					//PandaUI.Instance.allowUserInput = false;
				}
				
				if (Input.GetTouch(0).phase == TouchPhase.Moved)
				{
					deltaX_touch = -Input.GetTouch(0).position.x + currentPosition_touch.x;
					deltaZ_touch = -Input.GetTouch(0).position.y + currentPosition_touch.y;
					currentPosition_touch = Input.GetTouch(0).position;
					
					if (Mathf.Abs(downPosition_touch.x - currentPosition_touch.x) > MOVE_DELTA || 
					    Mathf.Abs(downPosition_touch.y - currentPosition_touch.y) > MOVE_DELTA )
					{
						touchMoveAway_touch = true;
						this.setTouchMoveAway(true);
					}
									
					//print("touch move "+deltaX_touch +" "+deltaZ_touch);
				}
				
				if (Input.GetTouch(0).phase == TouchPhase.Ended)
				{
					isReleased_touch = true;
					isPressed_touch = false;
					releasePosition_touch = Input.GetTouch(0).position;
					if(ReposHandlerX && ReposHandlerX.ReposNow){ReposHandlerX.Btn_ReposBuilding_release();}
				}
			}
		}
		// zoom in/out
		else if (fingerCount == 2)
		{
			touch2Finger_touch = true;
			
			if (touch3Finger)
			{
				touch2Finger_touch = false;
			}
			//print("2 touch move ");
			
			if (oldDistance_touch == 0)
			{
				touch2StartTime = _currTime;
				oldDistance_touch = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
				currentPosition_touch = Input.GetTouch(0).position;
				currentPosition2_touch = Input.GetTouch(1).position;
				downPosition_touch = currentPosition_touch;
				downPosition2_touch = currentPosition2_touch;
				touchMoveAway2_touch = false;
			}
			else
			{
				if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
				{					
					if (_currTime - touch2StartTime > FINGER_2_JUDGE_TIME)
					{
						if (!touchMoveAway2_touch)
						{
							if ((Mathf.Abs(downPosition_touch.x - Input.GetTouch(0).position.x) > MOVE_DELTA || Mathf.Abs(downPosition_touch.y - Input.GetTouch(0).position.y) > MOVE_DELTA)
								|| (Mathf.Abs(downPosition2_touch.x - Input.GetTouch(1).position.x) > MOVE_DELTA || Mathf.Abs(downPosition2_touch.y - Input.GetTouch(1).position.y) > MOVE_DELTA))
							{
								touchMoveAway2_touch = true;
							}
						}
						
						if (touchMoveAway2_touch)
						{
							if (!touch2TypeDecided)
							{
								//judge is room or rotate
								touch2Zooming = false;
								touch2Rotating = false;	
																			
						
								Vector3 newPos1 = Input.GetTouch(0).position;
								Vector3 newPos2 = Input.GetTouch(1).position;								
								
								float angleChange_all = Vector3.Angle(newPos2 - newPos1 , downPosition2_touch - downPosition_touch);
								float distanceChange = Vector3.Distance(newPos2 , newPos1) - Vector3.Distance(downPosition2_touch, downPosition_touch);
								float distanceChangePercent = Mathf.Abs(distanceChange) / Mathf.Abs(Vector3.Distance(downPosition2_touch, downPosition_touch));
								// DebugUtils.LogTouch("angleChange_all "+angleChange_all);
								
								if (distanceChangePercent >= FINGER_2_JUDGE_ZOOM_DISTANCE_CHANGE || angleChange_all <= FINGER_2_JUDGE_ZOOM_ANGLE_CHANGE) //if is zoom
								{			
									float angleChange_cross = Vector3.Angle(newPos2 - downPosition2_touch , newPos1 - downPosition_touch);	
									if (angleChange_cross > 90) // if move different direction
									{
										if (distanceChangePercent >= FINGER_2_JUDGE_ZOOM_DISTANCE_CHANGE)
										{
											touch2Zooming = true; 
											touch2Rotating = false;	
											touch2TypeDecided = true;	
											// DebugUtils.LogTouch("decided touch2Zooming");
										}
										else if (angleChange_all > FINGER_2_JUDGE_ZOOM_ANGLE_CHANGE)
										{
											touch2Rotating = true; 
											touch2Zooming = false;	
											touch2TypeDecided = true;	
											// DebugUtils.LogTouch("decided touch2Rotate 0");
										}
										else
										{
											//waiting
										}
									}
									else
									{
										touch2Rotating = true;
										touch2Zooming = false;	
										touch2TypeDecided = true;	 
										// DebugUtils.LogTouch("decided touch2Rotate 1");
									}	
									
									if (touch2TypeDecided)
									{
										mCameraState = CAMERA_STATE.eIdle;
										currentPosition_touch = Input.GetTouch(0).position;
										currentPosition2_touch = Input.GetTouch(1).position;	
									}
								}
								
								if (angleChange_all > FINGER_2_JUDGE_ZOOM_ANGLE_CHANGE)
								{
									touch2Rotating = true;
									touch2TypeDecided = true;	
									// DebugUtils.LogTouch("decided touch2Rotate 3");
									currentPosition_touch = Input.GetTouch(0).position;
									currentPosition2_touch = Input.GetTouch(1).position;	
								}
								
							}
							else
							{								
								if (touch2Zooming || mCameraState == CAMERA_STATE.eZooming)
								{
									mCameraState = CAMERA_STATE.eZooming;
									newDistance_touch = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
									deltaY_touch = newDistance_touch - oldDistance_touch;
									// DebugUtils.LogTouch("zooming deltaY "+deltaY_touch);
								}
								
								touch2FingerDeltaAngle = 0f;
								if (touch2Rotating)
								{
									mCameraState = CAMERA_STATE.eRotating;
									
									bool sameSide = false;									
									Vector3 newPos1 = Input.GetTouch(0).position;
									Vector3 newPos2 = Input.GetTouch(1).position;									
									
									float angleChange_cross = Vector3.Angle(newPos2 - currentPosition2_touch , newPos1 - currentPosition_touch);	
									if (angleChange_cross < 90) // if move together at same side
									{
										sameSide = true;
										
										//caculate angels by screen center point
										Vector3 midlePoint = new Vector3(ScreenWrapper.width/2, ScreenWrapper.height/2, currentPosition_touch.z);
										
										float angleChange_1 = SignAngle(currentPosition_touch - midlePoint, newPos1 - midlePoint );								
										float angleChange_2 = SignAngle(currentPosition2_touch - midlePoint, newPos2 - midlePoint );
										
										touch2FingerDeltaAngle = (angleChange_1 + angleChange_2) / 2;
										// DebugUtils.LogTouch("1 same touch2FingerDeltaAngle "+touch2FingerDeltaAngle);
									}						
									
									//judge by center point of two start point
									if (!sameSide)
									{
										Vector3 midlePoint = new Vector3((currentPosition_touch.x + currentPosition2_touch.x)/2,
											(currentPosition_touch.y + currentPosition2_touch.y)/2,
											(currentPosition_touch.z + currentPosition2_touch.z)/2);
										
										float angleChange_1 = SignAngle(currentPosition_touch - midlePoint,  newPos1 - midlePoint);								
										float angleChange_2 = SignAngle(currentPosition2_touch - midlePoint,  newPos2 - midlePoint);
										
										if ( angleChange_1 * angleChange_2 > 0 )
										{
											sameSide = true;
											
											touch2FingerDeltaAngle = SignAngle(currentPosition2_touch - currentPosition_touch, newPos2 - newPos1);
										//	int direction = touch2FingerDeltaAngle >= 0 ? 1: -1;
										//	touch2FingerDeltaAngle = FINGER_2_ROTATE_SPEED_PER_SECOND * direction * mDeltaTime;
											
											// DebugUtils.LogTouch("2 same touch2FingerDeltaAngle "+touch2FingerDeltaAngle);
										}
										
										
										if((angleChange_1 == 0 && angleChange_2 != 0) || (angleChange_2 == 0 && angleChange_1 != 0)) //if one finger move, one stop
										{
											sameSide = true;
											if (angleChange_1 == 0)
												touch2FingerDeltaAngle = SignAngle(currentPosition2_touch - currentPosition_touch, newPos2 - currentPosition_touch);	
											if (angleChange_2 == 0)
												touch2FingerDeltaAngle = SignAngle(currentPosition_touch - currentPosition2_touch, newPos1 - currentPosition2_touch);	
											
										//	int direction = touch2FingerDeltaAngle >= 0 ? 1: -1;
										//	touch2FingerDeltaAngle = FINGER_2_ROTATE_SPEED_PER_SECOND * direction * mDeltaTime;
											
											// DebugUtils.LogTouch("3 one touch one stop touch2FingerDeltaAngle "+touch2FingerDeltaAngle);	
										}
										
										if (angleChange_1 == 0 && angleChange_2 == 0)
										{
											touch2FingerDeltaAngle = 0;
											// DebugUtils.LogTouch("2 finger all stop ");	
										}
									
									}									
									
									//still not same direction, useless
									
									if (!sameSide)										
									{										
										// DebugUtils.LogTouch("not same direction, abandon");
										touch2FingerDeltaAngle = 0;
									}
								}
								
								touch2FingerDeltaAngle = -touch2FingerDeltaAngle;								
								
								currentPosition_touch = Input.GetTouch(0).position;
								currentPosition2_touch = Input.GetTouch(1).position;
							}
						}
					}					
				}			
			}			
			//PandaUI.Instance.allowUserInput = false;
		}
		// rotation z
		else if (fingerCount == 3)
		{
			touch3Finger = true;
			
			if (touch2Finger_touch && !touchMoveAway2_touch)
			{
				touch3Finger = true;
				startTouch3Finger = false;
				touch2Finger_touch = false;
				touch2TypeDecided = false;
				mCameraState = CAMERA_STATE.eIdle;
			}
			//print("3 touch move ");
			
			if (!startTouch3Finger)
			{
				startTouch3Finger = true;
				currentPosition_touch = Input.GetTouch(0).position;
				currentPosition2_touch = Input.GetTouch(1).position;
				currentPosition3 = Input.GetTouch(2).position;
				downPosition_touch = currentPosition_touch;
				downPosition2_touch = currentPosition2_touch;
				downPosition3 = currentPosition3;
				touchMoveAway3 = false;
				touch3Pitching = false;
				touch3Rotating = false;
				touch3TypeDecided = false;
			}
			else
			{
				//if (!touch3TypeDecided)
				{
					Vector3 delta0 = Vector3.zero;
					Vector3 delta1 = Vector3.zero;
					Vector3 delta2 = Vector3.zero;
					if (Input.GetTouch(0).phase == TouchPhase.Moved)
					{
						Vector3 newPos = Input.GetTouch(0).position;
						delta0 = newPos - downPosition_touch;
						//Vector3 newPos2 = Input.GetTouch(1).position;								
						/*
						float angleChange_all = Vector3.Angle(newPos2 - newPos1 , downPosition2_touch - downPosition_touch);
						float distanceChange = Vector3.Distance(newPos2 , newPos1) - Vector3.Distance(downPosition2_touch, downPosition_touch);
						float distanceChangePercent = Mathf.Abs(distanceChange) / Mathf.Abs(Vector3.Distance(downPosition2_touch, downPosition_touch));
						DebugUtils.LogTouch("angleChange_all "+angleChange_all);
						*/

						//float angleChange_cross = Vector3.Angle(newPos2 - downPosition2_touch , newPos1 - downPosition_touch);	

					}
					if (Input.GetTouch(1).phase == TouchPhase.Moved)
					{
						Vector3 newPos = Input.GetTouch(1).position;
						delta1 = newPos - downPosition2_touch;
					}
					if (Input.GetTouch(2).phase == TouchPhase.Moved)
					{
						Vector3 newPos = Input.GetTouch(2).position;
						delta2 = newPos - downPosition3;
					}

					float angleChange_cross = 0.0f;
					float SMALL_NUMBER = MOVE_DELTA * MOVE_DELTA;
					if (delta0.sqrMagnitude > SMALL_NUMBER && delta1.sqrMagnitude > SMALL_NUMBER)
					{
						angleChange_cross = Vector3.Angle(delta0 , delta1);	
						touch3Rotating_finger1 = 0;
						touch3Rotating_finger2 = 1;
					}
					if (delta0.sqrMagnitude > SMALL_NUMBER && delta2.sqrMagnitude > SMALL_NUMBER)
					{
						float another_angleChange_cross = Vector3.Angle(delta0 , delta2);	
						if (another_angleChange_cross > angleChange_cross)
						{
							angleChange_cross = another_angleChange_cross;
							touch3Rotating_finger1 = 0;
							touch3Rotating_finger2 = 2;
						}
					}
					if (delta1.sqrMagnitude > SMALL_NUMBER && delta2.sqrMagnitude > SMALL_NUMBER)
					{
						float another_angleChange_cross = Vector3.Angle(delta1 , delta2);	
						if (another_angleChange_cross > angleChange_cross)
						{
							angleChange_cross = another_angleChange_cross;
							touch3Rotating_finger1 = 1;
							touch3Rotating_finger2 = 2;
						}
					}
					if (angleChange_cross > 90)
					{
						touch3Pitching = false;
						touch3Rotating = true;
						touch3TypeDecided = true;
					}

					if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved || Input.GetTouch(2).phase == TouchPhase.Moved)
					{
						if (!touchMoveAway3)
						{
							if ((Mathf.Abs(downPosition_touch.x - Input.GetTouch(0).position.x) > MOVE_DELTA || Mathf.Abs(downPosition_touch.y - Input.GetTouch(0).position.y) > MOVE_DELTA)
								|| (Mathf.Abs(downPosition2_touch.x - Input.GetTouch(1).position.x) > MOVE_DELTA || Mathf.Abs(downPosition2_touch.y - Input.GetTouch(1).position.y) > MOVE_DELTA)							
								|| (Mathf.Abs(downPosition3.x - Input.GetTouch(2).position.x) > MOVE_DELTA || Mathf.Abs(downPosition3.y - Input.GetTouch(2).position.y) > MOVE_DELTA))
							{
								touchMoveAway3 = true;
							}
						}
						
						if (touchMoveAway3)
						{						
							if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved && Input.GetTouch(2).phase == TouchPhase.Moved)
							{
								if ((   
                                        -Input.GetTouch(0).position.y + currentPosition_touch.y > 0 || 
                                        -Input.GetTouch(1).position.y + currentPosition2_touch.y > 0 || 
                                        -Input.GetTouch(2).position.y + currentPosition3.y > 0 )
									||  (
                                        -Input.GetTouch(0).position.y + currentPosition_touch.y < 0 || 
                                        -Input.GetTouch(1).position.y + currentPosition2_touch.y < 0 || 
                                        -Input.GetTouch(2).position.y + currentPosition3.y < 0 ))
								{
									touch3Pitching = true;
									touch3Rotating = false;
									touch3TypeDecided = true;
									touch3FingerDeltaY = Input.GetTouch(0).position.y - currentPosition_touch.y;
								}

								if ((
                                        -Input.GetTouch(0).position.x + currentPosition_touch.x > 0 || 
                                        -Input.GetTouch(1).position.x + currentPosition2_touch.x > 0 || 
                                        -Input.GetTouch(2).position.x + currentPosition3.x > 0)
								    || (
                                        -Input.GetTouch(0).position.x + currentPosition_touch.x < 0 || 
                                        -Input.GetTouch(1).position.x + currentPosition2_touch.x < 0 || 
                                        -Input.GetTouch(2).position.x + currentPosition3.x < 0))
								{

									touch3FingerDeltaX = Input.GetTouch(0).position.x - currentPosition_touch.x;
									if (Input.GetTouch(0).position.y > ScreenWrapper.height /2 )
									{
										touch3FingerDeltaX *= (-1);
									}
								}
							}
						}
					}
				}
#region NotUsed
				/*
				else
				{
					if (touch3Pitching)
					{
						touch3FingerDeltaY = Input.GetTouch(0).position.y - currentPosition_touch.y;
					}
					else if (touch3Rotating)
					{
						touch2FingerDeltaAngle = 0f;
						
						Vector3 newPos1 = Input.GetTouch(touch3Rotating_finger1).position;
						Vector3 newPos2 = Input.GetTouch(touch3Rotating_finger2).position;	
						
						Vector3 _currentPosition2_touch = new Vector3();
						Vector3 _currentPosition_touch = new Vector3();

						if (touch3Rotating_finger1 == 0)
						{
							_currentPosition_touch = currentPosition_touch;
						}
						else if (touch3Rotating_finger1 == 1)
						{
							_currentPosition_touch = currentPosition2_touch;
						}
						else if (touch3Rotating_finger1 == 2)
						{
							_currentPosition_touch = currentPosition3;
						}

						if (touch3Rotating_finger2 == 0)
						{
							_currentPosition2_touch = currentPosition_touch;
						}
						else if (touch3Rotating_finger2 == 1)
						{
							_currentPosition2_touch = currentPosition2_touch;
						}
						else if (touch3Rotating_finger2 == 2)
						{
							_currentPosition2_touch = currentPosition3;
						}

								
								{
									mCameraState = CAMERA_STATE.eRotating;
									
									bool sameSide = false;									
																	
									
									float angleChange_cross = Vector3.Angle(newPos2 - _currentPosition2_touch , newPos1 - _currentPosition_touch);	
									if (angleChange_cross < 90) // if move together at same side
									{
										sameSide = true;
										
										//caculate angels by screen center point
										Vector3 midlePoint = new Vector3(ScreenWrapper.width/2, ScreenWrapper.height/2, _currentPosition_touch.z);
										
										float angleChange_1 = SignAngle(_currentPosition_touch - midlePoint, newPos1 - midlePoint );								
										float angleChange_2 = SignAngle(_currentPosition2_touch - midlePoint, newPos2 - midlePoint );
										
										touch2FingerDeltaAngle = (angleChange_1 + angleChange_2) / 2;
										DebugUtils.LogTouch("1 same touch2FingerDeltaAngle "+touch2FingerDeltaAngle);
									}						
									
									//judge by center point of two start point
									if (!sameSide)
									{
										Vector3 midlePoint = new Vector3((_currentPosition_touch.x + _currentPosition2_touch.x)/2,
											(_currentPosition_touch.y + _currentPosition2_touch.y)/2,
											(_currentPosition_touch.z + _currentPosition2_touch.z)/2);
										
										float angleChange_1 = SignAngle(_currentPosition_touch - midlePoint,  newPos1 - midlePoint);								
										float angleChange_2 = SignAngle(_currentPosition2_touch - midlePoint,  newPos2 - midlePoint);
										
										if ( angleChange_1 * angleChange_2 > 0 )
										{
											sameSide = true;
											
											touch2FingerDeltaAngle = SignAngle(_currentPosition2_touch - _currentPosition_touch, newPos2 - newPos1);
										//	int direction = touch2FingerDeltaAngle >= 0 ? 1: -1;
										//	touch2FingerDeltaAngle = FINGER_2_ROTATE_SPEED_PER_SECOND * direction * mDeltaTime;
											
											DebugUtils.LogTouch("2 same touch2FingerDeltaAngle "+touch2FingerDeltaAngle);
										}
										
										
										if((angleChange_1 == 0 && angleChange_2 != 0) || (angleChange_2 == 0 && angleChange_1 != 0)) //if one finger move, one stop
										{
											sameSide = true;
											if (angleChange_1 == 0)
												touch2FingerDeltaAngle = SignAngle(_currentPosition2_touch - _currentPosition_touch, newPos2 - _currentPosition_touch);	
											if (angleChange_2 == 0)
												touch2FingerDeltaAngle = SignAngle(_currentPosition_touch - _currentPosition2_touch, newPos1 - _currentPosition2_touch);	
											
										//	int direction = touch2FingerDeltaAngle >= 0 ? 1: -1;
										//	touch2FingerDeltaAngle = FINGER_2_ROTATE_SPEED_PER_SECOND * direction * mDeltaTime;
											
											DebugUtils.LogTouch("3 one touch one stop touch2FingerDeltaAngle "+touch2FingerDeltaAngle);	
										}
										
										if (angleChange_1 == 0 && angleChange_2 == 0)
										{
											touch2FingerDeltaAngle = 0;
											DebugUtils.LogTouch("2 finger all stop ");	
										}
									
									}									
									
									//still not same direction, useless
									
									if (!sameSide)										
									{										
										DebugUtils.LogTouch("not same direction, abandon");
										touch2FingerDeltaAngle = 0;
									}
								} 
					}
				}
				touch2FingerDeltaAngle = -touch2FingerDeltaAngle;		
*/
#endregion
				currentPosition_touch = Input.GetTouch(0).position;
				currentPosition2_touch = Input.GetTouch(1).position;
				currentPosition3 = Input.GetTouch(2).position;
			}
			
			//PandaUI.Instance.allowUserInput = false;
		}
		else
		{
			//print("no touch move ");
			if (touch2Finger_touch)
			{
				if (!touchMoveAway2_touch)
				{
					touch2FingerReleased_touch = true;
				}
			}
			
			if (touch3Finger)
			{
				if (!touchMoveAway3)
				{
					touch3FingerReleased = true;
				}
			}
			
			startTouch3Finger = false;
			touch2Finger_touch = false;
			touch3Finger = false;
			this.setTouchMoveAway(false);
			touchMoveAway_touch = false;
			touchMoveAway2_touch = false;
			touchMoveAway3 = false;
			touch2TypeDecided = false;
			touch2Rotating = false;
			touch2Zooming = false;
			oldDistance_touch = 0;
			currentPosition_touch = Vector3.zero;

			touch3TypeDecided = false;
			touch3Rotating = false;
			touch3Pitching = false;

			mCameraState = CAMERA_STATE.eIdle;
			//PandaUI.Instance.allowUserInput = true;
		}
#endif
		
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8) && !UNITY_EDITOR 
		deltaX = deltaX_touch;
		deltaZ = deltaZ_touch;
		
		isDown = isDown_touch;
		isPressed = isPressed_touch;
		isReleased = isReleased_touch;
		touch2FingerReleased = touch2FingerReleased_touch;
		
		downPosition = downPosition_touch;
		currentPosition = currentPosition_touch;
		releasePosition = releasePosition_touch;
		touch2Finger = touch2Finger_touch;

		this.setTouchMoveAway(touchMoveAway_touch);
#endif

#if UNITY_METRO
        // Determine isDown from touch OR mouse
        isDown = isDown_touch || isDown_mouse;
		if (isDown_touch)
			downPosition = downPosition_touch;
		else if (isDown_mouse)
			downPosition = downPosition_mouse;

        // Determine touchMoveAway from touch OR mouse
		this.setTouchMoveAway(touchMoveAway_touch || touchMoveAway_mouse);
		if (touchMoveAway_touch) {
			deltaX = deltaX_touch;
			deltaZ = deltaZ_touch;
		}
		else if (touchMoveAway_mouse) {
			deltaX = deltaX_mouse;
			deltaZ = deltaZ_mouse;
		}

        currentPosition = currentPosition_mouse;
	#if !UNITY_EDITOR
        // Override position with touch position if there were touches.
        if (fingerCount > 0)
        {
            currentPosition = currentPosition_touch;
        }
	#endif

        // Determine isReleased from touch OR mouse
        isReleased = isReleased_touch || isReleased_mouse;
		if (isReleased_touch)
			releasePosition = releasePosition_touch;
		else if (isReleased_mouse)
			releasePosition = releasePosition_mouse;

        // Determine isPressed from touch OR mouse
		isPressed = isPressed_mouse || isPressed_touch;

		touch2Finger = touch2Finger_mouse || touch2Finger_touch;
		touch2FingerReleased = touch2FingerReleased_mouse || touch2FingerReleased_touch;
#endif
		if(isDown)
		{
			RaycastHit hit;
			Ray ray = mCurCamera.ScreenPointToRay (currentPosition);
            if (Physics.Raycast (ray, out hit, Mathf.Infinity, 1 << MOVE_LAYER))
            {
                downPositionInWorldSpace = hit.point;
            }
		}
		{
			ControlCamera();
		}

        // unset wasDragged if we have finished processing isReleased during this update.
        if (isReleased && wasDragged)
        {
            wasDragged = false;
        }

		//Debug.LogError("ccccccccccccccccccccccccccccccc");
	}

	public bool IsDown()
	{
		return isDown;
	}

	public bool IsPressed()
	{
		return isPressed;
	}

	public bool IsReleased()
	{
		return isReleased;
	}

	public float GetDeltaX()
	{
		return deltaX;
	}

	public float GetDeltaZ()
	{
		return deltaZ;
	}

	public Vector3 GetDownPosition()
	{
		return downPosition;
	}

	public Vector3 GetCurrentPosition()
	{
		return currentPosition;
	}

	public Vector3 GetReleasePosition()
	{
		return releasePosition;
	}

	public Vector3 GetReleasePositionInWorld()
	{
		return downPositionInWorldSpace;
	}

	// public Vector3 GetReleaseUpPositionInWorld()
	// {
	// 	return upPositionInWorldSpace;
	// }

	public void ResetCameraParams()
	{
		DragCameraX = 0;
		DragCameraY = 0;
	
		deltaX = deltaZ = 0;
		isDown = isPressed = isReleased = touch2FingerReleased = false;
		
#if UNITY_EDITOR || UNITY_PLAYER || UNITY_STANDALONE_WIN || UNITY_METRO
		deltaX_mouse = deltaY_mouse = deltaZ_mouse = 0;
		touch2FingerDeltaX_mouse = touch2FingerDeltaY_mouse = 0;
		isDown_mouse = isPressed_mouse = isReleased_mouse = touch2FingerReleased_mouse = false;
#endif		
				
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_METRO	
		deltaX_touch = deltaY_touch = deltaZ_touch = 0;
		touch2FingerDeltaX_touch = touch2FingerDeltaY_touch = 0;
		isDown_touch = isPressed_touch = isReleased_touch = touch2FingerReleased_touch = false;
		
		touch3FingerDeltaX = 0;
		touch3FingerDeltaY = 0;
		touch3FingerReleased = false;
#endif
		mCameraState = CAMERA_STATE.eIdle;
	}
	
	void ControlCamera()
	{
		if (enableCamera && bCamera)
		{
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_METRO
            // Touch3Finger should have highest priority in this if/else rundown. 
            // This fixes issues with unresponsive 3 finger touching. -HH
            if (touch3Finger)
			{

				if (mCameraState == CAMERA_STATE.eIdle || mCameraState == CAMERA_STATE.eMoving)
				{
					if (touch3FingerDeltaX != 0 || touch3FingerDeltaY != 0)
						mCameraState = CAMERA_STATE.eAngle;
				}

				if (mCameraState == CAMERA_STATE.eAngle)
				{
					bCamera.Rotate(0, (touch3FingerDeltaY / ScreenWrapper.height),50);
					bCamera.Rotate(touch3FingerDeltaX * 0.3f, false);
				}
				else if (mCameraState == CAMERA_STATE.eRotating)
				{
					// DebugUtils.LogTouch("rotate "+touch2FingerDeltaAngle);
					bCamera.Rotate(touch2FingerDeltaAngle, false);
					//bCamera.Rotate(touch2FingerDeltaX_touch / ScreenWrapper.width, 0);
				}
			}
            else
#endif

			// Check for Camera MOVING.
			if ((deltaX != 0 || deltaZ != 0) && (mCameraState == CAMERA_STATE.eIdle || mCameraState == CAMERA_STATE.eMoving || mCameraState == CAMERA_STATE.ePressingBuilding))
			{
				//Debug.LogError("mCameraState  " + mCameraState);
				if (mCameraState == CAMERA_STATE.eIdle)
					mCameraState = CAMERA_STATE.eMoving;
				// if camera move, the touch position should always point to "downPositionInWorldSpace" 
				RaycastHit hit;
                Vector3 currentPointInWorldSpace = downPositionInWorldSpace;

                Ray ray = mCurCamera.ScreenPointToRay (currentPosition);
                if (Physics.Raycast (ray, out hit, Mathf.Infinity, 1 << MOVE_LAYER))
                {
                    currentPointInWorldSpace = hit.point;
                }
                
                bCamera.Move(downPositionInWorldSpace.x - currentPointInWorldSpace.x, downPositionInWorldSpace.z - currentPointInWorldSpace.z);
                
                // bCamera.Move((downPosition.x - currentPosition.x)*Time.deltaTime, (downPosition.y - currentPosition.y)*Time.deltaTime);
			}

#if UNITY_EDITOR || UNITY_PLAYER || UNITY_STANDALONE_WIN || UNITY_METRO
			else if (deltaY_mouse != 0 && (mCameraState == CAMERA_STATE.eIdle || mCameraState == CAMERA_STATE.eZooming))
			{
				mCameraState = CAMERA_STATE.eZooming;
				float dy = -Mathf.Sign(deltaY_mouse);
				bCamera.Zoom(dy);
			}
			
			else if (touch2FingerDeltaX_mouse != 0 || touch2FingerDeltaY_mouse != 0)
			{
				// see which rotation
				if (mCameraState == CAMERA_STATE.eIdle)
				{
					if (Mathf.Abs(touch2FingerDeltaX_mouse) >= Mathf.Abs(touch2FingerDeltaY_mouse))
					{
						mCameraState = CAMERA_STATE.eRotating;
					}
					else
					{
						mCameraState = CAMERA_STATE.eAngle;
					}
				}
				
				if (touch2FingerDeltaX_mouse != 0 && mCameraState == CAMERA_STATE.eRotating)
				{
					bCamera.Rotate(touch2FingerDeltaX_mouse / SCREEN_WIDTH, 0);
				}
				else if (touch2FingerDeltaY_mouse != 0 && mCameraState == CAMERA_STATE.eAngle)
				{
					bCamera.Rotate(0, touch2FingerDeltaY_mouse/ SCREEN_HEIGHT, 50);
				}
			}
#endif

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_METRO
			else if (touch2Finger_touch)
			{
				if (mCameraState == CAMERA_STATE.eIdle || mCameraState == CAMERA_STATE.eMoving)
				{
					if (touch2Zooming)
					{
						mCameraState = CAMERA_STATE.eZooming;
					}
					else if (touch2Rotating)
					{
						mCameraState = CAMERA_STATE.eRotating;
					}

					/*
					else if (touch2FingerDeltaX_touch != 0 || touch2FingerDeltaY_touch != 0)
					{
						// see which rotation
						if (Mathf.Abs(touch2FingerDeltaX_touch) >= Mathf.Abs(touch2FingerDeltaY_touch))
						{
							mCameraState = CAMERA_STATE.eRotating;
						}
						else
						{
							//mCameraState = CAMERA_STATE.eAngle;
						}
					}*/
				}
				
				if (mCameraState == CAMERA_STATE.eZooming)
				{
					if (Mathf.Abs(deltaY_touch) > iPhoneZoomDeltaValue)
					{
						float dy = -Mathf.Sign(deltaY_touch);
						oldDistance_touch = newDistance_touch;
						bCamera.Zoom(dy);
					}
					else
					{
						bCamera.Zoom(0);
					}
				}
				else if (mCameraState == CAMERA_STATE.eRotating)
				{
					// DebugUtils.LogTouch("rotate "+touch2FingerDeltaAngle);
					bCamera.Rotate(touch2FingerDeltaAngle, false);
					//bCamera.Rotate(touch2FingerDeltaX_touch / ScreenWrapper.width, 0);
				}
				//else if (mCameraState == CAMERA_STATE.eAngle)
				//{
				//	bCamera.Rotate(0, touch2FingerDeltaY_touch/ ScreenWrapper.height);
				//}
				
			}	
#endif		
			// rotate to default rotation
			else if (touch2FingerReleased && mCameraState == CAMERA_STATE.eIdle )
			{
				bCamera.RotateBack();
			}
			// no input and move
			else if (!isPressed && !isDown && !touch2Finger)
			{
				//DebugUtils.Log("SmoothStop ");
				bCamera.SmoothStop();
				
				//hasInput = false;
			}
		}
	}

	public void OnApplicationPause(bool pause)
	{ 
		if(pause)
		{
			//
		}
	}

	public bool enableCamera
	{
		set 
		{
			bCameraEnable = value;
		}
		get
		{
			return bCameraEnable;
		}
	}
		

    public void DragCamera()
    {
        DragCamera(currentPosition, deltaX, deltaZ);
    }

	public void DragCamera(Vector2 screenPosition, float _deltaX, float _deltaZ)
	{
        if (screenPosition.x < SCREEN_WIDTH * dragBuildingBorderX)
		{
            if (_deltaX > 1)
				DragCameraX = -1;
            else if (_deltaX < -1)
				DragCameraX = 0;
		}
        else if (screenPosition.x > SCREEN_WIDTH * (1 - dragBuildingBorderX))
		{
            if (_deltaX > 1)
				DragCameraX = 0;
            else if (_deltaX < -1)
				DragCameraX = 1;
		}
		else
		{
			DragCameraX = 0;
		}

        if (screenPosition.y < SCREEN_HEIGHT * dragBuildingBorderY)
		{
            if (_deltaZ > 1)
				DragCameraY = -1;
            else if (_deltaZ < -1)
				DragCameraY = 0;
		}
        else if (screenPosition.y > SCREEN_HEIGHT * (1 - dragBuildingBorderY))
		{
            if (_deltaZ > 1)
				DragCameraY = 0;
            else if (_deltaZ < -1)
				DragCameraY = 1;
		}
		else
		{
			DragCameraY = 0;
		}

        //Debug.Log("DragCamera " + screenPosition.ToString() + "DragCameraXY=("+DragCameraX+","+DragCameraY+")");
		if (DragCameraX != 0 || DragCameraY != 0)
		{
			float camMoveX = DragCameraX * dragBuildingSpeed.x * GetRealDeltaTime();
			float camMoveY = DragCameraY * dragBuildingSpeed.y * GetRealDeltaTime();
			bCamera.MoveByCameraDirection(camMoveX, camMoveY);
		}
		else
		{
			bCamera.Stop();
		}
	}


	void ControlGame()
	{
		if (isDown)
		{
			//
		}
		else if (isPressed)
		{
			//
		}
		// when release touch, change the UI
		else if (isReleased || touch2Finger)
		{
			//
		}
		else if (isReleased && !touchMoveAway)
		{
			//
		}
	}

	float SignAngle (Vector3 start, Vector3 end)
	{	
		float num1 = (end.x * start.y)-(start.x * end.y);		
		float num2 = (start.x * end.x) + (start.y * end.y);		
		return (Mathf.Atan2 (num1, num2) * Mathf.Rad2Deg);	
	}
	
	public float GetRealDeltaTime()
	{
		return mDeltaTime;
	}
}
