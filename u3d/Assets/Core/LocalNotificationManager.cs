using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocalNotificationManager
{
	static LocalNotificationManager		s_pInstance = null;
#if UNITY_ANDROID
	static int count = 0;
#endif
    public bool Enabled = true;
	public static LocalNotificationManager instance 
	{
		get 
		{
			if (s_pInstance == null) 
			{
				s_pInstance = new LocalNotificationManager();
			}
			return s_pInstance;
		}
	}
	
	public void Init()
	{
#if UNITY_ANDROID
		AndroidJNI.AttachCurrentThread();
#endif
	}
	
	public void AddNotification(float _delayTime, string _content)
	{
        if (!Enabled) return;
		Debug.Log("Add Local Notify "+"delay: "+(_delayTime).ToString()+". content: "+_content);
#if UNITY_IPHONE
		UnityEngine.iOS.LocalNotification notify = new UnityEngine.iOS.LocalNotification();
		notify.fireDate = System.DateTime.Now.AddSeconds(_delayTime);  
		notify.alertBody = _content;  
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notify);
		UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
		
		long time = (long)(_delayTime * 1000);
		using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				AndroidJavaClass cls_OpenNotificationAnd = new AndroidJavaClass("com.sd.glb.OpenNotification");
				cls_OpenNotificationAnd.CallStatic("Init", obj_Activity);				
				cls_OpenNotificationAnd.CallStatic("NotifyText", count++, time, _content);
			}
		}
#endif
	}
	
	public void CancelAllNotification()
	{
#if UNITY_IPHONE
		UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
		count = 0;	
		using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				AndroidJavaClass cls_OpenNotificationAnd = new AndroidJavaClass("com.sd.glb.OpenNotification");
				cls_OpenNotificationAnd.CallStatic("Clear");
			}
		}
#endif
	}
	
	public void AddClearNotification()
	{
        if (!Enabled) return;
#if UNITY_IPHONE
		UnityEngine.iOS.LocalNotification notify = new UnityEngine.iOS.LocalNotification();
		notify.fireDate = System.DateTime.Now;
		notify.applicationIconBadgeNumber = -1;
		notify.hasAction = false;
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notify);
#endif		
	}
}
