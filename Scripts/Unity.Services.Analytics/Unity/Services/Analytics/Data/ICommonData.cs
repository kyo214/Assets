namespace Unity.Services.Analytics.Data;

internal interface ICommonData
{
	string Version { get; }

	string GameBundleId { get; }

	string ProjectId { get; }

	string Platform { get; }

	string BuildGUID { get; }

	string Idfv { get; }

	string GameStoreId { get; }

	bool HasVolume { get; }

	float Volume { get; }

	double BatteryLevel { get; }

	string AnalyticsRegionLanguageCode { get; }
}
