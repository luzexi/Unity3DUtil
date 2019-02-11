
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class CMisc
{
	static string sDeviceId = "";

	// get DeviceID
    public static string GetDeviceId()
    {
        if(sDeviceId != null) return sDeviceId;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        sDeviceId = SystemInfo.deviceUniqueIdentifier;
#elif UNITY_ANDROID
        AndroidJavaClass deviceUtils = new AndroidJavaClass("com.sdo.qihang.lib.DeviceUtils");
        sDeviceId = deviceUtils.CallStatic<string> ("getUniqueId");
#elif UNITY_IPHONE
        sDeviceId = SystemInfo.deviceUniqueIdentifier;
#endif
        return sDeviceId;
    }
}