using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fusion.Photon.Realtime;
using I2.Loc;
using Toked;
using UnityEngine;
using _Modules.Achievement.Scripts;
using _Modules.Data.Scripts;

public class GlobalSaveData : MonoBehaviour
{
	[Serializable]
	public class LanguageObj
	{
		public string LangCode;

		public string LangName;

		public bool Enable;
	}

	[Serializable]
	public class OptionData
	{
		public int gameVer;

		public string lang;

		public int shakeLevel;

		public int volMaster;

		public int volMusic;

		public int volSFX;

		public int volAmbient;

		public int resHeight;

		public int resWidth;

		public int graphic;

		public int autoMinimap;

		public bool fullscreen;

		public bool showFpsRtt;

		public bool chatLog;

		public string region;

		public string lastRegion;

		public bool alreadyShowDisclaimerAnalytic;

		public int windowMode;

		public int volVoice;

		public bool vsync;

		public int limitFPS;

		public string lastRoomCode;

		public int lastSeed;

		public int voiceChatMode;

		public int timerCountdown;

		public bool sprintModeToggle;

		public bool hintShowed;

		public bool IsFirstTimeControlShowed;

		public bool SkipIntroControl;

		public bool SkipIntroDialogue;

		public bool EnableTutorial;

		public bool IsTutorialMoveCleared;

		public bool IsTutorialSprintCleared;

		public bool IsTutorialDashCleared;

		public bool IsTutorialMeleeCleared;

		public bool IsTutorialShootCleared;

		public OptionData(bool completed = true)
		{
			gameVer = 1;
			lang = "en-US";
			shakeLevel = 2;
			volMaster = 100;
			volMusic = 100;
			volSFX = 100;
			volAmbient = 100;
			resWidth = Screen.currentResolution.width;
			resHeight = Screen.currentResolution.height;
			fullscreen = Screen.fullScreen;
			showFpsRtt = false;
			chatLog = true;
			graphic = 2;
			autoMinimap = 1;
			region = "";
			lastRegion = "";
			windowMode = 0;
			volVoice = 100;
			vsync = false;
			limitFPS = 3;
			lastRoomCode = "";
			lastSeed = 0;
			voiceChatMode = 1;
			timerCountdown = 0;
			sprintModeToggle = false;
			IsTutorialMoveCleared = false;
			IsTutorialSprintCleared = false;
			IsTutorialDashCleared = false;
			IsTutorialMeleeCleared = false;
			IsTutorialShootCleared = false;
			SkipIntroControl = false;
			SkipIntroDialogue = false;
			EnableTutorial = true;
			IsFirstTimeControlShowed = false;
		}
	}

	[SerializeField]
	private string FusionAppID = "cf631f87-1fe3-4668-ad66-99ef54e0a76a";

	public int buildVer;

	public int langIdx;

	public bool dialogueOnboardingShowed;

	public bool IsTriggerSaveDataOnInitPlayer;

	public bool IsPatchNoteShown;

	public static ES3Settings eS3Settings;

	private static readonly string KEY = "KCsHCt8pvPqeCzpJltIy1SOnzHkTtsA8";

	public static readonly string SAVEFILE_EXTENSION = ".sav";

	public static readonly string INDEX_TAG = "[INDEX]";

	public int[] ListFPS = new int[8] { 24, 30, 40, 60, 90, 120, 144, 360 };

	public List<LanguageObj> arrLang = new List<LanguageObj>();

	public OptionData optionData;

	[SerializeField]
	private UserSaveData userSaveData = new UserSaveData();

	public int currentSelectedDataIndex;

	public GameData gameData;

	public static GlobalSaveData instance;

	public string currentFilePath;

	public static string ROOT_PATH
	{
		get
		{
			if (SteamManager.Initialized)
			{
				return "Data/" + SteamApi.GetAccountId() + "/";
			}
			return "Data/";
		}
	}

	public static string ROOT_DEMO_PATH
	{
		get
		{
			if (SteamManager.Initialized)
			{
				return "DemoData/" + SteamApi.GetAccountId() + "/";
			}
			return "DemoData/";
		}
	}

	public static string OPTION_SAVEDATA_PATH => ROOT_PATH + "OptionData" + SAVEFILE_EXTENSION;

	public static string MULTIPLAYER_SAVEDATA_PATH => ROOT_PATH + "Host/GameData" + INDEX_TAG + SAVEFILE_EXTENSION;

	public static string MULTIPLAYER_CLIENT_SAVEDATA_ROOT_PATH => ROOT_PATH + "Client/";

	public static string MULTIPLAYER_CLIENT_SAVEDATA_PATH => MULTIPLAYER_CLIENT_SAVEDATA_ROOT_PATH + "GameData_" + INDEX_TAG + SAVEFILE_EXTENSION;

	public static string SOLO_SAVEDATA_PATH => ROOT_PATH + "Solo/GameData" + INDEX_TAG + SAVEFILE_EXTENSION;

	public static string MULTIPLAYER_SAVEDATA_DEMO_PATH => ROOT_DEMO_PATH + "Host/GameData" + INDEX_TAG + SAVEFILE_EXTENSION;

	public static string MULTIPLAYER_CLIENT_SAVEDATA_DEMO_ROOT_PATH => ROOT_DEMO_PATH + "Client/";

	public static string MULTIPLAYER_CLIENT_SAVEDATA_DEMO_PATH => MULTIPLAYER_CLIENT_SAVEDATA_DEMO_ROOT_PATH + "GameData_" + INDEX_TAG + SAVEFILE_EXTENSION;

	public static string SOLO_SAVEDATA_DEMO_PATH => ROOT_DEMO_PATH + "Solo/GameData" + INDEX_TAG + SAVEFILE_EXTENSION;

	public static string USER_SAVEDATA_PATH => ROOT_PATH + "UserData" + SAVEFILE_EXTENSION;

	public UserSaveData UserSaveData => userSaveData ?? (userSaveData = LoadUserData());

	private void Awake()
	{
		eS3Settings = GetES3Settings();
		langIdx = 0;
		if (instance == null)
		{
			buildVer = 475;
			instance = this;
			optionData = new OptionData();
			gameData = new GameData();
			LoadOption();
			LoadUser();
			UnityEngine.Object.DontDestroyOnLoad(this);
		}
		else if (instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void LoadOption()
	{
		if (CheckOptionDataExists())
		{
			optionData = LoadOptionData();
			optionData.gameVer = buildVer;
			PhotonAppSettings.Instance.AppSettings.FixedRegion = optionData.lastRegion;
			SaveOptionData();
		}
		else
		{
			SaveOptionData();
		}
		Screen.fullScreen = optionData.fullscreen;
		LocalizationManager.CurrentLanguageCode = optionData.lang;
		Resolution[] resolutions = Screen.resolutions;
		bool flag = false;
		for (int i = 0; i < resolutions.Length; i++)
		{
			if (optionData.resWidth == resolutions[i].width && optionData.resHeight == resolutions[i].height)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			optionData.resWidth = Screen.currentResolution.width;
			optionData.resHeight = Screen.currentResolution.height;
			SaveOptionData();
		}
		if (optionData.fullscreen)
		{
			if (optionData.windowMode == 0)
			{
				Screen.SetResolution(optionData.resWidth, optionData.resHeight, FullScreenMode.FullScreenWindow);
			}
			else
			{
				Screen.SetResolution(optionData.resWidth, optionData.resHeight, FullScreenMode.ExclusiveFullScreen);
			}
		}
		else
		{
			Screen.SetResolution(optionData.resWidth, optionData.resHeight, FullScreenMode.Windowed);
		}
		if (optionData.vsync)
		{
			QualitySettings.vSyncCount = 1;
			return;
		}
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = ListFPS[optionData.limitFPS];
	}

	public void SaveOptionData()
	{
		Debug.Log("Save Option Data");
		try
		{
			ES3.Save("OptionData", optionData, OPTION_SAVEDATA_PATH);
		}
		catch (Exception value)
		{
			ES3.DeleteFile(OPTION_SAVEDATA_PATH);
			ES3.Save("OptionData", optionData, OPTION_SAVEDATA_PATH);
			Console.WriteLine(value);
			throw;
		}
	}

	public bool CheckOptionDataExists()
	{
		try
		{
			return ES3.FileExists(OPTION_SAVEDATA_PATH);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return false;
		}
	}

	public OptionData LoadOptionData()
	{
		return ES3.Load<OptionData>("OptionData", OPTION_SAVEDATA_PATH);
	}

	public void LoadUser()
	{
		if (CheckUserDataExists())
		{
			userSaveData = LoadUserData() ?? new UserSaveData();
			SaveUserData();
		}
		else
		{
			userSaveData = new UserSaveData();
			SaveUserData();
		}
	}

	public UserSaveData LoadUserData()
	{
		try
		{
			return ES3.Load<UserSaveData>("UserSaveData", USER_SAVEDATA_PATH, eS3Settings);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return null;
		}
	}

	public void SaveUserData()
	{
		Debug.Log("Save User Data");
		try
		{
			ES3.Save("UserSaveData", userSaveData, USER_SAVEDATA_PATH, eS3Settings);
		}
		catch (Exception value)
		{
			ES3.DeleteFile(USER_SAVEDATA_PATH);
			ES3.Save("UserSaveData", userSaveData, USER_SAVEDATA_PATH, eS3Settings);
			Console.WriteLine(value);
			throw;
		}
	}

	public bool CheckUserDataExists()
	{
		try
		{
			return ES3.FileExists(USER_SAVEDATA_PATH);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return false;
		}
	}

	public static string GetMultiplayerSaveDataPath()
	{
		if (!GameModes.Instance.isInitDemo)
		{
			return MULTIPLAYER_SAVEDATA_PATH;
		}
		return MULTIPLAYER_SAVEDATA_DEMO_PATH;
	}

	public static string GetMultiplayerClientSaveDataPath()
	{
		if (!GameModes.Instance.isInitDemo)
		{
			return MULTIPLAYER_CLIENT_SAVEDATA_PATH;
		}
		return MULTIPLAYER_CLIENT_SAVEDATA_DEMO_PATH;
	}

	public bool CheckMultiplayerInGameDataExists()
	{
		return CheckMultiplayerInGameDataExists(currentSelectedDataIndex);
	}

	public bool CheckMultiplayerInGameDataExists(int index)
	{
		if (CheckDisableSaveData())
		{
			return false;
		}
		try
		{
			return ES3.FileExists(GetMultiplayerSaveDataPath().Replace(INDEX_TAG, index.ToString()), eS3Settings);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return false;
		}
	}

	public bool CheckMultiplayerClientInGameDataExists(string hostName)
	{
		if (!GameModes.Instance.isInitDemo && CheckDisableSaveData())
		{
			return false;
		}
		try
		{
			return ES3.FileExists(GetMultiplayerClientSaveDataPath().Replace(INDEX_TAG, userSaveData.UserUniqueId + "_" + hostName), eS3Settings);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return false;
		}
	}

	public GameData LoadMultiplayerGameData()
	{
		return LoadMultiplayerGameData(currentSelectedDataIndex);
	}

	public GameData LoadMultiplayerGameData(int index)
	{
		try
		{
			return ES3.Load<GameData>("GameData", GetMultiplayerSaveDataPath().Replace(INDEX_TAG, index.ToString()), eS3Settings);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return null;
		}
	}

	public GameData LoadMultiplayerClientGameData(string hostName)
	{
		try
		{
			return ES3.Load<GameData>("GameData", GetMultiplayerClientSaveDataPath().Replace(INDEX_TAG, userSaveData.UserUniqueId + "_" + hostName), eS3Settings);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return null;
		}
	}

	public static void DeleteHostSaveData(int index)
	{
		DeleteSaveData(GetMultiplayerSaveDataPath().Replace(INDEX_TAG, index.ToString()));
	}

	public static string GetSoloSaveDataPath()
	{
		if (!GameModes.Instance.isInitDemo)
		{
			return SOLO_SAVEDATA_PATH;
		}
		return SOLO_SAVEDATA_DEMO_PATH;
	}

	public bool CheckInGameDataExists()
	{
		return CheckInGameDataExists(currentSelectedDataIndex);
	}

	public bool CheckInGameDataExists(int index)
	{
		if (CheckDisableSaveData())
		{
			return false;
		}
		try
		{
			return ES3.FileExists(GetSoloSaveDataPath().Replace(INDEX_TAG, index.ToString()), eS3Settings);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return false;
		}
	}

	public static void DeleteSoloSaveData(int index)
	{
		DeleteSaveData(GetSoloSaveDataPath().Replace(INDEX_TAG, index.ToString()));
	}

	public GameData LoadSoloGameData()
	{
		return LoadSoloGameData(currentSelectedDataIndex);
	}

	public GameData LoadSoloGameData(int index)
	{
		try
		{
			return ES3.Load<GameData>("GameData", GetSoloSaveDataPath().Replace(INDEX_TAG, index.ToString()), eS3Settings);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			return null;
		}
	}

	private void SaveGameData()
	{
		Debug.Log("Save Game Data");
		string currentGameDataPath = GetCurrentGameDataPath();
		if (!string.IsNullOrWhiteSpace(currentGameDataPath) && gameData != null)
		{
			gameData.SetCurrentSaveDateTime();
			SaveGameData(currentGameDataPath);
		}
	}

	private void SaveClientGameData(string serverName)
	{
		if (!string.IsNullOrWhiteSpace(serverName))
		{
			string currentGameDataPath = GetCurrentGameDataPath();
			if (!string.IsNullOrWhiteSpace(currentGameDataPath) && gameData != null)
			{
				gameData.SetCurrentSaveDateTime();
				currentGameDataPath = currentGameDataPath.Replace(INDEX_TAG, userSaveData.UserUniqueId + "_" + serverName);
				Debug.Log("Save Client Game Data Session:" + serverName + " Path:" + currentGameDataPath);
				SaveGameData(currentGameDataPath);
			}
		}
	}

	private void SaveGameData(string pathData)
	{
		try
		{
			ES3.Save("GameData", gameData, pathData, eS3Settings);
		}
		catch (Exception value)
		{
			ES3.DeleteFile(pathData);
			ES3.Save("GameData", gameData, pathData, eS3Settings);
			Console.WriteLine(value);
			throw;
		}
		SaveUserData();
		GlobalUIManager.Instance.ShowSaveIcon();
	}

	public void SaveGameData(PlayerController playerController, GameManagerPhoton gameManagerPhoton)
	{
		if (!(playerController == null) && playerController.data.firstInitialized)
		{
			if (gameData == null)
			{
				gameData = new GameData();
			}
			gameData.SetData(playerController, gameManagerPhoton);
			SaveCurrentGameData(gameData.SessionName);
		}
	}

	public void SaveCurrentGameData(string serverName)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			SaveGameData();
		}
		else
		{
			SaveClientGameData(serverName);
		}
	}

	public void SavePlayerDataGameData(PlayerController playerController)
	{
		if (!(playerController == null) && playerController.data.firstInitialized)
		{
			gameData.SetPlayerData(playerController);
			SaveCurrentGameData();
		}
	}

	public void SaveCurrentGameData()
	{
		SaveCurrentGameData(gameData.SessionName);
	}

	public void ResetGameData()
	{
		gameData = null;
		currentSelectedDataIndex = 0;
	}

	private string GetCurrentGameDataPath()
	{
		if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
		{
			if (CheckDisableSaveData())
			{
				return "";
			}
			return GetSoloSaveDataPath().Replace(INDEX_TAG, currentSelectedDataIndex.ToString());
		}
		if (NetworkGameManager.Instance.isServer)
		{
			if (CheckDisableSaveData())
			{
				return "";
			}
			return GetMultiplayerSaveDataPath().Replace(INDEX_TAG, currentSelectedDataIndex.ToString());
		}
		if (GameModes.Instance.isInitDemo)
		{
			return GetMultiplayerClientSaveDataPath();
		}
		if (CheckDisableSaveData())
		{
			return "";
		}
		return GetMultiplayerClientSaveDataPath();
	}

	public static ES3Settings GetES3Settings()
	{
		return new ES3Settings(ES3.EncryptionType.AES, KEY);
	}

	public static void DeleteSaveData(string path)
	{
		try
		{
			ES3.DeleteFile(path);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			throw;
		}
	}

	private bool CheckDisableSaveData()
	{
		return GameModes.Instance.CheckDisableSaveData();
	}

	public static void DeleteClientSaveFileContains(string containsName)
	{
		if (string.IsNullOrWhiteSpace(containsName))
		{
			return;
		}
		string path = Path.Join(Application.dataPath, GameModes.Instance.isInitDemo ? MULTIPLAYER_CLIENT_SAVEDATA_DEMO_ROOT_PATH : MULTIPLAYER_CLIENT_SAVEDATA_ROOT_PATH);
		if (!Directory.Exists(path))
		{
			return;
		}
		IEnumerable<string> enumerable = from f in Directory.GetFiles(path, "*" + SAVEFILE_EXTENSION, SearchOption.AllDirectories)
			where f.Contains(containsName, StringComparison.OrdinalIgnoreCase)
			select f;
		foreach (string item in enumerable)
		{
			MonoBehaviour.print("Delete " + item);
			DeleteSaveData(item);
		}
		if (!enumerable.Any())
		{
			MonoBehaviour.print("no save files found.");
		}
	}

	public void AddGameStatisticProgress(GameStatisticType statisticType, int value, string additionalKey = "")
	{
		if (!GameModes.Instance.CheckDisableMetaProgression())
		{
			UserSaveData?.AddGameStatisticProgress(GameStatisticData.ConvertToKey(statisticType, additionalKey), value);
		}
	}
}
