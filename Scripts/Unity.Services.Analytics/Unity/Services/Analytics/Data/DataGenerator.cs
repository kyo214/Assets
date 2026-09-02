using Unity.Services.Analytics.Internal;

namespace Unity.Services.Analytics.Data;

internal class DataGenerator : IDataGenerator
{
	internal enum SessionEndState
	{
		PAUSED = 0,
		KILLEDINBACKGROUND = 1,
		KILLEDINFOREGROUND = 2,
		QUIT = 3
	}

	private readonly IBuffer m_Buffer;

	private readonly ICommonData m_CommonData;

	private readonly IDeviceData m_DeviceData;

	public DataGenerator(IBuffer buffer, ICommonData staticData, IDeviceData deviceData)
	{
		m_Buffer = buffer;
		m_CommonData = staticData;
		m_DeviceData = deviceData;
	}

	public void SdkStartup(string callingMethodIdentifier)
	{
		m_Buffer.PushStandardEventStart("sdkStart", 1);
		m_Buffer.PushString("sdkVersion", SdkVersion.SDK_VERSION);
		PushCommonParams(callingMethodIdentifier);
		m_Buffer.PushString("sdkName", "com.unity.services.analytics");
		m_Buffer.PushEndEvent();
	}

	public void GameRunning(string callingMethodIdentifier)
	{
		m_Buffer.PushStandardEventStart("gameRunning", 1);
		PushCommonParams(callingMethodIdentifier);
		m_Buffer.PushEndEvent();
	}

	public void NewPlayer(string callingMethodIdentifier)
	{
		m_Buffer.PushStandardEventStart("newPlayer", 1);
		PushCommonParams(callingMethodIdentifier);
		m_Buffer.PushString("deviceModel", m_DeviceData.DeviceModel);
		m_Buffer.PushEndEvent();
	}

	public void GameStarted(string callingMethodIdentifier)
	{
		m_Buffer.PushStandardEventStart("gameStarted", 1);
		PushCommonParams(callingMethodIdentifier);
		m_Buffer.PushString("userLocale", m_CommonData.AnalyticsRegionLanguageCode);
		if (!string.IsNullOrEmpty(m_CommonData.BuildGUID))
		{
			m_Buffer.PushString("idLocalProject", m_CommonData.BuildGUID);
		}
		m_Buffer.PushString("osVersion", m_DeviceData.OperatingSystem);
		m_Buffer.PushBool("isTiny", m_DeviceData.IsTiny);
		m_Buffer.PushBool("debugDevice", m_DeviceData.IsDebugDevice);
		m_Buffer.PushEndEvent();
	}

	public void GameEnded(string callingMethodIdentifier, SessionEndState quitState)
	{
		m_Buffer.PushStandardEventStart("gameEnded", 1);
		PushCommonParams(callingMethodIdentifier);
		m_Buffer.PushString("sessionEndState", quitState.ToString());
		m_Buffer.PushEndEvent();
	}

	public void ClientDevice(string callingMethodIdentifier)
	{
		m_Buffer.PushStandardEventStart("clientDevice", 1);
		PushCommonParams(callingMethodIdentifier);
		m_Buffer.PushString("cpuType", m_DeviceData.CpuType);
		m_Buffer.PushString("gpuType", m_DeviceData.GpuType);
		m_Buffer.PushInt64("cpuCores", m_DeviceData.CpuCores);
		m_Buffer.PushInt64("ramTotal", m_DeviceData.RamTotal);
		m_Buffer.PushInt64("screenWidth", m_DeviceData.ScreenWidth);
		m_Buffer.PushInt64("screenHeight", m_DeviceData.ScreenHeight);
		m_Buffer.PushInt64("screenResolution", (int)m_DeviceData.ScreenDpi);
		m_Buffer.PushEndEvent();
	}

	public void PushCommonParams(string callingMethodIdentifier)
	{
		m_Buffer.PushString("sdkMethod", callingMethodIdentifier);
		m_Buffer.PushString("clientVersion", m_CommonData.Version);
		m_Buffer.PushDouble("batteryLoad", m_CommonData.BatteryLevel);
		m_Buffer.PushString("platform", m_CommonData.Platform);
		if (!string.IsNullOrEmpty(m_CommonData.GameStoreId))
		{
			m_Buffer.PushString("gameStoreID", m_CommonData.GameStoreId);
		}
		if (!string.IsNullOrEmpty(m_CommonData.GameBundleId))
		{
			m_Buffer.PushString("gameBundleID", m_CommonData.GameBundleId);
		}
		if (!string.IsNullOrEmpty(m_CommonData.Idfv))
		{
			m_Buffer.PushString("idfv", m_CommonData.Idfv);
		}
		if (!string.IsNullOrEmpty(m_CommonData.BuildGUID))
		{
			m_Buffer.PushString("buildGUUID", m_CommonData.BuildGUID);
		}
		if (m_CommonData.HasVolume)
		{
			m_Buffer.PushDouble("deviceVolume", m_CommonData.Volume);
		}
		if (!string.IsNullOrEmpty(m_CommonData.ProjectId))
		{
			m_Buffer.PushString("projectID", m_CommonData.ProjectId);
		}
	}

	public void PushEvent(string callingMethodIdentifier, Event e)
	{
		e.Validate();
		if (e.StandardEvent)
		{
			m_Buffer.PushStandardEventStart(e.Name, e.EventVersion);
			PushCommonParams(callingMethodIdentifier);
		}
		else
		{
			m_Buffer.PushCustomEventStart(e.Name);
		}
		e.Serialize(m_Buffer);
		e.Reset();
		m_Buffer.PushEndEvent();
	}

	public void PushEmptyEvent(string name)
	{
		m_Buffer.PushCustomEventStart(name);
		m_Buffer.PushEndEvent();
	}
}
