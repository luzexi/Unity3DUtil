using UnityEngine;
using System.Collections;


//	Polar.cs
//	Author: Lu Zexi
//	2014-12-20


//Polar coordinates can use in camera to move like sphere.
//A distance , an angle of the y-axis and an angle of the xz-axis to y-axis can make a point.
//So Polar can convert by position or direction x,y,z.
[System.Serializable]
public class Polar : System.ICloneable
{
    public float d;	//distance
    public float v;	//the angle of the x&z-axis to y-axis in PI
    public float h;	//the angle of the y-axis in PI
    public Polar() { }

	public Polar( float d_, float v_, float h_ )
	{
		d = d_;
		v = v_;
		h = h_;
	}
	
    //convert from position to polar
    public Polar(Vector3 pos)
    {
        d = pos.magnitude;
        v = Mathf.Atan2(Mathf.Sqrt(pos.x * pos.x + pos.z * pos.z), pos.y) * Mathf.Rad2Deg;
        h = Mathf.Atan2(pos.z, pos.x) * Mathf.Rad2Deg;
    }

    //clone
	public object Clone()
	{
		var ret = new Polar();
		ret.d	= d;
		ret.h	= h;
		ret.v	= v;
		return ret;
	}

    //convert this position to polar
    public Vector3 GetPos()
    {
		return Direction * d;
    }
	
	//lerp polar
	public static Polar Lerp( Polar a, Polar b, float rate )
	{
		return new Polar(
			Mathf.Lerp(			a.d, b.d, rate ),
			Mathf.LerpAngle( 	a.v, b.v, rate ),
			Mathf.LerpAngle( 	a.h, b.h, rate )
		);
	}

	//get direction in polar
	public Vector3 Direction{ get{
			return new Vector3(		Mathf.Sin(Mathf.Deg2Rad * v) * Mathf.Cos(Mathf.Deg2Rad * h),
			                   		Mathf.Cos(Mathf.Deg2Rad * v),
			                   		Mathf.Sin(Mathf.Deg2Rad * v) * Mathf.Sin(Mathf.Deg2Rad * h));
	}}
}

