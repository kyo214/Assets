namespace Unity.Services.Analytics;

internal interface IAnalyticsContainer
{
	void Initialize(AnalyticsServiceInstance service);

	void Enable();

	void Disable();
}
