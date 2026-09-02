using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using Fusion.CodeGen;
using TMPro;
using Toked;
using Toked.Inventory;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using _Modules.GameSystem.BaseScripts.Difficulty;

[NetworkBehaviourWeaved(644)]
public class GameManagerPhoton : NetworkBehaviour
{
	[SerializeField]
	[DefaultForProperty("ServerName", 0, 102)]
	private string _ServerName;

	[SerializeField]
	[DefaultForProperty("ScenarioId", 102, 102)]
	private string _ScenarioId;

	[SerializeField]
	[DefaultForProperty("Difficulty", 204, 1)]
	private int _Difficulty = 1;

	[SerializeField]
	[DefaultForProperty("PlayerList", 205, 202)]
	private string _PlayerList;

	[SerializeField]
	[DefaultForProperty("IsLoadGame", 407, 1)]
	private bool _IsLoadGame;

	[SerializeField]
	[DefaultForProperty("Seed", 408, 1)]
	private int _Seed;

	[SerializeField]
	[DefaultForProperty("SeedPuzzle", 409, 1)]
	private int _SeedPuzzle;

	[SerializeField]
	[DefaultForProperty("PerkSelectionIndex", 410, 5)]
	private int[] _PerkSelectionIndex;

	[SerializeField]
	[DefaultForProperty("showResult", 415, 1)]
	private bool _showResult;

	[SerializeField]
	[DefaultForProperty("arrPlayerReady", 416, 8)]
	private bool[] _arrPlayerReady;

	[SerializeField]
	[DefaultForProperty("arrObjective", 424, 2)]
	private bool[] _arrObjective;

	[SerializeField]
	[DefaultForProperty("TargetDestroyed", 426, 1)]
	private byte _TargetDestroyed;

	[SerializeField]
	[DefaultForProperty("HostLoadingGameComplete", 427, 1)]
	private NetworkBool _HostLoadingGameComplete;

	[SerializeField]
	[DefaultForProperty("TimerSyncTemp", 428, 1)]
	private short _TimerSyncTemp;

	[SerializeField]
	[DefaultForProperty("ArrMissionCleared", 429, 100)]
	private bool[] _ArrMissionCleared;

	[SerializeField]
	[DefaultForProperty("ArrMissionLocked", 529, 100)]
	private bool[] _ArrMissionLocked;

	[SerializeField]
	[DefaultForProperty("objectiveComplete", 629, 1)]
	private bool _objectiveComplete;

	[SerializeField]
	[DefaultForProperty("Mission", 630, 1)]
	private byte _Mission;

	[SerializeField]
	[DefaultForProperty("Scenario", 631, 1)]
	private byte _Scenario;

	[SerializeField]
	[DefaultForProperty("IsWin", 632, 1)]
	private bool _IsWin;

	[SerializeField]
	[DefaultForProperty("Life", 633, 1)]
	private int _Life = 3;

	[SerializeField]
	[DefaultForProperty("Wave", 634, 1)]
	private byte _Wave;

	[SerializeField]
	[DefaultForProperty("StateLobby", 635, 1)]
	private byte _StateLobby;

	[SerializeField]
	[DefaultForProperty("IsRandomizeMapOnDefeat", 636, 1)]
	private bool _IsRandomizeMapOnDefeat;

	[SerializeField]
	[DefaultForProperty("SpawnIdx", 637, 1)]
	private int _SpawnIdx;

	public const int MAX_LIFE = 3;

	[SerializeField]
	[DefaultForProperty("LobbyId", 638, 2)]
	private ulong _LobbyId;

	[SerializeField]
	[DefaultForProperty("StartTime", 640, 1)]
	private float _StartTime;

	[SerializeField]
	[DefaultForProperty("TotalMissionTime", 641, 1)]
	private float _TotalMissionTime;

	[SerializeField]
	[DefaultForProperty("EndTime", 642, 1)]
	private float _EndTime;

	[SerializeField]
	[DefaultForProperty("Phase", 643, 1)]
	private byte _Phase;

	public List<SO_MissionMap> ListMission = new List<SO_MissionMap>();

	public SO_MissionMap CurrentMission;

	public List<int> ListItemUIDLobbyPickedUp = new List<int>();

	public bool isInitializedLockedMap;

	public bool isInitializedRandomizeWeapon;

	public bool isLoadMap;

	private bool _initPlayerList;

	private List<string> _playerList = new List<string>();

	public SO_MissionMap MissionLastSurvivor;

	public List<ItemSpawn> ListItemSpawnToLobby = new List<ItemSpawn>();

	private static Changed<GameManagerPhoton> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<GameManagerPhoton> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<GameManagerPhoton> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	private string cache_ServerName;

	private string cache_ScenarioId;

	private string cache_PlayerList;

	[Networked]
	[Capacity(100)]
	[NetworkedWeaved(0, 102)]
	public unsafe string ServerName
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.ServerName. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.ReadStringUtf32WithHash((int*)((byte*)Ptr + 0), 100, ref cache_ServerName);
			return cache_ServerName;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.ServerName. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteStringUtf32WithHash((int*)((byte*)Ptr + 0), 100, value, ref cache_ServerName);
		}
	}

	[Networked]
	[Capacity(100)]
	[NetworkedWeaved(102, 102)]
	public unsafe string ScenarioId
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.ScenarioId. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.ReadStringUtf32WithHash(Ptr + 102, 100, ref cache_ScenarioId);
			return cache_ScenarioId;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.ScenarioId. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteStringUtf32WithHash(Ptr + 102, 100, value, ref cache_ScenarioId);
		}
	}

	[Networked]
	[NetworkedWeaved(204, 1)]
	public unsafe int Difficulty
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.Difficulty. Networked properties can only be accessed when Spawned() has been called.");
			}
			return Ptr[204];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.Difficulty. Networked properties can only be accessed when Spawned() has been called.");
			}
			Ptr[204] = value;
		}
	}

	[Networked]
	[Capacity(200)]
	[NetworkedWeaved(205, 202)]
	public unsafe string PlayerList
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.PlayerList. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.ReadStringUtf32WithHash(Ptr + 205, 200, ref cache_PlayerList);
			return cache_PlayerList;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.PlayerList. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteStringUtf32WithHash(Ptr + 205, 200, value, ref cache_PlayerList);
		}
	}

	[Networked]
	[NetworkedWeaved(407, 1)]
	public unsafe bool IsLoadGame
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.IsLoadGame. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 407);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.IsLoadGame. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 407, value);
		}
	}

	[Networked(OnChanged = "OnSeedChanged")]
	[NetworkedWeaved(408, 1)]
	public unsafe int Seed
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.Seed. Networked properties can only be accessed when Spawned() has been called.");
			}
			return Ptr[408];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.Seed. Networked properties can only be accessed when Spawned() has been called.");
			}
			Ptr[408] = value;
		}
	}

	[Networked]
	[NetworkedWeaved(409, 1)]
	public unsafe int SeedPuzzle
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.SeedPuzzle. Networked properties can only be accessed when Spawned() has been called.");
			}
			return Ptr[409];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.SeedPuzzle. Networked properties can only be accessed when Spawned() has been called.");
			}
			Ptr[409] = value;
		}
	}

	[Networked]
	[Capacity(5)]
	[NetworkedWeaved(410, 5)]
	public unsafe NetworkArray<int> PerkSelectionIndex
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.PerkSelectionIndex. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkArray<int>((byte*)Ptr + 1640, 5, ReaderWriter_0040System_Int32.GetInstance());
		}
	}

	public bool InitPerkSelectionIndex { get; set; }

	[Networked(OnChanged = "OnShowResultChanged")]
	[NetworkedWeaved(415, 1)]
	public unsafe bool showResult
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.showResult. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 415);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.showResult. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 415, value);
		}
	}

	[Networked(OnChanged = "OnArrPlayerReadyChanged")]
	[Capacity(8)]
	[NetworkedWeaved(416, 8)]
	public unsafe NetworkArray<bool> arrPlayerReady
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.arrPlayerReady. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkArray<bool>((byte*)Ptr + 1664, 8, ReaderWriter_0040System_Boolean.GetInstance());
		}
	}

	[Networked(OnChanged = "OnObjectiveChanged")]
	[Capacity(2)]
	[NetworkedWeaved(424, 2)]
	public unsafe NetworkArray<bool> arrObjective
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.arrObjective. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkArray<bool>((byte*)Ptr + 1696, 2, ReaderWriter_0040System_Boolean.GetInstance());
		}
	}

	[Networked(OnChanged = "OnTargetDestroyedChanged")]
	[NetworkedWeaved(426, 1)]
	public unsafe byte TargetDestroyed
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.TargetDestroyed. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[1704];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.TargetDestroyed. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[1704] = (sbyte)value;
		}
	}

	[Networked]
	[NetworkedWeaved(427, 1)]
	public unsafe NetworkBool HostLoadingGameComplete
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.HostLoadingGameComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkBool*)(Ptr + 427);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.HostLoadingGameComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkBool*)(Ptr + 427) = value;
		}
	}

	[Networked(OnChanged = "OnSyncTimer")]
	[NetworkedWeaved(428, 1)]
	public unsafe short TimerSyncTemp
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.TimerSyncTemp. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((short*)Ptr)[856];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.TimerSyncTemp. Networked properties can only be accessed when Spawned() has been called.");
			}
			((short*)Ptr)[856] = value;
		}
	}

	[Networked]
	[Capacity(100)]
	[NetworkedWeaved(429, 100)]
	public unsafe NetworkArray<bool> ArrMissionCleared
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.ArrMissionCleared. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkArray<bool>((byte*)Ptr + 1716, 100, ReaderWriter_0040System_Boolean.GetInstance());
		}
	}

	[Networked]
	[Capacity(100)]
	[NetworkedWeaved(529, 100)]
	public unsafe NetworkArray<bool> ArrMissionLocked
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.ArrMissionLocked. Networked properties can only be accessed when Spawned() has been called.");
			}
			return new NetworkArray<bool>((byte*)Ptr + 2116, 100, ReaderWriter_0040System_Boolean.GetInstance());
		}
	}

	[Networked(OnChanged = "OnObjectiveComplete")]
	[NetworkedWeaved(629, 1)]
	public unsafe bool objectiveComplete
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.objectiveComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 629);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.objectiveComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 629, value);
		}
	}

	[Networked(OnChanged = "OnMissionChanged")]
	[NetworkedWeaved(630, 1)]
	public unsafe byte Mission
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.Mission. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[2520];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.Mission. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[2520] = (sbyte)value;
		}
	}

	[Networked]
	[NetworkedWeaved(631, 1)]
	public unsafe byte Scenario
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.Scenario. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[2524];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.Scenario. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[2524] = (sbyte)value;
		}
	}

	[Networked]
	[NetworkedWeaved(632, 1)]
	public unsafe bool IsWin
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.IsWin. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 632);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.IsWin. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 632, value);
		}
	}

	[Networked]
	[NetworkedWeaved(633, 1)]
	public unsafe int Life
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.Life. Networked properties can only be accessed when Spawned() has been called.");
			}
			return Ptr[633];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.Life. Networked properties can only be accessed when Spawned() has been called.");
			}
			Ptr[633] = value;
		}
	}

	[Networked(OnChanged = "OnWaveChanged")]
	[NetworkedWeaved(634, 1)]
	public unsafe byte Wave
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.Wave. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[2536];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.Wave. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[2536] = (sbyte)value;
		}
	}

	[Networked(OnChanged = "OnStateLobbyChanged")]
	[NetworkedWeaved(635, 1)]
	public unsafe byte StateLobby
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.StateLobby. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[2540];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.StateLobby. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[2540] = (sbyte)value;
		}
	}

	[Networked(OnChanged = "OnMapRandomizedChanged")]
	[NetworkedWeaved(636, 1)]
	public unsafe bool IsRandomizeMapOnDefeat
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.IsRandomizeMapOnDefeat. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 636);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.IsRandomizeMapOnDefeat. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 636, value);
		}
	}

	[Networked]
	[NetworkedWeaved(637, 1)]
	public unsafe int SpawnIdx
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.SpawnIdx. Networked properties can only be accessed when Spawned() has been called.");
			}
			return Ptr[637];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.SpawnIdx. Networked properties can only be accessed when Spawned() has been called.");
			}
			Ptr[637] = value;
		}
	}

	[Networked]
	[NetworkedWeaved(638, 2)]
	public unsafe ulong LobbyId
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.LobbyId. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((ulong*)Ptr)[319];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.LobbyId. Networked properties can only be accessed when Spawned() has been called.");
			}
			((long*)Ptr)[319] = (long)value;
		}
	}

	[Networked]
	[NetworkedWeaved(640, 1)]
	public unsafe float StartTime
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.StartTime. Networked properties can only be accessed when Spawned() has been called.");
			}
			return (float)Ptr[640] * 0.001f;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.StartTime. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteFloat(Ptr + 640, 999.99994f, value);
		}
	}

	[Networked]
	[NetworkedWeaved(641, 1)]
	public unsafe float TotalMissionTime
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.TotalMissionTime. Networked properties can only be accessed when Spawned() has been called.");
			}
			return (float)Ptr[641] * 0.001f;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.TotalMissionTime. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteFloat(Ptr + 641, 999.99994f, value);
		}
	}

	[Networked]
	[NetworkedWeaved(642, 1)]
	public unsafe float EndTime
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.EndTime. Networked properties can only be accessed when Spawned() has been called.");
			}
			return (float)Ptr[642] * 0.001f;
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.EndTime. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteFloat(Ptr + 642, 999.99994f, value);
		}
	}

	[Networked]
	[NetworkedWeaved(643, 1)]
	public unsafe byte Phase
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.Phase. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[2572];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GameManagerPhoton.Phase. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[2572] = (sbyte)value;
		}
	}

	public static GameManagerPhoton Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			if (NetworkGameManager.Instance != null && GameManager.Instance != null)
			{
				GameManager.Instance.gameManagerPhoton = Instance;
			}
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		if (NetworkGameManager.Instance != null && GameManager.Instance != null)
		{
			GameManager.Instance.gameManagerPhoton = Instance;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private IEnumerator Start()
	{
		if (NetworkGameManager.Instance.isServer)
		{
			for (int i = 0; i < NetworkGameManager.Instance.arrPlayerNetworkController.Count; i++)
			{
				if ((bool)NetworkGameManager.Instance.arrPlayerNetworkController[i])
				{
					NetworkGameManager.Instance.arrPlayerNetworkController[i].ScorePlayerNetwork.ResetAllScore();
				}
			}
			UniTaskUtil.DelayedCall(this, 3f, () =>
			{
				StartTime = Time.time;
			}).Forget();
		}
		int ctr = 0;
		while (Seed == 0 && ctr < 20)
		{
			ctr++;
			yield return new WaitForSeconds(0.2f);
		}
		RandomizeSeedPuzzle();
		if ((bool)MissionLobbyManager.Instance && !MissionLobbyManager.Instance.initializedMap)
		{
			MissionLobbyManager.Instance.InitMap();
		}
		Time.timeScale = 1f;
	}

	public void Save()
	{
		if ((bool)LobbyManager.Instance)
		{
			ListItemUIDLobbyPickedUp = LobbyManager.Instance.GetLobbyPickUp();
			GlobalSaveData.instance.SaveGameData(NetworkGameManager.Instance.ownPlayer, this);
		}
	}

	private void InitGameModeSetting()
	{
		Difficulty = (int)GameModes.Instance.GetDifficultyData().DifficultySetting;
		ScenarioId = GameModes.Instance.GetScenarioId();
	}

	public bool CheckClientHasSaveData(string playerName)
	{
		if (string.IsNullOrEmpty(PlayerList))
		{
			return false;
		}
		ParseClientPlayerList(PlayerList);
		return _playerList.Contains(playerName);
	}

	private void ParseClientPlayerList(string playerList)
	{
		if (!_initPlayerList)
		{
			string[] collection = playerList.Split(',', StringSplitOptions.RemoveEmptyEntries);
			_playerList = new List<string>(collection);
			_initPlayerList = true;
		}
	}

	public void UpdatePlayerList()
	{
		if (!NetworkGameManager.Instance.isServer)
		{
			return;
		}
		ParseClientPlayerList(PlayerList);
		List<string> list = new List<string>(_playerList ?? new List<string>());
		bool flag = false;
		if (NetworkGameManager.Instance?.arrPlayerNetworkController != null)
		{
			foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerNetworkController)
			{
				if (!(item == null))
				{
					string userUniqueID = item.network.GetUserUniqueID();
					if (!string.IsNullOrWhiteSpace(userUniqueID) && !list.Contains(userUniqueID))
					{
						list.Add(userUniqueID);
						flag = true;
					}
				}
			}
		}
		_playerList = list;
		PlayerList = string.Join(",", _playerList.ToArray());
		if (flag)
		{
			GlobalSaveData.instance.gameData.PlayerList = new List<string>(list);
			GlobalSaveData.instance.SaveCurrentGameData(ServerName);
		}
	}

	public void ResetMissionClear(bool onlyNormalMap)
	{
		if (!MissionLobbyManager.Instance)
		{
			return;
		}
		foreach (MissionSelection item in MissionLobbyManager.Instance.MissionBoard.AllMissionSelection)
		{
			if (item.Phase >= Phase && item.MissionData.IsCleared && (((item.MissionData.Difficulty == 1) & onlyNormalMap) || !onlyNormalMap))
			{
				ArrMissionCleared.Set(item.MissionData.MissionID - 1, value: false);
				item.IsCleared = false;
				item.MissionData.IsCleared = false;
				item.MissionData.IsHide = false;
				item.GetComponent<Button>().enabled = true;
				item.MapImage.gameObject.SetActive(value: true);
				item.InactiveImage.gameObject.SetActive(value: false);
				item.IconCleared.gameObject.SetActive(value: false);
			}
		}
		if (NetworkGameManager.Instance.isServer)
		{
			MissionLobbyManager.Instance.SetUIMissionClear();
		}
	}

	public void SetMissionLockedData(bool[] arrMissionLocked)
	{
		for (int i = 0; i < arrMissionLocked.Length; i++)
		{
			bool value = arrMissionLocked[i];
			if (NetworkGameManager.Instance.isServer)
			{
				ArrMissionLocked.Set(i, value);
			}
		}
	}

	public void SetMissionClearData(bool[] arrMissionCleared)
	{
		for (int i = 0; i < arrMissionCleared.Length; i++)
		{
			bool value = arrMissionCleared[i];
			if (NetworkGameManager.Instance.isServer)
			{
				ArrMissionCleared.Set(i, value);
			}
		}
		MissionLobbyManager.Instance?.SetUIMissionClear();
	}

	public void SetMissionClear(int idx)
	{
		if (idx < 0)
		{
			idx = 0;
		}
		if (NetworkGameManager.Instance.isServer)
		{
			ArrMissionCleared.Set(idx, value: true);
		}
		GlobalSaveData.instance.gameData.ArrMapCleared[idx] = true;
		if ((bool)MissionLobbyManager.Instance)
		{
			MissionLobbyManager.Instance.SetUIMissionClear(idx);
		}
	}

	public void ResetLobbyVariables()
	{
		if (!NetworkGameManager.Instance.isServer)
		{
			return;
		}
		for (int i = 0; i < arrPlayerReady.Length; i++)
		{
			arrPlayerReady.Set(i, value: false);
		}
		for (int j = 0; j < NetworkGameManager.Instance.arrPlayerNetworkController.Count; j++)
		{
			if ((bool)NetworkGameManager.Instance.arrPlayerNetworkController[j])
			{
				NetworkGameManager.Instance.arrPlayerNetworkController[j].ScorePlayerNetwork.ResetScorePerMission();
			}
		}
		showResult = false;
		objectiveComplete = false;
		GameManager.Instance.gameManagerPhoton.arrObjective.Set(0, value: false);
		GameManager.Instance.gameManagerPhoton.arrObjective.Set(1, value: false);
		TargetDestroyed = 0;
	}

	public void RemoveLife(int remove = 1)
	{
		Life -= remove;
	}

	public override void FixedUpdateNetwork()
	{
		GameManager.Instance.NetworkUpdate();
	}

	[Preserve]
	private static void OnSeedChanged(Changed<GameManagerPhoton> changed)
	{
		GlobalOptionsManager.Instance.seed = changed.Behaviour.Seed;
	}

	[Preserve]
	private static void OnTargetDestroyedChanged(Changed<GameManagerPhoton> changed)
	{
		if (!changed.Behaviour.CurrentMission || !changed.Behaviour.CurrentMission.MissionObjective || changed.Behaviour.CurrentMission.MissionObjective.MinTargetDestroy <= 0)
		{
			return;
		}
		if ((bool)UIMissionObjective.Instance)
		{
			UIMissionObjective.Instance.ListTextObjective[0].SetTerm(changed.Behaviour.CurrentMission.MissionObjective.DetailObjectiveLocalization[0]);
			UIMissionObjective.Instance.ListTMPTextObjective[0].text = UIMissionObjective.Instance.ListTMPTextObjective[0].text + " (" + changed.Behaviour.TargetDestroyed + "/" + changed.Behaviour.CurrentMission.MissionObjective.MinTargetDestroy + ")";
		}
		if (changed.Behaviour.TargetDestroyed < changed.Behaviour.CurrentMission.MissionObjective.MinTargetDestroy)
		{
			return;
		}
		GameManager.Instance.gameManagerPhoton.arrObjective.Set(0, value: true);
		GameManager.Instance.gameManagerPhoton.arrObjective.Set(1, value: true);
		UIMissionObjective.Instance?.SetCheckboxRetrieveKeyItem();
		GameManager.Instance.waveManager.cueHordeTimer.StopDuration();
		if ((!GameManager.Instance.waveManager.hordeTimer.isRunning || GameManager.Instance.waveManager.hordeTimer.interval > 5f) && !UIGameManager.Instance.LabelHordeInfiniteIncoming.activeSelf)
		{
			GameManager.Instance.waveManager.AlertHorde(5);
		}
		foreach (ItemInteractable item in GameManager.Instance.ListBrimCarInteractable)
		{
			item?.EnableCollider();
			item?.lockMap.transform.GetChild(0).gameObject.SetActive(value: true);
		}
	}

	[Preserve]
	private static void OnShowResultChanged(Changed<GameManagerPhoton> changed)
	{
		if (changed.Behaviour.showResult)
		{
			PhotonMultiplayerManager.Instance._runner.SetActiveScene("Lobby");
			PhotonMultiplayerManager.Instance.activeIngameScene = "Lobby";
		}
	}

	[Preserve]
	private static void OnArrPlayerReadyChanged(Changed<GameManagerPhoton> changed)
	{
		if (!(LobbyManager.Instance != null))
		{
			return;
		}
		if (SurvivorLobbyManager.Instance != null)
		{
			SurvivorLobbyManager.Instance.Show();
		}
		for (int i = 0; i < NetworkGameManager.Instance.arrPlayerController.Count; i++)
		{
			int iDX = NetworkGameManager.Instance.arrPlayerController[i].network.GetIDX();
			UIGameManager.Instance.readyUIController?.GetUITabPlayer(iDX)?.SetCheckBox(changed.Behaviour.arrPlayerReady[iDX]);
			if (changed.Behaviour.arrPlayerReady[iDX])
			{
				UIGameManager.Instance.readyUIController?.GetUITabPlayer(iDX)?.SetReadyUI();
				PlayerBoard.Instance.boardPlayerList[iDX].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f);
				if (NetworkGameManager.Instance.arrPlayerController[i].network.isLocalPlayer && LobbyManager.Instance != null)
				{
					LobbyManager.Instance.textReady.SetActive(value: true);
					LobbyManager.Instance.textUnready.SetActive(value: false);
				}
				continue;
			}
			UIGameManager.Instance.readyUIController?.GetUITabPlayer(iDX)?.SetUnreadyUI();
			LobbyManager.Instance.timerCountDown.StopDuration();
			PlayerBoard.Instance.boardPlayerList[iDX].transform.GetChild(0).GetComponent<Image>().color = new Color(0.6f, 0.6f, 0.6f);
			if (NetworkGameManager.Instance.arrPlayerController[i].network.isLocalPlayer)
			{
				LobbyManager.Instance.textReady.SetActive(value: false);
				LobbyManager.Instance.textUnready.SetActive(value: true);
			}
		}
		LobbyManager.Instance.allReady = true;
		if (NetworkGameManager.Instance.arrPlayerController.Count > 0)
		{
			for (int j = 0; j < NetworkGameManager.Instance.arrPlayerController.Count; j++)
			{
				int iDX2 = NetworkGameManager.Instance.arrPlayerController[j].network.GetIDX();
				if (!changed.Behaviour.arrPlayerReady[iDX2])
				{
					LobbyManager.Instance.allReady = false;
				}
			}
		}
		else
		{
			LobbyManager.Instance.allReady = false;
		}
		if (LobbyManager.Instance.allReady)
		{
			UIGameManager.Instance.txtCountDown.gameObject.SetActive(value: true);
			LobbyManager.Instance.timerCountDown.StartDuration(4.9f);
		}
		else
		{
			UIGameManager.Instance.txtCountDown.gameObject.SetActive(value: false);
		}
	}

	[Preserve]
	private static void OnObjectiveChanged(Changed<GameManagerPhoton> changed)
	{
		int num = 0;
		for (int i = 0; i < changed.Behaviour.arrObjective.Length; i++)
		{
			if (changed.Behaviour.arrObjective[i])
			{
				num++;
			}
		}
		if (changed.Behaviour.objectiveComplete || num <= 0 || num != changed.Behaviour.arrObjective.Length)
		{
			return;
		}
		changed.Behaviour.objectiveComplete = true;
		if (GameManager.Instance.waveManager.levelHorde < 3)
		{
			if (changed.Behaviour.CurrentMission.IsEasyMap)
			{
				GameManager.Instance.waveManager.levelHorde = 2;
			}
			else
			{
				GameManager.Instance.waveManager.levelHorde = 3;
			}
		}
		GameManager.Instance.waveManager.InitHorde();
	}

	[Preserve]
	private static void OnObjectiveComplete(Changed<GameManagerPhoton> changed)
	{
	}

	[Preserve]
	private static void OnMissionChanged(Changed<GameManagerPhoton> changed)
	{
		NetworkGameManager.Instance.Mission = changed.Behaviour.Mission;
		if (MissionLobbyManager.Instance != null && !NetworkGameManager.Instance.isServer)
		{
			changed.LoadNew();
			if ((bool)MissionLobbyManager.Instance && !MissionLobbyManager.Instance.initializedMap && ((LobbyManager.Instance == null && !GameManager.Instance.gameOver) || !LobbyManager.Instance.UIResult.activeSelf))
			{
				MissionLobbyManager.Instance.InitMap();
			}
			SurvivorLobbyManager.Instance.SetMission();
			MissionSelection missionSelection = MissionLobbyManager.Instance.MissionBoard.GetMissionSelection(NetworkGameManager.Instance.Mission);
			if ((bool)missionSelection && NetworkGameManager.Instance.Mission != 0)
			{
				missionSelection.SetMissionGlobal();
			}
			else
			{
				MissionLobbyManager.Instance.MissionBoard.AllMissionSelection[0].SetMissionGlobal();
			}
			UIGameManager.Instance.SetMissionLocation(UIGameManager.Instance.missionLocationText, null, UIGameManager.Instance.missionLocationTextField);
		}
		if ((bool)MissionLobbyManager.Instance && (bool)MissionLobbyManager.Instance.MissionBoard.GetMissionSelection(NetworkGameManager.Instance.Mission))
		{
			changed.Behaviour.CurrentMission = MissionLobbyManager.Instance.MissionBoard.GetMissionSelection(NetworkGameManager.Instance.Mission).MissionData;
		}
		if (NetworkGameManager.Instance.isServer)
		{
			GlobalSaveData.instance.gameData.CurrentMission = changed.Behaviour.Mission;
		}
	}

	[Preserve]
	private static void OnWaveChanged(Changed<GameManagerPhoton> changed)
	{
		if ((bool)changed.Behaviour.CurrentMission && changed.Behaviour.CurrentMission.MissionObjective.IsSpawnEndlessHordeFromBeginning)
		{
			UIGameManager.Instance.txtTime.text = changed.Behaviour.Wave.ToString();
			if (Instance.CurrentMission.MissionObjective.MaxWave > 0)
			{
				TextMeshProUGUI txtTime = UIGameManager.Instance.txtTime;
				txtTime.text = txtTime.text + " / " + Instance.CurrentMission.MissionObjective.MaxWave;
			}
		}
	}

	[Preserve]
	private static void OnStateLobbyChanged(Changed<GameManagerPhoton> changed)
	{
		if ((bool)LobbyManager.Instance)
		{
			LobbyManager.Instance.LobbyState = (LobbyManager.LobbyStateEnum)changed.Behaviour.StateLobby;
			LobbyManager.Instance.CheckLobbyState();
		}
	}

	[Preserve]
	private static void OnSyncTimer(Changed<GameManagerPhoton> changed)
	{
		GameManager.Instance.timer.interval = changed.Behaviour.TimerSyncTemp;
		ScoreManager.Instance.time = Mathf.FloorToInt(GameManager.Instance.timer.interval);
	}

	public void SyncVariable(GameData data)
	{
		if (data.Seed == 0)
		{
			data.Seed = data.GetCurrentSeed();
		}
		Seed = data.Seed;
		ServerName = data.GetSessionName();
		if (data.PlayerList != null)
		{
			PlayerList = data.GetPlayerList();
		}
		Life = data.Life;
		if (data.ArrMapCleared != null)
		{
			SetMissionClearData(data.ArrMapCleared);
		}
		Mission = (byte)data.CurrentMission;
	}

	public bool ContainsPerkSelectionIndex(int i)
	{
		return PerkSelectionIndex.Contains(i);
	}

	public void SetPerkSelectionIndex(int[] indexArray)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			SetPerkSelectionIndexNetwork(indexArray);
		}
	}

	private void SetPerkSelectionIndexNetwork(int[] indexArray)
	{
		if (!InitPerkSelectionIndex)
		{
			for (int i = 0; i < indexArray.Length; i++)
			{
				PerkSelectionIndex.Set(i, indexArray[i]);
			}
			InitPerkSelectionIndex = true;
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcStartProgressInteract(short uniqueID, byte playerID)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcStartProgressInteract(System.Int16,System.Byte)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcStartProgressInteract(System.Int16,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 1), data);
				*(short*)(data + num2) = uniqueID;
				num2 += 5 & -4;
				data[num2] = playerID;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameManager.Instance.StartProgressInteract(uniqueID, playerID);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcStopProgressInteract(short uniqueID, byte playerID)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcStopProgressInteract(System.Int16,System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcStopProgressInteract(System.Int16,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 2), data);
				*(short*)(data + num2) = uniqueID;
				num2 += 5 & -4;
				data[num2] = playerID;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameManager.Instance.StopProgressInteract(uniqueID, playerID);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcStopProgressInteract(byte playerID)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcStopProgressInteract(System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcStopProgressInteract(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 3), data);
				data[num2] = playerID;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameManager.Instance.StopProgressInteract(-1, playerID);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcDropItem(int uIDItem, byte amount, byte ammo, ulong pos, short idxItem, bool isFading = false, bool isSpreading = true)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcDropItem(System.Int32,System.Byte,System.Byte,System.UInt64,System.Int16,System.Boolean,System.Boolean)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			num += 8;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcDropItem(System.Int32,System.Byte,System.Byte,System.UInt64,System.Int16,System.Boolean,System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 4), data);
				*(int*)(data + num2) = uIDItem;
				num2 += 4;
				data[num2] = amount;
				num2 += 4 & -4;
				data[num2] = ammo;
				num2 += 4 & -4;
				*(ulong*)(data + num2) = pos;
				num2 += 8;
				*(short*)(data + num2) = idxItem;
				num2 += 5 & -4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isFading);
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isSpreading);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameManager.Instance.DropItem(uIDItem, amount, ammo, MathFunc.DecodeVector3FromULong(pos), idxItem, isSpreading, null, -1, isFading);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcDropItemFromPlayer(int uIDItem, byte amount, byte ammo, byte playerIDX, short idxItem, byte idxInventory)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcDropItemFromPlayer(System.Int32,System.Byte,System.Byte,System.Byte,System.Int16,System.Byte)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcDropItemFromPlayer(System.Int32,System.Byte,System.Byte,System.Byte,System.Int16,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 5), data);
				*(int*)(data + num2) = uIDItem;
				num2 += 4;
				data[num2] = amount;
				num2 += 4 & -4;
				data[num2] = ammo;
				num2 += 4 & -4;
				data[num2] = playerIDX;
				num2 += 4 & -4;
				*(short*)(data + num2) = idxItem;
				num2 += 5 & -4;
				data[num2] = idxInventory;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		PlayerController player = NetworkGameManager.Instance.GetPlayer(playerIDX);
		GameManager.Instance.DropItem(uIDItem, amount, ammo, player.weaponPos.transform.position, idxItem, isSpreading: true, player, idxInventory, isFading: false, isRemoveFromLocalPlayer: false);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcSpawnItem(int uIDItem, ulong pos, short idxItem, bool isSpread = false)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcSpawnItem(System.Int32,System.UInt64,System.Int16,System.Boolean)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			num += 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcSpawnItem(System.Int32,System.UInt64,System.Int16,System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 6), data);
				*(int*)(data + num2) = uIDItem;
				num2 += 4;
				*(ulong*)(data + num2) = pos;
				num2 += 8;
				*(short*)(data + num2) = idxItem;
				num2 += 5 & -4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isSpread);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (uIDItem <= 0)
		{
			return;
		}
		if (uIDItem < 100)
		{
			int magazineSize = NetworkGameManager.Instance.ownPlayer.weaponController.GetMagazineSize(equipedWeapon: false, uIDItem, DataManager.Instance.GetBaseWeapon(uIDItem));
			GameManager.Instance.DropItem(uIDItem, 1, (byte)magazineSize, MathFunc.DecodeVector3FromULong(pos), idxItem, isSpread);
			return;
		}
		if (uIDItem < 200 && BGDatabase_Ammunition.GetEntityByKeyid(uIDItem) != null)
		{
			GameManager.Instance.DropItem(uIDItem, (byte)BGDatabase_Ammunition.GetEntityByKeyid(uIDItem).Amount, (byte)BGDatabase_Ammunition.GetEntityByKeyid(uIDItem).Amount, MathFunc.DecodeVector3FromULong(pos), idxItem, isSpread);
			return;
		}
		BGDatabase_Item entityByKeyid = BGDatabase_Item.GetEntityByKeyid(uIDItem);
		int num3 = 0;
		if (entityByKeyid != null && entityByKeyid.Durability > 0)
		{
			num3 = entityByKeyid.Durability;
		}
		GameManager.Instance.DropItem(uIDItem, 1, (byte)num3, MathFunc.DecodeVector3FromULong(pos), idxItem, isSpread);
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcSpawnItemAmount(int uIDItem, ulong pos, short idxItem, byte amount, bool isSpread)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcSpawnItemAmount(System.Int32,System.UInt64,System.Int16,System.Byte,System.Boolean)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			num += 8;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcSpawnItemAmount(System.Int32,System.UInt64,System.Int16,System.Byte,System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 7), data);
				*(int*)(data + num2) = uIDItem;
				num2 += 4;
				*(ulong*)(data + num2) = pos;
				num2 += 8;
				*(short*)(data + num2) = idxItem;
				num2 += 5 & -4;
				data[num2] = amount;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isSpread);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (uIDItem < 100)
		{
			int magazineSize = NetworkGameManager.Instance.ownPlayer.weaponController.GetMagazineSize(equipedWeapon: false, uIDItem, DataManager.Instance.GetBaseWeapon(uIDItem));
			GameManager.Instance.DropItem(uIDItem, 1, (byte)magazineSize, MathFunc.DecodeVector3FromULong(pos), idxItem, isSpread);
		}
		else if (uIDItem < 200)
		{
			GameManager.Instance.DropItem(uIDItem, amount, (byte)BGDatabase_Ammunition.GetEntityByKeyid(uIDItem).Amount, MathFunc.DecodeVector3FromULong(pos), idxItem, isSpread);
		}
		else
		{
			GameManager.Instance.DropItem(uIDItem, 1, 0, MathFunc.DecodeVector3FromULong(pos), idxItem);
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcSpawnItemAmountAmmo(int uIDItem, ulong pos, short idxItem, byte amount, byte ammo = 0, bool isSpread = false)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcSpawnItemAmountAmmo(System.Int32,System.UInt64,System.Int16,System.Byte,System.Byte,System.Boolean)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			num += 8;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcSpawnItemAmountAmmo(System.Int32,System.UInt64,System.Int16,System.Byte,System.Byte,System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 8), data);
				*(int*)(data + num2) = uIDItem;
				num2 += 4;
				*(ulong*)(data + num2) = pos;
				num2 += 8;
				*(short*)(data + num2) = idxItem;
				num2 += 5 & -4;
				data[num2] = amount;
				num2 += 4 & -4;
				data[num2] = ammo;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isSpread);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (uIDItem < 100)
		{
			GameManager.Instance.DropItem(uIDItem, 1, ammo, MathFunc.DecodeVector3FromULong(pos), idxItem, isSpread);
		}
		else if (uIDItem < 200)
		{
			GameManager.Instance.DropItem(uIDItem, amount, (byte)BGDatabase_Ammunition.GetEntityByKeyid(uIDItem).Amount, MathFunc.DecodeVector3FromULong(pos), idxItem, isSpread);
		}
		else
		{
			GameManager.Instance.DropItem(uIDItem, 1, 0, MathFunc.DecodeVector3FromULong(pos), idxItem, isSpread);
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcUnlockItem(byte uniqueID)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcUnlockItem(System.Byte)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcUnlockItem(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 9), data);
				data[num2] = uniqueID;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameManager.Instance.UnlockItem(uniqueID);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcInitEnemy(byte idxEnemy)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcInitEnemy(System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcInitEnemy(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 10), data);
				data[num2] = idxEnemy;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (!NetworkGameManager.Instance.isServer)
		{
			EnemyController enemy = GameManager.Instance.GetEnemy(idxEnemy);
			if (enemy != null)
			{
				enemy.InitForClient();
			}
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcExecHitEffect(byte _idx, bool isCloseInventory = true, bool isGreenBloodScreen = false)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcExecHitEffect(System.Byte,System.Boolean,System.Boolean)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcExecHitEffect(System.Byte,System.Boolean,System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 11), data);
				data[num2] = _idx;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isCloseInventory);
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isGreenBloodScreen);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		PlayerController player = NetworkGameManager.Instance.GetPlayer(_idx);
		if ((bool)player)
		{
			player.feedbackController.Hurt(isCloseInventory, isGreenBloodScreen).Forget();
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcExecAlertHorde()
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcExecAlertHorde()", Object, 2);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcExecAlertHorde()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 12), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if ((!GameManager.Instance.waveManager.hordeTimer.isRunning || GameManager.Instance.waveManager.hordeTimer.interval > 5f) && !UIGameManager.Instance.LabelHordeInfiniteIncoming.activeSelf && LobbyManager.Instance == null)
		{
			GameManager.Instance.waveManager.AlertHorde(49);
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcExecDisableHorde()
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcExecDisableHorde()", Object, 2);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcExecDisableHorde()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 13), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameManager.Instance.waveManager.HordeDisable();
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcExecSpawnPortal(byte idxPos, byte idEliteType)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcExecSpawnPortal(System.Byte,System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcExecSpawnPortal(System.Byte,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 14), data);
				data[num2] = idxPos;
				num2 += 4 & -4;
				data[num2] = idEliteType;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		byte idEliteType2 = idEliteType;
		EliteEffectPool portalObject = EliteEffectSpawner.Instance.Get();
		byte posElite = idxPos;
		if (!GameManager.Instance.arrInitPosEnemy[posElite])
		{
			return;
		}
		portalObject.transform.position = new Vector3(GameManager.Instance.arrInitPosEnemy[posElite].transform.position.x, 0f, GameManager.Instance.arrInitPosEnemy[posElite].transform.position.z);
		if (NetworkGameManager.Instance.isServer)
		{
			GameManager.Instance.IsEliteSpawning = true;
			UniTaskUtil.DelayedCall(this, 3f, () =>
			{
				if (posElite < GameManager.Instance.arrInitPosEnemy.Count && posElite >= 0)
				{
					EnemySpawner.Instance.SpawnEnemy(GameManager.Instance.arrInitPosEnemy[posElite], GameManager.Instance.arrInitPosEnemy[posElite].transform, idEliteType2, isHorde: true);
				}
				GameManager.Instance.IsEliteSpawning = false;
			}).Forget();
		}
		UniTaskUtil.DelayedCall(portalObject, 5f, () =>
		{
			portalObject.AnimatorObject.Play("FlameEnd");
		}).Forget();
		UniTaskUtil.DelayedCall(portalObject, 10f, () =>
		{
			EliteEffectSpawner.Instance.Release(portalObject);
		}).Forget();
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSpawnPortalPosition(ulong position, byte idEliteType)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcSpawnPortalPosition(System.UInt64,System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcSpawnPortalPosition(System.UInt64,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 15), data);
				*(ulong*)(data + num2) = position;
				num2 += 8;
				data[num2] = idEliteType;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		byte idEliteType2 = idEliteType;
		Vector3 posElite = MathFunc.DecodeVector3FromULong(position);
		EliteEffectPool portalObject = EliteEffectSpawner.Instance.Get();
		portalObject.transform.position = new Vector3(posElite.x, 0f, posElite.z);
		if (NetworkGameManager.Instance.isServer)
		{
			GameManager.Instance.IsEliteSpawning = true;
			UniTaskUtil.DelayedCall(this, 3f, () =>
			{
				EnemySpawner.Instance.SpawnEnemy(null, portalObject.transform, idEliteType2, isHorde: true, new Vector3(posElite.x, 0f, posElite.z));
				GameManager.Instance.IsEliteSpawning = false;
			}).Forget();
		}
		UniTaskUtil.DelayedCall(this, 5f, () =>
		{
			portalObject.AnimatorObject.Play("FlameEnd");
		}).Forget();
		UniTaskUtil.DelayedCall(this, 10f, () =>
		{
			EliteEffectSpawner.Instance.Release(portalObject);
		}).Forget();
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSetSeed(int newSeed)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcSetSeed(System.Int32)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcSetSeed(System.Int32)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 16), data);
				*(int*)(data + num2) = newSeed;
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GlobalOptionsManager.Instance.seed = newSeed;
		GlobalSaveData.instance.optionData.lastSeed = newSeed;
		GlobalSaveData.instance.SaveOptionData();
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncTimer()
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcSyncTimer()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcSyncTimer()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 17), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		TimerSyncTemp = (short)Mathf.RoundToInt(GameManager.Instance.timer.interval);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncTimerCountdown(float interval, bool isStartDuration = false)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcSyncTimerCountdown(System.Single,System.Boolean)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcSyncTimerCountdown(System.Single,System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 18), data);
				ReadWriteUtilsForWeaver.WriteFloat((int*)(data + num2), 999.99994f, interval);
				num2 += 4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isStartDuration);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (isStartDuration)
		{
			ChatSystem.Instance.timerCountdown.StartDuration(interval);
		}
		else
		{
			ChatSystem.Instance.timerCountdown.interval = interval;
		}
		for (int i = 0; i < GameManager.Instance.ListBrimCarInteractable.Count; i++)
		{
			if (GameManager.Instance.ListBrimCarInteractable[i].isActiveAndEnabled && interval > 3f)
			{
				if ((bool)CurrentMission && (bool)CurrentMission.MissionObjective && CurrentMission.MissionObjective.IsCarRepairingOnStart)
				{
					GameManager.Instance.ListBrimCarInteractable[i].ObjectActiveSpecial.SetActive(value: true);
				}
				ChatSystem.Instance.ItemInteractableCountdown = GameManager.Instance.ListBrimCarInteractable[i];
			}
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSyncTimeIntervalCountdown(short interval)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcSyncTimeIntervalCountdown(System.Int16)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcSyncTimeIntervalCountdown(System.Int16)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 19), data);
				*(short*)(data + num2) = interval;
				num2 += 5 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		ChatSystem.Instance.timerCountdown.interval = (float)interval / 10f;
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RPCExecuteResult()
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RPCExecuteResult()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RPCExecuteResult()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 20), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		NetworkGameManager.Instance.ownPlayer.network.StartCoroutine(NetworkGameManager.Instance.ownPlayer.network.ShowResultScene());
	}

	[Preserve]
	private static void OnMapRandomizedChanged(Changed<GameManagerPhoton> changed)
	{
		if (changed.Behaviour.IsRandomizeMapOnDefeat)
		{
			GameDebug.Instance.RandomizeMaptext.text = "Randomize Map on Defeat=On";
		}
		else
		{
			GameDebug.Instance.RandomizeMaptext.text = "Randomize Map on Defeat=Off";
		}
	}

	public void RandomizeSeedPuzzle()
	{
		if (NetworkGameManager.Instance.isServer)
		{
			SeedPuzzle = int.Parse(DateTime.Now.ToString("ddHHmmss"));
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcExecIncomingWave(byte hordeTimerInterval)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcExecIncomingWave(System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcExecIncomingWave(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 21), data);
				data[num2] = hordeTimerInterval;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameManager.Instance.waveManager.hordeTimer.StartDuration((int)hordeTimerInterval);
		GameManager.Instance.waveManager.TimerIncomingWave();
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcAddMaterialToAllPlayer(int itemID, int amount)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcAddMaterialToAllPlayer(System.Int32,System.Int32)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcAddMaterialToAllPlayer(System.Int32,System.Int32)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 22), data);
				*(int*)(data + num2) = itemID;
				num2 += 4;
				*(int*)(data + num2) = amount;
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		NetworkGameManager.Instance.ownPlayer?.data.MaterialInventoryManager.AddMaterial(MaterialInventoryManager.InventoryType.Auto, itemID, amount);
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSetDifficulty()
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcSetDifficulty()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcSetDifficulty()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 23), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		GameModes.Instance.SetDifficulty((DifficultySetting.Difficulty)Difficulty);
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public unsafe void RpcBarricadeTopBroken(byte uniqueID, Vector3 sourcePos)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 1) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcBarricadeTopBroken(System.Byte,UnityEngine.Vector3)", Object, 1);
				return;
			}
			int num = 8;
			num += 4;
			num += 12;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcBarricadeTopBroken(System.Byte,UnityEngine.Vector3)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 24), data);
				data[num2] = uniqueID;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteVector3((int*)(data + num2), 999.99994f, sourcePos);
				num2 += 12;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		ItemInteractable itemInteractable = GameManager.Instance.GetItemInteractable(uniqueID);
		if (itemInteractable != null)
		{
			itemInteractable.BrokeTopBarricade(base.transform.position);
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public unsafe void RpcBarricadeBotBroken(byte uniqueID, Vector3 sourcePos)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 1) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcBarricadeBotBroken(System.Byte,UnityEngine.Vector3)", Object, 1);
				return;
			}
			int num = 8;
			num += 4;
			num += 12;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcBarricadeBotBroken(System.Byte,UnityEngine.Vector3)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 25), data);
				data[num2] = uniqueID;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteVector3((int*)(data + num2), 999.99994f, sourcePos);
				num2 += 12;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		ItemInteractable itemInteractable = GameManager.Instance.GetItemInteractable(uniqueID);
		if (itemInteractable != null)
		{
			itemInteractable.BrokeBotBarricade(base.transform.position);
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public unsafe void RPCBarricadeAttacked(byte barricadeUniqueID, bool isDebugging = false)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 1) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RPCBarricadeAttacked(System.Byte,System.Boolean)", Object, 1);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RPCBarricadeAttacked(System.Byte,System.Boolean)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 26), data);
				data[num2] = barricadeUniqueID;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteBoolean((int*)(data + num2), isDebugging);
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		ItemInteractable itemInteractable = GameManager.Instance.GetItemInteractable(barricadeUniqueID);
		if (itemInteractable != null)
		{
			itemInteractable.AttackBarricade(isDebugging);
		}
	}

	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public unsafe void RPCSetEnemyDead(byte idx)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 1) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RPCSetEnemyDead(System.Byte)", Object, 1);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RPCSetEnemyDead(System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 27), data);
				data[num2] = idx;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (NetworkGameManager.Instance.isServer)
		{
			return;
		}
		EnemyController enemy = GameManager.Instance.GetEnemy(idx);
		if ((bool)enemy && !enemy.isDead)
		{
			enemy.isDead = true;
			enemy.Dead(1).Forget();
			UniTaskUtil.DelayedCall(this, 1f, () =>
			{
				enemy.bloodPool.gameObject.SetActive(value: false);
				enemy.Hide2DSprite();
				enemy.isSpriteInactive = true;
			}).Forget();
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcExecEnemyKnockback(byte idx, Vector3 posKnockback)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcExecEnemyKnockback(System.Byte,UnityEngine.Vector3)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 12;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcExecEnemyKnockback(System.Byte,UnityEngine.Vector3)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 28), data);
				data[num2] = idx;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteVector3((int*)(data + num2), 999.99994f, posKnockback);
				num2 += 12;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		EnemyController enemy = GameManager.Instance.GetEnemy(idx);
		if (!enemy)
		{
			return;
		}
		if (NetworkGameManager.Instance.isServer)
		{
			if (MathFunc.DistanceSameYPos(enemy.network.enemyController.transform.position, posKnockback) > 14f)
			{
				enemy.network.enemyController.myrigidbody.DOMove(posKnockback, 0f).SetEase(Ease.Linear);
			}
			else
			{
				enemy.network.enemyController.myrigidbody.DOMove(posKnockback, 0.15f).SetEase(Ease.Linear);
			}
		}
		else if (MathFunc.DistanceSameYPos(NetworkGameManager.Instance.ownPlayer.transform.position, posKnockback) > PhotonMultiplayerManager.Instance.areaOfInterest)
		{
			enemy.network.enemyController.object2D.DOMove(posKnockback, 0f).SetEase(Ease.Linear);
			UniTaskUtil.DelayedCall(enemy, 0.15f, () =>
			{
				enemy.animator.Play("Idle" + enemy.data.arrWeaponState[enemy.data.weaponState] + enemy.movement.angleAnim);
			}).Forget();
		}
		else
		{
			enemy.network.enemyController.object2D.DOMove(posKnockback, 0.15f).SetEase(Ease.Linear);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcSetPosEnemy(byte idx, Vector3 posEnemy)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RpcSetPosEnemy(System.Byte,UnityEngine.Vector3)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 12;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RpcSetPosEnemy(System.Byte,UnityEngine.Vector3)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 29), data);
				data[num2] = idx;
				num2 += 4 & -4;
				ReadWriteUtilsForWeaver.WriteVector3((int*)(data + num2), 999.99994f, posEnemy);
				num2 += 12;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		Vector3 posEnemy2 = posEnemy;
		EnemyController enemy = GameManager.Instance.GetEnemy(idx);
		if ((bool)enemy && !NetworkGameManager.Instance.isServer && MathFunc.DistanceSameYPos(NetworkGameManager.Instance.ownPlayer.transform.position, posEnemy2) > PhotonMultiplayerManager.Instance.areaOfInterest)
		{
			UniTaskUtil.DelayedCall(enemy, 0.15f, () =>
			{
				enemy.network.enemyController.object2D.DOMove(posEnemy2, 0f).SetEase(Ease.Linear);
				enemy.animator.Play("Idle" + enemy.data.arrWeaponState[enemy.data.weaponState] + enemy.movement.angleAnim);
			}).Forget();
		}
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPCUnlockAllMap()
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GameManagerPhoton::RPCUnlockAllMap()", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GameManagerPhoton::RPCUnlockAllMap()", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 30), data);
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		for (int num3 = Instance.ArrMissionCleared.Length - 1; num3 >= 0; num3--)
		{
			Instance.ArrMissionLocked.Set(num3, value: false);
		}
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		ServerName = _ServerName;
		ScenarioId = _ScenarioId;
		Difficulty = _Difficulty;
		PlayerList = _PlayerList;
		IsLoadGame = _IsLoadGame;
		Seed = _Seed;
		SeedPuzzle = _SeedPuzzle;
		NetworkBehaviourUtils.InitializeNetworkArray(PerkSelectionIndex, _PerkSelectionIndex, "PerkSelectionIndex");
		showResult = _showResult;
		NetworkBehaviourUtils.InitializeNetworkArray(arrPlayerReady, _arrPlayerReady, "arrPlayerReady");
		NetworkBehaviourUtils.InitializeNetworkArray(arrObjective, _arrObjective, "arrObjective");
		TargetDestroyed = _TargetDestroyed;
		HostLoadingGameComplete = _HostLoadingGameComplete;
		TimerSyncTemp = _TimerSyncTemp;
		NetworkBehaviourUtils.InitializeNetworkArray(ArrMissionCleared, _ArrMissionCleared, "ArrMissionCleared");
		NetworkBehaviourUtils.InitializeNetworkArray(ArrMissionLocked, _ArrMissionLocked, "ArrMissionLocked");
		objectiveComplete = _objectiveComplete;
		Mission = _Mission;
		Scenario = _Scenario;
		IsWin = _IsWin;
		Life = _Life;
		Wave = _Wave;
		StateLobby = _StateLobby;
		IsRandomizeMapOnDefeat = _IsRandomizeMapOnDefeat;
		SpawnIdx = _SpawnIdx;
		LobbyId = _LobbyId;
		StartTime = _StartTime;
		TotalMissionTime = _TotalMissionTime;
		EndTime = _EndTime;
		Phase = _Phase;
	}

	public override void CopyStateToBackingFields()
	{
		_ServerName = ServerName;
		_ScenarioId = ScenarioId;
		_Difficulty = Difficulty;
		_PlayerList = PlayerList;
		_IsLoadGame = IsLoadGame;
		_Seed = Seed;
		_SeedPuzzle = SeedPuzzle;
		NetworkBehaviourUtils.CopyFromNetworkArray(PerkSelectionIndex, ref _PerkSelectionIndex);
		_showResult = showResult;
		NetworkBehaviourUtils.CopyFromNetworkArray(arrPlayerReady, ref _arrPlayerReady);
		NetworkBehaviourUtils.CopyFromNetworkArray(arrObjective, ref _arrObjective);
		_TargetDestroyed = TargetDestroyed;
		_HostLoadingGameComplete = HostLoadingGameComplete;
		_TimerSyncTemp = TimerSyncTemp;
		NetworkBehaviourUtils.CopyFromNetworkArray(ArrMissionCleared, ref _ArrMissionCleared);
		NetworkBehaviourUtils.CopyFromNetworkArray(ArrMissionLocked, ref _ArrMissionLocked);
		_objectiveComplete = objectiveComplete;
		_Mission = Mission;
		_Scenario = Scenario;
		_IsWin = IsWin;
		_Life = Life;
		_Wave = Wave;
		_StateLobby = StateLobby;
		_IsRandomizeMapOnDefeat = IsRandomizeMapOnDefeat;
		_SpawnIdx = SpawnIdx;
		_LobbyId = LobbyId;
		_StartTime = StartTime;
		_TotalMissionTime = TotalMissionTime;
		_EndTime = EndTime;
		_Phase = Phase;
	}

	[NetworkRpcWeavedInvoker(1, 2, 7)]
	[Preserve]
	protected unsafe static void RpcStartProgressInteract_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short uniqueID = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte playerID = num3;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcStartProgressInteract(uniqueID, playerID);
	}

	[NetworkRpcWeavedInvoker(2, 7, 7)]
	[Preserve]
	protected unsafe static void RpcStopProgressInteract_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short uniqueID = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte playerID = num3;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcStopProgressInteract(uniqueID, playerID);
	}

	[NetworkRpcWeavedInvoker(3, 7, 7)]
	[Preserve]
	protected unsafe static void RpcStopProgressInteract_0040Invoker2(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte playerID = num2;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcStopProgressInteract(playerID);
	}

	[NetworkRpcWeavedInvoker(4, 2, 7)]
	[Preserve]
	protected unsafe static void RpcDropItem_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int uIDItem = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte amount = num3;
		byte num4 = data[num];
		num += 4 & -4;
		byte ammo = num4;
		long num5 = *(long*)(data + num);
		num += 8;
		ulong pos = (ulong)num5;
		short num6 = *(short*)(data + num);
		num += 5 & -4;
		short idxItem = num6;
		bool num7 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isFading = num7;
		bool num8 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isSpreading = num8;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcDropItem(uIDItem, amount, ammo, pos, idxItem, isFading, isSpreading);
	}

	[NetworkRpcWeavedInvoker(5, 2, 7)]
	[Preserve]
	protected unsafe static void RpcDropItemFromPlayer_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int uIDItem = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte amount = num3;
		byte num4 = data[num];
		num += 4 & -4;
		byte ammo = num4;
		byte num5 = data[num];
		num += 4 & -4;
		byte playerIDX = num5;
		short num6 = *(short*)(data + num);
		num += 5 & -4;
		short idxItem = num6;
		byte num7 = data[num];
		num += 4 & -4;
		byte idxInventory = num7;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcDropItemFromPlayer(uIDItem, amount, ammo, playerIDX, idxItem, idxInventory);
	}

	[NetworkRpcWeavedInvoker(6, 2, 7)]
	[Preserve]
	protected unsafe static void RpcSpawnItem_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int uIDItem = num2;
		long num3 = *(long*)(data + num);
		num += 8;
		ulong pos = (ulong)num3;
		short num4 = *(short*)(data + num);
		num += 5 & -4;
		short idxItem = num4;
		bool num5 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isSpread = num5;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcSpawnItem(uIDItem, pos, idxItem, isSpread);
	}

	[NetworkRpcWeavedInvoker(7, 2, 7)]
	[Preserve]
	protected unsafe static void RpcSpawnItemAmount_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int uIDItem = num2;
		long num3 = *(long*)(data + num);
		num += 8;
		ulong pos = (ulong)num3;
		short num4 = *(short*)(data + num);
		num += 5 & -4;
		short idxItem = num4;
		byte num5 = data[num];
		num += 4 & -4;
		byte amount = num5;
		bool num6 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isSpread = num6;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcSpawnItemAmount(uIDItem, pos, idxItem, amount, isSpread);
	}

	[NetworkRpcWeavedInvoker(8, 2, 7)]
	[Preserve]
	protected unsafe static void RpcSpawnItemAmountAmmo_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int uIDItem = num2;
		long num3 = *(long*)(data + num);
		num += 8;
		ulong pos = (ulong)num3;
		short num4 = *(short*)(data + num);
		num += 5 & -4;
		short idxItem = num4;
		byte num5 = data[num];
		num += 4 & -4;
		byte amount = num5;
		byte num6 = data[num];
		num += 4 & -4;
		byte ammo = num6;
		bool num7 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isSpread = num7;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcSpawnItemAmountAmmo(uIDItem, pos, idxItem, amount, ammo, isSpread);
	}

	[NetworkRpcWeavedInvoker(9, 2, 7)]
	[Preserve]
	protected unsafe static void RpcUnlockItem_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte uniqueID = num2;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcUnlockItem(uniqueID);
	}

	[NetworkRpcWeavedInvoker(10, 7, 7)]
	[Preserve]
	protected unsafe static void RpcInitEnemy_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idxEnemy = num2;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcInitEnemy(idxEnemy);
	}

	[NetworkRpcWeavedInvoker(11, 2, 7)]
	[Preserve]
	protected unsafe static void RpcExecHitEffect_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idx = num2;
		bool num3 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isCloseInventory = num3;
		bool num4 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isGreenBloodScreen = num4;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcExecHitEffect(idx, isCloseInventory, isGreenBloodScreen);
	}

	[NetworkRpcWeavedInvoker(12, 2, 7)]
	[Preserve]
	protected unsafe static void RpcExecAlertHorde_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcExecAlertHorde();
	}

	[NetworkRpcWeavedInvoker(13, 2, 7)]
	[Preserve]
	protected unsafe static void RpcExecDisableHorde_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcExecDisableHorde();
	}

	[NetworkRpcWeavedInvoker(14, 7, 7)]
	[Preserve]
	protected unsafe static void RpcExecSpawnPortal_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idxPos = num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte idEliteType = num3;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcExecSpawnPortal(idxPos, idEliteType);
	}

	[NetworkRpcWeavedInvoker(15, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSpawnPortalPosition_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		long num2 = *(long*)(data + num);
		num += 8;
		ulong position = (ulong)num2;
		byte num3 = data[num];
		num += 4 & -4;
		byte idEliteType = num3;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcSpawnPortalPosition(position, idEliteType);
	}

	[NetworkRpcWeavedInvoker(16, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSetSeed_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int newSeed = num2;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcSetSeed(newSeed);
	}

	[NetworkRpcWeavedInvoker(17, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncTimer_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcSyncTimer();
	}

	[NetworkRpcWeavedInvoker(18, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncTimerCountdown_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		float num2 = (float)(*(int*)(data + num)) * 0.001f;
		num += 4;
		float interval = num2;
		bool num3 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isStartDuration = num3;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcSyncTimerCountdown(interval, isStartDuration);
	}

	[NetworkRpcWeavedInvoker(19, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSyncTimeIntervalCountdown_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short interval = num2;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcSyncTimeIntervalCountdown(interval);
	}

	[NetworkRpcWeavedInvoker(20, 7, 7)]
	[Preserve]
	protected unsafe static void RPCExecuteResult_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RPCExecuteResult();
	}

	[NetworkRpcWeavedInvoker(21, 7, 7)]
	[Preserve]
	protected unsafe static void RpcExecIncomingWave_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte hordeTimerInterval = num2;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcExecIncomingWave(hordeTimerInterval);
	}

	[NetworkRpcWeavedInvoker(22, 7, 7)]
	[Preserve]
	protected unsafe static void RpcAddMaterialToAllPlayer_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int itemID = num2;
		int num3 = *(int*)(data + num);
		num += 4;
		int amount = num3;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcAddMaterialToAllPlayer(itemID, amount);
	}

	[NetworkRpcWeavedInvoker(23, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSetDifficulty_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcSetDifficulty();
	}

	[NetworkRpcWeavedInvoker(24, 1, 7)]
	[Preserve]
	protected unsafe static void RpcBarricadeTopBroken_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte uniqueID = num2;
		Vector3 vector = ReadWriteUtilsForWeaver.ReadVector3((int*)(data + num), 0.001f);
		num += 12;
		Vector3 sourcePos = vector;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcBarricadeTopBroken(uniqueID, sourcePos);
	}

	[NetworkRpcWeavedInvoker(25, 1, 7)]
	[Preserve]
	protected unsafe static void RpcBarricadeBotBroken_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte uniqueID = num2;
		Vector3 vector = ReadWriteUtilsForWeaver.ReadVector3((int*)(data + num), 0.001f);
		num += 12;
		Vector3 sourcePos = vector;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcBarricadeBotBroken(uniqueID, sourcePos);
	}

	[NetworkRpcWeavedInvoker(26, 1, 7)]
	[Preserve]
	protected unsafe static void RPCBarricadeAttacked_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte barricadeUniqueID = num2;
		bool num3 = ReadWriteUtilsForWeaver.ReadBoolean((int*)(data + num));
		num += 4;
		bool isDebugging = num3;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RPCBarricadeAttacked(barricadeUniqueID, isDebugging);
	}

	[NetworkRpcWeavedInvoker(27, 1, 7)]
	[Preserve]
	protected unsafe static void RPCSetEnemyDead_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idx = num2;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RPCSetEnemyDead(idx);
	}

	[NetworkRpcWeavedInvoker(28, 7, 7)]
	[Preserve]
	protected unsafe static void RpcExecEnemyKnockback_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idx = num2;
		Vector3 vector = ReadWriteUtilsForWeaver.ReadVector3((int*)(data + num), 0.001f);
		num += 12;
		Vector3 posKnockback = vector;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcExecEnemyKnockback(idx, posKnockback);
	}

	[NetworkRpcWeavedInvoker(29, 7, 7)]
	[Preserve]
	protected unsafe static void RpcSetPosEnemy_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte idx = num2;
		Vector3 vector = ReadWriteUtilsForWeaver.ReadVector3((int*)(data + num), 0.001f);
		num += 12;
		Vector3 posEnemy = vector;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RpcSetPosEnemy(idx, posEnemy);
	}

	[NetworkRpcWeavedInvoker(30, 7, 1)]
	[Preserve]
	protected unsafe static void RPCUnlockAllMap_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((GameManagerPhoton)behaviour).RPCUnlockAllMap();
	}
}
