using System;
using UnityEngine.Networking;

namespace Unity.Services.Analytics.Internal;

internal interface IWebRequest : IDisposable
{
	UploadHandler uploadHandler { get; set; }

	bool IsNetworkError { get; }

	UnityWebRequestAsyncOperation SendWebRequest();

	void SetRequestHeader(string key, string value);
}
