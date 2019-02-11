using UnityEngine;
using System.Collections;


public static class Vector3_Extensions
{
	public static Vector2 xy(this Vector3 this_)
	{
		return new UnityEngine.Vector2( this_.x,  this_.y);
	}
	
	public static Vector2 xz(this Vector3  this_)
	{
		return new UnityEngine.Vector2( this_.x,  this_.z);
	}
	
	public static Vector2 yz(this Vector3  this_)
	{
		return new UnityEngine.Vector2( this_.y,  this_.z);
	}
	
	public static Vector2 yx(this Vector3  this_)
	{
		return new UnityEngine.Vector2( this_.y,  this_.x);
	}
	
	public static Vector2 zx(this Vector3  this_)
	{
		return new UnityEngine.Vector2( this_.z,  this_.x);
	}
	
	public static Vector2 zy(this Vector3  this_)
	{
		return new UnityEngine.Vector2( this_.z,  this_.y);
	}
}