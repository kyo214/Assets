using Unity.Services.Analytics.Internal;
using UnityEngine;

namespace Unity.Services.Analytics;

internal class AnalyticsContainer : MonoBehaviour, IAnalyticsContainer, IContainerDebug
{
	private const float k_AutoFlushPeriod = 60f;

	private const float k_GameRunningPeriod = 60f;

	private static bool s_Created;

	private static GameObject s_Container;

	private static AnalyticsContainer m_Instance;

	private float m_AutoFlushTime;

	private float m_GameRunningTime;

	private AnalyticsServiceInstance m_Service;

	private float AutoFlushPeriod => 60f * (float)m_Service.AutoflushPeriodMultiplier;

	internal static IContainerDebug ContainerDebug => m_Instance;

	public float TimeUntilNextHeartbeat => AutoFlushPeriod - m_AutoFlushTime;

	internal static AnalyticsContainer CreateContainer()
	{
		if (!s_Created)
		{
			s_Container = new GameObject("AnalyticsContainer");
			m_Instance = s_Container.AddComponent<AnalyticsContainer>();
			s_Container.hideFlags = HideFlags.NotEditable | HideFlags.DontSaveInBuild;
			s_Container.hideFlags |= HideFlags.HideInInspector;
			Object.DontDestroyOnLoad(s_Container);
			s_Created = true;
			Application.quitting += m_Instance.CleanUp;
		}
		return m_Instance;
	}

	public void Initialize(AnalyticsServiceInstance service)
	{
		m_Service = service;
		base.enabled = false;
	}

	public void Enable()
	{
		base.enabled = true;
	}

	public void Disable()
	{
		base.enabled = false;
		m_AutoFlushTime = 0f;
	}

	private void Update()
	{
		m_GameRunningTime += Time.unscaledDeltaTime;
		if (m_GameRunningTime >= 60f)
		{
			m_Service.RecordGameRunningIfNecessary();
			m_GameRunningTime = 0f;
		}
		m_AutoFlushTime += Time.unscaledDeltaTime;
		if (m_AutoFlushTime >= AutoFlushPeriod)
		{
			m_Service.Flush();
			m_AutoFlushTime = 0f;
		}
	}

	private void OnApplicationPause(bool paused)
	{
		m_Service.ApplicationPaused(paused);
	}

	private void CleanUp()
	{
		Application.quitting -= m_Instance.CleanUp;
		m_Service.ApplicationQuit();
		s_Container = null;
		s_Created = false;
	}
}
