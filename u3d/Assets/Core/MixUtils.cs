using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
#if !UNITY_METRO && !UNITY_WP8
using System.Security.Cryptography;
#else
using UnityEngine.Windows;
#endif
using System.Net.NetworkInformation;


// mix utils
public class MixUtils
{

#if UNITY_ANDROID
    private static AndroidJavaClass sAndroidJavaClass;
#endif
    private static string sDeviceId = null;
    private static string sSavePath = null;
	
#if UNITY_ANDROID
    //获取当前App的Activity
    public static AndroidJavaObject GetAndroidContext()
    {
        if(sAndroidJavaClass != null)
        {
            return sAndroidJavaClass;
        }
        sAndroidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");

        return sAndroidJavaClass;
    }
#endif

    //获取DeviceID
    public static string getDeviceUniqueId()
    {
        if(sDeviceId != null) return sDeviceId;

#if UNITY_EDITOR
        sDeviceId = SystemInfo.deviceUniqueIdentifier;
#endif
#if UNITY_ANDROID
        AndroidJavaClass deviceUtils = new AndroidJavaClass("com.sdo.qihang.lib.DeviceUtils");
        sDeviceId = deviceUtils.CallStatic<string> ("getUniqueId");
#endif
#if UNITY_IPHONE
        sDeviceId = SystemInfo.deviceUniqueIdentifier;
#endif
        return sDeviceId;
    }
	
	public static string GetString(byte[] _data)
	{
		// Create a new Stringbuilder to collect the bytes
		// and create a string.
		StringBuilder sBuilder = new StringBuilder();
		
		// Loop through each byte of the hashed data 
		// and format each one as a hexadecimal string.
		for (int i = 0; i < _data.Length; i++)
		{
		    sBuilder.Append(_data[i].ToString("x2"));
		}
		
		// Return the hexadecimal string.
		return sBuilder.ToString();		
	}
	
	public static string GetMd5Hash(byte[] _data)
	{
#if !UNITY_METRO && !UNITY_WP8		
		// Create a new instance of the MD5CryptoServiceProvider object.
		MD5 md5Hasher = MD5.Create();
					
		byte[] data = md5Hasher.ComputeHash(_data);
#else		
		byte[] data = Crypto.ComputeMD5Hash(_data);
#endif		
		
		return GetString(data);
	}
	
	public static string GetMd5Hash(string input)
	{
#if !UNITY_METRO && !UNITY_WP8		
		// Create a new instance of the MD5CryptoServiceProvider object.
		MD5 md5Hasher = MD5.Create();
		
		// Convert the input string to a byte array and compute the hash.
		byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
#else		
		byte[] data = Crypto.ComputeMD5Hash(Encoding.UTF8.GetBytes(input));
#endif		
		
		return GetString(data);
	}
	
	//get save path which file can write or save
	private static string GetSavePath()
	{
		if(sSavePath != null) return sSavePath;

		string pathTemp = string.Empty;
#if UNITY_EDITOR || !UNITY_METRO
		if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
            //string path = Application.dataPath.Substring(0,Application.dataPath.Length - 5);
            //path = path.Substring(0,path.LastIndexOf('/'));
		
            //pathTemp = path + "/Documents/";
            pathTemp = Application.persistentDataPath;
            if (!pathTemp.EndsWith("/"))
                pathTemp += "/";
		}
		else if(Application.platform == RuntimePlatform.Android)
		{
			string path = Application.persistentDataPath;
			pathTemp = path + "/Saves";
			if ((pathTemp.Length > 0) && (!System.IO.Directory.Exists(pathTemp)))
		    {
				System.IO.Directory.CreateDirectory(pathTemp);
		    }
		}
		else
		{
			pathTemp = Application.dataPath+"/Saves";
			if ((pathTemp.Length > 0) && (!System.IO.Directory.Exists(pathTemp)))
		    {
				System.IO.Directory.CreateDirectory(pathTemp);
		    }
		}
#endif
		sSavePath = pathTemp;
		return sSavePath;
	}

	public static bool IsKindleDevice()
	{
		if (SystemInfo.deviceModel == "Amazon KFOT" || SystemInfo.deviceModel == "Amazon KFTT" || SystemInfo.deviceModel == "Amazon KFJWA" || SystemInfo.deviceModel == "Amazon KFJWI" 
		    || SystemInfo.deviceModel == "Amazon KFSOWI" || SystemInfo.deviceModel == "Amazon KFTHWA" || SystemInfo.deviceModel == "Amazon KFHWI" || SystemInfo.deviceModel == "Amazon KFAPWA"
		    || SystemInfo.deviceModel == "Amazon KFAPWI")
			return true;
		else
			return false;
	}
}