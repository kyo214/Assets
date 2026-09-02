using System;
using Doozy.Runtime.Common.Utils;
using UnityEngine;

namespace Doozy.Runtime.Common;

public static class Debugger
{
	public enum LogType
	{
		Assert = 0,
		Error = 1,
		Warning = 2,
		Log = 3,
		Exception = 4
	}

	private static ILogger s_logger;

	private const string ERROR_COLOR_CODE = "#D9534F";

	private const string INFO_COLOR_CODE = "#1C7CD5";

	private const string OK_COLOR_CODE = "#5CB85C";

	private const string WARNING_COLOR_CODE = "#EE9800";

	private static ILogger loggingSolution => new UnityDebug();

	private static ILogger logger => s_logger ?? (s_logger = loggingSolution);

	private static string DoozyPrefix(LogType logType)
	{
		string text = "#121212";
		return "<color=" + logType switch
		{
			LogType.Log => "#1C7CD5", 
			LogType.Warning => "#EE9800", 
			LogType.Error => "#D9534F", 
			LogType.Exception => "#D9534F", 
			LogType.Assert => "#5CB85C", 
			_ => throw new ArgumentOutOfRangeException("logType", logType, null), 
		} + "><b>DOOZY ››› </b></color>";
	}

	public static void Log(object message, UnityEngine.Object context = null)
	{
		message = DoozyPrefix(LogType.Log) + message;
		logger.Log(message, context);
	}

	public static void LogWarning(object message, UnityEngine.Object context = null)
	{
		message = DoozyPrefix(LogType.Warning) + message;
		logger.LogWarning(message, context);
	}

	public static void LogError(object message, UnityEngine.Object context = null)
	{
		message = DoozyPrefix(LogType.Error) + message;
		logger.LogError(message, context);
	}
}
