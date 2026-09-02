using UnityEngine;

namespace Lofelt.NiceVibrations;

public static class DeviceCapabilities
{
	private static bool _meetsAdvancedRequirements;

	private static bool _hasAmplitudeControl;

	private static bool _hasFrequencyControl;

	private static bool _hasAmplitudeModulation;

	private static bool _hasFrequencyModulation;

	private static bool _hasEmphasis;

	private static bool _canEmulateEmphasis;

	private static bool _canLoop;

	public static RuntimePlatform platform { get; }

	public static int platformVersion { get; }

	public static bool meetsAdvancedRequirements => _meetsAdvancedRequirements;

	public static bool isVersionSupported { get; }

	public static bool hasAmplitudeControl => _hasAmplitudeControl;

	public static bool hasFrequencyControl => _hasFrequencyControl;

	public static bool hasAmplitudeModulation => _hasAmplitudeModulation;

	public static bool hasFrequencyModulation => _hasFrequencyModulation;

	public static bool hasEmphasis => _hasEmphasis;

	public static bool canEmulateEmphasis => _canEmulateEmphasis;

	public static bool canLoop => _canLoop;

	static DeviceCapabilities()
	{
		platform = Application.platform;
		platformVersion = 0;
		isVersionSupported = false;
	}

	public static void Init()
	{
		_meetsAdvancedRequirements = LofeltHaptics.DeviceMeetsMinimumPlatformRequirements();
	}
}
