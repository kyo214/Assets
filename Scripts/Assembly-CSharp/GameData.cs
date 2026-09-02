using System;
using System.Collections.Generic;
using NPOI.Util;
using Sirenix.Utilities;
using UnityEngine;
using _Modules.Data.Scripts;

[Serializable]
public class GameData
{
	[SerializeField]
	private int _seed;

	[SerializeField]
	private string _gameVersion;

	[SerializeField]
	private bool _isCompleted;

	[SerializeField]
	private long _firstSaveDateTime;

	[SerializeField]
	private long _lastSaveDateTime;

	[SerializeField]
	private int _lastRoomSessionType;

	[SerializeField]
	private string _sessionName;

	[SerializeField]
	private string _scenarioId;

	[SerializeField]
	private int _difficulty;

	[SerializeField]
	private int _life;

	[SerializeField]
	private bool _resetData;

	[SerializeField]
	private int _currentMission;

	[SerializeField]
	private bool[] _arrMapCleared;

	[SerializeField]
	private bool[] _arrMapLocked;

	[SerializeField]
	private int _maxMission;

	[SerializeField]
	private PlayerSaveData _playerSaveData;

	[SerializeField]
	private List<string> _playerList = new List<string>();

	[SerializeField]
	private List<int> _itemLobbyList = new List<int>();

	[SerializeField]
	private float _totalMissionTime;

	public long FirstSaveDateTime
	{
		get
		{
			return _firstSaveDateTime;
		}
		set
		{
			_firstSaveDateTime = value;
		}
	}

	public string GameVersion
	{
		get
		{
			return _gameVersion;
		}
		set
		{
			_gameVersion = value;
		}
	}

	public int CurrentMission
	{
		get
		{
			return _currentMission;
		}
		set
		{
			_currentMission = value;
		}
	}

	public bool[] ArrMapCleared
	{
		get
		{
			return _arrMapCleared;
		}
		set
		{
			_arrMapCleared = value;
		}
	}

	public bool[] ArrMapLocked
	{
		get
		{
			return _arrMapLocked;
		}
		set
		{
			_arrMapLocked = value;
		}
	}

	public PlayerSaveData PlayerSaveData
	{
		get
		{
			return _playerSaveData;
		}
		set
		{
			_playerSaveData = value;
		}
	}

	public long LastSaveDateTime
	{
		get
		{
			return _lastSaveDateTime;
		}
		set
		{
			_lastSaveDateTime = value;
		}
	}

	public int LastRoomSessionType
	{
		get
		{
			return _lastRoomSessionType;
		}
		set
		{
			_lastRoomSessionType = value;
		}
	}

	public string ScenarioId
	{
		get
		{
			return _scenarioId;
		}
		set
		{
			_scenarioId = value;
		}
	}

	public int Difficulty
	{
		get
		{
			return _difficulty;
		}
		set
		{
			_difficulty = value;
		}
	}

	public int Life
	{
		get
		{
			return _life;
		}
		set
		{
			_life = value;
		}
	}

	public bool ResetData
	{
		get
		{
			return _resetData;
		}
		set
		{
			_resetData = value;
		}
	}

	public int Seed
	{
		get
		{
			return _seed;
		}
		set
		{
			_seed = value;
		}
	}

	public string SessionName
	{
		get
		{
			return _sessionName;
		}
		set
		{
			_sessionName = value;
		}
	}

	public List<string> PlayerList
	{
		get
		{
			return _playerList;
		}
		set
		{
			_playerList = value;
		}
	}

	public List<int> ItemLobbyList
	{
		get
		{
			return _itemLobbyList;
		}
		set
		{
			_itemLobbyList = value;
		}
	}

	public int MaxMission
	{
		get
		{
			return _maxMission;
		}
		set
		{
			_maxMission = value;
		}
	}

	public bool IsCompleted
	{
		get
		{
			return _isCompleted;
		}
		set
		{
			_isCompleted = value;
		}
	}

	public float TotalMissionTime
	{
		get
		{
			return _totalMissionTime;
		}
		set
		{
			_totalMissionTime = value;
		}
	}

	public string GetSessionName()
	{
		if (string.IsNullOrWhiteSpace(_sessionName))
		{
			_sessionName = GetServerName();
		}
		return _sessionName;
	}

	public GameData(bool completed = true)
	{
		Reset();
	}

	private void Reset()
	{
		_firstSaveDateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
		_gameVersion = 475.ToString();
		_isCompleted = false;
		_lastSaveDateTime = 0L;
		_seed = GetCurrentSeed();
		_sessionName = GetServerName();
		if (GameModes.Instance != null && GameModes.Instance.isEvent)
		{
			_life = 1;
		}
		else
		{
			_life = 3;
		}
		_difficulty = 1;
		_currentMission = -1;
		_arrMapCleared = new bool[100];
		_arrMapLocked = new bool[100];
		_playerSaveData = new PlayerSaveData();
		SetCurrentSaveDateTime();
		_playerList = new List<string>();
		_totalMissionTime = 0f;
	}

	public void SetGameData(GameData gameData)
	{
		_firstSaveDateTime = gameData.FirstSaveDateTime;
		_seed = gameData.Seed;
		_lastRoomSessionType = gameData.LastRoomSessionType;
		_sessionName = gameData.SessionName;
		_scenarioId = gameData.ScenarioId;
		_difficulty = gameData.Difficulty;
		_life = gameData.Life;
		_resetData = gameData.ResetData;
		_currentMission = gameData.CurrentMission;
		_arrMapCleared = gameData.ArrMapCleared;
		_arrMapLocked = gameData.ArrMapLocked;
		_playerSaveData = gameData.PlayerSaveData.Copy();
		_playerList = new List<string>(gameData.PlayerList);
		SetCurrentSaveDateTime();
		_totalMissionTime = gameData.TotalMissionTime;
	}

	public DateTime GetLastSaveDateTime()
	{
		return DateTimeOffset.FromUnixTimeMilliseconds(_lastSaveDateTime).LocalDateTime;
	}

	public void SetCurrentSaveDateTime()
	{
		_lastSaveDateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
	}

	public void SetData(PlayerController playerController, GameManagerPhoton gameManagerPhoton)
	{
		if ((bool)gameManagerPhoton)
		{
			_resetData = gameManagerPhoton.Life <= 0;
			_difficulty = gameManagerPhoton.Difficulty;
			_scenarioId = gameManagerPhoton.ScenarioId;
			_sessionName = gameManagerPhoton.ServerName;
			_seed = gameManagerPhoton.Seed;
			_life = gameManagerPhoton.Life;
			_currentMission = gameManagerPhoton.Mission;
			_arrMapCleared = gameManagerPhoton.ArrMissionCleared.ToArray();
			_arrMapLocked = gameManagerPhoton.ArrMissionLocked.ToArray();
			_playerSaveData.SetPlayerSaveData(playerController);
			_itemLobbyList = new List<int>(gameManagerPhoton.ListItemUIDLobbyPickedUp);
			if ((bool)MissionLobbyManager.Instance)
			{
				_maxMission = MissionLobbyManager.Instance.MissionBoard.AllMissionSelection.Count;
			}
			SetPlayerListData();
			_totalMissionTime = gameManagerPhoton.TotalMissionTime;
		}
		if ((bool)NetworkGameManager.Instance)
		{
			_lastRoomSessionType = ((!NetworkGameManager.Instance.isPrivateRoom) ? 1 : 0);
		}
	}

	public void SetPlayerData(PlayerController playerController)
	{
		_playerSaveData.SetPlayerSaveData(playerController);
	}

	public void LoadData(PlayerController playerController, GameManagerPhoton gameManagerPhoton)
	{
		GlobalOptionsManager.Instance.seed = _seed;
		if (NetworkGameManager.Instance.isServer)
		{
			gameManagerPhoton.ScenarioId = _scenarioId;
			gameManagerPhoton.Difficulty = _difficulty;
			gameManagerPhoton.Seed = _seed;
			gameManagerPhoton.ServerName = _sessionName;
			gameManagerPhoton.PlayerList = GetPlayerList();
			gameManagerPhoton.Life = _life;
			gameManagerPhoton.SetMissionLockedData(_arrMapLocked);
			gameManagerPhoton.SetMissionClearData(_arrMapCleared);
			gameManagerPhoton.Mission = (byte)_currentMission;
			gameManagerPhoton.ListItemUIDLobbyPickedUp = new List<int>(_itemLobbyList);
			if (NetworkGameManager.Instance.isServer && !_playerSaveData.PerkId.IsNullOrWhitespace())
			{
				gameManagerPhoton.isLoadMap = true;
			}
			gameManagerPhoton.TotalMissionTime = _totalMissionTime;
		}
		_playerSaveData.LoadPlayerSaveData(playerController);
	}

	private void SetPlayerListData()
	{
		if (!NetworkGameManager.Instance.isServer)
		{
			return;
		}
		List<string> list = new List<string>(_playerList ?? new List<string>());
		if (NetworkGameManager.Instance?.arrPlayerNetworkController != null)
		{
			foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
			{
				if (!(item == null))
				{
					string userUniqueID = item.network.GetUserUniqueID();
					if (!list.Contains(userUniqueID))
					{
						list.Add(userUniqueID);
					}
				}
			}
		}
		_playerList = list;
	}

	public int GetCurrentSeed()
	{
		if (_seed > 0)
		{
			return _seed;
		}
		GlobalOptionsManager instance = GlobalOptionsManager.Instance;
		if ((object)instance != null && instance.seed > 0)
		{
			return GlobalOptionsManager.Instance.seed;
		}
		if ((bool)GlobalOptionsManager.Instance)
		{
			return (GlobalSaveData.instance.optionData.lastSeed > 0) ? GlobalSaveData.instance.optionData.lastSeed : int.Parse(DateTime.Now.ToString("ddHHmmss"));
		}
		return int.Parse(DateTime.Now.ToString("ddHHmmss"));
	}

	private string GetServerName()
	{
		return GlobalSaveData.instance?.currentSelectedDataIndex + "_" + GlobalSaveData.instance?.UserSaveData?.UserUniqueId + "_" + GetUniqueId();
	}

	public string GetPlayerList()
	{
		return string.Join(",", PlayerList.ToArray());
	}

	public int GetTotalMissionsCleared()
	{
		int num = 0;
		bool[] arrMapCleared = _arrMapCleared;
		for (int i = 0; i < arrMapCleared.Length; i++)
		{
			if (arrMapCleared[i])
			{
				num++;
			}
		}
		return num;
	}

	public void SetGameCompleted(bool isCompleted = true)
	{
		_isCompleted = isCompleted;
	}

	private string GetUniqueId(int n = 8)
	{
		string text = _firstSaveDateTime.ToString();
		if (string.IsNullOrEmpty(text) || n <= 0)
		{
			return string.Empty;
		}
		if (n > text.Length)
		{
			n = text.Length;
		}
		return text.Substring(text.Length - n);
	}

	public bool CheckVersionCompability()
	{
		return SaveDataVersionValidator.CheckVersionCompability(_gameVersion);
	}
}
