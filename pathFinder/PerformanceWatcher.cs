using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PerformanceWatcher
{
	#region Varibles Declaration
	public static List<long>	mFinishTimeList;	
	private long mStartTime;
	//private int	 mCycleTimes;
	#endregion
	
	public void Awake()
	{
	}
	
	#region Function
	public void Start()
	{
		mStartTime = Decimal.ToInt64(Decimal.Divide(DateTime.UtcNow.Ticks - 621355968000000000, 10000));
	}
	
	public void Record(int times)
	{
		//mCycleTimes = times;
	}
	
	public long End()
	{
		if(mFinishTimeList == null)
		{	
			mFinishTimeList = new List<long>();
		}
		long costTime = Decimal.ToInt64(Decimal.Divide(DateTime.UtcNow.Ticks - 621355968000000000, 10000)) - mStartTime;
		mFinishTimeList.Add(costTime);
		//Debug.Log("cost "+costTime.ToString()+" mil seconds");
		return costTime;
	}
	
	#endregion
}