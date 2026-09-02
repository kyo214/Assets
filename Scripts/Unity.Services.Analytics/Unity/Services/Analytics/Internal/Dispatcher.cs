using System;
using UnityEngine;

namespace Unity.Services.Analytics.Internal;

internal class Dispatcher : IDispatcher, IDispatcherDebug
{
	private readonly IWebRequestHelper m_WebRequestHelper;

	private readonly string m_CollectUrl;

	internal const string k_PiplConsentHeaderKey = "PIPL_CONSENT";

	internal const string k_PiplExportHeaderKey = "PIPL_EXPORT";

	internal const string k_HeaderTrueValue = "true";

	private IBuffer m_DataBuffer;

	private IWebRequest m_FlushRequest;

	private byte[] m_LastFlushPayload;

	private int m_FlushBufferIndex;

	public int ConsecutiveFailedUploadCount { get; private set; }

	public bool FlushInProgress { get; private set; }

	public event Action<byte[]> FlushStarted;

	public event Action<int, bool, bool, bool, bool, byte[]> FlushFinished;

	public Dispatcher(IWebRequestHelper webRequestHelper, string collectUrl)
	{
		m_WebRequestHelper = webRequestHelper;
		m_CollectUrl = collectUrl;
	}

	public void SetBuffer(IBuffer buffer)
	{
		m_DataBuffer = buffer;
	}

	public void Flush()
	{
		if (FlushInProgress)
		{
			Debug.LogWarning("Analytics Dispatcher is already flushing.");
		}
		else
		{
			FlushBufferToService();
		}
	}

	private void FlushBufferToService()
	{
		FlushInProgress = true;
		byte[] array = m_DataBuffer.Serialize();
		m_FlushBufferIndex = m_DataBuffer.Length;
		m_LastFlushPayload = array;
		if (array == null || array.Length == 0)
		{
			FlushInProgress = false;
			m_FlushBufferIndex = 0;
			return;
		}
		m_FlushRequest = m_WebRequestHelper.CreateWebRequest(m_CollectUrl, "POST", array);
		m_FlushRequest.SetRequestHeader("PIPL_EXPORT", "true");
		m_FlushRequest.SetRequestHeader("PIPL_CONSENT", "true");
		m_WebRequestHelper.SendWebRequest(m_FlushRequest, UploadCompleted);
		if (FlushStarted != null)
		{
			FlushStarted(array);
		}
	}

	private void UploadCompleted(long responseCode)
	{
		bool flag = responseCode >= 200 && responseCode <= 299;
		bool flag2 = responseCode >= 400 && responseCode <= 499;
		bool flag3 = (responseCode >= 500 && responseCode <= 599) || m_FlushRequest.IsNetworkError;
		if (FlushFinished != null)
		{
			FlushFinished((int)responseCode, flag, flag2, flag3, m_FlushRequest.IsNetworkError, m_LastFlushPayload);
		}
		if (flag | flag2)
		{
			ConsecutiveFailedUploadCount = 0;
			m_DataBuffer.ClearBuffer(m_FlushBufferIndex);
			m_DataBuffer.ClearDiskCache();
		}
		else if (flag3)
		{
			ConsecutiveFailedUploadCount++;
			m_DataBuffer.FlushToDisk();
		}
		FlushInProgress = false;
		m_FlushBufferIndex = 0;
		m_FlushRequest.Dispose();
		m_FlushRequest = null;
	}
}
