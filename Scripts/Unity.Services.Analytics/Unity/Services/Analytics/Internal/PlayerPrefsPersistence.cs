using UnityEngine;

namespace Unity.Services.Analytics.Internal;

internal class PlayerPrefsPersistence : IPersistence
{
	public void SaveValue(string key, int value)
	{
		PlayerPrefs.SetInt(key, value);
		PlayerPrefs.Save();
	}

	public void SaveValue(string key, string value)
	{
		PlayerPrefs.SetString(key, value);
		PlayerPrefs.Save();
	}

	public int LoadInt(string key)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefs.GetInt(key);
		}
		return 0;
	}

	public string LoadString(string key)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefs.GetString(key);
		}
		return null;
	}

	public void ClearValue(string key)
	{
		PlayerPrefs.DeleteKey(key);
	}
}
