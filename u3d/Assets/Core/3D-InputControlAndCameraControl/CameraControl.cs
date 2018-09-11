using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	// NOTE: These are overridden in the city.unity level
	public float zoomSpeed = 300.0f;

	public float cameraLeftBorder = -50;
	public float cameraRightBorder = 50;
	public float cameraTopBorder = 90;
	public float cameraBottomBorder = -15;
	public float cameraZoomInBorder = 10;
	public float cameraZoomOutBorder = 50;
	
	public float autoTargetSmoothTime = 0.2f;
	public float autoTargetThreshold = 0.2f;
	public float noTargetRadius = 16.0f;
	public float frictionFactor = 0.1f;
	public float fricStopValue = 2;
	public float rotateFrictionFactor = 0.15f;
	public float autoSmoothNeedSpeed = 120f;
	public float autoSmoothMaxSpeed = 140f;
	public float autoSmoothSlowDownDistance = 180f;
	
	//public float maxFov = 50;
	
	// Rotation
	//public float rotateSpeed = 100f;
	float cameraAngleDelta = 15;

	public bool  isCameraAngleDeltaMaxMin = false;
	public float cameraAngleDeltaMax = 60;
	public float cameraAngleDeltaMin = 30;
	
	private Vector3 destinPoint = Vector3.zero;
	private bool setDestinPoint = false;
	private bool mIsRotateBack = false;
	
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
		if (setDestinPoint)
		{
			ApplyPositionDamping();
		}
		if (mIsRotateBack)
		{
			RotatingBack();
		}
        if (autoZoomInDecel)
        {
            zoomTimer += Time.deltaTime;
            ZoomInDecelerate(zoomTimer);
        }
	}
	
	public void Move(float dx, float dz)
	{
		deltaY = 0;
		deltaX = dx;
		deltaZ = dz;
		deltaRotateY = 0;
		deltaRotateZ = 0;
		deltaAngleY = 0;
		
		Vector3 newPosition = _transform.position + (cameraRight * dx + cameraForward * dz);
		
		// limit the border
		_transform.position = LimitCamera(newPosition);

		setDestinPoint = false;
	}
	
	public void MoveByCameraDirection(float dx, float dz)
	{
		if (HasRotate())
		{
			deltaY = 0;
			deltaX = dx;
			deltaZ = dz;
			deltaRotateY = 0;
			deltaRotateZ = 0;
			deltaAngleY = 0;
			
			Vector3 camRight = _camTransform.right;
			Vector3 camForward = _camTransform.forward;
			camForward.y = 0;
			camForward.Normalize();
			
			Vector3 newPosition = _transform.position + (camRight * dx + camForward * dz);
			
			// limit the border
			_transform.position = LimitCamera(newPosition);
	
			setDestinPoint = false;
		}
		else
		{
			Move(dx, dz);
		}
	}
	
	public void Zoom(float dy)
	{
		Zoom(dy , true);
	}
	
	public void Zoom(float dy , bool _limitCam)
	{
		Vector3 cameraZoomForward = _camTransform.forward;
		
		deltaY = dy;
		deltaX = deltaZ = deltaRotateY = deltaRotateZ = deltaAngleY = 0;
		
		dy *= zoomSpeed * Time.unscaledDeltaTime;
			
		Vector3 newPosition = _camTransform.position + cameraZoomForward * (-dy);
			
		// limit the zoom
		if (_limitCam)
		{
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
		
			_camTransform.position = newPosition;
			
			_transform.position = LimitCamera(GetTargetPoint());
		}
		else
		{
			_camTransform.position = newPosition;
		}
		
		//UpdateCameraFov();
		
		setDestinPoint = false;
	}
	
	// _dx _dy is the progress to the whole screen width and height
	public void Rotate(float _dx, float _dy , int _maxSpeedPerSecond = 100)
	{
		if (mIsRotateBack)
			return;
		
		Stop();
		deltaRotateY = _dx;
		
		if (_dx != 0)
		{
			_dx *= 180;
			_camTransform.RotateAround(_transform.position, Vector3.up, _dx);
			mHasRotate = true;
		}
		
		if (_dy != 0)
		{
			Vector3 eulerAngle = _camTransform.eulerAngles;
			float newX = eulerAngle.x - Mathf.Sign(_dy) * _maxSpeedPerSecond * Time.unscaledDeltaTime;
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
				_camTransform.RotateAround(_transform.position, _camTransform.right, newX - eulerAngle.x);
				deltaRotateZ = _dy;
			}
			else
			{
				deltaRotateZ = 0;
			}
			
			mHasRotate = true;
		}
	}
	
	public void Rotate(float _dAngleY, bool _smoothStop)
	{
		if (mIsRotateBack)
			return;
		
		deltaX = deltaY = deltaZ = deltaRotateY = deltaRotateZ = 0;
		
		if (_dAngleY != 0)
		{
			_camTransform.RotateAround(_transform.position, Vector3.up, _dAngleY);
			mHasRotate = true;			
			
			if (!_smoothStop)
			{
				mAutoRotatedAngle = 0f;
				float deltatime = Time.unscaledDeltaTime;
				float speed = (deltatime != 0)? _dAngleY / deltatime: _dAngleY;				
				
				if (Mathf.Abs(speed) >= autoSmoothNeedSpeed)
				{
					deltaAngleY = _dAngleY;
					
					if (Mathf.Abs(speed) > autoSmoothMaxSpeed)
					{
						_dAngleY =  autoSmoothMaxSpeed * deltatime * ( _dAngleY / Mathf.Abs(_dAngleY));
					}					
				}
				else
					deltaAngleY = 0;
			}
		}
	}
	
	public bool HasRotate()
	{
		return mHasRotate;
	}
	
	public void RotateBack()
	{
		if (mHasRotate)
		{
			Stop();
			mIsRotateBack = true;
			mRotateTime = 0;
			mRotateBackAnglesX = 0;
			mRotateBackAnglesY = 0;
						
			Vector3 eulerAngles = _camTransform.eulerAngles;
			mRotateAnglesX = mEulerAngleX - eulerAngles.x;
			mRotateAnglesY = -eulerAngles.y;
			if (mRotateAnglesY < -180) mRotateAnglesY = 360 + mRotateAnglesY;
			else if (mRotateAnglesY > 180) mRotateAnglesY = 360 - mRotateAnglesY;
			
			mRotateTotalTime = Mathf.Abs(mRotateAnglesX) / 180 * 0.3f;
			mRotateTotalTime += 0.1f;
		}
	}
	
	void RotatingBack()
	{
		if (mRotateTime > mRotateTotalTime)
		{
			if (mRotateBackAnglesY != mRotateAnglesY)
			{
				_camTransform.RotateAround(_transform.position, Vector3.up, mRotateAnglesY - mRotateBackAnglesY);
			}
			if (mRotateBackAnglesX != mRotateAnglesX)
			{
				_camTransform.RotateAround(_transform.position, _camTransform.right, mRotateAnglesX - mRotateBackAnglesX);
			}
			mIsRotateBack = false;
			mHasRotate = false;
		}
		else
		{
			float deltatime = Time.unscaledDeltaTime;
			mRotateTime += deltatime;
			
			if (mRotateAnglesY != 0)
			{
				float angle = mRotateAnglesY / mRotateTotalTime * deltatime;
				if ((mRotateAnglesY > 0 &&  mRotateBackAnglesY + angle < mRotateAnglesY) ||
					(mRotateAnglesY < 0 &&  mRotateBackAnglesY + angle > mRotateAnglesY))
				{
					mRotateBackAnglesY += angle;
					_camTransform.RotateAround(_transform.position, Vector3.up, angle);
				}
			}
			if (mRotateAnglesX != 0)
			{
				float angle = mRotateAnglesX / mRotateTotalTime *deltatime;
				if ((mRotateAnglesX > 0 &&  mRotateBackAnglesX + angle < mRotateAnglesX) ||
					(mRotateAnglesX < 0 &&  mRotateBackAnglesX + angle > mRotateAnglesX))
				{
					mRotateBackAnglesX += angle;
					_camTransform.RotateAround(_transform.position, _camTransform.right, angle);
				}
			}
		}
	}
	
	public void SmoothStop()
	{
		if (deltaX != 0 || deltaZ != 0)
		{
			deltaX *= (1-frictionFactor);
			deltaZ *= (1-frictionFactor);
			Move(deltaX, deltaZ);
			//print("dx = "+deltaX + " dz= "+deltaZ);
			if (Mathf.Abs(deltaX) < fricStopValue && Mathf.Abs(deltaZ) < fricStopValue)
			{
				deltaX = deltaZ = 0;
			}
		}
		if (deltaY != 0)
		{
			deltaY *= (1-frictionFactor);
			Zoom(deltaY);
			if (Mathf.Abs(deltaY) < 0.2f)
			{
				deltaY = 0;
			}
		}
		if (deltaRotateY != 0)
		{
			deltaRotateY *= (1-frictionFactor);
			Rotate(deltaRotateY, 0);
			if (Mathf.Abs(deltaRotateY) < 0.001f)
			{
				deltaRotateY = 0;
			}
		}
		if (deltaAngleY != 0)
		{			
			deltaAngleY *= (1-rotateFrictionFactor);
			Rotate(deltaAngleY, true);			
			
			
			mAutoRotatedAngle += deltaAngleY;			
			if (Mathf.Abs(mAutoRotatedAngle) >= autoSmoothSlowDownDistance)
				deltaAngleY = deltaAngleY / 2;			
			
			if (Mathf.Abs(deltaAngleY) < 0.001f)
			{
				deltaAngleY = 0;
			}
		}
		
		if (deltaRotateZ != 0)
		{
			deltaRotateZ *= (1-frictionFactor);
			Rotate(0,deltaRotateZ);
			if (Mathf.Abs(deltaRotateZ) < 0.001f)
			{
				deltaRotateZ = 0;
			}
		}
	}
	
	public void Stop()
	{
		deltaX = deltaY = deltaZ = deltaRotateY = deltaRotateZ = deltaAngleY = 0;
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
	
	public void FocusObject(GameObject obj)
	{
		FocusObject(obj, false);
	}
	
	public void FocusPos(Vector3 _destinPoint, bool forced)
	{		
		_transform.position = _destinPoint;
		
		Stop();
		setDestinPoint = false;
	}
	
	public Vector3 GetCurFocusPos()
	{
		return _transform.position;
	}
	
	public void FocusObject(GameObject obj, bool forced)
	{
		if(obj == null)
			return;

		Vector3 _destinPoint = obj.transform.position;

		FocusPosSmooth(_destinPoint, forced);
	}

	public void FocusPosSmooth(Vector3 _destinPoint, bool forced)
	{
		destinPoint = _destinPoint;
		destinPoint.y = 0;
		destinPoint = LimitCamera(destinPoint);

		Vector3 deltaPos = destinPoint - GetTargetPoint();
		if (deltaPos.sqrMagnitude > noTargetRadius || forced)
		{
			setDestinPoint = true;
			Stop();
		}
		else
		{
			setDestinPoint = false;
		}
	}

	public void FocusPosSmoothEx(Vector3 _destinPoint, bool forced)
	{
		destinPoint = _destinPoint;
		// destinPoint.y = 0;
		destinPoint = LimitCamera(destinPoint);

		Vector3 deltaPos = destinPoint - GetTargetPoint();
		if (deltaPos.sqrMagnitude > noTargetRadius || forced)
		{
			setDestinPoint = true;
			Stop();
		}
		else
		{
			setDestinPoint = false;
		}
	}

	void ApplyPositionDamping()
	{
		Vector3 newTargetPosition = Vector3.zero;
		Vector3 deltaPos = Vector3.zero;
		Vector3 targetPoint = GetTargetPoint();
		newTargetPosition.x = Mathf.SmoothDamp(targetPoint.x, destinPoint.x, ref autoTargetVelocity.x, autoTargetSmoothTime);
		newTargetPosition.z = Mathf.SmoothDamp(targetPoint.z, destinPoint.z, ref autoTargetVelocity.z, autoTargetSmoothTime);

		newTargetPosition.y = destinPoint.y;

		_transform.position = newTargetPosition;
		
		deltaPos = newTargetPosition - destinPoint;
		if (deltaPos.sqrMagnitude < autoTargetThreshold)
		{
			setDestinPoint = false;
		}
	}
	
	// for hero casting skill
	public void FocusObjectImmediately(GameObject obj)
	{
		if(obj == null)
			return;
		
		destinPoint = obj.transform.position;
		destinPoint.y = 0;
		destinPoint = LimitCamera(destinPoint);
		
		_transform.position = destinPoint;
		
		Stop();
		setDestinPoint = false;
	}

	public void ZoomImmediately(float _y)
	{
		Vector3 newPosition = _camTransform.position;
		newPosition.y = _y;
				
		float deltay = newPosition.y - _camTransform.position.y;
		
		Vector3 cameraZoomForward = _camTransform.forward;
		if (cameraZoomForward.y != 0)
		{
			float kk = deltay / cameraZoomForward.y;
			newPosition.x = _camTransform.position.x + cameraZoomForward.x * kk;
			newPosition.z = _camTransform.position.z + cameraZoomForward.z * kk;
		}
		
		_camTransform.position = newPosition;
		
		//UpdateCameraFov();
	}

	// for leaving city 
	public void ZoomOutAccelerate(float t)
	{
		Vector3 newPosition = _camTransform.position;
		if (t > 0) t = t * t;
		if (t > 1) t = 1;
		newPosition.y = Mathf.Lerp(newPosition.y, cameraZoomOutBorder, t);
				
		float deltay = newPosition.y - _camTransform.position.y;
		
		Vector3 cameraZoomForward = _camTransform.forward;
		if (cameraZoomForward.y != 0)
		{
			float kk = deltay / cameraZoomForward.y;
			newPosition.x = _camTransform.position.x + cameraZoomForward.x * kk;
			newPosition.z = _camTransform.position.z + cameraZoomForward.z * kk;
		}
		
		_camTransform.position = newPosition;
	}

    public void ZoomOutAccelerateForLightingStrike(float t,int posValue)
    {
        Vector3 newPosition = _camTransform.position;
        if (t > 0) t = t * t;
        if (t > 1) t = 1;
        newPosition.y = Mathf.Lerp(newPosition.y, posValue, t);

        float deltay = newPosition.y - _camTransform.position.y;

        Vector3 cameraZoomForward = _camTransform.forward;
        if (cameraZoomForward.y != 0)
        {
            float kk = deltay / cameraZoomForward.y;
            newPosition.x = _camTransform.position.x + cameraZoomForward.x * kk;
            newPosition.z = _camTransform.position.z + cameraZoomForward.z * kk;
        }

        _camTransform.position = newPosition;
    }
	
	// called before ZoomInDecelerate to init the location
	public void InitZoomIn()
	{
		_camTransform.position = new Vector3(_camTransform.position.x, cameraZoomInBorder, _camTransform.position.z);
		
		float backDis = (cameraZoomOutBorder - _camTransform.position.y) / _camTransform.forward.y;
		_camTransform.position += backDis * _camTransform.forward;
	}

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

	// for landing city 
	public void ZoomInDecelerate(float t)
    {
		Vector3 newPosition = _camTransform.position;
		if (t > 0) t = Mathf.Pow(t, 0.8f);
        if (t > 1) { 
            t = 1;
            if (autoZoomInDecel)
                autoZoomInDecel = false;
        }		
		
		newPosition.y = Mathf.Lerp(cameraZoomOutBorder, mCam_OriginalLocalPosition.y, t);
		float deltaY = newPosition.y - _camTransform.position.y;
		float dis = deltaY / _camTransform.forward.y;
		_camTransform.position += dis * _camTransform.forward;
	}
	
	/*void UpdateCameraFov()
	{
		float y = _camTransform.position.y;
		// update the fov of camera
		if (y < cameraZoomInBorder + (cameraZoomOutBorder - cameraZoomInBorder) * 0.3f)
		{
			if (y >= cameraZoomInBorder)
			{
				float t = (y - cameraZoomInBorder) / ((cameraZoomOutBorder - cameraZoomInBorder) * 0.3f);
				_camTransform.camera.fieldOfView = InuMisc.instance.easeOutCubic(maxFov, mCam_OriginalFov, t);
			}
		}
		else
		{
			_camTransform.camera.fieldOfView = mCam_OriginalFov;
		}
		
	}*/

	/*void UpdateThisToBeCenterOfScreen()
	{
		RaycastHit hit;
		Vector3 currentPosition = new Vector3(0.5f, 0.5f, 0.1f);
		
		Ray ray = GlobalObject.gCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.1f));

		Vector3 camPos = _camTransform.position;
		Vector3 camDirect = ray.direction;

		float deltay = 0 - camPos.y;
		float deltax = camDirect.x / camDirect.y * deltay;
		float deltaz = camDirect.z / camDirect.y * deltay;

		_transform.position = new Vector3(deltax, deltay, deltaz) + camPos;
		_camTransform.position = camPos;
	}*/
}
