using System;
using System.Text;
using UnityEngine;

namespace Unity.Services.Analytics.Internal;

internal class AnalyticsForgetter : IAnalyticsForgetter
{
	private enum DataDeletionStatus
	{
		DataAllowed = 0,
		DeletionInProgress = 1,
		SuccessfullyDeleted = 2
	}

	private const string k_ForgottenStatusKey = "unity.services.analytics.data_deletion_status";

	private readonly string m_CollectUrl;

	private readonly IPersistence m_Persistence;

	private readonly IWebRequestHelper m_WebRequestHelper;

	private Action m_Callback;

	private DataDeletionStatus m_DeletionStatus;

	private IWebRequest m_Request;

	public bool DeletionInProgress => m_DeletionStatus == DataDeletionStatus.DeletionInProgress;

	internal AnalyticsForgetter(string collectUrl, IPersistence persistence, IWebRequestHelper webRequestHelper)
	{
		m_CollectUrl = collectUrl;
		m_Persistence = persistence;
		m_WebRequestHelper = webRequestHelper;
		m_DeletionStatus = (DataDeletionStatus)persistence.LoadInt("unity.services.analytics.data_deletion_status");
	}

	public void ResetDataDeletionStatus()
	{
		SetForgettingStatus(DataDeletionStatus.DataAllowed);
	}

	private void SetForgettingStatus(DataDeletionStatus state)
	{
		m_DeletionStatus = state;
		m_Persistence.SaveValue("unity.services.analytics.data_deletion_status", (int)state);
	}

	public void AttemptToForget(string userId, string installationId, string playerId, string timestamp, string callingMethod, Action successfulUploadCallback)
	{
		if (m_Request == null)
		{
			SetForgettingStatus(DataDeletionStatus.DeletionInProgress);
			m_Callback = successfulUploadCallback;
			string s = "{\"eventList\":[{\"eventName\":\"ddnaForgetMe\",\"userID\":\"" + userId + "\",\"eventUUID\":\"" + Guid.NewGuid().ToString() + "\",\"eventTimestamp\":\"" + timestamp + "\",\"eventVersion\":1,\"unityInstallationID\":\"" + installationId + "\"," + (string.IsNullOrEmpty(playerId) ? "" : ("\"unityPlayerID\":\"" + playerId + "\",")) + "\"eventParams\":{\"clientVersion\":\"" + Application.version + "\",\"sdkMethod\":\"" + callingMethod + "\"}}]}";
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			m_Request = m_WebRequestHelper.CreateWebRequest(m_CollectUrl, "POST", bytes);
			m_Request.SetRequestHeader("PIPL_EXPORT", "true");
			m_Request.SetRequestHeader("PIPL_CONSENT", "true");
			m_WebRequestHelper.SendWebRequest(m_Request, UploadComplete);
		}
	}

	private void UploadComplete(long code)
	{
		bool flag = code >= 200 && code <= 299;
		if (!m_Request.IsNetworkError & flag)
		{
			SetForgettingStatus(DataDeletionStatus.SuccessfullyDeleted);
			m_Callback();
		}
		m_Request.Dispose();
		m_Request = null;
	}
}
