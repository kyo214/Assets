using System;
using System.Collections.Generic;
using Unity.Services.Analytics.Data;
using Unity.Services.Analytics.Internal;
using UnityEngine;

namespace Unity.Services.Analytics;

internal class AnalyticsServiceInstance : IAnalyticsService, IUnstructuredEventRecorder, IServiceDebug
{
	private const string k_ForgetCallingId = "com.unity.services.analytics.Events.RequestDataDeletion";

	private const string k_StartUpCallingId = "com.unity.services.analytics.Events.Startup";

	private const string k_PlayerChangedCallingId = "com.unity.services.analytics.Events.PlayerChanged";

	internal const string k_InvokedByUserCallingId = "com.unity.services.analytics.Events.UserInvoked";

	private readonly TimeSpan k_BackgroundSessionRefreshPeriod = TimeSpan.FromMinutes(5.0);

	private readonly TransactionCurrencyConverter converter = new TransactionCurrencyConverter();

	private readonly IIdentityManager m_UserIdentity;

	private readonly ISessionManager m_Session;

	private readonly IDataGenerator m_DataGenerator;

	private readonly ICoreStatsHelper m_CoreStatsHelper;

	private readonly IDispatcher m_DataDispatcher;

	private readonly IAnalyticsForgetter m_AnalyticsForgetter;

	private readonly IAnalyticsServiceSystemCalls m_SystemCalls;

	private readonly IAnalyticsContainer m_Container;

	internal IBuffer m_DataBuffer;

	private int m_BufferLengthAtLastGameRunning;

	private DateTime m_ApplicationPauseTime;

	private bool m_IsActive;

	private bool m_StartUpEventsRecorded;

	public string PrivacyUrl => "https://unity.com/legal/game-player-and-app-user-privacy-policy";

	public string SessionID => m_Session.SessionId;

	internal bool Active
	{
		get
		{
			return m_IsActive;
		}
		set
		{
			m_IsActive = value;
		}
	}

	public bool IsActive => m_IsActive;

	public IIdentityManager UserIdentity => m_UserIdentity;

	internal int AutoflushPeriodMultiplier => Mathf.Clamp(1 + m_DataDispatcher.ConsecutiveFailedUploadCount, 1, 8);

	public string GetAnalyticsUserID()
	{
		return m_UserIdentity.UserId;
	}

	internal AnalyticsServiceInstance(IDataGenerator dataGenerator, IBuffer realBuffer, ICoreStatsHelper coreStatsHelper, IDispatcher dispatcher, IAnalyticsForgetter forgetter, IIdentityManager userIdentity, string environment, IAnalyticsServiceSystemCalls systemCalls, IAnalyticsContainer container, ISessionManager session)
	{
		m_DataGenerator = dataGenerator;
		m_SystemCalls = systemCalls;
		m_CoreStatsHelper = coreStatsHelper;
		m_DataDispatcher = dispatcher;
		m_Container = container;
		m_DataBuffer = realBuffer;
		m_DataDispatcher.SetBuffer(realBuffer);
		m_IsActive = false;
		m_StartUpEventsRecorded = false;
		m_AnalyticsForgetter = forgetter;
		m_UserIdentity = userIdentity;
		m_UserIdentity.OnPlayerChanged += PlayerChanged;
		m_Session = session;
	}

	internal void ResumeDataDeletionIfNecessary()
	{
		if (m_AnalyticsForgetter.DeletionInProgress)
		{
			DeactivateWithDataDeletionRequest();
		}
	}

	public void StartDataCollection()
	{
		if (!m_IsActive)
		{
			m_AnalyticsForgetter.ResetDataDeletionStatus();
			m_CoreStatsHelper.SetCoreStatsConsent(userProvidedConsent: true);
			Activate();
		}
	}

	private void Activate()
	{
		if (!m_IsActive)
		{
			m_IsActive = true;
			m_Container.Enable();
			m_DataBuffer.LoadFromDisk();
			m_UserIdentity.Initialize();
			RecordStartupEvents("com.unity.services.analytics.Events.Startup");
			Flush();
		}
	}

	public void StopDataCollection()
	{
		if (m_IsActive)
		{
			m_DataDispatcher.Flush();
			Deactivate();
		}
	}

	internal void DeactivateWithDataDeletionRequest()
	{
		m_DataBuffer.ClearBuffer();
		m_DataBuffer.ClearDiskCache();
		m_Container.Enable();
		m_AnalyticsForgetter.AttemptToForget(m_UserIdentity.UserId, m_UserIdentity.InstallId, m_UserIdentity.PlayerId, BufferX.SerializeDateTime(DateTime.Now), "com.unity.services.analytics.Events.RequestDataDeletion", DataDeletionCompleted);
		Deactivate();
	}

	private void DataDeletionCompleted()
	{
		if (!m_IsActive)
		{
			m_Container.Disable();
		}
	}

	private void Deactivate()
	{
		if (m_IsActive)
		{
			m_IsActive = false;
			if (!m_AnalyticsForgetter.DeletionInProgress)
			{
				m_Container.Disable();
			}
		}
		m_CoreStatsHelper.SetCoreStatsConsent(userProvidedConsent: false);
	}

	private void RecordStartupEvents(string callingId)
	{
		if (!m_StartUpEventsRecorded)
		{
			m_StartUpEventsRecorded = true;
			m_DataGenerator.SdkStartup(callingId);
			m_DataGenerator.ClientDevice(callingId);
			m_DataGenerator.GameStarted(callingId);
			if (m_UserIdentity.IsNewPlayer)
			{
				m_DataGenerator.NewPlayer(callingId);
			}
		}
	}

	private void PlayerChanged()
	{
		if (m_UserIdentity.IsNewPlayer)
		{
			m_Session.StartNewSession();
			m_StartUpEventsRecorded = false;
			if (m_IsActive)
			{
				RecordStartupEvents("com.unity.services.analytics.Events.PlayerChanged");
			}
		}
	}

	internal void ApplicationPaused(bool paused)
	{
		if (paused)
		{
			m_ApplicationPauseTime = m_SystemCalls.UtcNow;
		}
		else if (m_SystemCalls.UtcNow > m_ApplicationPauseTime + k_BackgroundSessionRefreshPeriod)
		{
			m_Session.StartNewSession();
		}
	}

	public void Flush()
	{
		if (m_IsActive)
		{
			m_DataDispatcher.Flush();
		}
		else if (m_AnalyticsForgetter.DeletionInProgress)
		{
			DeactivateWithDataDeletionRequest();
		}
	}

	public void RequestDataDeletion()
	{
		DeactivateWithDataDeletionRequest();
	}

	internal void ApplicationQuit()
	{
		if (m_IsActive)
		{
			m_DataGenerator.GameEnded("com.unity.services.analytics.Events.Shutdown", DataGenerator.SessionEndState.QUIT);
			m_DataBuffer.FlushToDisk();
			Flush();
		}
		AnalyticsService.TearDown();
	}

	internal void RecordGameRunningIfNecessary()
	{
		if (m_IsActive)
		{
			if (m_DataBuffer.Length == 0 || m_DataBuffer.Length == m_BufferLengthAtLastGameRunning)
			{
				m_DataGenerator.GameRunning("com.unity.services.analytics.AnalyticsServiceInstance.RecordGameRunningIfNecessary");
				m_BufferLengthAtLastGameRunning = m_DataBuffer.Length;
			}
			else
			{
				m_BufferLengthAtLastGameRunning = m_DataBuffer.Length;
			}
		}
	}

	public long ConvertCurrencyToMinorUnits(string currencyCode, double value)
	{
		return converter.Convert(currencyCode, value);
	}

	public void CustomData(string eventName)
	{
		CustomData(eventName, null);
	}

	public void CustomData(string eventName, IDictionary<string, object> eventParams)
	{
		CustomData(eventName, eventParams, null, isStandardEvent: false, "AnalyticsServiceInstance.CustomData");
	}

	public void CustomData(string eventName, IDictionary<string, object> eventParams, int? eventVersion, bool isStandardEvent, string callingMethodIdentifier)
	{
		if (!m_IsActive)
		{
			return;
		}
		if (isStandardEvent)
		{
			m_DataBuffer.PushStandardEventStart(eventName, eventVersion.Value);
			m_DataGenerator.PushCommonParams(callingMethodIdentifier);
		}
		else
		{
			m_DataBuffer.PushCustomEventStart(eventName);
		}
		if (eventParams != null)
		{
			foreach (KeyValuePair<string, object> eventParam in eventParams)
			{
				m_DataBuffer.PushObject(eventParam.Key, eventParam.Value);
			}
		}
		m_DataBuffer.PushEndEvent();
	}

	public void RecordEvent(string name)
	{
		if (m_IsActive)
		{
			m_DataGenerator.PushEmptyEvent(name);
		}
	}

	public void RecordEvent(Event e)
	{
		RecordEvent(e, "com.unity.services.analytics.Events.UserInvoked");
	}

	internal void RecordEvent(Event e, string callingMethodIdentifier)
	{
		if (m_IsActive)
		{
			m_DataGenerator.PushEvent(callingMethodIdentifier, e);
		}
	}
}
