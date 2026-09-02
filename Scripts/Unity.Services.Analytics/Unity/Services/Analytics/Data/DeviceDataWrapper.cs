using UnityEngine;

namespace Unity.Services.Analytics.Data;

internal class DeviceDataWrapper : IDeviceData
{
	public string CpuType => SystemInfo.processorType;

	public string GpuType => SystemInfo.graphicsDeviceName;

	public int CpuCores => SystemInfo.processorCount;

	public int RamTotal => SystemInfo.systemMemorySize;

	public int ScreenWidth => Screen.width;

	public int ScreenHeight => Screen.height;

	public float ScreenDpi => Screen.dpi;

	public string OperatingSystem => SystemInfo.operatingSystem;

	public string DeviceModel => SystemInfo.deviceModel;

	public bool IsDebugDevice => false;

	public bool IsTiny => false;
}
