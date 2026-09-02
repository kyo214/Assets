using UnityEngine;

namespace Doozy.Runtime.Common.Utils;

public class UnityDebug : ILogger
{
	public void Log(object message)
	{
		Debug.Log(message);
	}

	public void Log(object message, Object context)
	{
		Debug.Log(message, context);
	}

	public void LogWarning(object message)
	{
		Debug.LogWarning(message);
	}

	public void LogWarning(object message, Object context)
	{
		Debug.Log(message, context);
	}

	public void LogError(object message)
	{
		Debug.Log(message);
	}

	public void LogError(object message, Object context)
	{
		Debug.Log(message, context);
	}
}
