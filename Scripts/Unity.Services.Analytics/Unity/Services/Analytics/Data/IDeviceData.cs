namespace Unity.Services.Analytics.Data;

internal interface IDeviceData
{
	string CpuType { get; }

	string GpuType { get; }

	int CpuCores { get; }

	int RamTotal { get; }

	int ScreenWidth { get; }

	int ScreenHeight { get; }

	float ScreenDpi { get; }

	string OperatingSystem { get; }

	string DeviceModel { get; }

	bool IsDebugDevice { get; }

	bool IsTiny { get; }
}
