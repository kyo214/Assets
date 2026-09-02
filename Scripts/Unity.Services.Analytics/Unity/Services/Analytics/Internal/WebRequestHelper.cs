using System;
using UnityEngine.Networking;

namespace Unity.Services.Analytics.Internal;

internal class WebRequestHelper : IWebRequestHelper
{
	private readonly string k_ClientIdHeaderValue = "com.unity.services.analytics@" + SdkVersion.SDK_VERSION;

	public IWebRequest CreateWebRequest(string url, string method, byte[] postBytes)
	{
		AnalyticsWebRequest analyticsWebRequest = new AnalyticsWebRequest(url, method);
		UploadHandlerRaw uploadHandler = new UploadHandlerRaw(postBytes)
		{
			contentType = "application/json"
		};
		analyticsWebRequest.uploadHandler = uploadHandler;
		analyticsWebRequest.SetRequestHeader("x-client-id", k_ClientIdHeaderValue);
		return analyticsWebRequest;
	}

	public void SendWebRequest(IWebRequest request, Action<long> onCompleted)
	{
		UnityWebRequestAsyncOperation requestOp = request.SendWebRequest();
		requestOp.completed += delegate
		{
			onCompleted(requestOp.webRequest.responseCode);
		};
	}
}
