namespace Unity.Services.Analytics.Platform;

internal static class DeviceVolumeProvider
{
	internal static bool VolumeAvailable => false;

	internal static float GetDeviceVolume()
	{
		return 0f;
	}
}
