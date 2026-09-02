namespace Unity.Services.Analytics;

public interface IAnalyticsService
{
	string PrivacyUrl { get; }

	string SessionID { get; }

	void StartDataCollection();

	string GetAnalyticsUserID();

	long ConvertCurrencyToMinorUnits(string currencyCode, double value);

	void RecordEvent(Event e);

	void RecordEvent(string eventName);

	void Flush();

	void StopDataCollection();

	void RequestDataDeletion();
}
