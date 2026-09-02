using System;
using System.Collections.Generic;
using Toked;
using UnityEngine;
using _Modules.Achievement.Scripts;
using _Modules.Generator;

namespace _Modules.Data.Scripts;

[Serializable]
public class UserSaveData
{
	[SerializeField]
	private string _userUniqueId;

	[SerializeField]
	private string _userName;

	[ES3Serializable]
	private Dictionary<string, GameStatisticSaveData> _gameStatisticSaveDataDictionary = new Dictionary<string, GameStatisticSaveData>();

	[ES3Serializable]
	private Dictionary<string, AchievementSaveData> _achievementSaveDataDictionary = new Dictionary<string, AchievementSaveData>();

	[ES3Serializable]
	public Dictionary<string, GameStatisticSaveData> GameStatisticSaveDataDictionary
	{
		get
		{
			return _gameStatisticSaveDataDictionary ?? (_gameStatisticSaveDataDictionary = new Dictionary<string, GameStatisticSaveData>());
		}
		set
		{
			_gameStatisticSaveDataDictionary = value;
		}
	}

	[ES3Serializable]
	public Dictionary<string, AchievementSaveData> AchievementSaveDataDictionary
	{
		get
		{
			return _achievementSaveDataDictionary ?? (_achievementSaveDataDictionary = new Dictionary<string, AchievementSaveData>());
		}
		set
		{
			_achievementSaveDataDictionary = value;
		}
	}

	public string UserUniqueId
	{
		get
		{
			return _userUniqueId;
		}
		set
		{
			_userUniqueId = value;
		}
	}

	public string UserName
	{
		get
		{
			return _userName;
		}
		set
		{
			_userName = value;
		}
	}

	public UserSaveData()
	{
		UserUniqueId = (SteamManager.Initialized ? SteamApi.GetAccountId() : RandomUniqueIdGenerator.GenerateID());
		UserName = "";
		_gameStatisticSaveDataDictionary = new Dictionary<string, GameStatisticSaveData>();
		_achievementSaveDataDictionary = new Dictionary<string, AchievementSaveData>();
	}

	public void AddGameStatisticProgress(string key, float val)
	{
		if (_gameStatisticSaveDataDictionary.TryGetValue(key, out var value))
		{
			value.Value += val;
		}
		else
		{
			_gameStatisticSaveDataDictionary.Add(key, new GameStatisticSaveData(key, val));
		}
		AchievementManager.AddProgress(key, val);
	}

	public float GetGameStatisticProgress(string key)
	{
		if (_gameStatisticSaveDataDictionary.TryGetValue(key, out var value))
		{
			return value.Value;
		}
		return 0f;
	}

	public bool GetAchievementStatusCompleted(string key)
	{
		if (_achievementSaveDataDictionary.TryGetValue(key, out var value))
		{
			return value.Completed;
		}
		return false;
	}
}
