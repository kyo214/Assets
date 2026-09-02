using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doozy.Runtime.Common.Extensions;
using Fusion;
using Fusion.Photon.Realtime;
using I2.Loc;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Modules.UIGlobal;

public class PhotonMultiplayerManager : MonoBehaviour
{
	public float areaOfInterest;

	public float areaOfInterestOffset;

	public string activeIngameScene;

	public bool sceneLoaded;

	public string roomType;

	public string buildType;

	public bool disableSessionCreation;

	public GameMode photonGameMode;

	public bool _sessionConnected;

	public bool _ListRoomUpdated;

	public static readonly int MAX_PLAYERS = 4;

	public NetworkRunner _runner;

	public static PhotonMultiplayerManager Instance { get; private set; }

	public static event Action<NetworkGameManager.MultiplayerMode, PhotonMultiplayerManager> OnStartServer;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		_runner = base.gameObject.AddComponent<NetworkRunner>();
	}

	private void Start()
	{
		areaOfInterest = 28f;
		areaOfInterestOffset = areaOfInterest - 0.1f;
		NetworkGameManager.Instance.photonNetworking = this;
		PhotonAppSettings.Instance.AppSettings.Server = "";
	}

	public void RandomIdSession()
	{
		string text = "0123456789QWERTYUIOPASDFGHJKLZCVBNM";
		NetworkGameManager.Instance.sessionName = "";
		bool flag = true;
		while (flag)
		{
			for (int i = 0; i < 6; i++)
			{
				NetworkGameManager.Instance.sessionName = NetworkGameManager.Instance.sessionName + text.Substring(UnityEngine.Random.Range(0, text.Length), 1);
			}
			flag = false;
			foreach (SessionInfo session in NetworkGameManager.Instance.sessionList)
			{
				if (session.Name == NetworkGameManager.Instance.sessionName)
				{
					flag = true;
				}
			}
		}
	}

	public async void StartGame(NetworkGameManager.MultiplayerMode mode, string roomCode)
	{
		if (!NetworkGameManager.Instance.sessionName.IsNullOrEmpty() && NetworkGameManager.Instance.sessionName.IndexOf("Test", StringComparison.Ordinal) >= 0)
		{
			_sessionConnected = true;
		}
		if (_sessionConnected || mode == NetworkGameManager.MultiplayerMode.Solo || mode == NetworkGameManager.MultiplayerMode.Server)
		{
			NetworkGameManager.Instance.SpawnedCharacters.Clear();
			Debug.Log("Connecting");
			switch (mode)
			{
			case NetworkGameManager.MultiplayerMode.Server:
				if (roomType == "")
				{
					roomType = "Private";
				}
				if (buildType == "")
				{
					buildType = "Test";
				}
				if (roomCode == "")
				{
					roomCode = NetworkGameManager.Instance.sessionName;
				}
				photonGameMode = GameMode.Host;
				break;
			case NetworkGameManager.MultiplayerMode.Client:
				photonGameMode = GameMode.Client;
				break;
			}
			_runner.ProvideInput = true;
			Dictionary<string, SessionProperty> sessionProperties = new Dictionary<string, SessionProperty>
			{
				["buildVer"] = GlobalSaveData.instance.buildVer,
				["status"] = "Open",
				["RoomType"] = roomType,
				["BuildType"] = buildType,
				["PlayersDisconnect"] = "",
				["Difficulty"] = (int)GameModes.Instance.GetDifficultyData().DifficultySetting
			};
			string deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
			byte[] bytes = Encoding.UTF8.GetBytes(deviceUniqueIdentifier);
			if (roomCode.IsNullOrWhitespace() || roomCode.IndexOf("Error", StringComparison.Ordinal) < 0)
			{
				GlobalSaveData.instance.optionData.lastRegion = PhotonAppSettings.Instance.AppSettings.FixedRegion;
				GlobalSaveData.instance.SaveOptionData();
				if (mode == NetworkGameManager.MultiplayerMode.Solo)
				{
					StartGameResult startGameResult = await _runner.StartGame(new StartGameArgs
					{
						SessionName = roomCode,
						CustomLobbyName = "WMO",
						GameMode = photonGameMode,
						SessionProperties = sessionProperties,
						PlayerCount = MAX_PLAYERS,
						ObjectPool = NetworkGameManager.Instance.networkPool,
						Scene = SceneManager.GetSceneByName("Lobby").buildIndex,
						SceneManager = base.gameObject.AddComponent<NetworkSceneManagerDefault>(),
						CustomPhotonAppSettings = PhotonAppSettings.Instance.AppSettings,
						IsOpen = false,
						IsVisible = false
					});
					if (startGameResult.Ok)
					{
						Debug.Log("Connection all good");
					}
					else
					{
						Debug.Log($"Failed to Start: {startGameResult.ShutdownReason}");
					}
				}
				else
				{
					StartGameResult startGameResult2 = await _runner.StartGame(new StartGameArgs
					{
						SessionName = roomCode,
						CustomLobbyName = "WMO",
						GameMode = photonGameMode,
						SessionProperties = sessionProperties,
						PlayerCount = MAX_PLAYERS,
						ObjectPool = NetworkGameManager.Instance.networkPool,
						Scene = SceneManager.GetSceneByName("Lobby").buildIndex,
						SceneManager = base.gameObject.AddComponent<NetworkSceneManagerDefault>(),
						DisableClientSessionCreation = disableSessionCreation,
						CustomPhotonAppSettings = PhotonAppSettings.Instance.AppSettings,
						IsOpen = true,
						IsVisible = true,
						ConnectionToken = bytes
					});
					if (startGameResult2.Ok)
					{
						Debug.Log("Connection all good");
					}
					else
					{
						Debug.Log($"Failed to Start: {startGameResult2.ShutdownReason}");
						if (mode == NetworkGameManager.MultiplayerMode.Server)
						{
							UIGameManager.Instance.ShowFailedConnect("ErrorCreateRoom");
						}
						else if (startGameResult2.ShutdownReason == ShutdownReason.GameNotFound)
						{
							UIGameManager.Instance.ShowFailedConnect("ErrorRoomNotFound");
						}
						else if (startGameResult2.ShutdownReason == ShutdownReason.GameIsFull)
						{
							UIGameManager.Instance.ShowFailedConnect("ErrorRoomFul");
						}
						else
						{
							UIGameManager.Instance.ShowFailedConnect("ErrorJoinRoom");
						}
					}
				}
				OnStartServer?.Invoke(mode, this);
			}
			else
			{
				Debug.Log("Failed to Start: " + roomCode);
				UIGameManager.Instance.ShowFailedConnect(roomCode);
			}
		}
		else
		{
			UIGameManager.Instance.ShowFailedConnect("ErrorConnection");
		}
	}

	public void DespawnObject(GameObject theObject)
	{
		_runner.Despawn(theObject.GetComponent<NetworkObject>());
	}

	public void Shutdown()
	{
		Debug.Log("Shutdown");
		if (_runner != null)
		{
			if (NetworkGameManager.Instance.mode != NetworkGameManager.MultiplayerMode.Solo)
			{
				_runner.Shutdown();
			}
			UnityEngine.Object.Destroy(_runner);
		}
	}

	public async void JoinSession(NetworkGameManager.MultiplayerMode mode, string roomCode)
	{
		Debug.Log("Joining Session");
		GameModes.Instance.isDemo = GameModes.Instance.isInitDemo;
		bool flag = false;
		if (NetworkGameManager.Instance.sessionName != null && NetworkGameManager.Instance.sessionName.IndexOf("Test") >= 0)
		{
			flag = true;
		}
		if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
		{
			NetworkGameManager.Instance.sessionName = null;
		}
		else if ((NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Server || NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Auto) && !flag)
		{
			RandomIdSession();
			roomCode = NetworkGameManager.Instance.sessionName;
		}
		Debug.Log("Start Game");
		int ctr = 0;
		_sessionConnected = false;
		Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
		dictionary.Add("asia", new string[7] { "asia", "jp", "kr", "eu", "usw", "us", "sa" });
		dictionary.Add("jp", new string[7] { "jp", "kr", "asia", "eu", "usw", "us", "sa" });
		dictionary.Add("kr", new string[7] { "kr", "jp", "asia", "eu", "usw", "us", "sa" });
		dictionary.Add("eu", new string[7] { "eu", "us", "usw", "kr", "jp", "asia", "sa" });
		dictionary.Add("us", new string[7] { "us", "usw", "eu", "sa", "kr", "jp", "asia" });
		dictionary.Add("usw", new string[7] { "usw", "us", "eu", "kr", "jp", "asia", "sa" });
		dictionary.Add("sa", new string[7] { "sa", "us", "usw", "eu", "kr", "jp", "asia" });
		Dictionary<string, string[]> dictionary2 = dictionary;
		List<string> list = new List<string>();
		if (mode == NetworkGameManager.MultiplayerMode.Auto)
		{
			Debug.Log("--CEK Listing Region");
			string[] array = dictionary2[GlobalSaveData.instance.optionData.region];
			foreach (string item in array)
			{
				list.Add(item);
			}
		}
		else if (NetworkGameManager.Instance.isReconnecting)
		{
			list.Add(GlobalSaveData.instance.optionData.lastRegion);
		}
		else
		{
			list.Add(GlobalSaveData.instance.optionData.region);
		}
		roomType = "Public";
		buildType = "Release";
		if (GameModes.Instance.isDemo)
		{
			buildType = "Demo";
		}
		bool sessionRegionConnected = false;
		foreach (string region in list)
		{
			sessionRegionConnected = false;
			AppSettings customAppSetting = new AppSettings
			{
				AppIdFusion = PhotonAppSettings.Instance.AppSettings.AppIdFusion,
				FixedRegion = region
			};
			PhotonAppSettings.Instance.AppSettings.FixedRegion = region;
			_ListRoomUpdated = false;
			while (!sessionRegionConnected && ctr < 20 && mode != NetworkGameManager.MultiplayerMode.Solo)
			{
				if (mode == NetworkGameManager.MultiplayerMode.Auto)
				{
					UITitleMenuManager.Instance.FindingRoomText.text = LocalizationManager.GetTranslation("Menu/FindingRoom").ToUpper();
					Debug.Log(region + " " + GlobalOptionsManager.Instance.GetRegionName(region));
					UITitleMenuManager.Instance.RegionText.text = GlobalOptionsManager.Instance.GetRegionName(region);
				}
				Debug.Log("--CEK Session Joining " + region);
				StartGameResult resultSession = await _runner.JoinSessionLobby(SessionLobby.ClientServer, "WMO", null, customAppSetting, false);
				Debug.Log("--CEK Session Joined " + region);
				await Task.Delay(TimeSpan.FromSeconds(0.5));
				Debug.Log(resultSession.ToString());
				if (resultSession.Ok)
				{
					sessionRegionConnected = true;
				}
				ctr++;
			}
			for (ctr = 0; ctr < 20; ctr++)
			{
				if (mode == NetworkGameManager.MultiplayerMode.Solo)
				{
					break;
				}
				if (_ListRoomUpdated)
				{
					break;
				}
			}
			if (sessionRegionConnected || mode == NetworkGameManager.MultiplayerMode.Solo)
			{
				Debug.Log("--CEK Connecting");
				GlobalOptionsManager.Instance.seed = GlobalSaveData.instance.gameData?.GetCurrentSeed() ?? 0;
				photonGameMode = GameMode.Single;
				disableSessionCreation = true;
				bool flag2 = false;
				switch (mode)
				{
				case NetworkGameManager.MultiplayerMode.Server:
					photonGameMode = GameMode.Host;
					if (NetworkGameManager.Instance.isPrivateRoom)
					{
						roomType = "Private";
					}
					break;
				case NetworkGameManager.MultiplayerMode.Client:
				{
					photonGameMode = GameMode.Client;
					bool flag4 = true;
					bool flag5 = true;
					bool flag6 = false;
					bool flag7 = true;
					foreach (SessionInfo session in NetworkGameManager.Instance.sessionList)
					{
						if (!(session.Name == roomCode) || !session.IsValid || !session.IsVisible || !session.IsOpen)
						{
							continue;
						}
						flag6 = true;
						if ((int)session.Properties["buildVer"] != GlobalSaveData.instance.buildVer)
						{
							flag4 = false;
							break;
						}
						if (session.Properties["status"].PropertyValue.ToString() != "Open")
						{
							flag5 = false;
						}
						int num2 = session.PlayerCount;
						if (session.Properties["PlayersDisconnect"] != "")
						{
							num2 += session.Properties["PlayersDisconnect"].ToString().Count((char c) => c == '|');
							Debug.Log("Total player on server = " + num2);
							if (session.Properties["PlayersDisconnect"].ToString().IndexOf(SystemInfo.deviceUniqueIdentifier, StringComparison.Ordinal) >= 0)
							{
								Debug.Log("Disconnected player join room");
								flag2 = true;
								NetworkGameManager.Instance.isReconnecting = true;
								num2--;
								flag7 = false;
							}
						}
						Debug.Log("PlayerCount = " + num2);
						if (num2 < MAX_PLAYERS && !flag2)
						{
							flag7 = false;
						}
						roomType = session.Properties["RoomType"].PropertyValue.ToString();
						buildType = session.Properties["BuildType"].PropertyValue.ToString();
						if (buildType == "Demo")
						{
							GameModes.Instance.isDemo = true;
						}
						else
						{
							GameModes.Instance.isDemo = false;
						}
						break;
					}
					if (!flag6)
					{
						NetworkGameManager.Instance.sessionName = "ErrorRoomNotFound";
					}
					else if (!flag4)
					{
						NetworkGameManager.Instance.sessionName = "ErrorVersion";
					}
					else if (!flag5 && !flag2)
					{
						NetworkGameManager.Instance.sessionName = "ErrorJoinRoom";
					}
					else if (flag7)
					{
						NetworkGameManager.Instance.sessionName = "ErrorRoomFul";
					}
					_sessionConnected = true;
					break;
				}
				case NetworkGameManager.MultiplayerMode.Auto:
					photonGameMode = GameMode.AutoHostOrClient;
					Debug.Log("--CEK Check Session");
					roomCode = "";
					foreach (SessionInfo session2 in NetworkGameManager.Instance.sessionList)
					{
						Debug.Log("--CEK Session Name = " + session2.Name + "     Region = " + session2.Region);
						int num = session2.PlayerCount;
						bool flag3 = true;
						if (session2.Properties.ContainsKey("Difficulty") && (int)session2.Properties["Difficulty"].PropertyValue != (int)GameModes.Instance.GetDifficultyData().DifficultySetting)
						{
							flag3 = false;
						}
						if (session2.Properties.ContainsKey("PlayersDisconnect"))
						{
							num += session2.Properties["PlayersDisconnect"].ToString().Count((char c) => c == '|');
							Debug.Log("Total player on server = " + num);
							if (session2.Properties["PlayersDisconnect"].ToString().IndexOf(SystemInfo.deviceUniqueIdentifier, StringComparison.Ordinal) >= 0)
							{
								Debug.Log("Disconnected player join room");
								NetworkGameManager.Instance.isReconnecting = true;
								num--;
							}
						}
						if (flag3 && session2.Properties.ContainsKey("RoomType") && session2.Properties["RoomType"].PropertyValue.ToString() == "Public" && session2.IsValid && session2.IsVisible && session2.IsOpen && (int)session2.Properties["buildVer"].PropertyValue == GlobalSaveData.instance.buildVer && num < 4 && session2.Properties["status"].PropertyValue.ToString() == "Open")
						{
							roomCode = session2.Name;
							NetworkGameManager.Instance.sessionName = roomCode;
							_sessionConnected = true;
							mode = NetworkGameManager.MultiplayerMode.Client;
							Debug.Log("--CEK Session Connected");
							break;
						}
					}
					break;
				}
			}
			if (!_sessionConnected && mode == NetworkGameManager.MultiplayerMode.Auto)
			{
				UnityEngine.Object.Destroy(_runner);
				await Task.Delay(TimeSpan.FromSeconds(0.10000000149011612));
				_runner = base.gameObject.AddComponent<NetworkRunner>();
				await Task.Delay(TimeSpan.FromSeconds(0.10000000149011612));
				continue;
			}
			break;
		}
		Debug.Log("--CEK Session Connected = " + _sessionConnected);
		if ((sessionRegionConnected && mode == NetworkGameManager.MultiplayerMode.Server) || mode == NetworkGameManager.MultiplayerMode.Solo)
		{
			_sessionConnected = true;
		}
		if (sessionRegionConnected && mode == NetworkGameManager.MultiplayerMode.Auto && !_sessionConnected)
		{
			UITitleMenuManager.Instance.playerInput.ActivateInput();
			GenericSingleton<PopupUIManager>.Instance.Show(PopupUIManager.Type.YesNo, "Menu/UnableFindRoom", async () =>
			{
				NetworkGameManager.Instance.mode = NetworkGameManager.MultiplayerMode.Auto;
				PhotonAppSettings.Instance.AppSettings.FixedRegion = GlobalSaveData.instance.optionData.region;
				sessionRegionConnected = false;
				_sessionConnected = false;
				JoinSession(NetworkGameManager.Instance.mode, NetworkGameManager.Instance.sessionName);
			}, () =>
			{
				if ((bool)UITitleMenuManager.Instance)
				{
					UITitleMenuManager.Instance.LoadingText.gameObject.SetActive(value: true);
					UITitleMenuManager.Instance.FindingRoomText.gameObject.SetActive(value: false);
				}
				GlobalUIManager.Instance.ClickGoToScene("MainMenu");
			});
		}
		if (!sessionRegionConnected && mode != NetworkGameManager.MultiplayerMode.Solo)
		{
			UITitleMenuManager.Instance.playerInput.ActivateInput();
			GenericSingleton<PopupUIManager>.Instance.Show(PopupUIManager.Type.OK, "Menu/ErrorConnection", () =>
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			});
		}
	}

	public void UpdateSessionDisconnectedPlayer()
	{
		string text = "";
		foreach (string item in NetworkGameManager.Instance.arrPlayerIDDisconnected)
		{
			text = text + item + "|";
		}
		Dictionary<string, SessionProperty> customProperties = new Dictionary<string, SessionProperty> { ["PlayersDisconnect"] = text };
		_runner.SessionInfo.UpdateCustomProperties(customProperties);
	}
}
