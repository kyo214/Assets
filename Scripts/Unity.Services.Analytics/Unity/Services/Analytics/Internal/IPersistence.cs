namespace Unity.Services.Analytics.Internal;

internal interface IPersistence
{
	void SaveValue(string key, int value);

	void SaveValue(string key, string value);

	int LoadInt(string key);

	string LoadString(string key);

	void ClearValue(string key);
}
