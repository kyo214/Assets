using Unity.Services.Analytics.Internal;
using Unity.Services.Analytics.Platform;
using UnityEngine;

namespace Unity.Services.Analytics.Data;

internal class CommonDataWrapper : ICommonData
{
	public string Version { get; }

	public string GameBundleId { get; }

	public string ProjectId { get; }

	public string Platform { get; }

	public string BuildGUID { get; }

	public string Idfv { get; }

	public string GameStoreId { get; }

	public bool HasVolume { get; }

	public float Volume => DeviceVolumeProvider.GetDeviceVolume();

	public double BatteryLevel => SystemInfo.batteryLevel;

	public string AnalyticsRegionLanguageCode => Locale.AnalyticsRegionLanguageCode();

	public CommonDataWrapper(string cloudProjectId)
	{
		Version = Application.version;
		ProjectId = cloudProjectId;
		GameBundleId = Application.identifier;
		Platform = GetPlatform();
		BuildGUID = Application.buildGUID;
		Idfv = SystemInfo.deviceUniqueIdentifier;
		GameStoreId = null;
		HasVolume = DeviceVolumeProvider.VolumeAvailable;
	}

	private static string GetPlatform()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.OSXPlayer:
			return "MAC_CLIENT";
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor:
		case RuntimePlatform.LinuxPlayer:
		case RuntimePlatform.LinuxEditor:
			return "PC_CLIENT";
		case RuntimePlatform.IPhonePlayer:
			return "IOS";
		case RuntimePlatform.Android:
			return "ANDROID";
		case RuntimePlatform.WebGLPlayer:
			return "WEB";
		case RuntimePlatform.MetroPlayerX86:
		case RuntimePlatform.MetroPlayerX64:
		case RuntimePlatform.MetroPlayerARM:
			if (SystemInfo.deviceType != DeviceType.Handheld)
			{
				return "PC_CLIENT";
			}
			return "WINDOWS_MOBILE";
		case RuntimePlatform.PS4:
			return "PS4";
		case RuntimePlatform.XboxOne:
			return "XBOXONE";
		case RuntimePlatform.tvOS:
			return "IOS_TV";
		case RuntimePlatform.Switch:
			return "SWITCH";
		default:
			return "UNKNOWN";
		}
	}
}
