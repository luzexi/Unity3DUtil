using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    //三级的高度
	public float[] viewHeightGrad = new float[3]{10f,30f,60f};
	// public float[] viewAngleGrad = new float[3]{40f,40f,40f};
	// public float[] viewFieldGrad = new float[3]{25f,25f,25f};

	//目标等级
	private float adaptZoomTime = 0.5f;
	private float adaptZoomHeightSpeed = 1f;
	private float adaptZoomAngleStartTime = 0f;
	private float adaptZoomTotalTime = 0f;
	private int adaptZoomGrad = 0;
	/////////////////////////////////////////////////////

    // NOTE: These are overridden in the city.unity level
	// public float zoomSpeed = 300.0f;

	public float cameraLeftBorder = -50;
	public float cameraRightBorder = 50;
	public float cameraTopBorder = 90;
	public float cameraBottomBorder = -15;
	public float cameraZoomInBorder = 10;
	public float cameraZoomOutBorder = 50;
	
	// public float autoTargetSmoothTime = 0.2f;
	// public float autoTargetThreshold = 0.2f;
	// public float noTargetRadius = 16.0f;
	// public float frictionFactor = 0.1f;
	// public float fricStopValue = 2;
	// public float rotateFrictionFactor = 0.15f;
	// public float autoSmoothNeedSpeed = 120f;
	// public float autoSmoothMaxSpeed = 140f;
	// public float autoSmoothSlowDownDistance = 180f;
	
	//public float maxFov = 50;
	
	// Rotation
	//public float rotateSpeed = 100f;
	float cameraAngleDelta = 15;

	public bool  isCameraAngleDeltaMaxMin = false;
	public float cameraAngleDeltaMax = 60;
	public float cameraAngleDeltaMin = 30;
	
	private Vector3 destinPoint = Vector3.zero;
	private bool setDestinPoint = false;
    private bool mIsZoomGrad = false;
	private bool mIsRotateBack = false;

	private bool mIsMoving = false;
	private bool mIsMovingSmooth = false;
	private float mMovingSmoothStartTime;
	private Vector3 mMovingSmoothSpeed;
	private float mMovingSmoothTime = 0.5F;
	private float mMovingSmoothDecrease = 30F;
	
	private Vector3 autoTargetVelocity = Vector3.zero;
	private Vector3 cameraRight;
	private Vector3 cameraForward;

	private float deltaX = 0;
	private float deltaY = 0;
	private float deltaZ = 0;
	private float deltaRotateY;
	private float deltaRotateZ;
	private float deltaAngleY;
	
	private Transform _transform;
	public Transform _camTransform;
	
	// cache for triangle functions
	private float tan1;
	private float tan2;
	private float tan3;
	private float cos1;
	
	private bool mHasRotate;
	private bool mIsRotateY;
	private bool mIsRotateX;
	private int mIsZooming = 0;
	private float mRotateAnglesX;
	private float mRotateAnglesY;
	private float mRotateBackAnglesX;
	private float mRotateBackAnglesY;
	private float mRotateTime;
	private float mRotateTotalTime;
	private float mEulerAngleX;
	private float mAutoRotatedAngle;
	
	private Vector3 mCam_OriginalLocalPosition = Vector3.zero;
	private Quaternion mCam_OriginalLocalRotation = Quaternion.identity;

    private Vector3 mCam_OriginalPosition = Vector3.zero;
    private Quaternion mCam_OriginalRotation = Quaternion.identity;

    private Vector3 mCam_SavedLocalPosition = Vector3.zero;
    private Quaternion mCam_SavedLocalRotation = Quaternion.identity;

    private Vector3 mSavedLocalPosition = Vector3.zero;
    private Quaternion mSavedLocalRotation = Quaternion.identity;

    private bool autoZoomInDecel = false;
    private float zoomTimer = 0.0f;

	//private float mCam_OriginalFov = 25;
	
	void Awake()
	{
		_transform = transform;
		if(_camTransform == null)
			_camTransform = _transform.Find("Main Camera");
		mEulerAngleX =_camTransform.eulerAngles.x;
		mCam_OriginalLocalPosition = _camTransform.localPosition;
		mCam_OriginalLocalRotation = _camTransform.localRotation;

        mCam_OriginalPosition = _camTransform.position;
        mCam_OriginalRotation = _camTransform.rotation;

		cameraRight = _transform.right;
		cameraForward = _transform.forward;
	}
	
	// Use this for initialization
	void Start () 
	{
		float fov = _camTransform.GetComponent<Camera>().fieldOfView;
		//mCam_OriginalFov = fov;
		tan1 = Mathf.Tan((90-mEulerAngleX-fov*0.5f)*(Mathf.PI)/180) - Mathf.Tan((90-mEulerAngleX)*(Mathf.PI)/180);
		tan2 = Mathf.Tan((90-mEulerAngleX+fov*0.5f)*(Mathf.PI)/180) - Mathf.Tan((90-mEulerAngleX)*(Mathf.PI)/180);
		tan3 = Mathf.Tan((fov*0.5f)*(Mathf.PI)/180);
		cos1 = tan3 * _camTransform.GetComponent<Camera>().aspect / Mathf.Cos((90-mEulerAngleX)*(Mathf.PI)/180);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(mIsMovingSmooth)
		{
			MoveSmooth();
		}

		// if (setDestinPoint)
		// {
		// 	ApplyPositionDamping();
		// }


		if (mIsRotateBack)
		{
			RotatingBack();
		}
        // if (autoZoomInDecel)
        // {
        //     zoomTimer += Time.deltaTime;
        //     ZoomInDecelerate(zoomTimer);
        // }
		
        if (mIsZoomGrad)
        {
            AdaptZoomGrad();
        }
	}

	public void FinishMove()
	{
		if(mIsMoving)
		{
			// qhEvent.GlobalEvent.fire<float>(qhEvent.ID.input_camera_move,null,1);
			mIsMoving = false;
			mIsMovingSmooth = true;
			MoveSmooth();
		}
	}

	public void FinishFinger2Touch()
	{
		if(mIsZooming != 0)
		{
			// qhEvent.GlobalEvent.fire<float>(qhEvent.ID.input_camera_zoom,null,mIsZooming);
			mIsZooming = 0;
			EndZoom();
		}
		if(mIsRotateY)
		{
			mIsRotateY = false;
			// qhEvent.GlobalEvent.fire<float>(qhEvent.ID.input_camera_rotate,null,1);
		}
		if(mIsRotateX)
		{
			mIsRotateX = false;
			RotateBack();
		}
	}
	
#region 平移

	void MoveSmooth()
	{
		float dif_time = Time.time - mMovingSmoothStartTime;
		if(dif_time >= mMovingSmoothTime)
		{
			dif_time = mMovingSmoothTime;
			mIsMovingSmooth = false;
		}
		float rate = dif_time / mMovingSmoothTime;

		Vector3 move_dail = Vector3.Lerp(mMovingSmoothSpeed, Vector3.zero, rate);

		MoveDetail(move_dail.x, move_dail.z);
	}

	public void MoveEx(Vector3 src_position, float dx, float dz)
	{
		Vector3 camera_forward = _camTransform.forward;
		camera_forward.y = 0;
		Vector3 camera_right = _camTransform.right;
		camera_right.y = 0;
		Vector3 newPosition = src_position + (camera_right * dx + camera_forward * dz);
		
		//deltaX = _transform.position.x - newPosition.x;
		//deltaY = _transform.position.x - newPosition.x;

		mMovingSmoothSpeed = _transform.position;

		// limit the border
		_transform.position = LimitCamera(newPosition);

		mMovingSmoothSpeed = _transform.position - mMovingSmoothSpeed;
		mMovingSmoothTime = mMovingSmoothSpeed.magnitude/mMovingSmoothDecrease;
		if(mMovingSmoothTime < Time.unscaledDeltaTime)
		{
			mMovingSmoothTime = Time.unscaledDeltaTime;
		}

		mIsMoving = true;
		setDestinPoint = false;
		mIsMovingSmooth = false;
		mMovingSmoothStartTime = Time.time;
	}

	void MoveDetail(float dx, float dz)
	{
		Vector3 newPosition = _transform.position + new Vector3(dx, 0, dz);
		// limit the border
		_transform.position = LimitCamera(newPosition);
	}

	public void Move(float dx, float dz)
	{
		// deltaY = 0;
		// deltaX = dx;
		// deltaZ = dz;
		// deltaRotateY = 0;
		// deltaRotateZ = 0;
		// deltaAngleY = 0;
		
		Vector3 newPosition = _transform.position + (cameraRight * dx + cameraForward * dz);
		
		// limit the border
		_transform.position = LimitCamera(newPosition);

		// setDestinPoint = false;
	}
	
	public void MoveByCameraDirection(float dx, float dz)
	{
		
		if (HasRotate())
		{
			// deltaY = 0;
			// deltaX = dx;
			// deltaZ = dz;
			// deltaRotateY = 0;
			// deltaRotateZ = 0;
			// deltaAngleY = 0;
			
			Vector3 camRight = _camTransform.right;
			Vector3 camForward = _camTransform.forward;
			camForward.y = 0;
			camForward.Normalize();
			
			Vector3 newPosition = _transform.position + (camRight * dx + camForward * dz);
			
			// limit the border
			_transform.position = LimitCamera(newPosition);
	
			// setDestinPoint = false;
		}
		else
		{
			Move(dx, dz);
		}
	}

#endregion

#region 缩放
	public void ZoomEx(float dy)
	{
		if (mIsRotateBack)
			return;

		//相机朝向
		Vector3 cameraZoomForward = _camTransform.forward;
			
		//新的位置
		Vector3 newPosition = _camTransform.position + cameraZoomForward * (-dy);

		newPosition = LimitZoom(newPosition);
			
		_camTransform.position = newPosition;

		if(dy > 0)
		{
			mIsZooming = 1;
		}
		else
		{
			mIsZooming = -1;
		}
		mIsZoomGrad = false;
	}

	Vector3 LimitZoom(Vector3 newPosition)
	{
		Vector3 cameraZoomForward = _camTransform.forward;
		bool outside = false;
		if (newPosition.y < cameraZoomInBorder)
		{
			newPosition.y = cameraZoomInBorder;
			outside = true;
		}
		else if (newPosition.y > cameraZoomOutBorder)
		{
			newPosition.y = cameraZoomOutBorder;
			outside = true;
		}
		
		if (outside)
		{
			float deltay = newPosition.y - _camTransform.position.y;
			if (cameraZoomForward.y != 0)
			{
				float kk = deltay / cameraZoomForward.y;
				newPosition.x = _camTransform.position.x + cameraZoomForward.x * kk;
				newPosition.z = _camTransform.position.z + cameraZoomForward.z * kk;
			}
		}
		return newPosition;
	}

	// public void Zoom(float dy)
	// {
 //        mIsZoomGrad = false;
	// 	Zoom(dy , true);
	// }
	
	// public void Zoom(float dy , bool _limitCam)
	// {
	// 	//相机朝向
	// 	Vector3 cameraZoomForward = _camTransform.forward;
		
	// 	deltaY = dy;
	// 	deltaX = deltaZ = deltaRotateY = deltaRotateZ = deltaAngleY = 0;
		
	// 	//手指移动距离*zoom速度*时间
	// 	dy *= zoomSpeed * Time.unscaledDeltaTime;
			
	// 	//新的位置
	// 	Vector3 newPosition = _camTransform.position + cameraZoomForward * (-dy);
			
	// 	// limit the zoom
	// 	if (_limitCam)
	// 	{
	// 		bool outside = false;
	// 		if (newPosition.y < cameraZoomInBorder)
	// 		{
	// 			newPosition.y = cameraZoomInBorder;
	// 			outside = true;
	// 		}
	// 		else if (newPosition.y > cameraZoomOutBorder)
	// 		{
	// 			newPosition.y = cameraZoomOutBorder;
	// 			outside = true;
	// 		}
			
	// 		if (outside)
	// 		{
	// 			float deltay = newPosition.y - _camTransform.position.y;
	// 			if (cameraZoomForward.y != 0)
	// 			{
	// 				float kk = deltay / cameraZoomForward.y;
	// 				newPosition.x = _camTransform.position.x + cameraZoomForward.x * kk;
	// 				newPosition.z = _camTransform.position.z + cameraZoomForward.z * kk;
	// 			}
	// 		}
		
	// 		_camTransform.position = newPosition;
			
	// 		_transform.position = LimitCamera(GetTargetPoint());
	// 	}
	// 	else
	// 	{
	// 		_camTransform.position = newPosition;
	// 	}
		
	// 	//UpdateCameraFov();
		
	// 	setDestinPoint = false;
	// }

    public void EndZoom()
    {
		float nowY = _camTransform.position.y;
		// Quaternion nowRotation = _camTransform.rotation;
		// Quaternion targetRotation = nowRotation;
		// Vector3 nowAngle = nowRotation.eulerAngles;
		adaptZoomAngleStartTime = Time.unscaledTime;

		if(nowY <= viewHeightGrad[0])
		{
			adaptZoomHeightSpeed = (viewHeightGrad[0] - nowY)/adaptZoomTime;
			adaptZoomGrad = 0;
		}
		// else if(nowY == viewHeightGrad[0])
		// {
		// 	adaptZoomGrad = 0;
		// }
		else if(nowY > viewHeightGrad[0] && nowY <= viewHeightGrad[1])
		{
			if(nowY > viewHeightGrad[0] + (viewHeightGrad[1] - viewHeightGrad[0])/2)
			{
				adaptZoomHeightSpeed = (viewHeightGrad[1] - nowY)/adaptZoomTime;
				adaptZoomGrad = 1;
			}
			else
			{
				adaptZoomHeightSpeed = (viewHeightGrad[0] - nowY)/adaptZoomTime;
				adaptZoomGrad = 0;
			}
		}
		// else if(nowY == viewHeightGrad[1])
		// {
		// 	adaptZoomGrad = 1;
		// }
		else if(nowY > viewHeightGrad[1] && nowY <= viewHeightGrad[2])
		{
			if(nowY > viewHeightGrad[1] + (viewHeightGrad[2] - viewHeightGrad[1])/2)
			{
				adaptZoomHeightSpeed = (viewHeightGrad[2] - nowY)/adaptZoomTime;
				adaptZoomGrad = 2;
			}
			else
			{
				adaptZoomHeightSpeed = (viewHeightGrad[1] - nowY)/adaptZoomTime;
				adaptZoomGrad = 1;
			}
		}
		// else if(nowY == viewHeightGrad[2])
		// {
		// 	adaptZoomGrad = 2;
		// }
		else if(nowY > viewHeightGrad[2])
		{
			adaptZoomHeightSpeed = (viewHeightGrad[2] - nowY)/adaptZoomTime;
			adaptZoomGrad = 2;
		}

		// nowAngle.x = viewAngleGrad[adaptZoomGrad];
		// targetRotation = Quaternion.Euler(nowAngle);
		// adaptZoomTargetAngle = targetRotation;

        mIsZoomGrad = true;
    }

	public void AdaptZoomGrad()
	{
		float targetZoomGradHeight = viewHeightGrad[adaptZoomGrad];
		// float targetZoomGradAngle = viewAngleGrad[adaptZoomGrad];

		//相机朝向
		Vector3 cameraZoomForward = _camTransform.forward;
		float dy = 0f;
		Vector3 newPosition = Vector3.zero;
		//新的旋转
		// float l = Mathf.InverseLerp(adaptZoomAngleStartTime, adaptZoomAngleStartTime + adaptZoomTime, Time.unscaledTime);
		// _camTransform.rotation = Quaternion.Lerp(_camTransform.rotation, adaptZoomTargetAngle, l);

		//新的透视值

		adaptZoomTotalTime += Time.unscaledDeltaTime; 

		if(adaptZoomTotalTime >= adaptZoomTime)
		{
			mIsZoomGrad = false;
			adaptZoomTotalTime = 0f;
			dy = 0;
			//dy = targetZoomGradHeight - _camTransform.position.y;
			// _camTransform.rotation = adaptZoomTargetAngle;
		}
		else
		{
			//zoom速度*时间
			dy = adaptZoomHeightSpeed * Time.unscaledDeltaTime;
			//新的位置
		}
		newPosition = _camTransform.position + cameraZoomForward*(cameraZoomForward.magnitude/cameraZoomForward.y * dy);

		newPosition = LimitZoom(newPosition);

		_camTransform.position = newPosition;

	}



#endregion

#region 旋转

	public void RotateY(float _dx)
	{
		if (mIsRotateBack)
			return;
		
		Stop();
		//deltaRotateY = _dx;
		
		if (_dx != 0)
		{
			//_dx *= 180;

			//绕中心点旋转
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (new Vector2(Screen.width/2,Screen.height/2));
            if (Physics.Raycast (ray, out hit, Mathf.Infinity, 1<<9))
            {
				_camTransform.RotateAround(hit.point,Vector3.up, _dx);
            }
			//mHasRotate = true;
			mIsRotateY = true;
		}
	}

	public void RotateX(float _dy)
	{
		if (mIsRotateBack)
			return;
		
		Stop();
		
		if (_dy != 0)
		{
			Vector3 eulerAngle = _camTransform.eulerAngles;
			float newX = eulerAngle.x - Mathf.Sign(_dy);
			if(isCameraAngleDeltaMaxMin)
			{
				if(newX > cameraAngleDeltaMax)
					newX = cameraAngleDeltaMax;
				if(newX < cameraAngleDeltaMin)
					newX = cameraAngleDeltaMin;
			}
			else
			{
				if (newX > mEulerAngleX + cameraAngleDelta)
					newX = mEulerAngleX + cameraAngleDelta;
				else if (newX < mEulerAngleX - cameraAngleDelta)
					newX = mEulerAngleX - cameraAngleDelta;
			}
			
			if (Mathf.Abs(newX - eulerAngle.x) > 0.1f)
			{
				_camTransform.Rotate(new Vector3(newX - eulerAngle.x,0,0));
				//deltaRotateZ = _dy;
			}
			else
			{
				//deltaRotateZ = 0;
			}
			
			mIsRotateX = true;
		}
	}

	// // _dx _dy is the progress to the whole screen width and height
	// public void Rotate(float _dx, float _dy , int _maxSpeedPerSecond = 100)
	// {
	// 	if (mIsRotateBack)
	// 		return;
		
	// 	Stop();
	// 	deltaRotateY = _dx;
		
	// 	if (_dx != 0)
	// 	{
	// 		_dx *= 180;

	// 		//绕中心点旋转
	// 		RaycastHit hit;
	// 		Ray ray = Camera.main.ScreenPointToRay (new Vector2(Screen.width/2,Screen.height/2));
 //            if (Physics.Raycast (ray, out hit, Mathf.Infinity, 1<<9))
 //            {
	// 			_camTransform.RotateAround(hit.point,Vector3.up, _dx);
 //            }
	// 		mHasRotate = true;
	// 	}
		
	// 	if (_dy != 0)
	// 	{
	// 		Vector3 eulerAngle = _camTransform.eulerAngles;
	// 		float newX = eulerAngle.x - Mathf.Sign(_dy) * _maxSpeedPerSecond * Time.unscaledDeltaTime;
	// 		if(isCameraAngleDeltaMaxMin)
	// 		{
	// 			if(newX > cameraAngleDeltaMax)
	// 				newX = cameraAngleDeltaMax;
	// 			if(newX < cameraAngleDeltaMin)
	// 				newX = cameraAngleDeltaMin;
	// 		}
	// 		else
	// 		{
	// 			if (newX > mEulerAngleX + cameraAngleDelta)
	// 				newX = mEulerAngleX + cameraAngleDelta;
	// 			else if (newX < mEulerAngleX - cameraAngleDelta)
	// 				newX = mEulerAngleX - cameraAngleDelta;
	// 		}
			
	// 		if (Mathf.Abs(newX - eulerAngle.x) > 0.1f)
	// 		{
	// 			_camTransform.Rotate(new Vector3(newX - eulerAngle.x,0,0));
	// 			deltaRotateZ = _dy;
	// 		}
	// 		else
	// 		{
	// 			deltaRotateZ = 0;
	// 		}
			
	// 		mHasRotate = true;
	// 	}
	// }
	
	// public void Rotate(float _dAngleY, bool _smoothStop)
	// {
	// 	if (mIsRotateBack)
	// 		return;
		
	// 	deltaX = deltaY = deltaZ = deltaRotateY = deltaRotateZ = 0;
		
	// 	if (_dAngleY != 0)
	// 	{
	// 		// _camTransform.RotateAround(_transform.position, Vector3.up, _dAngleY);
	// 		//绕中心点旋转
	// 		RaycastHit hit;
	// 		Ray ray = Camera.main.ScreenPointToRay (new Vector2(Screen.width/2,Screen.height/2));
 //            if (Physics.Raycast (ray, out hit, Mathf.Infinity, 1<<9))
 //            {
	// 			_camTransform.RotateAround(hit.point,Vector3.up, _dAngleY);
 //            }
	// 		mHasRotate = true;			
			
	// 		if (!_smoothStop)
	// 		{
	// 			mAutoRotatedAngle = 0f;
	// 			float deltatime = Time.unscaledDeltaTime;
	// 			float speed = (deltatime != 0)? _dAngleY / deltatime: _dAngleY;				
				
	// 			if (Mathf.Abs(speed) >= autoSmoothNeedSpeed)
	// 			{
	// 				deltaAngleY = _dAngleY;
					
	// 				if (Mathf.Abs(speed) > autoSmoothMaxSpeed)
	// 				{
	// 					_dAngleY =  autoSmoothMaxSpeed * deltatime * ( _dAngleY / Mathf.Abs(_dAngleY));
	// 				}					
	// 			}
	// 			else
	// 				deltaAngleY = 0;
	// 		}
	// 	}
	// }
	
	public bool HasRotate()
	{
		return mHasRotate;
	}
	
	void RotateBack()
	{
		mIsRotateBack = true;
		mRotateTime = 0;
		mRotateBackAnglesX = 0;
		mRotateBackAnglesY = 0;
					
		Vector3 eulerAngles = _camTransform.eulerAngles;
		mRotateAnglesX = mEulerAngleX - eulerAngles.x;
		mRotateAnglesY = -eulerAngles.y;
		if (mRotateAnglesY < -180) mRotateAnglesY = 360 + mRotateAnglesY;
		else if (mRotateAnglesY > 180) mRotateAnglesY = 360 - mRotateAnglesY;
		mRotateTotalTime = Mathf.Abs(mRotateAnglesY) / 180 * 0.3f;
		mRotateTotalTime += 0.1f;
	}
	
	void RotatingBack()
	{
		if (mRotateTime > mRotateTotalTime)
		{
			// if (mRotateBackAnglesY != mRotateAnglesY)
			// {
			// 	_camTransform.RotateAround(_transform.position, Vector3.up, mRotateAnglesY - mRotateBackAnglesY);
			// }
			if (mRotateBackAnglesX != mRotateAnglesX)
			{
				// _camTransform.RotateAround(_transform.position, _camTransform.right, mRotateAnglesX - mRotateBackAnglesX);
				_camTransform.Rotate(new Vector3(mRotateAnglesX - mRotateBackAnglesX,0,0));
			}
			mIsRotateBack = false;
			mHasRotate = false;
		}
		else
		{
			float deltatime = Time.unscaledDeltaTime;
			mRotateTime += deltatime;
			
			// if (mRotateAnglesY != 0)
			// {
			// 	float angle = mRotateAnglesY / mRotateTotalTime * deltatime;
			// 	if ((mRotateAnglesY > 0 &&  mRotateBackAnglesY + angle < mRotateAnglesY) ||
			// 		(mRotateAnglesY < 0 &&  mRotateBackAnglesY + angle > mRotateAnglesY))
			// 	{
			// 		mRotateBackAnglesY += angle;
			// 		_camTransform.RotateAround(_transform.position, Vector3.up, angle);
			// 	}
			// }
			if (mRotateAnglesX != 0)
			{
				float angle = mRotateAnglesX / mRotateTotalTime *deltatime;
				if ((mRotateAnglesX > 0 &&  mRotateBackAnglesX + angle < mRotateAnglesX) ||
					(mRotateAnglesX < 0 &&  mRotateBackAnglesX + angle > mRotateAnglesX))
				{
					mRotateBackAnglesX += angle;
					// _camTransform.RotateAround(_transform.position, _camTransform.right, angle);
					_camTransform.Rotate(new Vector3(angle,0,0));
				}
			}
		}
	}
	
#endregion

	// public void SmoothStop()
	// {
	// 	if (deltaX != 0 || deltaZ != 0)
	// 	{
	// 		qhEvent.GlobalEvent.fire<float>(qhEvent.ID.input_camera_move,null,deltaX);
	// 		deltaX *= (1-frictionFactor);
	// 		deltaZ *= (1-frictionFactor);
	// 		Move(deltaX, deltaZ);
	// 		//print("dx = "+deltaX + " dz= "+deltaZ);
	// 		if (Mathf.Abs(deltaX) < fricStopValue && Mathf.Abs(deltaZ) < fricStopValue)
	// 		{
	// 			deltaX = deltaZ = 0;
	// 		}
	// 	}
	// 	if (deltaY != 0)
	// 	{
	// 		// deltaY *= (1-frictionFactor);
	// 		// Zoom(deltaY);
	// 		// if (Mathf.Abs(deltaY) < 0.2f)
	// 		// {
	// 		// 	deltaY = 0;
	// 		// }
	// 		qhEvent.GlobalEvent.fire<float>(qhEvent.ID.input_camera_zoom,null,deltaY);
	// 		EndZoom();
	// 		deltaY = 0;
	// 	}
	// 	if (deltaRotateY != 0)
	// 	{
	// 		qhEvent.GlobalEvent.fire<float>(qhEvent.ID.input_camera_rotate,null,deltaRotateY);
	// 		deltaRotateY *= (1-frictionFactor);
	// 		Rotate(deltaRotateY, 0);
	// 		if (Mathf.Abs(deltaRotateY) < 0.001f)
	// 		{
	// 			deltaRotateY = 0;
	// 		}
	// 	}
	// 	if (deltaAngleY != 0)
	// 	{	
	// 		qhEvent.GlobalEvent.fire<float>(qhEvent.ID.input_camera_rotate,null,deltaAngleY);		
	// 		deltaAngleY *= (1-rotateFrictionFactor);
	// 		Rotate(deltaAngleY, true);			
			
			
	// 		mAutoRotatedAngle += deltaAngleY;			
	// 		if (Mathf.Abs(mAutoRotatedAngle) >= autoSmoothSlowDownDistance)
	// 			deltaAngleY = deltaAngleY / 2;			
			
	// 		if (Mathf.Abs(deltaAngleY) < 0.001f)
	// 		{
	// 			deltaAngleY = 0;
	// 		}
	// 	}
		
	// 	if (deltaRotateZ != 0)
	// 	{
	// 		deltaRotateZ *= (1-frictionFactor);
	// 		Rotate(0,deltaRotateZ);
	// 		if (Mathf.Abs(deltaRotateZ) < 0.001f)
	// 		{
	// 			deltaRotateZ = 0;
	// 		}
	// 	}
	// }
	
	public void Stop()
	{
		mIsMovingSmooth = false;
		//deltaX = deltaY = deltaZ = deltaRotateY = deltaRotateZ = deltaAngleY = 0;
	}
	
	public Vector3 LimitCamera(Vector3 target_pos)
	{
		float camY = _camTransform.position.y;
		float dz1 = camY * tan1;
		float dz2 = camY * tan2;
		float width2 = camY * cos1;
		
		if (-dz1 + dz2 <= cameraTopBorder - cameraBottomBorder)
		{
			if (target_pos.z + dz1 < cameraBottomBorder)
			{
				target_pos.z = cameraBottomBorder - dz1;
			}
			else if (target_pos.z + dz2 > cameraTopBorder)
			{
				target_pos.z = cameraTopBorder - dz2;
			}
		}
		else
		{
			target_pos.z = 0;
		}
		
		if (width2 * 2 <= cameraRightBorder - cameraLeftBorder)
		{
			if (target_pos.x - width2 < cameraLeftBorder)
			{
				target_pos.x = cameraLeftBorder + width2;
			}
			else if (target_pos.x + width2 > cameraRightBorder)
			{
				target_pos.x = cameraRightBorder - width2;
			}
		}
		else
		{
			target_pos.x = 0;
		}
		return target_pos;
	}
	
	public Vector3 GetTargetPoint()
	{
		return _transform.position;
	}
	
	public Vector3 GetCameraPosition()
	{
		return _camTransform.position;
	}
	
	// public void FocusObject(GameObject obj)
	// {
	// 	FocusObject(obj, false);
	// }
	
	// public void FocusPos(Vector3 _destinPoint, bool forced)
	// {		
	// 	_transform.position = _destinPoint;
		
	// 	Stop();
	// 	setDestinPoint = false;
	// }
	
	public Vector3 GetCurFocusPos()
	{
		return _transform.position;
	}
	
	// public void FocusObject(GameObject obj, bool forced)
	// {
	// 	if(obj == null)
	// 		return;

	// 	Vector3 _destinPoint = obj.transform.position;

	// 	FocusPosSmooth(_destinPoint, forced);
	// }

	// public void FocusPosSmooth(Vector3 _destinPoint, bool forced)
	// {
	// 	destinPoint = _destinPoint;
	// 	destinPoint.y = 0;
	// 	destinPoint = LimitCamera(destinPoint);

	// 	Vector3 deltaPos = destinPoint - GetTargetPoint();
	// 	if (deltaPos.sqrMagnitude > noTargetRadius || forced)
	// 	{
	// 		setDestinPoint = true;
	// 		Stop();
	// 	}
	// 	else
	// 	{
	// 		setDestinPoint = false;
	// 	}
	// }

	// public void FocusPosSmoothEx(Vector3 _destinPoint, bool forced)
	// {
	// 	destinPoint = _destinPoint;
	// 	// destinPoint.y = 0;
	// 	destinPoint = LimitCamera(destinPoint);

	// 	Vector3 deltaPos = destinPoint - GetTargetPoint();
	// 	if (deltaPos.sqrMagnitude > noTargetRadius || forced)
	// 	{
	// 		setDestinPoint = true;
	// 		Stop();
	// 	}
	// 	else
	// 	{
	// 		setDestinPoint = false;
	// 	}
	// }

	// void ApplyPositionDamping()
	// {
	// 	Vector3 newTargetPosition = Vector3.zero;
	// 	Vector3 deltaPos = Vector3.zero;
	// 	Vector3 targetPoint = GetTargetPoint();
	// 	newTargetPosition.x = Mathf.SmoothDamp(targetPoint.x, destinPoint.x, ref autoTargetVelocity.x, autoTargetSmoothTime);
	// 	newTargetPosition.z = Mathf.SmoothDamp(targetPoint.z, destinPoint.z, ref autoTargetVelocity.z, autoTargetSmoothTime);

	// 	newTargetPosition.y = destinPoint.y;

	// 	_transform.position = newTargetPosition;
		
	// 	deltaPos = newTargetPosition - destinPoint;
	// 	if (deltaPos.sqrMagnitude < autoTargetThreshold)
	// 	{
	// 		setDestinPoint = false;
	// 	}
	// }
	
	// // for hero casting skill
	// public void FocusObjectImmediately(GameObject obj)
	// {
	// 	if(obj == null)
	// 		return;
		
	// 	destinPoint = obj.transform.position;
	// 	destinPoint.y = 0;
	// 	destinPoint = LimitCamera(destinPoint);
		
	// 	_transform.position = destinPoint;
		
	// 	Stop();
	// 	setDestinPoint = false;
	// }

	// public void ZoomImmediately(float _y)
	// {
	// 	Vector3 newPosition = _camTransform.position;
	// 	newPosition.y = _y;
				
	// 	float deltay = newPosition.y - _camTransform.position.y;
		
	// 	Vector3 cameraZoomForward = _camTransform.forward;
	// 	if (cameraZoomForward.y != 0)
	// 	{
	// 		float kk = deltay / cameraZoomForward.y;
	// 		newPosition.x = _camTransform.position.x + cameraZoomForward.x * kk;
	// 		newPosition.z = _camTransform.position.z + cameraZoomForward.z * kk;
	// 	}
		
	// 	_camTransform.position = newPosition;
		
	// 	//UpdateCameraFov();
	// }

	// // for leaving city 
	// public void ZoomOutAccelerate(float t)
	// {
	// 	Vector3 newPosition = _camTransform.position;
	// 	if (t > 0) t = t * t;
	// 	if (t > 1) t = 1;
	// 	newPosition.y = Mathf.Lerp(newPosition.y, cameraZoomOutBorder, t);
				
	// 	float deltay = newPosition.y - _camTransform.position.y;
		
	// 	Vector3 cameraZoomForward = _camTransform.forward;
	// 	if (cameraZoomForward.y != 0)
	// 	{
	// 		float kk = deltay / cameraZoomForward.y;
	// 		newPosition.x = _camTransform.position.x + cameraZoomForward.x * kk;
	// 		newPosition.z = _camTransform.position.z + cameraZoomForward.z * kk;
	// 	}
		
	// 	_camTransform.position = newPosition;
	// }

    // public void ZoomOutAccelerateForLightingStrike(float t,int posValue)
    // {
    //     Vector3 newPosition = _camTransform.position;
    //     if (t > 0) t = t * t;
    //     if (t > 1) t = 1;
    //     newPosition.y = Mathf.Lerp(newPosition.y, posValue, t);

    //     float deltay = newPosition.y - _camTransform.position.y;

    //     Vector3 cameraZoomForward = _camTransform.forward;
    //     if (cameraZoomForward.y != 0)
    //     {
    //         float kk = deltay / cameraZoomForward.y;
    //         newPosition.x = _camTransform.position.x + cameraZoomForward.x * kk;
    //         newPosition.z = _camTransform.position.z + cameraZoomForward.z * kk;
    //     }

    //     _camTransform.position = newPosition;
    // }
	
	// // called before ZoomInDecelerate to init the location
	// public void InitZoomIn()
	// {
	// 	_camTransform.position = new Vector3(_camTransform.position.x, cameraZoomInBorder, _camTransform.position.z);
		
	// 	float backDis = (cameraZoomOutBorder - _camTransform.position.y) / _camTransform.forward.y;
	// 	_camTransform.position += backDis * _camTransform.forward;
	// }

    public void SaveCamera()
    {
        mCam_SavedLocalPosition = _camTransform.localPosition;
        mCam_SavedLocalRotation = _camTransform.localRotation;

        mSavedLocalPosition = transform.localPosition;
        mSavedLocalRotation = transform.localRotation;
    
    }
    public void LoadSavedCamera()
    {
        _camTransform.localPosition = mCam_SavedLocalPosition;
        _camTransform.localRotation = mCam_SavedLocalRotation;

        transform.localPosition = mSavedLocalPosition;
        transform.localRotation = mSavedLocalRotation;
    }

	// // for landing city 
	// public void ZoomInDecelerate(float t)
 //    {
	// 	Vector3 newPosition = _camTransform.position;
	// 	if (t > 0) t = Mathf.Pow(t, 0.8f);
 //        if (t > 1) { 
 //            t = 1;
 //            if (autoZoomInDecel)
 //                autoZoomInDecel = false;
 //        }		
		
	// 	newPosition.y = Mathf.Lerp(cameraZoomOutBorder, mCam_OriginalLocalPosition.y, t);
	// 	float deltaY = newPosition.y - _camTransform.position.y;
	// 	float dis = deltaY / _camTransform.forward.y;
	// 	_camTransform.position += dis * _camTransform.forward;
	// }
}