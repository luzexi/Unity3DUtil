using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Input3DController : CSingletonMono<Input3DController>
{
	// private const int MOVE_LAYER = 9;
	private const int SCREEN_WIDTH = 960;
	private const int SCREEN_HEIGHT = 640;
	public Camera mCurCamera;
	// public CameraControl bCamera;
	public CameraControl bCamera;
	public float iPhoneZoomDeltaValue = 10.0f;

	private bool bCameraEnable = true;
	private bool bFocusOnUI = false;
	public bool ForcsOnUI
	{
		get {return bFocusOnUI;}
		set {bFocusOnUI = value;}
	}
		
	private Vector3 downPosition = Vector3.zero;
	private Vector3 releasePosition = Vector3.zero;
	private Vector3 currentPosition = Vector3.zero;
	
	// private Vector3 originHitPoint;
	private Vector3 downPositionInWorldSpace;
	private Vector3 downPositionInScreen;
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
	private Vector3 downPosition3_mouse = Vector3.zero;
	private Vector3 currentPosition3_mouse = Vector3.zero;
#endif
	
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_METRO
	private Vector3 downPosition_touch = Vector3.zero;
	private Vector3 releasePosition_touch = Vector3.zero;
	private Vector3 currentPosition_touch = Vector3.zero;
	private Vector3 detailPosition_touch = Vector3.zero;
	private Vector3 prePosition_touch = Vector3.zero;
	
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
		
	private float touch2FingerSameDeltaY_touch;	
	private Vector3 downPosition2_touch = Vector3.zero;
	private Vector3 currentPosition2_touch = Vector3.zero;
	private Vector3 detailPosition2_touch = Vector3.zero;
	private Vector3 prePosition2_touch = Vector3.zero;
	
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
	private bool touch2Angleing = false;//whether touch2 angle
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


	private int mFrameNum = 0;
	
	// Use this for initialization
	public override void Awake ()
	{
		base.Awake();
		MOVE_DELTA = SCREEN_WIDTH * 0.02f;
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

		if(Input.GetKeyDown(KeyCode.PageUp))
		{
			deltaY_mouse = -0.5f;
		}
		if(Input.GetKeyDown(KeyCode.PageDown))
		{
			deltaY_mouse = 0.5f;
		}

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
			
			// if (Mathf.Abs(downPosition2_mouse.x - currentPosition2_mouse.x) > MOVE_DELTA || 
			//     Mathf.Abs(downPosition2_mouse.y - currentPosition2_mouse.y) > MOVE_DELTA )
			// {
			// 	touchMoveAway2_mouse = true;
			// }
		}
		else if (Input.GetMouseButtonUp(1))
		{ 
			if (!touchMoveAway2_mouse)
			{
				touch2FingerReleased_mouse = true;
			}
			touch2Finger_mouse = false;

        }
        else if (Input.GetMouseButtonDown(2))
		{ 
			currentPosition3_mouse = Input.mousePosition;
			downPosition3_mouse = Input.mousePosition;
			touchMoveAway2_mouse = false;
			touch2Finger_mouse = true;
		}
		else if (Input.GetMouseButton(2))
		{
			Vector3 deltaMouse = Input.mousePosition - currentPosition3_mouse;
			touch2FingerDeltaX_mouse = deltaMouse.x;
			touch2FingerDeltaY_mouse = deltaMouse.y;
			currentPosition3_mouse = Input.mousePosition;
		}
		else if (Input.GetMouseButtonUp(2))
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
					prePosition_touch = Vector3.zero;
					currentPosition_touch = downPosition_touch;
					detailPosition_touch = Vector3.zero;
					isDown_touch = true;
					isPressed_touch = false;
					// if(ReposHandlerX && ReposHandlerX.ReposNow){ReposHandlerX.Btn_ReposBuilding_release();}
					//PandaUI.Instance.allowUserInput = false;
				}
				
				if (Input.GetTouch(0).phase == TouchPhase.Moved)
				{
					deltaX_touch = -Input.GetTouch(0).position.x + currentPosition_touch.x;
					deltaZ_touch = -Input.GetTouch(0).position.y + currentPosition_touch.y;
					prePosition_touch = currentPosition_touch;
					currentPosition_touch = Input.GetTouch(0).position;
					detailPosition_touch = currentPosition_touch - prePosition_touch;
					
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
					// if(ReposHandlerX && ReposHandlerX.ReposNow){ReposHandlerX.Btn_ReposBuilding_release();}
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
				prePosition_touch = Vector3.zero;
				currentPosition_touch = Input.GetTouch(0).position;
				detailPosition_touch = Vector3.zero;
				currentPosition2_touch = Input.GetTouch(1).position;
				detailPosition2_touch = Vector3.zero;
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

						prePosition_touch = currentPosition_touch;
						currentPosition_touch = Input.GetTouch(0).position;
						detailPosition_touch = currentPosition_touch - prePosition_touch;
						prePosition2_touch = currentPosition2_touch;
						currentPosition2_touch = Input.GetTouch(1).position;
						detailPosition2_touch = currentPosition2_touch - prePosition2_touch;
						
						// if (touchMoveAway2_touch)
						// {
						// 	if (!touch2TypeDecided)
						// 	{
						// 		//judge is room or rotate
						// 		touch2Zooming = false;
						// 		touch2Rotating = false;	
						// 		touch2Angleing = false;											
						
						// 		Vector3 newPos1 = Input.GetTouch(0).position;
						// 		Vector3 newPos2 = Input.GetTouch(1).position;								
								
						// 		float angleChange_all = Vector3.Angle(newPos2 - newPos1 , downPosition2_touch - downPosition_touch);
						// 		float distanceChange = Vector3.Distance(newPos2 , newPos1) - Vector3.Distance(downPosition2_touch, downPosition_touch);
						// 		float distanceChangePercent = Mathf.Abs(distanceChange) / Mathf.Abs(Vector3.Distance(downPosition2_touch, downPosition_touch));
						// 		// DebugUtils.LogTouch("angleChange_all "+angleChange_all);
								
						// 		if (distanceChangePercent >= FINGER_2_JUDGE_ZOOM_DISTANCE_CHANGE || angleChange_all <= FINGER_2_JUDGE_ZOOM_ANGLE_CHANGE) //if is zoom
						// 		{			
						// 			float angleChange_cross = Vector3.Angle(newPos2 - downPosition2_touch , newPos1 - downPosition_touch);	
						// 			if (angleChange_cross > 60) // if move different direction
						// 			{
						// 				if (distanceChangePercent >= FINGER_2_JUDGE_ZOOM_DISTANCE_CHANGE)
						// 				{
						// 					touch2Zooming = true; 
						// 					touch2Rotating = false;	
						// 					touch2TypeDecided = true;	
						// 					// DebugUtils.LogTouch("decided touch2Zooming");
						// 				}
						// 				else if (angleChange_all > FINGER_2_JUDGE_ZOOM_ANGLE_CHANGE)
						// 				{
						// 					touch2Rotating = true; 
						// 					touch2Zooming = false;	
						// 					touch2TypeDecided = true;	
						// 					// DebugUtils.LogTouch("decided touch2Rotate 0");
						// 				}
						// 				else
						// 				{
						// 					//waiting
						// 				}
						// 			}
						// 			else
						// 			{
						// 				touch2FingerSameDeltaY_touch = (newPos1.y - downPosition_touch.y + newPos2.y - downPosition2_touch.y)/2;
						// 				touch2Rotating = false;
						// 				touch2Angleing = true;
						// 				touch2Zooming = false;	
						// 				touch2TypeDecided = true;	
						// 				// DebugUtils.LogTouch("decided touch2Rotate 1");
						// 			}	
									
						// 			if (touch2TypeDecided)
						// 			{
						// 				mCameraState = CAMERA_STATE.eIdle;

						// 				prePosition_touch = currentPosition_touch;
						// 				currentPosition_touch = Input.GetTouch(0).position;
						// 				detailPosition_touch = currentPosition_touch - prePosition_touch;
						// 				prePosition2_touch = currentPosition2_touch;
						// 				currentPosition2_touch = Input.GetTouch(1).position;
						// 				detailPosition2_touch = currentPosition2_touch - prePosition2_touch;
						// 			}
						// 		}
								
						// 		if (angleChange_all > FINGER_2_JUDGE_ZOOM_ANGLE_CHANGE)
						// 		{
						// 			touch2Rotating = true;
						// 			touch2Angleing = false;
						// 			touch2TypeDecided = true;	
						// 			// DebugUtils.LogTouch("decided touch2Rotate 3");
						// 			prePosition_touch = currentPosition_touch;
						// 			currentPosition_touch = Input.GetTouch(0).position;
						// 			detailPosition_touch = currentPosition_touch - prePosition_touch;
						// 			prePosition2_touch = currentPosition2_touch;
						// 			currentPosition2_touch = Input.GetTouch(1).position;
						// 			detailPosition2_touch = currentPosition2_touch - prePosition2_touch;
						// 		}
								
						// 	}
						// 	else
						// 	{								
						// 		if (touch2Zooming || mCameraState == CAMERA_STATE.eZooming)
						// 		{
						// 			//mCameraState = CAMERA_STATE.eZooming;
						// 			newDistance_touch = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
						// 			deltaY_touch = newDistance_touch - oldDistance_touch;
						// 			// DebugUtils.LogTouch("zooming deltaY "+deltaY_touch);
						// 		}
								
						// 		touch2FingerDeltaAngle = 0f;
						// 		if (touch2Rotating)
						// 		{
						// 			//mCameraState = CAMERA_STATE.eRotating;
									
						// 			// bool sameSide = false;									
						// 			Vector3 newPos1 = Input.GetTouch(0).position;
						// 			Vector3 newPos2 = Input.GetTouch(1).position;									
									
						// 			// float angleChange_cross = Vector3.Angle(newPos2 - currentPosition2_touch , newPos1 - currentPosition_touch);	
						// 			// if (angleChange_cross < 90) // if move together at same side
						// 			// {
						// 			// 	sameSide = true;
										
						// 			// 	//caculate angels by screen center point
						// 			// 	Vector3 midlePoint = new Vector3(SCREEN_WIDTH/2, SCREEN_HEIGHT/2, currentPosition_touch.z);
										
						// 			// 	float angleChange_1 = SignAngle(currentPosition_touch - midlePoint, newPos1 - midlePoint );								
						// 			// 	float angleChange_2 = SignAngle(currentPosition2_touch - midlePoint, newPos2 - midlePoint );
										
						// 			// 	touch2FingerDeltaAngle = (angleChange_1 + angleChange_2) / 2;
						// 			// 	// DebugUtils.LogTouch("1 same touch2FingerDeltaAngle "+touch2FingerDeltaAngle);
						// 			// }						
									
						// 			// //judge by center point of two start point
						// 			// if (!sameSide)
						// 			// {
						// 				Vector3 midlePoint = new Vector3((currentPosition_touch.x + currentPosition2_touch.x)/2,
						// 					(currentPosition_touch.y + currentPosition2_touch.y)/2,
						// 					(currentPosition_touch.z + currentPosition2_touch.z)/2);
										
						// 				float angleChange_1 = SignAngle(currentPosition_touch - midlePoint,  newPos1 - midlePoint);								
						// 				float angleChange_2 = SignAngle(currentPosition2_touch - midlePoint,  newPos2 - midlePoint);
										
						// 				if ( angleChange_1 * angleChange_2 > 0 )
						// 				{
						// 					// sameSide = true;
											
						// 					touch2FingerDeltaAngle = SignAngle(currentPosition2_touch - currentPosition_touch, newPos2 - newPos1);
						// 				//	int direction = touch2FingerDeltaAngle >= 0 ? 1: -1;
						// 				//	touch2FingerDeltaAngle = FINGER_2_ROTATE_SPEED_PER_SECOND * direction * mDeltaTime;
											
						// 					// DebugUtils.LogTouch("2 same touch2FingerDeltaAngle "+touch2FingerDeltaAngle);
						// 				}
										
										
						// 				if((angleChange_1 == 0 && angleChange_2 != 0) || (angleChange_2 == 0 && angleChange_1 != 0)) //if one finger move, one stop
						// 				{
						// 					// sameSide = true;
						// 					if (angleChange_1 == 0)
						// 						touch2FingerDeltaAngle = SignAngle(currentPosition2_touch - currentPosition_touch, newPos2 - currentPosition_touch);	
						// 					if (angleChange_2 == 0)
						// 						touch2FingerDeltaAngle = SignAngle(currentPosition_touch - currentPosition2_touch, newPos1 - currentPosition2_touch);	
											
						// 				//	int direction = touch2FingerDeltaAngle >= 0 ? 1: -1;
						// 				//	touch2FingerDeltaAngle = FINGER_2_ROTATE_SPEED_PER_SECOND * direction * mDeltaTime;
											
						// 					// DebugUtils.LogTouch("3 one touch one stop touch2FingerDeltaAngle "+touch2FingerDeltaAngle);	
						// 				}
										
						// 				if (angleChange_1 == 0 && angleChange_2 == 0)
						// 				{
						// 					touch2FingerDeltaAngle = 0;
						// 					// DebugUtils.LogTouch("2 finger all stop ");	
						// 				}
									
						// 			}									
									
						// 			//still not same direction, useless
									
						// 			// if (!sameSide)										
						// 			// {										
						// 			// 	// DebugUtils.LogTouch("not same direction, abandon");
						// 			// 	touch2FingerDeltaAngle = 0;
						// 			// }
						// 		}

						// 		if(touch2Angleing)
						// 		{
						// 			mCameraState = CAMERA_STATE.eAngle;
						// 		}
								
						// 		touch2FingerDeltaAngle = -touch2FingerDeltaAngle;								
								
						// 		prePosition_touch = currentPosition_touch;
						// 		currentPosition_touch = Input.GetTouch(0).position;
						// 		detailPosition_touch = currentPosition_touch - prePosition_touch;
						// 		prePosition2_touch = currentPosition2_touch;
						// 		currentPosition2_touch = Input.GetTouch(1).position;
						// 		detailPosition2_touch = currentPosition2_touch - prePosition2_touch;
						// 	}
						// }
					}					
				}			
			}
		}
		else
		{
			//print("no touch move ");
			if (touch2Finger_touch)
			{
				// if (!touchMoveAway2_touch)
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
			touch2Angleing = false;
			oldDistance_touch = 0;
			currentPosition_touch = Vector3.zero;
			detailPosition_touch = Vector3.zero;
			prePosition_touch = Vector3.zero;

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
			downPositionInScreen = currentPosition;
			downPositionInWorldSpace = bCamera.transform.position;
			// RaycastHit hit;
			// Ray ray = mCurCamera.ScreenPointToRay (currentPosition);
   //          if (Physics.Raycast (ray, out hit, Mathf.Infinity, 1 << Consts.CAMERA_MOVE_LAYER))
   //          {
   //              downPositionInWorldSpace = hit.point;
   //          }
		}

		// if(UI_Event.sIsEvent)
		// {
		// 	UI_Event.sIsEvent = false;
		// 	// Debug.Log("is ui event");
		// 	return;
		// }

		
		// if(!ForcsOnUI)
		// {
		// 	if(BuildingManager.instance.mCurrentBuildingControl == null && !RoadManager.instance.mInputLock)
		// 	{
		// 		ControlCamera();
		// 	}
		// 	if(!RoadManager.instance.mInputLock)
  //           	ControlBuilding();
		// 	ControlBuildRoadConstruction();
		// }
	

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

	// public bool mCanControlSeaMap = false;
	void ControlSeaMap()
	{
// 		if(!mCanControlSeaMap) return;

// 		UISeaMap ui_sea = MenuManager.instance.FindMenu<UISeaMap>();
// 		if(ui_sea == null) return;

// #if UNITY_EDITOR || UNITY_PLAYER || UNITY_STANDALONE_WIN || UNITY_METRO
// 		if (deltaY_mouse != 0)
// 		{
// 			float dy = -Mathf.Sign(deltaY_mouse);
// 			ui_sea.Zoom(dy);
// 		}
// 		else if ((deltaX != 0 || deltaZ != 0))
// 		{
//             ui_sea.Move((downPosition.x - currentPosition.x)*Time.deltaTime, (downPosition.y - currentPosition.y)*Time.deltaTime);
// 		}
// #endif
// #if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_METRO
// 		if (touch2Finger_touch)
// 		{
// 			if (Mathf.Abs(deltaY_touch) > iPhoneZoomDeltaValue)
// 			{
// 				float dy = -Mathf.Sign(deltaY_touch);
// 				oldDistance_touch = newDistance_touch;
// 				// bCamera.Zoom(dy);
// 				ui_sea.Zoom(dy);
// 			}
// 		}
// 		else
// 		{
// 			//
// 		}
// #endif
// 		// rotate to default rotation
// 		else if (touch2FingerReleased && mCameraState == CAMERA_STATE.eIdle )
// 		{
// 			// bCamera.RotateBack();
// 		}
// 		// no input and move
// 		else if (!isPressed && !isDown && !touch2Finger)
// 		{
// 			//DebugUtils.Log("SmoothStop ");
// 			// bCamera.SmoothStop();
			
// 			//hasInput = false;
// 		}

	}
	
	void ControlCamera()
	{
		if (enableCamera && bCamera)
		{
			// Check for Camera MOVING.
			if ((deltaX != 0 || deltaZ != 0) && (mCameraState == CAMERA_STATE.eIdle || mCameraState == CAMERA_STATE.eMoving || mCameraState == CAMERA_STATE.ePressingBuilding))
			{
				//Debug.LogError("mCameraState  " + mCameraState);
				if (mCameraState == CAMERA_STATE.eIdle)
					mCameraState = CAMERA_STATE.eMoving;

				Vector3 move_position = (downPositionInScreen - currentPosition) * 60f;
				move_position.x = move_position.x / Screen.width;
				move_position.y = move_position.y / Screen.height;
				//Debug.Log("move_position " + move_position);
				bCamera.MoveEx(downPositionInWorldSpace, move_position.x , move_position.y);

				// // if camera move, the touch position should always point to "downPositionInWorldSpace" 
				// RaycastHit hit;
    //             Vector3 currentPointInWorldSpace = downPositionInWorldSpace;

    //             Ray ray = mCurCamera.ScreenPointToRay (currentPosition);
    //             if (Physics.Raycast (ray, out hit, Mathf.Infinity, 1 << Consts.CAMERA_MOVE_LAYER))
    //             {
    //                 currentPointInWorldSpace = hit.point;
    //             }
                
    //             bCamera.Move(downPositionInWorldSpace.x - currentPointInWorldSpace.x, downPositionInWorldSpace.z - currentPointInWorldSpace.z);
                
    //             // bCamera.Move((downPosition.x - currentPosition.x)*Time.deltaTime, (downPosition.y - currentPosition.y)*Time.deltaTime);
			}

#if UNITY_EDITOR || UNITY_PLAYER || UNITY_STANDALONE_WIN
			else if(touch2Finger_mouse)
			{
				if (Input.GetMouseButton(2) && (mCameraState == CAMERA_STATE.eIdle || mCameraState == CAMERA_STATE.eAngle))
				{
					mCameraState = CAMERA_STATE.eAngle;
					bCamera.RotateX(touch2FingerDeltaY_mouse);
				}
				else if (touch2FingerDeltaX_mouse != 0 || touch2FingerDeltaY_mouse != 0)
				{
					//downPosition2_mouse
					mCameraState = CAMERA_STATE.eRotating;
					{
						bCamera.RotateY(touch2FingerDeltaX_mouse);
						//float dy = -Mathf.Sign(touch2FingerDeltaY_mouse)*20f;
						bCamera.ZoomEx(touch2FingerDeltaY_mouse);
						//bCamera.Rotate(0, touch2FingerDeltaY_mouse/ SCREEN_HEIGHT, 50);
					}
				}
			}
#endif

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_METRO
			else if (touch2Finger_touch && touchMoveAway2_touch)
			{
				Vector3 newPos1 = currentPosition_touch;
				Vector3 newPos2 = currentPosition2_touch;

				GMath.LinePointSide side1 = GMath.CheckLinePointSide(downPosition_touch, downPosition2_touch, newPos1);
				GMath.LinePointSide side2 = GMath.CheckLinePointSide(downPosition_touch, downPosition2_touch, newPos2);

				float point_distance = Mathf.Abs((newPos1 - downPosition_touch).y + (newPos2 - downPosition2_touch).y)/Screen.height/2f;

				// float distance_x = Mathf.Abs(Mathf.Abs((downPosition_touch - downPosition2_touch).x) - Mathf.Abs((newPos1 - newPos2).x))/Screen.width;
				float distance_x = ((downPosition_touch - downPosition2_touch).magnitude - (newPos1 - newPos2).magnitude)/Screen.width;

				Vector3 tmpv1 = downPosition_touch - downPosition2_touch;
				Vector3 tmpv2 = newPos1 - newPos2;
				float tmp_detail_angle = Vector3.SignedAngle(tmpv1, tmpv2, Vector2.right);

				// Debug.Log("distance_x " + distance_x + " distance_y " + distance_y
				// 	+ " point_distance " + point_distance + " mCameraState " + mCameraState);

				if(( point_distance > 0.1f && mCameraState == CAMERA_STATE.eIdle && side1 != GMath.LinePointSide.ON_LINE && side1 == side2)
					|| mCameraState == CAMERA_STATE.eAngle)
				{
					mCameraState = CAMERA_STATE.eAngle;
					float detail_distance = (detailPosition_touch.y + detailPosition2_touch.y)/2f;
					// Debug.Log("detail_distance " + detail_distance);
					bCamera.RotateX(detail_distance * Time.unscaledDeltaTime);
				}
				else if(((tmp_detail_angle > 3f || Mathf.Abs(distance_x) > 0.06f) && mCameraState == CAMERA_STATE.eIdle)
					|| (mCameraState == CAMERA_STATE.eRotating))
				{
					mCameraState = CAMERA_STATE.eRotating;
					float detail_x = ((prePosition_touch - prePosition2_touch).magnitude - (newPos1 - newPos2).magnitude)/Screen.width;
					// float detail_x = distance_x * 1f;
					
					Vector3 v1 = prePosition_touch - prePosition2_touch;
					Vector3 v2 = newPos1 - newPos2;
					float detail_angle = Vector3.SignedAngle(v1, v2, Vector2.right);
					float cross_product = GMath.LineCrossProduct(v1,v2);
					if (cross_product < 0)
					{
						detail_angle *= -1f;
					}
					// Debug.Log("detail_x " + detail_x);
					// Debug.Log("detail_angle " + detail_angle);
					bCamera.RotateY(detail_angle * Time.unscaledDeltaTime * 50f);
					bCamera.ZoomEx(detail_x * Time.unscaledDeltaTime * 3500f);
				}
			}	
#endif		
			// // rotate to default rotation
			// else if ((touch2FingerReleased) && mCameraState == CAMERA_STATE.eIdle )
			// {
			// 	bCamera.RotateBack();
			// }
			// no input and move
			else if(touch2FingerReleased)
			{	
				bCamera.FinishFinger2Touch();
				//bCamera.SmoothStop();
			}
			else if(isReleased)
			{
				bCamera.FinishMove();
			}
			else if (!isPressed && !isDown && !touch2Finger)
			{
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

	// void ControlBuilding()
	// {
	// 	BuildingManager.instance.InputControlBuilding(isDown, isPressed, isReleased, touchMoveAway,
	// 		downPosition, currentPosition, releasePosition);
	// }
 //    GameObject building = null;
 //    void ControlBuildingConstruction()
	// {
 //        if (isDown)
	// 	{
 //            building = RaycastUtil.instance.GetHitInfoInLay<GameObject>(downPosition, "Building");
 //        }
	// 	else if (isPressed && touchMoveAway)
	// 	{
 //            Vector3 worldPos = RaycastUtil.instance.GetHitInfoInLay<Vector3>(currentPosition, "Terrain");
 //            worldPos.y = 0;

 //            if (building != null)
 //            {
 //                building.GetComponent<BuildingConstructor>().UpdateBuilding(isDown, isPressed,
 //                    isReleased, touch2Finger, touchMoveAway, worldPos);
 //            }
 //        }
	// 	if (isReleased && !touchMoveAway)
	// 	{
 //            if (building != null)
 //            {
 //                Vector3 worldPos = RaycastUtil.instance.GetHitInfoInLay<Vector3>(currentPosition, "Terrain");
 //                building.GetComponent<BuildingConstructor>().UpdateBuilding(isDown, isPressed,
 //                    isReleased, touch2Finger, touchMoveAway, releasePosition);
 //            }
 //            else if(!RaycastUtil.instance.HitObjectInLay(releasePosition, "Road"))
 //            {
 //                //Ö÷½¨Öþ½¨ÔìUI
 //                UILoadBuilding loading_building = MenuManager.instance.CreateMenu<UILoadBuilding>();
 //                loading_building.ChangePanelPosition(releasePosition);
 //                if (!loading_building.IsShow)
 //                {
 //                    loading_building.OpenScreen();
 //                    //ÕÒµ½ÆäËûUI²¢ÇÒ¹Øµô
 //                    UIBuildingUpgrate loading_buildingUpgrate =  MenuManager.instance.FindMenu<UIBuildingUpgrate>();
 //                    if(loading_buildingUpgrate != null)
 //                    {
 //                        loading_buildingUpgrate.CloseScreen();
 //                    }
 //                }
 //                else
 //                {
 //                    loading_building.CloseScreen();
 //                }
 //            }   
 //        }
	// }

	// void ControlBuildRoadConstruction()
	// {
	// 	RoadManager.instance.InputControlRoad(isDown,isPressed,isReleased,touchMoveAway,downPosition,currentPosition,releasePosition);
		
	// }

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
