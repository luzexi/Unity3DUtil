using System;
using System.Collections.Generic;

public class Point
{
	#region Varibles Declaration	
	private int mPosX;
	private int mPosY;		
	#endregion
	
	#region Properties	
	public int X
	{
		get{return mPosX;}
		set{mPosX = value;}
	}
	
	public int Y
	{
		get{return mPosY;}
		set{mPosY = value;}
	}	
	#endregion
	
	#region Functions
	public Point(int _x, int _y)
	{
		X = _x;
		Y = _y;
	}
	#endregion
}