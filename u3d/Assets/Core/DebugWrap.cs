using UnityEngine;
using System.Collections;

#if DEBUG_LOG || UNITY_EDITOR
public static class Debug
{
	public static bool isDebugBuild
	{
		get
		{
			return UnityEngine.Debug.isDebugBuild;
		}
	}
	public static bool developerConsoleVisible
	{
		get
		{
			return UnityEngine.Debug.developerConsoleVisible;
		}
	}
	private static string GetLogMessage(object _message) {
		string time = GetLogTimeInfo();
		string source = GetLogSourceInfo(3);
		string message = (_message == null ? "(null)" : _message.ToString());
		return time + " " + source + ": " + message;
	}
	
	private static string GetLogTimeInfo() {
		return System.DateTime.Now.ToString("hh:mm:ss:ffffff");
	}
	
	private static string GetLogSourceInfo(int skipFrames) {
		System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame(skipFrames, true);
		if(stackFrame == null)
			return "";
		
		string fileName = stackFrame.GetFileName();
		fileName = (fileName == null) ? "UnknownFile" : fileName.Substring(fileName.LastIndexOf('/') + 1);
		int lineNumber = stackFrame.GetFileLineNumber();
		System.Reflection.MethodBase methodBase = stackFrame.GetMethod();
		string methodName = (methodBase == null) ? "UnknownMethod" : methodBase.ToString();
		
		return fileName + "(" + lineNumber + "): " + methodName;
	}

	//it will be crash when the log is too much.
#if UNITY_EDITOR 
	private static int MaxMessageLength = 10000;
#else 
	private static int MaxMessageLength = 1000; 
#endif

	public static void Assert(bool condition)
	{
		UnityEngine.Debug.Assert(condition);
	}

	public static void Assert(bool condition, Object context)
	{
		UnityEngine.Debug.Assert(condition, context);
	}

	public static void Assert(bool condition, object message)
	{
		UnityEngine.Debug.Assert(condition, message);
	}

	public static void LogAssertion(object message)
	{
		UnityEngine.Debug.LogAssertion(message);
	}

	public static void LogFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogFormat(format, args);
	}

	public static void LogFormat(Object context, string format, params object[] args)
	{
		UnityEngine.Debug.LogFormat(context, format, args);
	}

	public static void Log(object message)
	{
		var m = message.ToString();
		for(int i=0;;++i)
		{
			var startIndex = i * MaxMessageLength;
			var length = Mathf.Min(MaxMessageLength, m.Length - startIndex);
			if (length <= 0)
				break;
			UnityEngine.Debug.Log(m.Substring(startIndex, length));
		}
	}
	public static void Log(object message, Object context)
	{
		var m = message.ToString();
		for(int i=0;;++i)
		{
			var startIndex = i * MaxMessageLength;
			var length = Mathf.Min(MaxMessageLength, m.Length - startIndex);
			if (length <= 0)
				break;
			UnityEngine.Debug.Log(m.Substring(startIndex, length), context);
		}
	}
	
	public static void LogWarning(object message)
	{
		var m = message.ToString();
		for(int i=0;;++i)
		{
			var startIndex = i * MaxMessageLength;
			var length = Mathf.Min(MaxMessageLength, m.Length - startIndex);
			if (length <= 0)
				break;
			UnityEngine.Debug.LogWarning(m.Substring(startIndex, length));
		}
	}
	public static void LogWarning(object message, Object context)
	{
		var m = message.ToString();
		for(int i=0;;++i)
		{
			var startIndex = i * MaxMessageLength;
			var length = Mathf.Min(MaxMessageLength, m.Length - startIndex);
			if (length <= 0)
				break;
			UnityEngine.Debug.LogWarning(m.Substring(startIndex, length), context);
		}
	}

	public static void LogWarningFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogWarningFormat(format, args);
	}

	public static void LogWarningFormat(Object context, string format, params object[] args)
	{
		UnityEngine.Debug.LogWarningFormat(context, format, args);
	}
	
	public static void LogError(object message)
	{
		var m = message.ToString();
		for(int i=0;;++i)
		{
			var startIndex = i * MaxMessageLength;
			var length = Mathf.Min(MaxMessageLength, m.Length - startIndex);
			if (length <= 0)
				break;
// #if !DISABLE_DEBUG_ERROR
			UnityEngine.Debug.LogError(m.Substring(startIndex, length));
// #else
// 			UnityEngine.Debug.Log(m.Substring(startIndex, length));
// #endif
		}
	}
	public static void LogError(object message, Object context)
	{
        return;
        var m = message.ToString();
		for(int i=0;;++i)
		{
			var startIndex = i * MaxMessageLength;
			var length = Mathf.Min(MaxMessageLength, m.Length - startIndex);
			if (length <= 0)
				break;
#if !DISABLE_DEBUG_ERROR
			UnityEngine.Debug.LogError(m.Substring(startIndex, length), context);
#else
			UnityEngine.Debug.Log(m.Substring(startIndex, length));
#endif
		}
	}

	public static void LogErrorFormat(string format, params object[] args)
	{
		UnityEngine.Debug.LogErrorFormat(format, args);
	}

	public static void LogErrorFormat(Object context, string format, params object[] args)
	{
		UnityEngine.Debug.LogErrorFormat(context, format, args);
	}

	public static void LogException(System.Exception exception)
	{
		UnityEngine.Debug.LogException(exception);
	}

	public static void LogException(System.Exception exception, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogException(exception, context);
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest)
	{
		UnityEngine.Debug.DrawLine( start, end, color, duration, depthTest );
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
	{
		UnityEngine.Debug.DrawLine( start, end, color, duration );
	}

	public static void DrawLine(Vector3 start, Vector3 end, Color color)
	{
		UnityEngine.Debug.DrawLine( start, end, color );
	}

	public static void DrawLine(Vector3 start, Vector3 end)
	{
		UnityEngine.Debug.DrawLine( start, end );
	}

	public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
	{
		UnityEngine.Debug.DrawRay( start, dir, color, duration );
	}

	public static void DrawRay(Vector3 start, Vector3 dir, Color color)
	{
		UnityEngine.Debug.DrawRay( start, dir, color );
	}

	public static void DrawRay(Vector3 start, Vector3 dir)
	{
		UnityEngine.Debug.DrawRay( start, dir  );
	}

	public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest)
	{
		UnityEngine.Debug.DrawRay( start, dir, color, duration, depthTest );
	}

	public static void Break()
	{
		UnityEngine.Debug.Break();
	}

	public static void DebugBreak()
	{
		UnityEngine.Debug.DebugBreak();
	}
}
#else
public static class Debug
{
	public static bool isDebugBuild
	{
		get
		{
			return false;
		}
	}
	public static bool developerConsoleVisible
	{
		get
		{
			return false;
		}
	}

	public static void Assert(bool condition){}
	public static void Assert(bool condition, Object context){}
	public static void Assert(bool condition, object message){}
	public static void LogAssertion(object message){}
	public static void LogFormat(string format, params object[] args){}
	public static void LogFormat(Object context, string format, params object[] args){}
	public static void Log(object message) {}
	public static void Log(object message, Object context) {}
	
	public static void LogWarning(object message) {}
	public static void LogWarning(object message, Object context) {}
	public static void LogWarningFormat(string format, params object[] args){}
	public static void LogWarningFormat(Object context, string format, params object[] args){}
	
	public static void LogError(object message) {}
	public static void LogError(object message, Object context) {}
	public static void LogErrorFormat(string format, params object[] args){}
	public static void LogErrorFormat(Object context, string format, params object[] args){}

	public static void LogException(System.Exception message) {}
	public static void LogException(System.Exception message, Object context) {}

	public static void DrawLine (Vector3 start, Vector3 end, Color color, float duration, bool depthTest){}
	public static void DrawLine (Vector3 start, Vector3 end, Color color, float duration){}
	public static void DrawLine (Vector3 start, Vector3 end, Color color){}
	public static void DrawLine (Vector3 start, Vector3 end){}
	public static void DrawRay (Vector3 start, Vector3 dir, Color color, float duration){}
	public static void DrawRay (Vector3 start, Vector3 dir, Color color){}
	public static void DrawRay (Vector3 start, Vector3 dir){}
	public static void DrawRay (Vector3 start, Vector3 dir, Color color, float duration, bool depthTest){}

	public static void Break() {}
	public static void DebugBreak() {}
}
#endif