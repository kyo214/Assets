using System;
using UnityEngine.Networking;

namespace Unity.Services.Analytics.Internal;

internal class AnalyticsWebRequest : UnityWebRequest, IWebRequest, IDisposable
{
	public bool IsNetworkError => base.result == Result.ConnectionError;

	UploadHandler IWebRequest.uploadHandler
	{
		get
		{
			return base.uploadHandler;
		}
		set
		{
			base.uploadHandler = value;
		}
	}

	internal AnalyticsWebRequest(string url, string method)
		: base(url, method)
	{
	}

	UnityWebRequestAsyncOperation IWebRequest.SendWebRequest()
	{
		return SendWebRequest();
	}

	void IWebRequest.SetRequestHeader(string key, string value)
	{
		SetRequestHeader(key, value);
	}
}
