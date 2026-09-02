using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace UGSAnalytics;

public static class DataCollection
{
	private static bool _debugMode;

	public static async void Initialize(bool isDebug, bool isProduction)
	{
		_debugMode = isDebug;
		InitializationOptions initializationOptions = new InitializationOptions();
		if (isProduction)
		{
			initializationOptions.SetEnvironmentName("production");
		}
		else
		{
			initializationOptions.SetEnvironmentName("development");
		}
		await UnityServices.InitializeAsync(initializationOptions);
	}

	public static void SendAccept()
	{
	}

	public static void SendDecline()
	{
	}

	public static void SendCustomEvent(string customEventName)
	{
	}

	public static void SendCustomEvent(string customEventName, Dictionary<string, object> param)
	{
	}

	private static void Logger(string message)
	{
		if (_debugMode)
		{
			Debug.Log("UGS Analytics : " + message.ToString() + ".");
		}
	}

	private static void Logger(string message, Dictionary<string, object> param)
	{
		if (!_debugMode)
		{
			return;
		}
		Debug.Log("UGS Analytics : " + message.ToString() + ".");
		string text = "Sent data :";
		foreach (KeyValuePair<string, object> item in param)
		{
			text = text + " " + item.Key + ":" + item.Value.ToString() + ".";
		}
		Debug.Log(text);
	}
}
