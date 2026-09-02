using System;
using System.Collections.Generic;
using Unity.Services.Analytics.Data;
using Unity.Services.Analytics.Internal;
using Unity.Services.Authentication.Internal;
using Unity.Services.Core;
using Unity.Services.Core.Analytics.Internal;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Device.Internal;
using Unity.Services.Core.Environments.Internal;
using Unity.Services.Core.Internal;
using UnityEngine;

namespace Unity.Services.Analytics;

public static class AnalyticsService
{
	private const string k_CollectUrlPattern = "https://collect.analytics.unity3d.com/api/analytics/collect/v2/projects/{0}/environments/{1}";

	private static AnalyticsServiceInstance m_Instance;

	private static IDispatcherDebug m_DispatcherDebug;

	private static IBufferDebug m_BufferDebug;

	private static Action<string, string, DateTime, byte[]> m_EventRecordedCallback;

	private static Action<HashSet<string>> m_EventsClearingCallback;

	private static Action<byte[]> m_FlushStartedCallback;

	private static Action<int, bool, bool, bool, bool, byte[]> m_FlushCompletedCallback;

	internal static bool IsInitialized => m_Instance != null;

	internal static IServiceDebug ServiceDebug => m_Instance;

	internal static IDispatcherDebug DispatcherDebug => m_DispatcherDebug;

	public static IAnalyticsService Instance
	{
		get
		{
			if (m_Instance == null)
			{
				throw new ServicesInitializationException("The Analytics service has not been initialized. Please initialize Unity Services.");
			}
			return m_Instance;
		}
	}

	internal static void Initialize(CoreRegistry registry)
	{
		ICloudProjectId serviceComponent = registry.GetServiceComponent<ICloudProjectId>();
		IInstallationId serviceComponent2 = registry.GetServiceComponent<IInstallationId>();
		IPlayerId serviceComponent3 = registry.GetServiceComponent<IPlayerId>();
		IEnvironments serviceComponent4 = registry.GetServiceComponent<IEnvironments>();
		IExternalUserId serviceComponent5 = registry.GetServiceComponent<IExternalUserId>();
		CoreStatsHelper coreStatsHelper = new CoreStatsHelper();
		PlayerPrefsPersistence persistence = new PlayerPrefsPersistence();
		IdentityManager userIdentity = new IdentityManager(serviceComponent2, serviceComponent3, serviceComponent5, persistence);
		SessionManager session = new SessionManager();
		string text = serviceComponent?.GetCloudProjectId() ?? Application.cloudProjectId;
		string collectUrl = $"https://collect.analytics.unity3d.com/api/analytics/collect/v2/projects/{text}/environments/{serviceComponent4.Current.ToLowerInvariant()}";
		BufferX bufferX = new BufferX(new BufferSystemCalls(), new DiskCache(new FileSystemCalls()), userIdentity, session);
		AnalyticsContainer analyticsContainer = AnalyticsContainer.CreateContainer();
		WebRequestHelper webRequestHelper = new WebRequestHelper();
		Dispatcher dispatcher = new Dispatcher(webRequestHelper, collectUrl);
		m_Instance = new AnalyticsServiceInstance(new DataGenerator(bufferX, new CommonDataWrapper(text), new DeviceDataWrapper()), bufferX, coreStatsHelper, dispatcher, new AnalyticsForgetter(collectUrl, persistence, webRequestHelper), userIdentity, serviceComponent4.Current, new AnalyticsServiceSystemCalls(), analyticsContainer, session);
		analyticsContainer.Initialize(m_Instance);
		m_Instance.ResumeDataDeletionIfNecessary();
		m_DispatcherDebug = dispatcher;
		m_BufferDebug = bufferX;
		if (m_EventRecordedCallback != null)
		{
			m_BufferDebug.EventRecorded += m_EventRecordedCallback;
			m_BufferDebug.EventsClearing += m_EventsClearingCallback;
			m_DispatcherDebug.FlushStarted += m_FlushStartedCallback;
			m_DispatcherDebug.FlushFinished += m_FlushCompletedCallback;
		}
		StandardEventServiceComponent component = new StandardEventServiceComponent(registry.GetServiceComponent<IProjectConfiguration>(), m_Instance);
		registry.RegisterServiceComponent((IAnalyticsStandardEventComponent)component);
		AnalyticsUserIdServiceComponent component2 = new AnalyticsUserIdServiceComponent(m_Instance);
		registry.RegisterServiceComponent((IAnalyticsUserId)component2);
	}

	internal static void SubscribeDebugEvents(Action<string, string, DateTime, byte[]> eventRecordedCallback, Action<HashSet<string>> eventsUploadingCallback, Action<byte[]> flushStarted, Action<int, bool, bool, bool, bool, byte[]> flushCompleted)
	{
		m_EventRecordedCallback = eventRecordedCallback;
		m_EventsClearingCallback = eventsUploadingCallback;
		m_FlushStartedCallback = flushStarted;
		m_FlushCompletedCallback = flushCompleted;
		if (m_BufferDebug != null)
		{
			m_BufferDebug.EventRecorded += m_EventRecordedCallback;
			m_BufferDebug.EventsClearing += m_EventsClearingCallback;
			m_DispatcherDebug.FlushStarted += m_FlushStartedCallback;
			m_DispatcherDebug.FlushFinished += m_FlushCompletedCallback;
		}
	}

	internal static void UnsubscribeDebugEvents()
	{
		if (m_BufferDebug != null)
		{
			m_BufferDebug.EventRecorded -= m_EventRecordedCallback;
			m_BufferDebug.EventsClearing -= m_EventsClearingCallback;
			m_DispatcherDebug.FlushStarted -= m_FlushStartedCallback;
			m_DispatcherDebug.FlushFinished -= m_FlushCompletedCallback;
		}
		m_EventRecordedCallback = null;
		m_EventsClearingCallback = null;
	}

	internal static void TearDown()
	{
		if (m_BufferDebug != null && m_EventRecordedCallback != null)
		{
			m_BufferDebug.EventRecorded -= m_EventRecordedCallback;
			m_BufferDebug.EventsClearing -= m_EventsClearingCallback;
			m_DispatcherDebug.FlushStarted -= m_FlushStartedCallback;
			m_DispatcherDebug.FlushFinished -= m_FlushCompletedCallback;
		}
		m_Instance = null;
		m_DispatcherDebug = null;
		m_BufferDebug = null;
	}
}
