using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
[AddComponentMenu("Fusion/Prototyping/Network Debug Start")]
[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
public class NetworkDebugStart : Fusion.Behaviour
{
	public enum StartModes
	{
		UserInterface = 0,
		Automatic = 1,
		Manual = 2
	}

	public enum Stage
	{
		Disconnected = 0,
		StartingUp = 1,
		UnloadOriginalScene = 2,
		ConnectingServer = 3,
		ConnectingClients = 4,
		AllConnected = 5
	}

	[InlineHelp]
	[WarnIf("RunnerPrefab", false, "No RunnerPrefab supplied. Will search for a NetworkRunner in the scene at startup.")]
	[MultiPropertyDrawersFix]
	public NetworkRunner RunnerPrefab;

	[InlineHelp]
	[MultiPropertyDrawersFix]
	[WarnIf("StartMode", 2.0, "Start network by calling the methods StartHost(), StartServer(), StartClient(), StartHostPlusClients(), or StartServerPlusClients()", MsgType = 1)]
	public StartModes StartMode;

	[InlineHelp]
	[FormerlySerializedAs("Server")]
	[DrawIf("StartMode", 1.0, Hide = true)]
	public GameMode AutoStartAs = GameMode.Shared;

	[InlineHelp]
	[DrawIf("StartMode", 0.0, Hide = true)]
	public bool AutoHideGUI = true;

	[InlineHelp]
	[DrawIf("ShowAutoClients", Hide = true)]
	public int AutoClients = 1;

	[InlineHelp]
	public ushort ServerPort = 27015;

	[InlineHelp]
	public string DefaultRoomName = "";

	[InlineHelp]
	public bool AlwaysShowStats;

	[NonSerialized]
	private NetworkRunner _server;

	[InlineHelp]
	[ScenePath]
	[MultiPropertyDrawersFix]
	public string InitialScenePath;

	private static string _initialScenePath;

	[InlineHelp]
	[SerializeField]
	[EditorDisabled(false)]
	[MultiPropertyDrawersFix]
	protected Stage _currentStage;

	public Stage CurrentStage
	{
		get
		{
			return _currentStage;
		}
		internal set
		{
			_currentStage = value;
		}
	}

	public int LastCreatedClientIndex { get; internal set; }

	public GameMode CurrentServerMode { get; internal set; }

	protected bool CanAddClients
	{
		get
		{
			if (CurrentStage == Stage.AllConnected && CurrentServerMode > (GameMode)0 && CurrentServerMode != GameMode.Shared)
			{
				return CurrentServerMode != GameMode.Single;
			}
			return false;
		}
	}

	protected bool CanAddSharedClients
	{
		get
		{
			if (CurrentStage == Stage.AllConnected && CurrentServerMode > (GameMode)0)
			{
				return CurrentServerMode == GameMode.Shared;
			}
			return false;
		}
	}

	protected bool IsShutdown => CurrentStage == Stage.Disconnected;

	protected bool IsShutdownAndMultiPeer
	{
		get
		{
			if (CurrentStage == Stage.Disconnected)
			{
				return UsingMultiPeerMode;
			}
			return false;
		}
	}

	protected bool UsingMultiPeerMode => NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple;

	protected bool ShowAutoClients
	{
		get
		{
			if (StartMode != StartModes.Manual && UsingMultiPeerMode)
			{
				return AutoStartAs != GameMode.Single;
			}
			return false;
		}
	}

	protected virtual void Start()
	{
		if (_initialScenePath == null)
		{
			if (string.IsNullOrEmpty(InitialScenePath))
			{
				Scene activeScene = SceneManager.GetActiveScene();
				if (activeScene.IsValid())
				{
					_initialScenePath = activeScene.path;
				}
				else
				{
					_initialScenePath = SceneManager.GetSceneByBuildIndex(0).path;
				}
				InitialScenePath = _initialScenePath;
			}
			else
			{
				_initialScenePath = InitialScenePath;
			}
		}
		bool flag = NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple;
		NetworkRunner networkRunner = UnityEngine.Object.FindObjectOfType<NetworkRunner>();
		if ((bool)networkRunner && networkRunner != RunnerPrefab)
		{
			if (networkRunner.State != NetworkRunner.States.Shutdown)
			{
				base.enabled = false;
				NetworkDebugStartGUI component = GetComponent<NetworkDebugStartGUI>();
				if ((bool)component)
				{
					UnityEngine.Object.Destroy(component);
				}
				UnityEngine.Object.Destroy(this);
				return;
			}
			if (RunnerPrefab == null)
			{
				RunnerPrefab = networkRunner;
			}
		}
		if (StartMode == StartModes.Manual)
		{
			return;
		}
		NetworkDebugStartGUI component2;
		if (StartMode == StartModes.Automatic)
		{
			if (TryGetSceneRef(out var sceneRef))
			{
				StartCoroutine(StartWithClients(AutoStartAs, sceneRef, flag ? AutoClients : ((AutoStartAs == GameMode.Client || AutoStartAs == GameMode.Shared || AutoStartAs == GameMode.AutoHostOrClient) ? 1 : 0)));
			}
		}
		else if (!TryGetComponent<NetworkDebugStartGUI>(out component2))
		{
			base.gameObject.AddComponent<NetworkDebugStartGUI>();
		}
	}

	protected bool TryGetSceneRef(out SceneRef sceneRef)
	{
		Scene activeScene = SceneManager.GetActiveScene();
		if (activeScene.buildIndex < 0 || activeScene.buildIndex >= SceneManager.sceneCountInBuildSettings)
		{
			sceneRef = default;
			return false;
		}
		sceneRef = activeScene.buildIndex;
		return true;
	}

	[BehaviourButtonAction("StartSinglePlayer", true, false, "IsShutdown")]
	public virtual void StartSinglePlayer()
	{
		if (TryGetSceneRef(out var sceneRef))
		{
			StartCoroutine(StartWithClients(GameMode.Single, sceneRef, 0));
		}
	}

	[BehaviourButtonAction("StartServer", true, false, "IsShutdown")]
	public virtual void StartServer()
	{
		if (TryGetSceneRef(out var sceneRef))
		{
			StartCoroutine(StartWithClients(GameMode.Server, sceneRef, 0));
		}
	}

	[BehaviourButtonAction("StartHost", true, false, "IsShutdown")]
	public virtual void StartHost()
	{
		if (TryGetSceneRef(out var sceneRef))
		{
			StartCoroutine(StartWithClients(GameMode.Host, sceneRef, 0));
		}
	}

	[BehaviourButtonAction("Start Client", true, false, "IsShutdown")]
	public virtual void StartClient()
	{
		StartCoroutine(StartWithClients(GameMode.Client, default, 1));
	}

	[BehaviourButtonAction("Start Shared Client", true, false, "IsShutdown")]
	public virtual void StartSharedClient()
	{
		if (TryGetSceneRef(out var sceneRef))
		{
			StartCoroutine(StartWithClients(GameMode.Shared, sceneRef, 1));
		}
	}

	[BehaviourButtonAction("Start Auto Host Or Client", true, false, "IsShutdown")]
	public virtual void StartAutoClient()
	{
		if (TryGetSceneRef(out var sceneRef))
		{
			StartCoroutine(StartWithClients(GameMode.AutoHostOrClient, sceneRef, 1));
		}
	}

	[BehaviourButtonAction("Start Server Plus Clients", true, false, "IsShutdownAndMultiPeer")]
	public virtual void StartServerPlusClients()
	{
		StartServerPlusClients(AutoClients);
	}

	[BehaviourButtonAction("Start Host Plus Clients", true, false, "IsShutdownAndMultiPeer")]
	public void StartHostPlusClients()
	{
		StartHostPlusClients(AutoClients);
	}

	[BehaviourButtonAction("Shutdown", true, false, "CurrentStage")]
	public void Shutdown()
	{
		ShutdownAll();
	}

	public virtual void StartServerPlusClients(int clientCount)
	{
		if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			if (TryGetSceneRef(out var sceneRef))
			{
				StartCoroutine(StartWithClients(GameMode.Server, sceneRef, clientCount));
			}
		}
		else
		{
			Debug.LogWarning("Unable to start multiple NetworkRunners in Unique Instance mode.");
		}
	}

	public void StartHostPlusClients(int clientCount)
	{
		if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			if (TryGetSceneRef(out var sceneRef))
			{
				StartCoroutine(StartWithClients(GameMode.Host, sceneRef, clientCount));
			}
		}
		else
		{
			Debug.LogWarning("Unable to start multiple NetworkRunners in Unique Instance mode.");
		}
	}

	public void StartMultipleClients(int clientCount)
	{
		if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			if (TryGetSceneRef(out var sceneRef))
			{
				StartCoroutine(StartWithClients(GameMode.Client, sceneRef, clientCount));
			}
		}
		else
		{
			Debug.LogWarning("Unable to start multiple NetworkRunners in Unique Instance mode.");
		}
	}

	public void StartMultipleSharedClients(int clientCount)
	{
		if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			if (TryGetSceneRef(out var sceneRef))
			{
				StartCoroutine(StartWithClients(GameMode.Shared, sceneRef, clientCount));
			}
		}
		else
		{
			Debug.LogWarning("Unable to start multiple NetworkRunners in Unique Instance mode.");
		}
	}

	public void StartMultipleAutoClients(int clientCount)
	{
		if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			if (TryGetSceneRef(out var sceneRef))
			{
				StartCoroutine(StartWithClients(GameMode.AutoHostOrClient, sceneRef, clientCount));
			}
		}
		else
		{
			Debug.LogWarning("Unable to start multiple NetworkRunners in Unique Instance mode.");
		}
	}

	public void ShutdownAll()
	{
		foreach (NetworkRunner item in NetworkRunner.Instances.ToList())
		{
			if (item != null && item.IsRunning)
			{
				item.Shutdown();
			}
		}
		SceneManager.LoadSceneAsync(_initialScenePath);
		UnityEngine.Object.Destroy(RunnerPrefab.gameObject);
		UnityEngine.Object.Destroy(base.gameObject);
		CurrentStage = Stage.Disconnected;
		CurrentServerMode = (GameMode)0;
	}

	protected IEnumerator StartWithClients(GameMode serverMode, SceneRef sceneRef, int clientCount)
	{
		if (CurrentStage != Stage.Disconnected)
		{
			yield break;
		}
		bool includesServerStart = serverMode != GameMode.Shared && serverMode != GameMode.Client && serverMode != GameMode.AutoHostOrClient;
		if (!includesServerStart && clientCount == 0)
		{
			Debug.LogError(string.Format("{0} is set to {1}, and {2} is set to zero. Starting no network runners.", "GameMode", serverMode, "clientCount"));
			yield break;
		}
		CurrentStage = Stage.StartingUp;
		SceneManager.GetActiveScene();
		if (!RunnerPrefab)
		{
			Debug.LogError("RunnerPrefab not set, can't perform debug start.");
			yield break;
		}
		RunnerPrefab = UnityEngine.Object.Instantiate(RunnerPrefab);
		UnityEngine.Object.DontDestroyOnLoad(RunnerPrefab);
		RunnerPrefab.name = "Temporary Runner Prefab";
		NetworkProjectConfig global = NetworkProjectConfig.Global;
		if (global.PeerMode != NetworkProjectConfig.PeerModes.Multiple)
		{
			int num = ((!includesServerStart) ? 1 : 0);
			if (clientCount > num)
			{
				Debug.LogWarning(string.Format("Instance mode must be set to {0} to perform a debug start multiple peers. Restricting client count to {1}.", "Multiple", num));
				clientCount = num;
			}
		}
		if ((serverMode == GameMode.Shared || serverMode == GameMode.AutoHostOrClient || serverMode == GameMode.Server || serverMode == GameMode.Host) && clientCount > 1 && global.PeerMode == NetworkProjectConfig.PeerModes.Multiple && string.IsNullOrEmpty(DefaultRoomName))
		{
			DefaultRoomName = Guid.NewGuid().ToString();
			Debug.Log("Generated Session Name: " + DefaultRoomName);
		}
		if ((bool)base.gameObject.transform.parent)
		{
			Debug.LogWarning("NetworkDebugStart can't be a child game object, un-parenting.");
			base.gameObject.transform.parent = null;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		CurrentServerMode = serverMode;
		if (includesServerStart)
		{
			_server = UnityEngine.Object.Instantiate(RunnerPrefab);
			_server.name = serverMode.ToString();
			Task serverTask = InitializeNetworkRunner(_server, serverMode, NetAddress.Any(ServerPort), sceneRef, (NetworkRunner runner) =>
			{
			});
			while (!serverTask.IsCompleted)
			{
				yield return new WaitForSeconds(1f);
			}
			if (serverTask.IsFaulted)
			{
				ShutdownAll();
				yield break;
			}
			yield return StartClients(clientCount, serverMode, sceneRef);
		}
		else
		{
			yield return StartClients(clientCount, serverMode, sceneRef);
		}
		if (includesServerStart && AlwaysShowStats && serverMode != GameMode.Shared)
		{
			FusionStats.Create(null, _server, FusionStats.DefaultLayouts.Left, FusionStats.DefaultLayouts.Left);
		}
	}

	[BehaviourButtonAction("Add Additional Client", null, "CanAddClients")]
	public void AddClient()
	{
		if (TryGetSceneRef(out var sceneRef))
		{
			AddClient(GameMode.Client, sceneRef);
		}
	}

	[BehaviourButtonAction("Add Additional Shared Client", null, "CanAddSharedClients")]
	public void AddSharedClient()
	{
		if (TryGetSceneRef(out var sceneRef))
		{
			AddClient(GameMode.Shared, sceneRef);
		}
	}

	public Task AddClient(GameMode serverMode, SceneRef sceneRef)
	{
		NetworkRunner networkRunner = UnityEngine.Object.Instantiate(RunnerPrefab);
		UnityEngine.Object.DontDestroyOnLoad(networkRunner);
		networkRunner.name = $"Client {(char)(65 + LastCreatedClientIndex++)}";
		GameMode gameMode = GameMode.Client;
		if (serverMode == GameMode.Shared || serverMode == GameMode.AutoHostOrClient)
		{
			gameMode = serverMode;
		}
		Task result = InitializeNetworkRunner(networkRunner, gameMode, NetAddress.Any(0), sceneRef, null);
		if (AlwaysShowStats && LastCreatedClientIndex == 0)
		{
			FusionStats.Create(null, networkRunner, FusionStats.DefaultLayouts.Right, FusionStats.DefaultLayouts.Right);
		}
		return result;
	}

	protected IEnumerator StartClients(int clientCount, GameMode serverMode, SceneRef sceneRef = default(SceneRef))
	{
		CurrentStage = Stage.ConnectingClients;
		List<Task> clientTasks = new List<Task>();
		int i = 0;
		while (i < clientCount)
		{
			clientTasks.Add(AddClient(serverMode, sceneRef));
			yield return new WaitForSeconds(0.1f);
			int num = i + 1;
			i = num;
		}
		Task clientsStartTask = Task.WhenAll(clientTasks);
		while (!clientsStartTask.IsCompleted)
		{
			yield return new WaitForSeconds(1f);
		}
		if (clientsStartTask.IsFaulted)
		{
			Debug.LogWarning(clientsStartTask.Exception);
		}
		CurrentStage = Stage.AllConnected;
	}

	protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
	{
		INetworkSceneManager networkSceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();
		if (networkSceneManager == null)
		{
			Debug.Log("NetworkRunner does not have any component implementing INetworkSceneManager interface, adding NetworkSceneManagerDefault.", runner);
			networkSceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
		}
		return runner.StartGame(new StartGameArgs
		{
			GameMode = gameMode,
			Address = address,
			Scene = scene,
			SessionName = DefaultRoomName,
			Initialized = initialized,
			SceneManager = networkSceneManager
		});
	}
}
