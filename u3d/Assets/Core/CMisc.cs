
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class CMisc
{
	static string sDeviceId = "";
    static string s_persistentDataPath = string.Empty;

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

    public static string GetPersistentDataPath()
    {
        if (string.IsNullOrEmpty(s_persistentDataPath))
        {
            if (string.IsNullOrEmpty(UnityEngine.Application.persistentDataPath))
            {
#if UNITY_ANDROID
                s_persistentDataPath = InternalGetPersistentDataPath();
#endif
            }
            else
            {
                s_persistentDataPath = UnityEngine.Application.persistentDataPath;
            }
        }
        return s_persistentDataPath;
    }

#if UNITY_ANDROID
    private static string InternalGetPersistentDataPath()
    {
        string path = "";
        try
        {
            IntPtr obj_context = AndroidJNI.FindClass("android.os.Environment");
            IntPtr method_getExternalStorageDirectory = AndroidJNIHelper.GetMethodID(obj_context, "getExternalStorageDirectory", "()Ljava/io/File;", true);
            IntPtr file = AndroidJNI.CallStaticObjectMethod(obj_context, method_getExternalStorageDirectory, new jvalue[0]);
            IntPtr obj_file = AndroidJNI.FindClass("java/io/File");
            IntPtr method_getAbsolutePath = AndroidJNIHelper.GetMethodID(obj_file, "getAbsolutePath", "()Ljava/lang/String;");

            path = AndroidJNI.CallStringMethod(file, method_getAbsolutePath, new jvalue[0]);

            if (path != null)
            {
                path += "/Android/data/" + GetPackageName() + "/files";
            }
            else
            {
                path = "";
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        return path;
    }

    private static string GetPackageName()
    {
        string packageName = "";
        try
        {
            using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    IntPtr obj_context = AndroidJNI.FindClass("android/content/ContextWrapper");
                    IntPtr method_getPackageName = AndroidJNIHelper.GetMethodID(obj_context, "getPackageName", "()Ljava/lang/String;");
                    packageName = AndroidJNI.CallStringMethod(obj_Activity.GetRawObject(), method_getPackageName, new jvalue[0]);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        return packageName;
    }
#endif
}