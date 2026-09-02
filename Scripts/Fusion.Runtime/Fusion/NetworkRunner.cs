#define DEBUG
#define FUSION_UNITY
#define TRACE
#define ENABLE_PROFILER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fusion.Async;
using Fusion.Photon.Realtime;
using Fusion.Photon.Realtime.Extension;
using Fusion.Protocol;
using Fusion.Sockets;
using Fusion.Sockets.Stun;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion;

[AddComponentMenu("Fusion/Network Runner")]
[DisallowMultipleComponent]
[HelpURL("https://doc.photonengine.com/fusion/current/manual/prebuilt-components#networkrunner")]
[ScriptHelp(BackColor = EditorHeaderBackColor.Red, Icon = EditorHeaderIcon.FusionBlue)]
public sealed class NetworkRunner : Behaviour, ILogBuilder, Simulation.ICallbacks
{
	public enum BuildTypes
	{
		Debug = 0,
		Release = 1
	}

	public enum States
	{
		Starting = 1,
		Running = 2,
		Shutdown = 3
	}

	[Flags]
	private enum ShutdownFlags
	{
		Regular = 1
	}

	public delegate void OnBeforeSpawned(NetworkRunner runner, NetworkObject obj);

	private struct SpawnQueueEntry
	{
		public NetworkPrefabId PrefabId;

		public Vector3? Position;

		public Quaternion? Rotation;

		public PlayerRef? InputAuthority;

		public object OnBeforeSpawned;

		public NetworkObjectPredictionKey? PredictionKey;

		public bool SyncPhysics;
	}

	private struct DeferredShutdownParams
	{
		public bool ShutdownRequested;

		public ShutdownReason ShutdownReason;

		public bool DestroyGO;
	}

	private enum SimulationPhase
	{
		None = 0,
		Update = 1,
		Render = 2
	}

	[Flags]
	private enum AttachOptions
	{
		LocalSpawn = 1
	}

	internal struct HostSnapshotCompressionJob : IJob
	{
		internal int CurrentTick;

		internal int Length;

		internal uint LastID;

		[NativeDisableUnsafePtrRestriction]
		internal unsafe int* Previous;

		[NativeDisableUnsafePtrRestriction]
		internal unsafe int* Current;

		[NativeDisableUnsafePtrRestriction]
		internal unsafe int* Result;

		public unsafe int ResultLength
		{
			get
			{
				return *Result;
			}
			internal set
			{
				*Result = value;
			}
		}

		public unsafe int ResultTick
		{
			get
			{
				return Result[1];
			}
			internal set
			{
				Result[1] = value;
			}
		}

		public unsafe int* ResultData => Result + 2;

		public unsafe void Execute()
		{
			int* resultData = ResultData;
			CompressionUtils.SnapshotCompress(Current, Previous, resultData, Length, out var count);
			ResultTick = CurrentTick;
			ResultLength = count;
		}
	}

	[DefaultExecutionOrder(-10000)]
	private class NetworkObjectInactivityGuard : Behaviour
	{
		[NonSerialized]
		public NetworkObject Object;

		private void OnEnable()
		{
			if (!BehaviourUtils.IsNull(Object))
			{
				NetworkRunner runner = Object.Runner;
				Object = null;
				if ((bool)runner)
				{
					runner._inactivityGuardPool.Push(this);
					base.transform.SetParent(runner.transform);
				}
				else
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		private void OnDestroy()
		{
			if (!BehaviourUtils.IsNull(Object))
			{
				Object.OnDestroyNeverActive();
			}
		}
	}

	public delegate void CloudConnectionLostHandler(NetworkRunner networkRunner, ShutdownReason shutdownReason, bool reconnecting);

	private static Dictionary<int, NetworkRunner> _instancesByMultiPeerScene = new Dictionary<int, NetworkRunner>();

	[NonSerialized]
	private DeferredShutdownParams _deferredShutdownParams = default;

	public static Simulation.IDeltaCompressor BurstDeltaCompressor;

	[NonSerialized]
	internal Simulation _simulation;

	[NonSerialized]
	private SimulationPhase _simulationPhase;

	[NonSerialized]
	private ShutdownFlags _simulationShutdown = ShutdownFlags.Regular;

	[NonSerialized]
	private NetworkObjectRefMap<NetworkObject> _objects;

	[NonSerialized]
	private SimulationBehaviourUpdater _behaviourUpdater;

	[NonSerialized]
	private List<INetworkRunnerCallbacks> _callbacks;

	[NonSerialized]
	private unsafe Allocator* _changedAllocator;

	[NonSerialized]
	private List<NetworkId> _destroyIdsBuffer = new List<NetworkId>();

	[NonSerialized]
	internal LinkedList<RunnerVisibilityNode> _visibilityNodes;

	[NonSerialized]
	private bool _isVisible = true;

	[NonSerialized]
	private Queue<SpawnQueueEntry> _spawnQueue;

	private bool _printedInterestGroupsWarning;

	internal TaskCompletionSource<bool> _initializeOperation;

	[NonSerialized]
	private NetworkProjectConfig _config;

	[NonSerialized]
	private int _ticksExecuted;

	[NonSerialized]
	private INetworkObjectPool _networkObjectPool;

	[NonSerialized]
	private uint _idCounter = 1u;

	[NonSerialized]
	private List<NetworkObject> _predictionSpawns = new List<NetworkObject>();

	[NonSerialized]
	private List<NetworkObject> _predictionDespawns = new List<NetworkObject>();

	[NonSerialized]
	private List<NetworkObject> _activeSceneObjectsBuffer = new List<NetworkObject>();

	internal ReadAccuracy _positionReadAccuracy;

	internal WriteAccuracy _positionWriteAccuracy;

	internal ReadAccuracy _rotationReadAccuracy;

	internal WriteAccuracy _rotationWriteAccuracy;

	internal byte[] _connectionToken;

	[NonSerialized]
	private bool? _provideInput;

	private CancellationTokenSource OperationsCancellationTokenSource = new CancellationTokenSource();

	private List<NetworkObject> _remotePrefabsWaitingForSpawnedCallback = new List<NetworkObject>();

	private List<INetworkRunnerCallbacks> _callbacksBuffer = new List<INetworkRunnerCallbacks>();

	private string _debugNameThreadSafe;

	private unsafe byte* _hostSnapshotData0;

	private unsafe byte* _hostSnapshotData1;

	private unsafe byte* _hostSnapshotDelta;

	private TaskCompletionSource<(bool, int, int, uint, byte[])> _buildHostSnapshotTask;

	private HostSnapshotCompressionJob? _buildHostSnapshotJob;

	private JobHandle? _buildHostSnapshotHandler;

	private HostMigration _lastHostMigrationInfo;

	private Stack<NetworkObjectInactivityGuard> _inactivityGuardPool = new Stack<NetworkObjectInactivityGuard>();

	private static List<NetworkRunner> _instances = new List<NetworkRunner>();

	private static NetworkRunner[] _instancesSnapshot = Array.Empty<NetworkRunner>();

	private static int _instancesSnapshotCount;

	public Func<string, ServerConnection, string> CloudAddressRewriter = null;

	internal TaskCompletionSource<(ShutdownReason, string)> _cloudOperation;

	internal CloudServices _cloudServices;

	private static string _cachedRegionSummary = string.Empty;

	private INetworkSceneManager _sceneManager;

	private INetworkSceneManagerObjectResolver _sceneObjectResolver;

	private Dictionary<NetworkObjectGuid, NetworkObject> _sceneObjectLoopkup;

	[NonSerialized]
	private SceneRef? _sharedModeStartSceneRef;

	[NonSerialized]
	private Scene _multiplePeerUnityScene;

	[NonSerialized]
	private bool _isMultiplePeerUnitySceneTemp = false;

	private unsafe int* _tempWords;

	private int _tempWordsCapacity;

	public static CloudConnectionLostHandler CloudConnectionLost;

	public static BuildTypes BuildType => BuildTypes.Debug;

	internal bool IsSimulationUpdating => _simulationPhase == SimulationPhase.Update;

	internal bool IsInitialized => _initializeOperation != null && _initializeOperation.Task.IsCompleted && _initializeOperation.Task.Result;

	public bool IsVisible
	{
		get
		{
			return _isVisible;
		}
		set
		{
			if (_isVisible != value)
			{
				_isVisible = value;
				if (Config != null && Config.PeerMode != NetworkProjectConfig.PeerModes.Multiple)
				{
					Log.Warn(this, "NetworkRunner.IsVisible only applies to Multi-Peer mode.");
				}
				RunnerVisibilityNode.RefreshRunnerVisibility(this);
			}
		}
	}

	public bool ProvideInput
	{
		get
		{
			return _provideInput == true;
		}
		set
		{
			_provideInput = value;
		}
	}

	public SimulationConfig.Topologies Topology => _simulation?.Config.Topology ?? SimulationConfig.Topologies.ClientServer;

	public Simulation Simulation => _simulation;

	public SimulationModes Mode => _simulation?.Mode ?? ((SimulationModes)0);

	public SimulationStages Stage => _simulation?.Stage ?? ((SimulationStages)0);

	public float DeltaTime => _simulation?.DeltaTime ?? 0f;

	public float SimulationTime => (float)(_simulation?.State.Time ?? 0.0);

	public float SimulationRenderTime => (_simulation != null) ? ((float)_simulation.StatePrevious.Time + _simulation.StateAlpha * _simulation.DeltaTime) : 0f;

	public float InterpolationRenderTime => (_simulation == null) ? 0f : (IsServer ? SimulationRenderTime : ((float)_simulation.InterpFrom.Time + Simulation.InterpAlpha * _simulation.DeltaTime));

	public bool IsRunning => _simulation != null && _simulation.IsRunning;

	public bool IsShutdown => _simulationShutdown != (ShutdownFlags)0;

	private bool IsRegularShutdown
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (_simulationShutdown & ShutdownFlags.Regular) != 0;
		}
	}

	public bool IsStarting => !IsRunning && !IsShutdown;

	public bool IsClient => _simulation != null && _simulation.IsClient;

	public bool IsConnectedToServer => IsClient && ((Simulation.Client)_simulation).IsConnectedToServer;

	public bool IsServer => _simulation != null && _simulation.IsServer;

	public bool IsPlayer => _simulation != null && _simulation.IsPlayer;

	public bool IsSinglePlayer => _simulation != null && _simulation.IsSinglePlayer;

	public bool IsLastTick => _simulation?.IsLastTick ?? false;

	public bool IsFirstTick => _simulation?.IsFirstTick ?? false;

	public bool IsForward => _simulation?.IsForward ?? false;

	public bool IsResimulation => _simulation?.IsResimulation ?? false;

	public States State => IsShutdown ? States.Shutdown : ((!IsRunning) ? States.Starting : States.Running);

	public PlayerRef LocalPlayer => _simulation?.LocalPlayer ?? default(PlayerRef);

	public Tick Tick
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _simulation?.Tick ?? default(Tick);
		}
	}

	public NetworkProjectConfig Config => _config;

	public int TicksExecuted => _ticksExecuted;

	public IEnumerable<PlayerRef> ActivePlayers => _simulation?.ActivePlayers ?? Enumerable.Empty<PlayerRef>();

	public INetworkSceneManager SceneManager => _sceneManager;

	public INetworkObjectPool ObjectPool => _networkObjectPool;

	internal CancellationToken OperationsCancellationToken
	{
		get
		{
			if (OperationsCancellationTokenSource == null || OperationsCancellationTokenSource.IsCancellationRequested)
			{
				Log.Warn("Trying to access an invalid OperationsCancellationTokenSource");
				return CancellationToken.None;
			}
			return OperationsCancellationTokenSource.Token;
		}
	}

	public HitboxManager LagCompensation => GetGlobal<HitboxManager>();

	public bool IsHostMigrationEnabled { get; private set; }

	public bool IsResume => _simulation != null && _simulation.IsResume && _initializeOperation != null && !_initializeOperation.Task.IsCompleted;

	internal uint HostMigrationSnapshotDelay { get; private set; }

	public static IReadOnlyList<NetworkRunner> Instances => _instances;

	public bool IsCloudReady => _cloudServices?.IsCloudReady == true;

	public bool IsInSession => _cloudServices?.IsInRoom == true;

	public string UserId => IsCloudReady ? _cloudServices.UserId : null;

	public AuthenticationValues AuthenticationValues => IsCloudReady ? _cloudServices.AuthenticationValues : null;

	public GameMode GameMode { get; private set; }

	public SessionInfo SessionInfo { get; private set; } = new SessionInfo();

	public LobbyInfo LobbyInfo { get; private set; } = new LobbyInfo();

	public ConnectionType CurrentConnectionType
	{
		get
		{
			if (IsConnectedToServer)
			{
				if (((Simulation.Client)_simulation).ServerAddress.IsRelayAddr)
				{
					return ConnectionType.Relayed;
				}
				return ConnectionType.Direct;
			}
			return ConnectionType.None;
		}
	}

	public NATType NATType => (_cloudServices != null) ? _cloudServices.NATType : NATType.Invalid;

	public bool IsSharedModeMasterClient => GameMode == GameMode.Shared && IsClient && _cloudServices != null && _cloudServices.IsMasterClient;

	public unsafe SceneRef CurrentScene => (_simulation != null) ? _simulation.State.GlobalState->Scene : default(SceneRef);

	private bool IsSceneMaster => IsServer || IsSharedModeMasterClient;

	public Scene SimulationUnityScene
	{
		get
		{
			if (_config == null)
			{
				return default;
			}
			if (_config.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
			{
				return MultiplePeerUnityScene;
			}
			return UnityEngine.SceneManagement.SceneManager.GetActiveScene();
		}
	}

	public Scene MultiplePeerUnityScene
	{
		get
		{
			return _multiplePeerUnityScene;
		}
		set
		{
			if (_config.PeerMode != NetworkProjectConfig.PeerModes.Multiple)
			{
				throw new InvalidOperationException($"Only supported in {NetworkProjectConfig.PeerModes.Multiple} peer mode");
			}
			if (_multiplePeerUnityScene.IsValid())
			{
				_instancesByMultiPeerScene.Remove(_multiplePeerUnityScene.handle);
			}
			_isMultiplePeerUnitySceneTemp = false;
			_multiplePeerUnityScene = value;
			if (_multiplePeerUnityScene.IsValid())
			{
				_instancesByMultiPeerScene.Add(_multiplePeerUnityScene.handle, this);
			}
		}
	}

	public bool IsMultiplePeerSceneTemp => _isMultiplePeerUnitySceneTemp;

	bool Simulation.ICallbacks.CanReceivePlayerJoinLeaveCallbacks => IsInitialized && (_sceneManager?.IsReady(this) ?? true);

	bool Simulation.ICallbacks.IsSharedModeMasterClient => IsSharedModeMasterClient;

	internal static void ResetStatics()
	{
		_instancesByMultiPeerScene.Clear();
		_instances.Clear();
	}

	[BehaviourWarn("NetworkRunner will not work properly with NetworkObject on the same GameObject.", "_hasNetworkObject")]
	private bool _hasNetworkObject()
	{
		return BehaviourUtils.IsAlive(GetBehaviour<NetworkObject>());
	}

	public void Disconnect(PlayerRef player)
	{
		if (_simulation != null)
		{
			if (_simulation is Simulation.Server server)
			{
				server.Disconnect(player);
			}
			else
			{
				Log.Error(this, "Only server can disconnect players");
			}
		}
	}

	internal void Disconnect(NetAddress address)
	{
		if (_simulation != null)
		{
			if (_simulation is Simulation.Server server)
			{
				server.Disconnect(address);
			}
			else
			{
				Log.Error(this, "Only server can disconnect players");
			}
		}
	}

	internal void Connect(NetAddress address, byte[] token, byte[] uniqueId)
	{
		if (IsServer)
		{
			throw new InvalidOperationException("Only clients can connect");
		}
		((Simulation.Client)Simulation).Connect(address, token, uniqueId);
	}

	[BehaviourButtonAction("Shutdown", true, false, null)]
	internal void ShutdownAction()
	{
		StartCoroutine(ShutdownWithCleanupCoroutine());
	}

	internal IEnumerator ShutdownWithCleanupCoroutine()
	{
		yield return Shutdown();
		if (Config != null && Config.PeerMode == NetworkProjectConfig.PeerModes.Multiple && UnityEngine.SceneManagement.SceneManager.sceneCount > 1)
		{
			yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(MultiplePeerUnityScene);
			RunnerVisibilityNode.RefreshAllRunnerVisibilities();
		}
		else
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(0);
		}
	}

	public unsafe Task Shutdown(bool destroyGameObject = true, ShutdownReason shutdownReason = ShutdownReason.Ok, bool forceShutdownProcedure = false)
	{
		if (_simulationPhase != SimulationPhase.None)
		{
			_deferredShutdownParams = new DeferredShutdownParams
			{
				ShutdownRequested = true,
				ShutdownReason = shutdownReason,
				DestroyGO = destroyGameObject
			};
			_simulation?.NotifyWaitingForShutdown();
			return Task.CompletedTask;
		}
		_deferredShutdownParams = default;
		RegisterNetworkCallbacks();
		if (IsShutdown)
		{
			RemoveInstance(this);
			if (!IsRegularShutdown & forceShutdownProcedure)
			{
				InvokeOnShutdownCallbacks();
				return ContinueTasksWithDestroy(new Task[1] { DisconnectFromCloud() });
			}
			return Task.CompletedTask;
		}
		_simulationShutdown |= ShutdownFlags.Regular;
		try
		{
			_simulation?.ShutdownNativeSocket();
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
		RemoveInstance(this);
		Scene scene = default;
		if (_simulation != null && Config != null && Config.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			if (IsMultiplePeerSceneTemp)
			{
				scene = MultiplePeerUnityScene;
			}
			MultiplePeerUnityScene = default;
		}
		InvokeOnShutdownCallbacks();
		if (_objects != null)
		{
			_objects.GetIterateBufferStartCount(out var entries, out var start, out var count);
			for (int i = start; i < count; i++)
			{
				if (BehaviourUtils.IsAlive(entries[i].Value))
				{
					PerformPrefabCleanup(entries[i].Value, destroyedByEngine: false, hasState: false);
				}
			}
			_objects.Clear();
		}
		Allocator.Dispose(_changedAllocator);
		_simulation?.Dispose();
		_simulation = null;
		_sceneManager?.Shutdown(this);
		_sceneManager = null;
		_sceneObjectResolver = null;
		CleanHostMigrationSnapshots();
		GameMode = (GameMode)0;
		SessionInfo = new SessionInfo();
		Task task;
		if (scene.IsValid())
		{
			Log.Debug(this, "Unloading temp scene");
			TaskCompletionSource<int> completionSource = new TaskCompletionSource<int>();
			if (UnityEngine.SceneManagement.SceneManager.sceneCount == 1)
			{
				Assert.Check(UnityEngine.SceneManagement.SceneManager.GetActiveScene() == scene);
				UnityEngine.SceneManagement.SceneManager.CreateScene($"EmptyScene_{Guid.NewGuid()}");
			}
			AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
			asyncOperation.completed += (AsyncOperation asyncOperation2) =>
			{
				completionSource.SetResult(0);
			};
			task = completionSource.Task;
		}
		else
		{
			task = Task.CompletedTask;
		}
		Task task2 = DisconnectFromCloud();
		return ContinueTasksWithDestroy(new Task[2] { task, task2 });
		Task ContinueTasksWithDestroy(Task[] precedingTasks)
		{
			return TaskManager.ContinueWhenAll(precedingTasks, (CancellationToken token) =>
			{
				Log.Debug(this, "Shutdown complete.");
				if (destroyGameObject && (bool)this && (bool)base.gameObject)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				if (!OperationsCancellationTokenSource.IsCancellationRequested)
				{
					OperationsCancellationTokenSource.Cancel();
				}
				OperationsCancellationTokenSource.Dispose();
				return Task.CompletedTask;
			}, OperationsCancellationToken);
		}
		void InvokeOnShutdownCallbacks()
		{
			for (int j = 0; j < _callbacks.Count; j++)
			{
				try
				{
					_callbacks[j].OnShutdown(this, shutdownReason);
				}
				catch (Exception exn2)
				{
					Log.Exception(this, exn2);
				}
			}
		}
	}

	private INetSocket CreateCloudSocket()
	{
		if (_cloudServices == null || !_cloudServices.IsCloudReady)
		{
			throw new InvalidOperationException("Fusion Relay Client is not ready. Make sure the call Runner.ConnectToCloud before start with Runner.StartGame");
		}
		if (!_cloudServices.IsNATPunchthroughEnabled || RuntimeUnityFlagsSetup.IsUNITY_WEBGL)
		{
			return new NetSocketRelay(_cloudServices.Communicator);
		}
		return new NetSocketHybrid(_cloudServices.Communicator);
	}

	internal void SetInitializationDone(NetworkRunnerInitializeArgs args)
	{
		_initializeOperation?.TrySetResult(result: true);
		try
		{
			args.Initialized?.Invoke(this);
		}
		catch (Exception exn)
		{
			Log.Exception(this, exn);
		}
		_cloudServices?.StartBackgroundCloudServices();
	}

	internal unsafe Task<bool> Initialize(NetworkRunnerInitializeArgs args)
	{
		_initializeOperation = new TaskCompletionSource<bool>();
		InitFusionLogSystem();
		if (!args.SimulationMode.HasValue)
		{
			throw new InvalidOperationException("SimulationMode must have a value");
		}
		if (!args.Address.HasValue && !args.IsSinglePlayer)
		{
			throw new InvalidOperationException("Address must have a value");
		}
		if (args.Config == null)
		{
			throw new InvalidOperationException("Config must have a value");
		}
		if (_callbacks == null)
		{
			_callbacks = new List<INetworkRunnerCallbacks>();
		}
		INetSocket netSocket = ((!args.IsSinglePlayer) ? CreateCloudSocket() : new NetSocketNull());
		Assert.Check(netSocket);
		Assert.Check(sizeof(SimulationGlobalState) == 128, sizeof(SimulationGlobalState), 128);
		_config = SetupNetworkProjectConfig(args);
		_positionReadAccuracy = _config.AccuracyDefaults.GetAccuracyOrThrow("Position").GetReadAccuracy(_config);
		_positionWriteAccuracy = _config.AccuracyDefaults.GetAccuracyOrThrow("Position").GetWriteAccuracy(_config);
		_rotationReadAccuracy = _config.AccuracyDefaults.GetAccuracyOrThrow("Rotation").GetReadAccuracy(_config);
		_rotationWriteAccuracy = _config.AccuracyDefaults.GetAccuracyOrThrow("Rotation").GetWriteAccuracy(_config);
		_connectionToken = args.ConnectionToken;
		_spawnQueue = new Queue<SpawnQueueEntry>();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (args.ObjectPool == null)
		{
			_networkObjectPool = new NetworkObjectPoolDefault();
		}
		else
		{
			_networkObjectPool = args.ObjectPool;
		}
		Simulation.IDeltaCompressor deltaCompressor = Simulation.GetDefaultDeltaCompressor();
		if (_config.DeltaCompressor == NetworkProjectConfig.DeltaCompressors.Burst)
		{
			if (BurstDeltaCompressor == null)
			{
				Log.Error(this, "DeltaCompressor set to 'Burst' on configuration, but no burst delta compressor was found. Did you install the Burst package and enable the delta compressor with the FUSION_BURST define?");
			}
			else
			{
				deltaCompressor = BurstDeltaCompressor;
			}
		}
		else if (_config.DeltaCompressor == NetworkProjectConfig.DeltaCompressors.DebugUncompressed)
		{
			deltaCompressor = Simulation.GetDebugDeltaCompressor();
		}
		SimulationArgs args2 = default;
		args2.Mode = args.SimulationMode.Value;
		args2.Tick = args.ResumeTick.GetValueOrDefault();
		args2.State = args.ResumeState;
		args2.Config = _config;
		args2.Callbacks = this;
		args2.DeltaCompressor = deltaCompressor;
		args2.Socket = netSocket;
		args2.Address = args.Address.GetValueOrDefault();
		if (args2.IsServer)
		{
			_simulation = new Simulation.Server(args2);
		}
		else
		{
			args2.Tick = default;
			args2.State = null;
			_simulation = new Simulation.Client(args2);
		}
		_objects = new NetworkObjectRefMap<NetworkObject>();
		_behaviourUpdater = new SimulationBehaviourUpdater();
		_behaviourUpdater.BuildTypeOrder(args.CustomCallbackInterfaces);
		_changedAllocator = Allocator.Create(_simulation.State.Allocator->Configuration);
		_simulationShutdown = (ShutdownFlags)0;
		_deferredShutdownParams = default;
		if (args.SceneManager == null)
		{
			Type type = Type.GetType("Fusion.NetworkSceneManagerDefault, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
			if (type != null)
			{
				Log.Warn(this, "No SceneManager passed. To let Fusion attach to scene NetworkObjects the default provider (" + type.FullName + ") will be created and added to the runner's GameObject. Please review your code to set SceneManager property.");
				_sceneManager = (INetworkSceneManager)base.gameObject.AddComponent(type);
			}
			else
			{
				Log.Error(this, "No SceneManager passed and the default provider component type (Fusion.NetworkSceneManagerDefault, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null) was not found. Fusion will not be able to attach to scene NetworkObjects.");
				_sceneManager = new NetworkSceneManagerDummy();
			}
		}
		else
		{
			_sceneManager = args.SceneManager;
		}
		if (_sceneManager is INetworkSceneManagerObjectResolver sceneObjectResolver)
		{
			_sceneObjectResolver = sceneObjectResolver;
		}
		else
		{
			_sceneObjectLoopkup = new Dictionary<NetworkObjectGuid, NetworkObject>();
		}
		if (_config.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			TryMultiplePeerAssignTempScene();
		}
		SimulationBehaviour[] components = GetComponents<SimulationBehaviour>();
		foreach (SimulationBehaviour behaviour in components)
		{
			AddSimulationBehaviour(behaviour);
		}
		switch (_config.PhysicsEngine)
		{
		case NetworkProjectConfig.PhysicsEngines.Physics2D:
			AddOrGetGlobal<NetworkPhysicsSimulation2D>();
			break;
		case NetworkProjectConfig.PhysicsEngines.Physics3D:
			AddOrGetGlobal<NetworkPhysicsSimulation3D>();
			break;
		}
		if (_config.UseLagCompensation)
		{
			AddOrGetGlobal<HitboxManager>();
		}
		Log.Debug(this, string.Format("Starting with {0}:\n{1}", "NetworkProjectConfig", _config));
		IsHostMigrationEnabled = _config.EnableHostMigration;
		HostMigrationSnapshotDelay = _config.HostMigrationSnapshotInterval * 1000;
		AddInstance(this);
		if (args.Scene.HasValue)
		{
			if (IsServer)
			{
				try
				{
					SetActiveScene(args.Scene.Value);
				}
				catch (Exception exn)
				{
					Log.Exception(this, exn);
				}
			}
			else if (IsSharedModeMasterClient)
			{
				_sharedModeStartSceneRef = args.Scene;
			}
		}
		try
		{
			_sceneManager.Initialize(this);
		}
		catch (Exception exn2)
		{
			Log.Exception(this, exn2);
		}
		if (!_provideInput.HasValue)
		{
			ProvideInput = Simulation.IsPlayer;
		}
		_cachedRegionSummary = _cloudServices?.CachedRegionSummary ?? string.Empty;
		if (Simulation.IsServer && Simulation.IsResume)
		{
			_idCounter = args.ResumeId.Value.Raw;
			_simulation.Callbacks.OnServerStart();
			StartCoroutine(RunHostMigrationResume(args));
		}
		else
		{
			SetInitializationDone(args);
		}
		return _initializeOperation.Task;
	}

	public void SinglePlayerPause()
	{
		Simulation.SinglePlayerSetPaused(paused: true);
	}

	public void SinglePlayerContinue()
	{
		Simulation.SinglePlayerSetPaused(paused: false);
	}

	public void SinglePlayerPause(bool paused)
	{
		Simulation.SinglePlayerSetPaused(paused);
	}

	public void SetInterestGroup(NetworkObject obj, PlayerRef player, string group, bool interested)
	{
		if (Simulation.Config.ReplicationMode == SimulationConfig.StateReplicationModes.DeltaSnapshots || Simulation.Config.Topology == SimulationConfig.Topologies.Shared)
		{
			if (!_printedInterestGroupsWarning)
			{
				_printedInterestGroupsWarning = true;
				if (Simulation.Config.ReplicationMode == SimulationConfig.StateReplicationModes.DeltaSnapshots)
				{
					Log.DebugWarn(this, "Interest groups are only usable in Host/Server mode using Eventual Consistency. You are currently using Delta Snapshots. This message can be ignored and is only printed once when using debug mode.");
				}
				else if (Simulation.Config.Topology == SimulationConfig.Topologies.Shared)
				{
					Log.DebugWarn(this, "Interest groups are only usable in Host/Server mode using Eventual Consistency. You are currently using Shared Mode. This message can be ignored and is only printed once when using debug mode.");
				}
			}
		}
		else if (IsServer && player.IsValid && !IsHostPlayer(player) && Exists(obj))
		{
			Simulation.Replicator.OnObjectInterestGroupChange(player, obj.Id, group, interested);
		}
	}

	public int GetInterfaceListsCount(Type type)
	{
		Assert.Check(type.IsInterface);
		return _behaviourUpdater.GetCallbackCount(type);
	}

	[Obsolete("Use GetInterfaceListHead(Type, int, out SimulationBehaviour) instead (in using scope)")]
	public SimulationBehaviour GetInterfaceListHead(Type type, int index)
	{
		return _behaviourUpdater.GetCallbackHead(type, index);
	}

	public SimulationBehaviourListScope GetInterfaceListHead(Type type, int index, out SimulationBehaviour head)
	{
		return _behaviourUpdater.GetCallbackHead(type, index, out head);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SimulationBehaviour GetInterfaceListPrev(SimulationBehaviour behaviour)
	{
		return behaviour.Prev;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SimulationBehaviour GetInterfaceListNext(SimulationBehaviour behaviour)
	{
		return behaviour.Next;
	}

	public int? GetPlayerActorId(PlayerRef player)
	{
		switch (Simulation.Config.Topology)
		{
		case SimulationConfig.Topologies.ClientServer:
		{
			if (Simulation.IsServer && _cloudServices != null && _cloudServices.TryGetActorIdByUniqueId(Simulation.GetPlayerUniqueId(player), out var actorId))
			{
				return actorId;
			}
			break;
		}
		case SimulationConfig.Topologies.Shared:
			return Simulation.GetPlayerActorId(player);
		}
		return null;
	}

	public string GetPlayerUserId(PlayerRef player = default(PlayerRef))
	{
		if (!IsCloudReady)
		{
			return null;
		}
		if (LocalPlayer == player || player == default(PlayerRef))
		{
			return UserId;
		}
		int? playerActorId = GetPlayerActorId(player);
		return (!playerActorId.HasValue) ? null : _cloudServices?.GetActorUserID(playerActorId.Value);
	}

	public void SetPlayerObject(PlayerRef player, NetworkObject networkObject)
	{
		if (BehaviourUtils.IsNull(networkObject) || Exists(networkObject))
		{
			Simulation.SetPlayerObjectId(player, networkObject);
		}
		else
		{
			Log.DebugError(this, $"Invalid {networkObject}");
		}
	}

	public NetworkObject GetPlayerObject(PlayerRef player)
	{
		NetworkObject value;
		return _objects.TryGet(Simulation.GetPlayerObjectId(player), out value) ? value : null;
	}

	public bool TryGetPlayerObject(PlayerRef player, out NetworkObject networkObject)
	{
		return _objects.TryGet(Simulation.GetPlayerObjectId(player), out networkObject);
	}

	public List<T> GetAllBehaviours<T>() where T : SimulationBehaviour
	{
		List<T> result = new List<T>();
		GetAllBehaviours(result);
		return result;
	}

	public void GetAllBehaviours<T>(List<T> result) where T : SimulationBehaviour
	{
		SimulationBehaviour[] allBehaviours = GetAllBehaviours(typeof(T));
		for (int i = 0; i < allBehaviours.Length; i++)
		{
			SimulationBehaviour simulationBehaviour = allBehaviours[i];
			while (BehaviourUtils.IsNotNull(simulationBehaviour))
			{
				if (simulationBehaviour.CanReceiveCallback)
				{
					result.Add((T)simulationBehaviour);
				}
				simulationBehaviour = simulationBehaviour.Next;
			}
		}
	}

	public double GetPlayerRtt(PlayerRef playerRef)
	{
		return Simulation.GetPlayerRtt(playerRef);
	}

	public unsafe void SendRpc(SimulationMessage* message)
	{
		Simulation.SendMessage(message, null, null);
	}

	public unsafe void SendRpc(SimulationMessage* message, out RpcSendResult info)
	{
		RpcSendResult rpcSendResult = default;
		rpcSendResult.MessageSize = message->Offset;
		rpcSendResult.Result = Simulation.SendMessage(message, &rpcSendResult.Receivers, &rpcSendResult.CulledReceivers);
		info = rpcSendResult;
	}

	public bool IsPlayerValid(PlayerRef player)
	{
		return Simulation.PlayerValid(player);
	}

	public bool IsPlayerActive(PlayerRef player)
	{
		return Simulation.PlayerActive(player);
	}

	public byte[] GetPlayerConnectionToken(PlayerRef player = default(PlayerRef))
	{
		if (player == LocalPlayer || player == PlayerRef.None)
		{
			return _connectionToken;
		}
		if (IsServer)
		{
			return Simulation.GetPlayerConnectionToken(player);
		}
		return null;
	}

	public ConnectionType GetPlayerConnectionType(PlayerRef player)
	{
		if (IsServer)
		{
			NetAddress playerAddress = Simulation.GetPlayerAddress(player);
			if (!playerAddress.Equals(default))
			{
				if (playerAddress.IsRelayAddr)
				{
					return ConnectionType.Relayed;
				}
				return ConnectionType.Direct;
			}
		}
		return ConnectionType.None;
	}

	public SimulationBehaviour[] GetAllBehaviours(Type type)
	{
		return _behaviourUpdater.GetTypeHeads(type);
	}

	public void AddCallbacks(params INetworkRunnerCallbacks[] callbacks)
	{
		if (_callbacks == null)
		{
			_callbacks = new List<INetworkRunnerCallbacks>();
		}
		foreach (INetworkRunnerCallbacks item in callbacks)
		{
			if (!_callbacks.Contains(item))
			{
				_callbacks.Add(item);
			}
		}
	}

	public void RemoveCallbacks(params INetworkRunnerCallbacks[] callbacks)
	{
		if (_callbacks == null)
		{
			_callbacks = new List<INetworkRunnerCallbacks>();
		}
		foreach (INetworkRunnerCallbacks item in callbacks)
		{
			if (_callbacks.Contains(item))
			{
				_callbacks.Remove(item);
			}
		}
	}

	private void OnApplicationQuit()
	{
		StunClient.PendingRequests.Clear();
		Shutdown();
	}

	internal void RenderInternal()
	{
		if (IsRegularShutdown || _simulation == null)
		{
			return;
		}
		if (_config.InvokeRenderInBatchMode || !Application.isBatchMode)
		{
			try
			{
				_simulationPhase = SimulationPhase.Render;
				_behaviourUpdater.InvokeRender();
				foreach (NetworkObject predictionSpawn in _predictionSpawns)
				{
					InvokeMethodOnPredictedSpawnedObject(predictionSpawn, (NetworkObject o, IPredictedSpawnBehaviour b) =>
					{
						b.PredictedSpawnRender();
					});
				}
			}
			finally
			{
				_simulationPhase = SimulationPhase.None;
			}
		}
		if (_deferredShutdownParams.ShutdownRequested)
		{
			Shutdown(_deferredShutdownParams.DestroyGO, _deferredShutdownParams.ShutdownReason);
		}
	}

	private void Awake()
	{
		DebugAwake();
		if (_callbacks == null)
		{
			_callbacks = new List<INetworkRunnerCallbacks>();
		}
		RegisterNetworkCallbacks();
		AddInstance(this);
		TaskManager.Setup();
	}

	private void OnDestroy()
	{
		DebugOnDestroy();
		Shutdown(destroyGameObject: false);
	}

	private void Update()
	{
		DebugUpdate();
		_cloudServices?.Update();
		Service_HostSnapshot();
	}

	internal void UpdateInternal(double dt)
	{
		Assert.Check(!_deferredShutdownParams.ShutdownRequested);
		if (dt != 0.0)
		{
			if (IsRegularShutdown)
			{
				return;
			}
			if (_simulation != null)
			{
				try
				{
					if (_simulation.IsPaused)
					{
						Assert.Check(_simulation.IsSinglePlayer, "Simulation is paused, but is not running in SinglePlayer Mode");
						return;
					}
					_simulationPhase = SimulationPhase.Update;
					RegisterNetworkCallbacks();
					InvokeBeforeUpdate();
					ProcessSpawnQueue();
					_ticksExecuted = _simulation.Update(dt);
					InvokeAfterUpdate();
					if (IsClient)
					{
						if (Simulation.Replicator.SharedTick > 0)
						{
							Simulation.SnapshotHistory.DisposeOlderThan(Math.Min(Simulation.Replicator.SharedTick, (Simulation.InterpFrom?.Tick).GetValueOrDefault()));
						}
						else
						{
							Simulation.SnapshotHistory.DisposeOlderThan((Simulation.InterpFrom?.Tick).GetValueOrDefault());
						}
					}
				}
				catch (Exception msg)
				{
					Log.Error(this, msg);
					Shutdown(destroyGameObject: true, ShutdownReason.Error);
				}
				finally
				{
					_simulationPhase = SimulationPhase.None;
				}
			}
		}
		if (_deferredShutdownParams.ShutdownRequested)
		{
			Shutdown(_deferredShutdownParams.DestroyGO, _deferredShutdownParams.ShutdownReason);
		}
	}

	private void ExpirePredictedSpawns()
	{
		if (!IsClient || _simulation.LatestServerState == null)
		{
			return;
		}
		for (int i = 0; i < _predictionSpawns.Count; i++)
		{
			NetworkObject networkObject = _predictionSpawns[i];
			if (networkObject.PredictedSpawn.Tick <= _simulation.LatestServerState.Tick)
			{
				_predictionSpawns.RemoveAt(i--);
				InvokeMethodOnPredictedSpawnedObject(networkObject, (NetworkObject o, IPredictedSpawnBehaviour b) =>
				{
					b.PredictedSpawnFailed();
				});
			}
		}
		for (int num = 0; num < _predictionDespawns.Count; num++)
		{
			NetworkObject networkObject2 = _predictionDespawns[num];
			if (!(networkObject2.PredictedSpawn.Tick <= _simulation.LatestServerState.Tick))
			{
				continue;
			}
			_predictionDespawns.RemoveAt(num--);
			if (Exists(networkObject2))
			{
				InvokeMethodOnPredictedDespawnedObject(networkObject2, (NetworkObject o, IPredictedDespawnBehaviour b) =>
				{
					b.PredictedDespawnFailed();
				});
				_behaviourUpdater.AddObject(this, networkObject2, _simulation.IsInTick);
			}
		}
	}

	private void RegisterNetworkCallbacks()
	{
		if (!this || !base.gameObject || _callbacks == null || _callbacks.Count != 0)
		{
			return;
		}
		base.gameObject.GetComponents(_callbacksBuffer);
		try
		{
			foreach (INetworkRunnerCallbacks item in _callbacksBuffer)
			{
				AddCallbacks(item);
			}
		}
		finally
		{
			_callbacksBuffer.Clear();
		}
	}

	public void SendReliableDataToPlayer(PlayerRef player, byte[] data)
	{
		if (Simulation.IsPlayer && Simulation.LocalPlayer == player)
		{
			Simulation.Callbacks.OnReliableData(player, data);
		}
		else if (IsServer)
		{
			Simulation.SendReliableData(player, player, data);
		}
		else
		{
			Simulation.SendReliableData(0, player, data);
		}
	}

	public void SendReliableDataToServer(byte[] data)
	{
		if (IsClient)
		{
			Simulation.SendReliableData(0, PlayerRef.None, data);
		}
		else
		{
			Simulation.Callbacks.OnReliableData(PlayerRef.None, data);
		}
	}

	public void SetPlayerAlwaysInterested(PlayerRef player, NetworkObject networkObject, bool alwaysInterested)
	{
		if (Exists(networkObject))
		{
			Simulation.SetPlayerAlwaysInterested(player, networkObject, alwaysInterested);
		}
	}

	public void AddPlayerAreaOfInterest(PlayerRef player, Vector3 position, float extent, int layersMask = -1)
	{
		Simulation.AddPlayerAreaOfInterest(player, position, extent, layersMask);
	}

	public unsafe T? GetInputForPlayer<T>(PlayerRef player) where T : unmanaged, INetworkInput
	{
		SimulationInput inputForPlayer = _simulation.GetInputForPlayer(player);
		if (inputForPlayer != null && new NetworkInput(inputForPlayer.Data, Simulation.Config.InputDataWordCount).TryGet<T>(out var input))
		{
			return input;
		}
		return null;
	}

	public unsafe NetworkInput? GetRawInputForPlayer(PlayerRef player)
	{
		SimulationInput inputForPlayer = _simulation.GetInputForPlayer(player);
		if (inputForPlayer != null)
		{
			return new NetworkInput(inputForPlayer.Data, Simulation.Config.InputDataWordCount);
		}
		return null;
	}

	public bool TryGetInputForPlayer<T>(PlayerRef player, out T input) where T : unmanaged, INetworkInput
	{
		T? inputForPlayer = GetInputForPlayer<T>(player);
		if (inputForPlayer.HasValue)
		{
			input = inputForPlayer.Value;
			return true;
		}
		input = default;
		return false;
	}

	public NetworkObject FindObject(NetworkId oref)
	{
		if (_objects.TryGet(oref, out var value))
		{
			return value;
		}
		return null;
	}

	public bool TryFindObject(NetworkId oref, out NetworkObject obj)
	{
		return _objects.TryGet(oref, out obj);
	}

	public bool TryFindBehaviour(NetworkBehaviourId bref, out NetworkBehaviour behaviour)
	{
		if (_objects.TryGet(bref.Object, out var value) && bref.Behaviour >= 0 && bref.Behaviour < value.NetworkedBehaviours.Length)
		{
			behaviour = value.NetworkedBehaviours[bref.Behaviour];
			return true;
		}
		behaviour = null;
		return false;
	}

	public bool TryFindBehaviour<T>(NetworkBehaviourId id, out T behaviour) where T : NetworkBehaviour
	{
		if (TryFindBehaviour(id, out var behaviour2))
		{
			return BehaviourUtils.IsAlive(behaviour = behaviour2 as T);
		}
		behaviour = null;
		return false;
	}

	public unsafe bool GetInterpolationData(NetworkBehaviour behaviour, bool predicted, out InterpolationData data)
	{
		predicted = predicted || IsServer;
		SimulationSnapshot simulationSnapshot = (predicted ? _simulation.StatePrevious : _simulation.InterpFrom);
		SimulationSnapshot simulationSnapshot2 = (predicted ? _simulation.State : _simulation.InterpTo);
		if (_simulation != null && simulationSnapshot != null && simulationSnapshot.TryGetObject(behaviour.Object.Id, out var header) && simulationSnapshot2.TryGetObject(behaviour.Object.Id, out var header2))
		{
			data.Alpha = (predicted ? _simulation.StateAlpha : _simulation.InterpAlpha);
			data.FromTick = simulationSnapshot.Tick;
			data.ToTick = simulationSnapshot2.Tick;
			data.From = (int*)header + behaviour.WordOffset;
			data.To = (int*)header2 + behaviour.WordOffset;
			return true;
		}
		data = default;
		return false;
	}

	public T TryGetNetworkedBehaviourFromNetworkedObjectRef<T>(NetworkId id) where T : NetworkBehaviour
	{
		if (_objects.TryGet(id, out var value))
		{
			if (value.TryGetBehaviour<T>(out var behaviour))
			{
				return behaviour;
			}
			return value.GetBehaviour<T>();
		}
		return null;
	}

	public NetworkId TryGetObjectRefFromNetworkedBehaviour(NetworkBehaviour behaviour)
	{
		if (BehaviourUtils.IsAlive(behaviour) && behaviour.Object.IsValid)
		{
			return behaviour.Object.Id;
		}
		return default;
	}

	public NetworkBehaviourId TryGetNetworkedBehaviourId(NetworkBehaviour behaviour)
	{
		if (BehaviourUtils.IsAlive(behaviour) && behaviour.Object.IsValid)
		{
			NetworkBehaviourId result = default;
			result.Behaviour = behaviour.ObjectIndex;
			result.Object = behaviour.Object.Id;
			return result;
		}
		return default;
	}

	public bool SetSimulationState(NetworkObject obj, bool simulate)
	{
		if (Exists(obj) && obj.InSimulation != simulate)
		{
			if (simulate)
			{
				ObjectJoinSimulation(obj.Id);
				return true;
			}
			ObjectLeaveSimulation(obj.Id);
			return true;
		}
		return false;
	}

	public bool Exists(NetworkObject obj)
	{
		return BehaviourUtils.IsNotNull(obj) && _simulation != null && _simulation.State.ContainsObject(obj.Id);
	}

	public bool Exists(NetworkId id)
	{
		return id.IsValid && _simulation != null && _simulation.State.ContainsObject(id);
	}

	internal bool ExistsIn(NetworkObject obj, SimulationSnapshot snapshot)
	{
		return snapshot != null && BehaviourUtils.IsNotNull(obj) && snapshot.ContainsObject(obj.Id);
	}

	public T Spawn<T>(T prefab, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, OnBeforeSpawned onBeforeSpawned = null, NetworkObjectPredictionKey? predictionKey = null, bool syncPhysics = true) where T : SimulationBehaviour
	{
		if (BehaviourUtils.IsNull(prefab))
		{
			throw new ArgumentNullException("prefab");
		}
		NetworkObject component = prefab.GetComponent<NetworkObject>();
		if (BehaviourUtils.IsAlive(component))
		{
			NetworkObject networkObject = Spawn(component, position, rotation, inputAuthority, onBeforeSpawned, predictionKey, syncPhysics);
			if (BehaviourUtils.IsAlive(networkObject))
			{
				return networkObject.GetComponent<T>();
			}
		}
		else
		{
			Log.Warn(this, "Found no NetworkObject on the same gameobject as " + typeof(T).Name);
		}
		return null;
	}

	public NetworkObject Spawn(GameObject prefab, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, OnBeforeSpawned onBeforeSpawned = null, NetworkObjectPredictionKey? predictionKey = null, bool syncPhysics = true)
	{
		if ((object)prefab == null)
		{
			throw new ArgumentNullException("prefab");
		}
		NetworkObject component = prefab.GetComponent<NetworkObject>();
		if (BehaviourUtils.IsAlive(component))
		{
			NetworkObject networkObject = Spawn(component, position, rotation, inputAuthority, onBeforeSpawned, predictionKey, syncPhysics);
			if (BehaviourUtils.IsAlive(networkObject))
			{
				return networkObject;
			}
		}
		else
		{
			Log.Warn(this, "Found no NetworkObject on the gameobject " + prefab.name);
		}
		return null;
	}

	public NetworkObject Spawn(NetworkObject prefab, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, OnBeforeSpawned onBeforeSpawned = null, NetworkObjectPredictionKey? predictionKey = null, bool syncPhysics = true)
	{
		Assert.Always(BehaviourUtils.IsAlive(prefab), "prefab can't be null");
		if (BehaviourUtils.IsNull(prefab))
		{
			throw new ArgumentNullException("prefab");
		}
		if (!prefab.Flags.IsPrefab() || !prefab.NetworkGuid.IsValid)
		{
			throw new InvalidOperationException($"Not a prefab or has not been baked: {prefab}");
		}
		if (!Config.PrefabTable.TryGetId(prefab.NetworkGuid, out var id))
		{
			throw new InvalidOperationException($"Prefab {prefab} has not been added to the ObjectTable.");
		}
		NetworkObject resumeNO = ((IsResume && prefab.Id.IsValid) ? prefab : null);
		Assert.Check(id.IsValid);
		return Spawn(id, position, rotation, inputAuthority, onBeforeSpawned, predictionKey, syncPhysics, resumeNO);
	}

	public unsafe NetworkObject Spawn(NetworkPrefabRef prefabRef, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, OnBeforeSpawned onBeforeSpawned = null, NetworkObjectPredictionKey? predictionKey = null)
	{
		return Spawn(new NetworkObjectGuid((byte*)(&prefabRef)), position, rotation, inputAuthority, onBeforeSpawned, predictionKey);
	}

	public NetworkObject Spawn(NetworkPrefabAsset prefabAsset, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, OnBeforeSpawned onBeforeSpawned = null, NetworkObjectPredictionKey? predictionKey = null)
	{
		if (prefabAsset == null)
		{
			throw new ArgumentNullException("prefabAsset");
		}
		return Spawn(prefabAsset.AssetGuid, position, rotation, inputAuthority, onBeforeSpawned, predictionKey);
	}

	public NetworkObject Spawn(NetworkObjectGuid prefabGuid, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, OnBeforeSpawned onBeforeSpawned = null, NetworkObjectPredictionKey? predictionKey = null, bool syncPhysics = true)
	{
		if (!prefabGuid.IsValid)
		{
			throw new ArgumentException("Not valid.", "prefabGuid");
		}
		if (!Config.PrefabTable.TryGetId(prefabGuid, out var id))
		{
			throw new InvalidOperationException($"Prefab {prefabGuid} has not been added to the ObjectTable.");
		}
		Assert.Check(id.IsValid);
		return Spawn(id, position, rotation, inputAuthority, onBeforeSpawned, predictionKey, syncPhysics);
	}

	public NetworkObject Spawn(NetworkPrefabId prefabId, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, OnBeforeSpawned onBeforeSpawned = null, NetworkObjectPredictionKey? predictionKey = null, bool syncPhysics = true, NetworkObject resumeNO = null)
	{
		resumeNO = ((IsResume && resumeNO != null && resumeNO.Id.IsValid) ? resumeNO : null);
		return SpawnInternal(prefabId, position, rotation, inputAuthority, onBeforeSpawned, predictionKey, syncPhysics, resumeNO);
	}

	public void Despawn(NetworkObject networkObject, bool allowPredicted = false)
	{
		if (Exists(networkObject))
		{
			if (networkObject.HasStateAuthority)
			{
				if (!BehaviourUtils.IsSame(this, networkObject.Runner))
				{
					throw new InvalidOperationException("Object does not belong to this runner");
				}
				Destroy(networkObject, NetworkObjectDestroyFlags.DestroyState | NetworkObjectDestroyFlags.DestroyedByDespawn);
			}
			else if (allowPredicted && Topology == SimulationConfig.Topologies.ClientServer && !IsResimulation)
			{
				InvokeMethodOnPredictedDespawnedObject(networkObject, (NetworkObject o, IPredictedDespawnBehaviour b) =>
				{
					b.PredictedDespawn();
				});
				networkObject.PredictedSpawn.Tick = _simulation.Tick;
				_predictionDespawns.Add(networkObject);
				_behaviourUpdater.RemoveObject(this, networkObject);
			}
		}
		else if (allowPredicted && BehaviourUtils.IsAlive(networkObject) && networkObject.IsPredictedSpawn)
		{
			_networkObjectPool.ReleaseInstance(this, networkObject, isSceneObject: false);
		}
	}

	public T GetGlobal<T>() where T : SimulationBehaviour
	{
		T behaviour;
		return TryGetBehaviour<T>(out behaviour) ? behaviour : null;
	}

	public T AddOrGetGlobal<T>() where T : SimulationBehaviour
	{
		if (!TryGetBehaviour<T>(out var behaviour))
		{
			behaviour = AddBehaviour<T>();
			AddSimulationBehaviour(behaviour);
		}
		return behaviour;
	}

	public void AddGlobal<T>() where T : SimulationBehaviour
	{
		if (!TryGetBehaviour<T>(out var behaviour))
		{
			behaviour = AddBehaviour<T>();
			AddSimulationBehaviour(behaviour);
		}
	}

	public void RemoveGlobal<T>() where T : SimulationBehaviour
	{
		if (TryGetBehaviour<T>(out var behaviour))
		{
			behaviour.Runner = null;
			Behaviour.DestroyBehaviour(behaviour);
		}
	}

	public void AddSimulationBehaviour(SimulationBehaviour behaviour, NetworkObject obj = null)
	{
		behaviour.Runner = this;
		behaviour.Object = obj;
		if (_behaviourUpdater == null)
		{
			throw new NullReferenceException("SimulationBehaviourUpdater is null. Are you trying to AddSimulationBehaviour on a NetworkRunner which has not yet been started?");
		}
		_behaviourUpdater.AddBehaviour(behaviour, skipFirstCall: false);
	}

	public void RemoveSimulationBehavior(SimulationBehaviour behaviour)
	{
		behaviour.Runner = null;
		behaviour.Object = null;
		if (_behaviourUpdater == null)
		{
			throw new NullReferenceException("SimulationBehaviourUpdater is null. Are you trying to RemoveSimulationBehavior on a NetworkRunner which has not yet been started?");
		}
		_behaviourUpdater.RemoveBehaviour(behaviour);
	}

	internal void Destroy(NetworkObject networkObject, NetworkObjectDestroyFlags flags)
	{
		if (networkObject.ObjectInterest == NetworkObject.ObjectInterestModes.AllPlayers)
		{
			_simulation.RemoveFromGlobalObjectInterest(networkObject);
		}
		if (!Exists(networkObject))
		{
			return;
		}
		if (IsClient && networkObject.IsPredictedSpawn)
		{
			_predictionDespawns.Remove(networkObject);
		}
		int count = _destroyIdsBuffer.Count;
		_destroyIdsBuffer.Add(networkObject.Id);
		if (!networkObject.IsSceneObject)
		{
			NetworkObject[] nestedObjects = networkObject.NestedObjects;
			foreach (NetworkObject networkObject2 in nestedObjects)
			{
				_destroyIdsBuffer.Add(networkObject2.Id);
			}
		}
		int count2 = _destroyIdsBuffer.Count;
		try
		{
			PerformPrefabCleanup(networkObject, flags.Get(NetworkObjectDestroyFlags.DestroyedByEngine), hasState: true);
			Assert.Check(count2 <= _destroyIdsBuffer.Count);
			Assert.Check(count <= _destroyIdsBuffer.Count);
			for (int j = count; j < count2; j++)
			{
				NetworkId id = _destroyIdsBuffer[j];
				if (Exists(id))
				{
					_simulation.Replicator.OnObjectDestroyed(id, flags);
				}
			}
		}
		finally
		{
			_destroyIdsBuffer.RemoveRange(count, count2 - count);
		}
	}

	internal unsafe void DestroyOrphaned(NetworkObject networkObject, bool destroyedByEngine)
	{
		if (networkObject.ObjectInterest == NetworkObject.ObjectInterestModes.AllPlayers)
		{
			_simulation.RemoveFromGlobalObjectInterest(networkObject);
		}
		if (networkObject.Id.IsValid && networkObject.Changed != null)
		{
			PerformPrefabCleanup(networkObject, destroyedByEngine, hasState: false);
		}
	}

	internal void DestroyOrphanedUnattached(NetworkObject networkObject)
	{
		if (networkObject.IsSceneObject)
		{
			PerformSceneObjectCleanup(networkObject);
		}
	}

	internal unsafe void PerformPrefabCleanup(NetworkObject networkObject, bool destroyedByEngine, bool hasState)
	{
		Assert.Check(networkObject.Changed);
		Assert.Check(networkObject.Id.IsValid);
		NetworkId id = networkObject.Id;
		bool isSceneObject = networkObject.IsSceneObject;
		if ((networkObject.Flags & NetworkObjectFlags.Spawned) != NetworkObjectFlags.None)
		{
			InvokeDespawnedCallback(networkObject, hasState);
		}
		if (!destroyedByEngine && !isSceneObject)
		{
			NetworkObject[] nestedObjects = networkObject.NestedObjects;
			foreach (NetworkObject networkObject2 in nestedObjects)
			{
				if ((networkObject2.Flags & NetworkObjectFlags.Spawned) != NetworkObjectFlags.None)
				{
					InvokeDespawnedCallback(networkObject2, hasState);
				}
			}
		}
		FreeObject(networkObject);
		if (!destroyedByEngine && !isSceneObject)
		{
			NetworkObject[] nestedObjects2 = networkObject.NestedObjects;
			foreach (NetworkObject networkObject3 in nestedObjects2)
			{
				if (BehaviourUtils.IsAlive(networkObject3) && networkObject3.Id.IsValid)
				{
					FreeObject(networkObject3);
				}
			}
		}
		if (isSceneObject)
		{
			PerformSceneObjectCleanup(networkObject);
		}
		if (!destroyedByEngine)
		{
			_networkObjectPool.ReleaseInstance(this, networkObject, isSceneObject);
		}
		unsafe void FreeObject(NetworkObject obj)
		{
			Allocator.Free(_changedAllocator, obj.Changed);
			obj.Changed = null;
			if (!_objects.Remove(obj.Id))
			{
				Assert.Fail();
			}
			for (int k = 0; k < obj.SimulationBehaviours.Length; k++)
			{
				RemoveSimulationBehavior(obj.SimulationBehaviours[k]);
			}
			for (int l = 0; l < obj.NetworkedBehaviours.Length; l++)
			{
				NetworkBehaviour networkBehaviour = obj.NetworkedBehaviours[l];
				_behaviourUpdater.RemoveBehaviour(networkBehaviour);
				networkBehaviour.Object = null;
				networkBehaviour.Runner = null;
				networkBehaviour.Ptr = default;
			}
			obj.ResetNetworkState();
		}
	}

	private void PerformSceneObjectCleanup(NetworkObject networkObject)
	{
		if (_sceneObjectResolver == null)
		{
			Assert.Check(_sceneObjectLoopkup);
			if (!IsSceneMaster && !_sceneObjectLoopkup.Remove(networkObject.NetworkGuid))
			{
				Log.TraceWarn(networkObject, $"Was a scene object, but removing from scene object lookup failed. ({networkObject.NetworkGuid})");
			}
		}
	}

	public unsafe void Attach(NetworkObject networkObject, PlayerRef? inputAuthority = null)
	{
		NetworkObjectHeader* header = Simulation.AllocateObject(GetNextId(), default, NetworkObject.GetWordCount(networkObject), out var groups);
		InitializeNetworkObjectAssignRunner(networkObject);
		InitializeNetworkObjectInstance(header, networkObject, inputAuthority, AttachOptions.LocalSpawn, groups);
		InitializeNetworkObjectState(networkObject, AttachOptions.LocalSpawn);
		InvokeBeforeSpawnedCallbacks(networkObject, AttachOptions.LocalSpawn, null);
		InvokeSpawnedCallback(networkObject);
		InvokeAfterSpawnedCallback(networkObject);
	}

	internal void AttachActivatedByUser(NetworkObject networkObject)
	{
		AttachOptions options = NetworkObjectFlagsToAttachOptions(networkObject.Flags);
		InitializeNetworkObjectState(networkObject, options);
		InvokeBeforeSpawnedCallbacks(networkObject, options, null);
		InvokeSpawnedCallback(networkObject);
		InvokeAfterSpawnedCallback(networkObject);
		if ((networkObject.Flags & NetworkObjectFlags.PredictedSpawn) == NetworkObjectFlags.PredictedSpawn)
		{
			InvokeMethodOnPredictedSpawnedObject(networkObject, (NetworkObject o, IPredictedSpawnBehaviour b) =>
			{
				b.PredictedSpawnSuccess();
			});
		}
	}

	[Obsolete("Use RegisterSceneObjects instead")]
	public void RegisterUniqueObjects(IEnumerable<NetworkObject> objects)
	{
		RegisterSceneObjects(objects);
	}

	public unsafe void RegisterSceneObjects(IEnumerable<NetworkObject> objects)
	{
		if (objects == null)
		{
			throw new ArgumentNullException("objects");
		}
		if (IsSceneMaster)
		{
			Assert.Check(_activeSceneObjectsBuffer.Count == 0);
			AttachOptions attachOptions = AttachOptions.LocalSpawn;
			bool isSharedModeMasterClient = IsSharedModeMasterClient;
			try
			{
				foreach (NetworkObject @object in objects)
				{
					ThrowIfInvalidSceneObject(@object);
					NetworkId nextId = GetNextId();
					NetworkObjectHeader* header = Simulation.AllocateObject(nextId, default, NetworkObject.GetWordCount(@object), out var groups);
					if (Simulation.IsResume && _sceneObjectResolver == null)
					{
						_sceneObjectLoopkup.Add(@object.NetworkGuid, @object);
					}
					InitializeNetworkObjectAssignRunner(@object);
					InitializeNetworkObjectInstance(header, @object, null, attachOptions, groups);
					if (!@object.gameObject.activeInHierarchy)
					{
						Assert.Check(!IsAwakeAtInitialization(@object));
						Assert.Check(NetworkObjectFlagsToAttachOptions(@object.Flags) == attachOptions);
						if (isSharedModeMasterClient)
						{
							Simulation.SendInternalSimulationMessage(SimulationMessageInternalTypes.SharedModeSceneObjectData, @object.Header, @object.Header->WordCount * 4, null);
						}
					}
					else
					{
						_activeSceneObjectsBuffer.Add(@object);
					}
				}
				foreach (NetworkObject item in _activeSceneObjectsBuffer)
				{
					Assert.Check(BehaviourUtils.IsAlive(item), "Object has been destroyed while attaching scene objects");
					InitializeNetworkObjectState(item, attachOptions);
				}
				foreach (NetworkObject item2 in _activeSceneObjectsBuffer)
				{
					Assert.Check(BehaviourUtils.IsAlive(item2), "Object has been destroyed while attaching scene objects");
					InvokeBeforeSpawnedCallbacks(item2, attachOptions, null);
				}
				foreach (NetworkObject item3 in _activeSceneObjectsBuffer)
				{
					Assert.Check(BehaviourUtils.IsAlive(item3), "Object has been destroyed while attaching scene objects");
					if (isSharedModeMasterClient)
					{
						Simulation.SendInternalSimulationMessage(SimulationMessageInternalTypes.SharedModeSceneObjectData, item3.Header, item3.Header->WordCount * 4, null);
					}
					InvokeSpawnedCallback(item3);
				}
				foreach (NetworkObject item4 in _activeSceneObjectsBuffer)
				{
					Assert.Check(BehaviourUtils.IsAlive(item4), "Object has been destroyed while attaching scene objects");
					InvokeAfterSpawnedCallback(item4);
				}
				return;
			}
			finally
			{
				_activeSceneObjectsBuffer.Clear();
			}
		}
		if (_sceneObjectResolver != null)
		{
			return;
		}
		foreach (NetworkObject object2 in objects)
		{
			ThrowIfInvalidSceneObject(object2);
			try
			{
				_sceneObjectLoopkup.Add(object2.NetworkGuid, object2);
				InitializeNetworkObjectAssignRunner(object2);
			}
			catch (ArgumentException innerException)
			{
				throw new InvalidOperationException($"Object already registered: {BehaviourUtils.GetDump(object2)}", innerException);
			}
		}
		static void ThrowIfInvalidSceneObject(NetworkObject obj)
		{
			if (!BehaviourUtils.IsAlive(obj))
			{
				throw new ArgumentException("Sequence contains null or destroyed elements", "objects");
			}
			if (!obj.Flags.IsSceneObject())
			{
				throw new InvalidOperationException($"{BehaviourUtils.GetName(obj)}: not a scene object, according to flags. Possibly the scene was not baked.");
			}
			if (!obj.NetworkGuid.IsValid)
			{
				throw new InvalidOperationException(string.Format("{0}: invalid {1}.", BehaviourUtils.GetName(obj), "NetworkGuid"));
			}
			if (obj.IsValid)
			{
				throw new InvalidOperationException($"{BehaviourUtils.GetName(obj)}: already attached.");
			}
		}
	}

	internal void InvokeOnBeforePhysicsStep()
	{
		EngineProfiler.Begin("NetworkRunner.InvokeOnBeforePhysicsStep");
		CallbackInterfaceInvoker.IBeforePhysicsStep(_behaviourUpdater);
		EngineProfiler.End();
	}

	internal void InvokeOnAfterPhysicsStep()
	{
		EngineProfiler.Begin("NetworkRunner.InvokeOnAfterPhysicsStep");
		CallbackInterfaceInvoker.IAfterPhysicsStep(_behaviourUpdater);
		EngineProfiler.End();
	}

	internal void InvokeOnAfterPhysicsSyncTransforms2D()
	{
		EngineProfiler.Begin("NetworkRunner.InvokeOnAfterPhysicsSyncTransforms2D");
		CallbackInterfaceInvoker.IAfterPhysicsSyncTransforms2D(_behaviourUpdater);
		EngineProfiler.End();
	}

	internal void InvokeOnAfterPhysicsSyncTransforms3D()
	{
		EngineProfiler.Begin("NetworkRunner.InvokeOnAfterPhysicsSyncTransforms3D");
		CallbackInterfaceInvoker.IAfterPhysicsSyncTransforms3D(_behaviourUpdater);
		EngineProfiler.End();
	}

	internal void InvokeOnBeforeHitboxRegistration()
	{
		EngineProfiler.Begin("NetworkRunner.InvokeOnBeforeHitboxRegistration");
		CallbackInterfaceInvoker.IBeforeHitboxRegistration(_behaviourUpdater);
		EngineProfiler.End();
	}

	private bool ExistsIn(SimulationSnapshot snapshot, NetworkObject obj)
	{
		return snapshot?.ContainsObject(obj.Id) ?? false;
	}

	private void InvokeMethodOnPredictedSpawnedObject(NetworkObject obj, Action<NetworkObject, IPredictedSpawnBehaviour> callback)
	{
		for (int i = 0; i < obj.SimulationBehaviours.Length; i++)
		{
			if (obj.SimulationBehaviours[i] is IPredictedSpawnBehaviour arg)
			{
				try
				{
					callback(obj, arg);
				}
				catch (Exception exn)
				{
					Log.Exception(this, exn);
				}
			}
		}
		for (int j = 0; j < obj.NetworkedBehaviours.Length; j++)
		{
			if (obj.NetworkedBehaviours[j] is IPredictedSpawnBehaviour arg2)
			{
				try
				{
					callback(obj, arg2);
				}
				catch (Exception exn2)
				{
					Log.Exception(this, exn2);
				}
			}
		}
	}

	private void InvokeMethodOnPredictedDespawnedObject(NetworkObject obj, Action<NetworkObject, IPredictedDespawnBehaviour> callback)
	{
		for (int i = 0; i < obj.SimulationBehaviours.Length; i++)
		{
			if (obj.SimulationBehaviours[i] is IPredictedDespawnBehaviour arg)
			{
				try
				{
					callback(obj, arg);
				}
				catch (Exception exn)
				{
					Log.Exception(this, exn);
				}
			}
		}
		for (int j = 0; j < obj.NetworkedBehaviours.Length; j++)
		{
			if (obj.NetworkedBehaviours[j] is IPredictedDespawnBehaviour arg2)
			{
				try
				{
					callback(obj, arg2);
				}
				catch (Exception exn2)
				{
					Log.Exception(this, exn2);
				}
			}
		}
	}

	private unsafe NetworkObject SpawnInternal(NetworkPrefabId prefabId, Vector3? position = null, Quaternion? rotation = null, PlayerRef? inputAuthority = null, object onBeforeSpawned = null, NetworkObjectPredictionKey? predictionKey = null, bool syncPhysics = true, NetworkObject resumeNO = null)
	{
		Assert.Check(prefabId.IsValid);
		NetworkProjectConfig.SceneLoadSpawnModes sceneLoadSpawnModes = _config.SceneLoadSpawnMode;
		if (IsResume)
		{
			sceneLoadSpawnModes = NetworkProjectConfig.SceneLoadSpawnModes.Allowed;
		}
		bool flag = Topology == SimulationConfig.Topologies.Shared && !LocalPlayer.IsValid;
		if (flag)
		{
			sceneLoadSpawnModes = NetworkProjectConfig.SceneLoadSpawnModes.Queued;
		}
		switch (sceneLoadSpawnModes)
		{
		case NetworkProjectConfig.SceneLoadSpawnModes.NotAllowed:
			if (!_sceneManager.IsReady(this))
			{
				Log.Error(this, "Trying to spawn object during scene load or runner initialization is not allowed. To change this set the SceneLoadSpawnMode setting to either Allowed or Queued.");
				return null;
			}
			break;
		case NetworkProjectConfig.SceneLoadSpawnModes.Queued:
			if (!_sceneManager.IsReady(this) | flag)
			{
				_spawnQueue.Enqueue(new SpawnQueueEntry
				{
					PrefabId = prefabId,
					Position = position,
					Rotation = rotation,
					InputAuthority = inputAuthority,
					OnBeforeSpawned = onBeforeSpawned,
					PredictionKey = predictionKey,
					SyncPhysics = syncPhysics
				});
				return null;
			}
			break;
		}
		if (IsClient && _config.Simulation.Topology == SimulationConfig.Topologies.ClientServer)
		{
			if (!predictionKey.HasValue)
			{
				return null;
			}
			if (Simulation.Stage == SimulationStages.Forward)
			{
				NetworkObject networkObject = CreateInstance(prefabId, null);
				if (BehaviourUtils.IsAlive(networkObject))
				{
					networkObject.Runner = this;
					networkObject.PredictedSpawn.Key = predictionKey.Value;
					networkObject.PredictedSpawn.Tick = Simulation.Tick;
					networkObject.PredictedSpawn.Prefab = prefabId;
					bool hasValue = position.HasValue;
					bool hasValue2 = rotation.HasValue;
					if (hasValue)
					{
						networkObject.transform.position = position.Value;
					}
					if (hasValue2)
					{
						networkObject.transform.rotation = ((rotation.Value == default(Quaternion)) ? Quaternion.identity : rotation.Value);
					}
					if (syncPhysics && (hasValue | hasValue2))
					{
						Physics.SyncTransforms();
						Physics2D.SyncTransforms();
					}
					_predictionSpawns.Add(networkObject);
					for (int i = 0; i < networkObject.SimulationBehaviours.Length; i++)
					{
						networkObject.SimulationBehaviours[i].Object = networkObject;
						networkObject.SimulationBehaviours[i].Runner = this;
					}
					for (int j = 0; j < networkObject.NetworkedBehaviours.Length; j++)
					{
						networkObject.NetworkedBehaviours[j].Object = networkObject;
						networkObject.NetworkedBehaviours[j].Runner = this;
					}
					if (onBeforeSpawned is OnBeforeSpawned onBeforeSpawned2)
					{
						try
						{
							onBeforeSpawned2(this, networkObject);
						}
						catch (Exception exn)
						{
							Log.Exception(this, exn);
						}
					}
					InvokeMethodOnPredictedSpawnedObject(networkObject, (NetworkObject o, IPredictedSpawnBehaviour b) =>
					{
						b.PredictedSpawnSpawned();
					});
					MoveToRunnerScene(networkObject.gameObject);
				}
				return networkObject;
			}
			for (int num = 0; num < _predictionSpawns.Count; num++)
			{
				if (_predictionSpawns[num].PredictedSpawn.Key == predictionKey.Value)
				{
					return _predictionSpawns[num];
				}
			}
			return null;
		}
		NetworkObject networkObject2 = CreateInstance(prefabId, null);
		if (BehaviourUtils.IsAlive(networkObject2))
		{
			bool hasValue3 = position.HasValue;
			bool hasValue4 = rotation.HasValue;
			if (hasValue3)
			{
				networkObject2.transform.position = position.Value;
			}
			if (hasValue4)
			{
				networkObject2.transform.rotation = ((rotation.Value == default(Quaternion)) ? Quaternion.identity : rotation.Value);
			}
			if (syncPhysics && (hasValue3 | hasValue4))
			{
				Physics.SyncTransforms();
				Physics2D.SyncTransforms();
			}
			resumeNO = ((IsResume && resumeNO != null && resumeNO.Id.IsValid) ? resumeNO : null);
			networkObject2.IsResume = IsResume;
			NetworkId id = (IsResume ? CheckIdOrGetNewId(resumeNO) : GetNextId());
			NetworkObjectHeader* ptr = Simulation.AllocateObject(id, prefabId, NetworkObject.GetWordCount(networkObject2), out var groups);
			AttachOptions options = AttachOptions.LocalSpawn;
			InitializeNetworkObjectAssignRunner(networkObject2);
			InitializeNetworkObjectInstance(ptr, networkObject2, inputAuthority, options, groups);
			if (predictionKey.HasValue && _config.Simulation.Topology != SimulationConfig.Topologies.Shared)
			{
				ptr->PredictionKey = predictionKey.Value;
			}
			for (int num2 = 0; num2 < networkObject2.NestedObjects.Length; num2++)
			{
				NetworkObject networkObject3 = networkObject2.NestedObjects[num2];
				networkObject3.IsResume = IsResume;
				NetworkId id2 = (IsResume ? CheckIdOrGetNewId(((object)resumeNO != null) ? resumeNO.NestedObjects[num2] : null) : GetNextId());
				NetworkObjectHeader* header = Simulation.AllocateObject(id2, default, NetworkObject.GetWordCount(networkObject3), out var groups2, ptr->Id, new NetworkObjectNestingKey(num2 + 1));
				InitializeNetworkObjectAssignRunner(networkObject3);
				InitializeNetworkObjectInstance(header, networkObject3, inputAuthority, options, groups2);
			}
			if (IsAwakeAtInitialization(networkObject2))
			{
				InitializeNetworkObjectState(networkObject2, options);
				NetworkObject[] nestedObjects = networkObject2.NestedObjects;
				foreach (NetworkObject networkObject4 in nestedObjects)
				{
					if (IsAwakeAtInitialization(networkObject4))
					{
						InitializeNetworkObjectState(networkObject4, options);
					}
				}
				InvokeBeforeSpawnedCallbacks(networkObject2, options, onBeforeSpawned as OnBeforeSpawned);
				NetworkObject[] nestedObjects2 = networkObject2.NestedObjects;
				foreach (NetworkObject networkObject5 in nestedObjects2)
				{
					if (IsAwakeAtInitialization(networkObject5))
					{
						InvokeBeforeSpawnedCallbacks(networkObject5, options, null);
					}
				}
				InvokeSpawnedCallback(networkObject2);
				NetworkObject[] nestedObjects3 = networkObject2.NestedObjects;
				foreach (NetworkObject networkObject6 in nestedObjects3)
				{
					if (IsAwakeAtInitialization(networkObject6))
					{
						InvokeSpawnedCallback(networkObject6);
					}
				}
				InvokeAfterSpawnedCallback(networkObject2);
				NetworkObject[] nestedObjects4 = networkObject2.NestedObjects;
				foreach (NetworkObject networkObject7 in nestedObjects4)
				{
					if (IsAwakeAtInitialization(networkObject7))
					{
						InvokeAfterSpawnedCallback(networkObject7);
					}
				}
			}
			else
			{
				Assert.Check(!networkObject2.gameObject.activeInHierarchy, "Expected to be inactive", networkObject2.Name);
			}
			if (networkObject2.DestroyWhenStateAuthorityLeaves)
			{
				ptr->Flags |= NetworkObjectHeaderFlags.DestroyWhenStateAuthorityLeaves;
			}
			if (networkObject2.AllowStateAuthorityOverride)
			{
				ptr->Flags |= NetworkObjectHeaderFlags.AllowStateAuthorityOverride;
			}
			if (IsClient)
			{
				ptr->Flags |= NetworkObjectHeaderFlags.SpawnedByClient;
				if (_config.Simulation.Topology == SimulationConfig.Topologies.Shared)
				{
					Simulation.Replicator.OnObjectSpawnedLocal(ptr->Id);
					NetworkObject[] nestedObjects5 = networkObject2.NestedObjects;
					foreach (NetworkObject networkObject8 in nestedObjects5)
					{
						Assert.Check(BehaviourUtils.IsNotNull(networkObject8));
						if (BehaviourUtils.IsAlive(networkObject8) && networkObject8.Id.IsValid)
						{
							Simulation.Replicator.OnObjectSpawnedLocal(networkObject8.Id);
						}
					}
				}
			}
			return networkObject2;
		}
		return null;
		NetworkId CheckIdOrGetNewId(NetworkObject obj)
		{
			return (obj != null && obj.Id.IsValid) ? obj.Id : GetNextId();
		}
	}

	private unsafe NetworkId GetNextId()
	{
		NetworkId result = default;
		result.Raw = ++_idCounter;
		if (IsClient)
		{
			Assert.Check(Topology == SimulationConfig.Topologies.Shared);
			Assert.Check(LocalPlayer.IsValid);
			result.Raw &= 524287u;
			result.Raw |= (((Simulation.Client)Simulation).ServerConnection->Counter << 19) & 0xFFF80000u;
		}
		return result;
	}

	private unsafe NetworkObject CreateInstance(NetworkPrefabId prefab, NetworkObjectHeader* header)
	{
		if (header != null)
		{
			Assert.Check(header->Type.Equals(prefab));
		}
		if (prefab.IsNone)
		{
			return null;
		}
		NetworkObject networkObject = _networkObjectPool.AcquireInstance(this, new NetworkPrefabInfo
		{
			Prefab = prefab,
			Header = header
		});
		if (BehaviourUtils.IsAlive(networkObject))
		{
			networkObject.Flags = networkObject.Flags.SetType(NetworkObjectFlags.TypeSpawnedPrefab);
			NetworkObject[] nestedObjects = networkObject.NestedObjects;
			foreach (NetworkObject networkObject2 in nestedObjects)
			{
				networkObject2.Flags = networkObject2.Flags.SetType(NetworkObjectFlags.TypeSpawnedPrefabChild);
			}
			return networkObject;
		}
		Log.Error(this, $"Unknown {prefab}");
		return null;
	}

	private unsafe bool TryResolvePrefabInstance(NetworkObjectHeader* header, out NetworkObject result)
	{
		Assert.Check(header->SceneGuid == Guid.Empty);
		if ((bool)header->PredictionKey)
		{
			for (int i = 0; i < _predictionSpawns.Count; i++)
			{
				if (_predictionSpawns[i].PredictedSpawn.Key == header->PredictionKey && _predictionSpawns[i].PredictedSpawn.Prefab == header->Type)
				{
					NetworkObject networkObject = _predictionSpawns[i];
					_predictionSpawns.RemoveAt(i);
					result = networkObject;
					return true;
				}
			}
		}
		if (header->NestingKey.IsValid)
		{
			NetworkObject networkObject2 = FindObject(header->NestingRoot);
			if (BehaviourUtils.IsNotAlive(networkObject2))
			{
				result = null;
			}
			else if (header->NestingKey.Value > networkObject2.NestedObjects.Length)
			{
				result = null;
			}
			else
			{
				result = networkObject2.NestedObjects[header->NestingKey.Value - 1];
			}
			return BehaviourUtils.IsNotNull(result);
		}
		NetworkObject networkObject3 = CreateInstance(header->Type, header);
		if (BehaviourUtils.IsAlive(networkObject3))
		{
			NetworkObject[] nestedObjects = networkObject3.NestedObjects;
			foreach (NetworkObject networkObject4 in nestedObjects)
			{
				networkObject4.gameObject.SetActive(value: false);
			}
			result = networkObject3;
			return true;
		}
		result = null;
		return false;
	}

	private unsafe void InitializeNetworkObjectAssignRunner(NetworkObject instance)
	{
		Assert.Always(!instance.Id.IsValid, "The instance has already been initialized", BehaviourUtils.GetDump(instance));
		Assert.Check(instance.Ptr == null);
		instance.Runner = this;
		if (instance.Flags.IsActivatedByUser() && !instance.gameObject.activeInHierarchy)
		{
			AddInactiveObjectGuard(instance);
		}
	}

	private unsafe void InitializeNetworkObjectInstance(NetworkObjectHeader* header, NetworkObject instance, PlayerRef? inputAuthority, AttachOptions options, int* interestGroups)
	{
		Assert.Always(!instance.Id.IsValid, "The instance has already been initialized", BehaviourUtils.GetDump(instance), header->Id);
		Assert.Check(instance.Ptr == null);
		Assert.Check(instance.Runner == this, "Should have called InitializeNetworkObjectAssignRunner before");
		if (_config.NetworkIdIsObjectName)
		{
			instance.gameObject.name = header->Id.ToNamePrefixString() + instance.gameObject.name;
		}
		bool flag = (options & AttachOptions.LocalSpawn) == AttachOptions.LocalSpawn;
		instance.Id = header->Id;
		MoveToRunnerScene(instance.gameObject);
		if (instance.Flags.IsActivatedByUser())
		{
			if (instance.gameObject.activeInHierarchy)
			{
				Log.DebugWarn(instance, "Already active despite having ActivatedByUser flag, getting rid of the flag");
				instance.Flags = instance.Flags.SetActivatedByUser(value: false);
			}
			else
			{
				instance.Flags |= AttachOptionsToNetworkObjectFlags(options);
			}
		}
		else
		{
			if (!instance.gameObject.activeSelf)
			{
			}
			instance.gameObject.SetActive(value: true);
		}
		if (instance.Flags.IsSceneObject())
		{
			Assert.Check(flag || header->SceneGuid != default(Guid));
		}
		else
		{
			Assert.Check(header->SceneGuid == default(Guid));
		}
		_objects.Add(instance.Id, instance);
		instance.Ptr = (int*)header;
		instance.Changed = (int*)Allocator.Alloc(_changedAllocator, header->WordCount * 4);
		if (instance.CallbackBehaviours == null)
		{
			instance.CallbackBehaviours = new FastReferenceList<NetworkBehaviour>();
		}
		int num = NetworkStructUtils.GetWordCount<NetworkObjectHeader>();
		for (int i = 0; i < instance.NetworkedBehaviours.Length; i++)
		{
			instance.NetworkedBehaviours[i].WordOffset = num;
			instance.NetworkedBehaviours[i].WordCount = NetworkBehaviourUtils.GetWordCount(instance.NetworkedBehaviours[i]);
			instance.NetworkedBehaviours[i].Runner = this;
			instance.NetworkedBehaviours[i].Object = instance;
			instance.NetworkedBehaviours[i].ObjectIndex = i;
			instance.NetworkedBehaviours[i].Ptr = instance.Ptr + num;
			num += NetworkBehaviourUtils.GetWordCount(instance.NetworkedBehaviours[i]);
			if (NetworkBehaviourUtils.HasStaticCallbacks(instance.NetworkedBehaviours[i].GetType()))
			{
				instance.CallbackBehaviours.Add(instance.NetworkedBehaviours[i]);
			}
		}
		for (int j = 0; j < instance.SimulationBehaviours.Length; j++)
		{
			if (instance.SimulationBehaviours[j] is NetworkBehaviour)
			{
				throw new Exception("Found NetworkBehaviour reference in SimulationBehaviours[] list on " + instance.Name + ". Re-baking of object required. Please check prefab or scene object and make sure NetworkBehaviour list is up to date.");
			}
			instance.SimulationBehaviours[j].Runner = this;
			instance.SimulationBehaviours[j].Object = instance;
		}
		instance.InSimulation = false;
		_behaviourUpdater.AddObject(this, instance, _simulation.IsInTick);
		if (flag)
		{
			instance.Defaults();
			if (_simulation.Topology == SimulationConfig.Topologies.Shared)
			{
				header->StateAuthority = LocalPlayer;
			}
		}
		if (instance.ObjectInterest == NetworkObject.ObjectInterestModes.AreaOfInterest && _config.Simulation.ObjectInterest)
		{
			if (BehaviourUtils.IsNotAlive(instance.AoiPositionSource))
			{
				instance.AoiPositionSource = instance.transform.GetNestedComponentInChildren<NetworkAreaOfInterestBehaviour, NetworkObject>(includeInactive: false);
			}
			if (BehaviourUtils.IsAlive(instance.AoiPositionSource))
			{
				header->TransformOffset = instance.AoiPositionSource.WordOffset + instance.AoiPositionSource.PositionWordOffset;
			}
			else
			{
				Log.DebugError(this, "Area Of Interest mode set to 'Position' but no valid NetworkAreaOfInterestBehaviour found on " + instance.gameObject.name);
			}
		}
		if (inputAuthority.HasValue)
		{
			instance.AssignInputAuthority(inputAuthority.Value);
		}
		if (interestGroups != null)
		{
			Assert.Check(IsServer);
			Assert.Check(Config.Simulation.Topology == SimulationConfig.Topologies.ClientServer);
			for (int k = 0; k < instance.NetworkedBehaviours.Length; k++)
			{
				NetworkBehaviour networkBehaviour = instance.NetworkedBehaviours[k];
				if (!NetworkBehaviourUtils.TryGetInterestGroupProvider(networkBehaviour.GetType(), out var provider) || provider == null)
				{
					continue;
				}
				int[] array = provider(networkBehaviour.GetType(), networkBehaviour);
				if (array != null)
				{
					for (int l = 0; l < array.Length; l++)
					{
						interestGroups[networkBehaviour.WordOffset + l] = array[l];
					}
				}
			}
			string[] defaultInterestGroups = instance.DefaultInterestGroups;
			if (defaultInterestGroups == null || defaultInterestGroups.Length != 0)
			{
				header->Flags |= NetworkObjectHeaderFlags.HasDefaultInterestGroups;
			}
		}
		if (instance.DestroyWhenStateAuthorityLeaves)
		{
			header->Flags |= NetworkObjectHeaderFlags.DestroyWhenStateAuthorityLeaves;
		}
		if (instance.AllowStateAuthorityOverride)
		{
			header->Flags |= NetworkObjectHeaderFlags.AllowStateAuthorityOverride;
		}
		if (flag && instance.ObjectInterest == NetworkObject.ObjectInterestModes.AllPlayers)
		{
			_simulation.AddToGlobalObjectInterest(instance);
			header->Flags |= NetworkObjectHeaderFlags.GlobalObjectInterest;
		}
	}

	private unsafe void InitializeNetworkObjectState(NetworkObject instance, AttachOptions options)
	{
		Assert.Check(instance.Id.IsValid, "Already despawned", BehaviourUtils.GetDump(instance));
		Assert.Check(instance.Ptr != null, "Already despawned", BehaviourUtils.GetDump(instance));
		if ((options & AttachOptions.LocalSpawn) == AttachOptions.LocalSpawn)
		{
			NetworkBehaviour[] networkedBehaviours = instance.NetworkedBehaviours;
			foreach (NetworkBehaviour networkBehaviour in networkedBehaviours)
			{
				networkBehaviour.CopyBackingFieldsToState(firstTime: true);
			}
		}
		NetworkBehaviour[] networkedBehaviours2 = instance.NetworkedBehaviours;
		foreach (NetworkBehaviour networkBehaviour2 in networkedBehaviours2)
		{
			Assert.Check<string, int, BehaviourUtils.DumpDeferred>(networkBehaviour2.WordCount >= 0, "Invalid word count", networkBehaviour2.WordCount, BehaviourUtils.GetDump(networkBehaviour2));
			if (networkBehaviour2.InvokeOnChangedForInitialNonZeroValues)
			{
				Native.MemClear(instance.Changed + networkBehaviour2.WordOffset, networkBehaviour2.WordCount * 4);
			}
			else
			{
				Native.MemCpy(instance.Changed + networkBehaviour2.WordOffset, networkBehaviour2.Ptr, networkBehaviour2.WordCount * 4);
			}
		}
	}

	private void InvokeBeforeSpawnedCallbacks(NetworkObject instance, AttachOptions options, OnBeforeSpawned onBeforeSpawned)
	{
		if ((options & AttachOptions.LocalSpawn) == AttachOptions.LocalSpawn)
		{
			for (int i = 0; i < instance.SimulationBehaviours.Length; i++)
			{
				if (instance.SimulationBehaviours[i] is ILocalPrefabCreated localPrefabCreated)
				{
					localPrefabCreated.LocalPrefabCreated();
				}
			}
			for (int j = 0; j < instance.NetworkedBehaviours.Length; j++)
			{
				if (instance.NetworkedBehaviours[j] is ILocalPrefabCreated localPrefabCreated2)
				{
					localPrefabCreated2.LocalPrefabCreated();
				}
			}
		}
		else
		{
			for (int k = 0; k < instance.SimulationBehaviours.Length; k++)
			{
				if (instance.SimulationBehaviours[k] is IRemotePrefabCreated remotePrefabCreated)
				{
					remotePrefabCreated.RemotePrefabCreated();
				}
			}
			for (int l = 0; l < instance.NetworkedBehaviours.Length; l++)
			{
				if (instance.NetworkedBehaviours[l] is IRemotePrefabCreated remotePrefabCreated2)
				{
					remotePrefabCreated2.RemotePrefabCreated();
				}
			}
		}
		if (onBeforeSpawned != null)
		{
			try
			{
				onBeforeSpawned(this, instance);
			}
			catch (Exception exn)
			{
				Log.Exception(this, exn);
			}
		}
	}

	private void InvokeSpawnedCallback(NetworkObject instance)
	{
		instance.DebugNotifySpawned();
		Assert.Check((instance.Flags & NetworkObjectFlags.Spawned) == 0, "Already spawned", BehaviourUtils.GetName(this), instance.Id, instance.GetHashCode(), BehaviourUtils.GetName(instance));
		instance.Flags |= NetworkObjectFlags.Spawned;
		for (int i = 0; i < instance.SimulationBehaviours.Length; i++)
		{
			if (instance.SimulationBehaviours[i] is ISpawned spawned)
			{
				instance.SimulationBehaviours[i].DebugNotifySpawned();
				spawned.Spawned();
			}
		}
		for (int j = 0; j < instance.NetworkedBehaviours.Length; j++)
		{
			instance.NetworkedBehaviours[j].DebugNotifySpawned();
			instance.NetworkedBehaviours[j].Spawned();
		}
	}

	internal void InvokeDespawnedCallback(NetworkObject instance, bool hasState)
	{
		instance.DebugNotifyDespawning();
		Assert.Check((instance.Flags & NetworkObjectFlags.Spawned) != 0, "Not spawned", instance.Name);
		instance.Flags &= ~NetworkObjectFlags.Spawned;
		for (int i = 0; i < instance.SimulationBehaviours.Length; i++)
		{
			if (instance.SimulationBehaviours[i] is IDespawned despawned)
			{
				instance.SimulationBehaviours[i].DebugNotifyDespawned();
				despawned.Despawned(this, hasState);
			}
		}
		for (int j = 0; j < instance.NetworkedBehaviours.Length; j++)
		{
			instance.NetworkedBehaviours[j].DebugNotifyDespawned();
			instance.NetworkedBehaviours[j].Despawned(this, hasState);
			instance.NetworkedBehaviours[j].OnChangeClearAll();
		}
	}

	private void InvokeAfterSpawnedCallback(NetworkObject instance)
	{
		for (int i = 0; i < instance.SimulationBehaviours.Length; i++)
		{
			if (instance.SimulationBehaviours[i] is IAfterSpawned afterSpawned)
			{
				afterSpawned.AfterSpawned();
			}
		}
		for (int j = 0; j < instance.NetworkedBehaviours.Length; j++)
		{
			if (instance.NetworkedBehaviours[j] is IAfterSpawned afterSpawned2)
			{
				afterSpawned2.AfterSpawned();
			}
		}
	}

	private void InvokeBeforeUpdate()
	{
		CallbackInterfaceInvoker.IBeforeUpdate(_behaviourUpdater);
	}

	private void InvokeAfterUpdate()
	{
		CallbackInterfaceInvoker.IAfterUpdate(_behaviourUpdater);
	}

	internal void InitFusionLogSystem()
	{
		if (!Log.Initialized)
		{
			Debug.LogWarning("Fusion.Log has not been initialized, using legacy logger");
			Log.Init(Debug.Log, Debug.LogWarning, Debug.LogError, Debug.LogException);
		}
	}

	internal static NetworkProjectConfig SetupNetworkProjectConfig(NetworkRunnerInitializeArgs args)
	{
		int globalSize = 128 + NetworkObjectRefMapPtr.ComputeMemoryNeeded(args.Config.MaxNetworkedObjectCount);
		return args.Config.Init(globalSize, args.PlayerCount, Math.Max(NetworkInputUtils.GetMaxWordCount(), args.InputWordCount.GetValueOrDefault() + 1));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public RpcTargetStatus GetRpcTargetStatus(PlayerRef target)
	{
		return Simulation.GetRpcTargetStatus(target);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasAnyActiveConnections()
	{
		return Simulation.HasAnyActiveConnections();
	}

	internal unsafe bool IsHostPlayer(PlayerRef player)
	{
		SimulationGlobalState* globalState = Simulation.State.GlobalState;
		if (globalState->ServerMode != SimulationModes.Host)
		{
			return false;
		}
		return player == globalState->MaxPlayers - 1;
	}

	internal unsafe bool TryGetHostPlayer(out PlayerRef player)
	{
		SimulationGlobalState* globalState = Simulation.State.GlobalState;
		if (globalState->ServerMode != SimulationModes.Host)
		{
			player = default;
			return false;
		}
		player = globalState->MaxPlayers - 1;
		return true;
	}

	private static NetworkObjectFlags AttachOptionsToNetworkObjectFlags(AttachOptions options)
	{
		NetworkObjectFlags networkObjectFlags = NetworkObjectFlags.None;
		if ((options & AttachOptions.LocalSpawn) == AttachOptions.LocalSpawn)
		{
			networkObjectFlags |= NetworkObjectFlags.AttachOptionLocalSpawn;
		}
		return networkObjectFlags;
	}

	private static AttachOptions NetworkObjectFlagsToAttachOptions(NetworkObjectFlags flags)
	{
		AttachOptions attachOptions = (AttachOptions)0;
		if ((flags & NetworkObjectFlags.AttachOptionLocalSpawn) == NetworkObjectFlags.AttachOptionLocalSpawn)
		{
			attachOptions |= AttachOptions.LocalSpawn;
		}
		return attachOptions;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsAwakeAtInitialization(NetworkObject obj)
	{
		return (obj.Flags & NetworkObjectFlags.ActivatedByUser) == 0;
	}

	private void DebugAwake()
	{
		_debugNameThreadSafe = base.name;
	}

	private void DebugOnDestroy()
	{
		Log.Trace(this, "OnDestroy: " + base.name);
		_debugNameThreadSafe = base.name;
	}

	private void DebugUpdate()
	{
		_debugNameThreadSafe = base.name;
	}

	public int GetHashCodeForLogger()
	{
		return (_debugNameThreadSafe ?? string.Empty).GetHashDeterministic();
	}

	internal static bool TryGetPrettyRunnerName(StringBuilder output, NetworkRunner runner, in LogOptions options)
	{
		if ((object)runner == null || runner.Config?.PeerMode != NetworkProjectConfig.PeerModes.Multiple)
		{
			return false;
		}
		if (options.UseColorTags)
		{
			output.Append("<color=");
			output.AppendFormat("#{0:X6}", options.GetColor(runner));
			output.Append(">");
		}
		PlayerRef playerRef = runner.Simulation?.LocalPlayer ?? default(PlayerRef);
		if (playerRef.IsValid)
		{
			output.Append("[P").Append(playerRef.PlayerId).Append("] ");
		}
		else
		{
			output.Append("[P-] ");
		}
		output.Append(runner._debugNameThreadSafe ?? string.Empty);
		if (BehaviourUtils.IsNotAlive(runner))
		{
			output.Append(" (destroyed)");
		}
		if (options.UseColorTags)
		{
			output.Append("</color>");
		}
		return true;
	}

	void ILogBuilder.BuildLogMessage(StringBuilder builder, string message, in LogOptions options)
	{
		if (TryGetPrettyRunnerName(builder, this, in options))
		{
			builder.Append(": ");
		}
		builder.Append(message);
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
	private static void ResetAllSimulationStatics()
	{
		NetworkBehaviourUtils.ResetStatics();
		RunnerVisibilityNode.ResetStatics();
		NetworkInputUtils.ResetStatics();
		NetworkProjectConfig.ResetStatics();
		ResetStatics();
		NetworkStructUtils.ResetStatics();
	}

	public Task<bool> PushHostMigrationSnapshot()
	{
		if (IsServer && GameMode == GameMode.Host && IsCloudReady)
		{
			return _cloudServices.SendHostMigrationSnapshot();
		}
		return Task.FromResult(result: false);
	}

	public IEnumerable<NetworkObject> GetResumeSnapshotNetworkObjects()
	{
		Assert.Check(IsServer, "Only a Server instance can execute this action");
		Assert.Check(Simulation.IsResume, "Current Simulation does not come from a Resume Server Snapshot");
		var (headerMapping, nestedMapping) = Simulation.StateResume.GetObjectHeaderPtrs();
		foreach (NetworkObjectHeaderPtr header in headerMapping.Values)
		{
			NetworkObject tempNO;
			try
			{
				tempNO = GetNetworkObjectFromResumeSnapshot(header, headerMapping, nestedMapping);
			}
			catch
			{
				continue;
			}
			if (BehaviourUtils.IsAlive(tempNO))
			{
				yield return tempNO;
				UnityEngine.Object.Destroy(tempNO.gameObject);
			}
		}
	}

	public IEnumerable<(NetworkObject, NetworkObjectHeaderPtr)> GetResumeSnapshotNetworkSceneObjects()
	{
		Assert.Check(IsServer, "Only a Server instance can execute this action");
		Assert.Check(Simulation.IsResume, "Current Simulation does not come from a Resume Server Snapshot");
		(Dictionary<NetworkId, NetworkObjectHeaderPtr>, Dictionary<NetworkId, List<NetworkId>>) objectHeaderPtrs = Simulation.StateResume.GetObjectHeaderPtrs();
		var (headerMapping, _) = objectHeaderPtrs;
		_ = objectHeaderPtrs.Item2;
		foreach (NetworkObjectHeaderPtr header in headerMapping.Values)
		{
			NetworkObject tempNO;
			try
			{
				tempNO = GetSceneNetworkObjectFromResumeSnapshot(header);
			}
			catch
			{
				continue;
			}
			if (BehaviourUtils.IsAlive(tempNO))
			{
				yield return (tempNO, header);
			}
		}
	}

	private IEnumerator RunHostMigrationResume(NetworkRunnerInitializeArgs args)
	{
		yield return new WaitUntil(() => _sceneManager.IsReady(this));
		args.HostMigrationResume?.Invoke(this);
		if (_sceneObjectResolver == null)
		{
			_sceneObjectLoopkup.Clear();
		}
		CallbackInterfaceInvoker.IAfterHostMigration(_behaviourUpdater);
		SetInitializationDone(args);
	}

	private unsafe void CleanHostMigrationSnapshots()
	{
		if (_hostSnapshotData0 != null)
		{
			Native.Free(_hostSnapshotData0);
			_hostSnapshotData0 = null;
		}
		if (_hostSnapshotData1 != null)
		{
			Native.Free(_hostSnapshotData1);
			_hostSnapshotData1 = null;
		}
		if (_hostSnapshotDelta != null)
		{
			Native.Free(_hostSnapshotDelta);
			_hostSnapshotDelta = null;
		}
	}

	private uint GetResumeNetworkId()
	{
		return _idCounter;
	}

	private unsafe NetworkObject GetNetworkObjectFromResumeSnapshot(NetworkObjectHeaderPtr networkObjectPtr, Dictionary<NetworkId, NetworkObjectHeaderPtr> headerList, Dictionary<NetworkId, List<NetworkId>> nestedMapping)
	{
		if (networkObjectPtr.Ptr->SceneGuid != default(Guid))
		{
			return null;
		}
		NetworkObject networkObject = CreateInstance(networkObjectPtr.Ptr->Type, networkObjectPtr.Ptr);
		if (networkObject != null)
		{
			networkObject.Flags = networkObject.Flags.SetType(NetworkObjectFlags.TypePrefab);
			InitialzeTempNetworkObjectInstance(networkObjectPtr.Ptr, networkObject);
			if (nestedMapping.TryGetValue(networkObjectPtr.Ptr->Id, out var value))
			{
				for (int i = 0; i < value.Count; i++)
				{
					NetworkId key = value[i];
					NetworkObjectHeaderPtr networkObjectHeaderPtr = headerList[key];
					NetworkObject instance = networkObject.NestedObjects[i];
					Assert.Check(networkObjectHeaderPtr.Ptr->NestingRoot.Equals(networkObjectPtr.Ptr->Id), "Nested NetworkObject with wrong NetworkId for the Nesting Root");
					InitialzeTempNetworkObjectInstance(networkObjectHeaderPtr.Ptr, instance);
				}
			}
		}
		return networkObject;
	}

	private unsafe NetworkObject GetSceneNetworkObjectFromResumeSnapshot(NetworkObjectHeaderPtr networkObjectPtr)
	{
		if (networkObjectPtr.Ptr->SceneGuid == default(Guid))
		{
			return null;
		}
		NetworkObject value;
		if (_sceneObjectResolver != null)
		{
			_sceneObjectResolver.TryResolveSceneObject(this, networkObjectPtr.Ptr->SceneGuid, out value);
		}
		else
		{
			_sceneObjectLoopkup.TryGetValue(networkObjectPtr.Ptr->SceneGuid, out value);
		}
		return value;
	}

	private unsafe void InitialzeTempNetworkObjectInstance(NetworkObjectHeader* header, NetworkObject instance)
	{
		instance.Ptr = (int*)header;
		instance.Id = header->Id;
		int num = NetworkStructUtils.GetWordCount<NetworkObjectHeader>();
		for (int i = 0; i < instance.NetworkedBehaviours.Length; i++)
		{
			instance.NetworkedBehaviours[i].WordOffset = num;
			instance.NetworkedBehaviours[i].WordCount = NetworkBehaviourUtils.GetWordCount(instance.NetworkedBehaviours[i]);
			instance.NetworkedBehaviours[i].Runner = this;
			instance.NetworkedBehaviours[i].Object = instance;
			instance.NetworkedBehaviours[i].ObjectIndex = i;
			instance.NetworkedBehaviours[i].Ptr = instance.Ptr + num;
			num += NetworkBehaviourUtils.GetWordCount(instance.NetworkedBehaviours[i]);
		}
	}

	internal void SetupHostMigration(HostMigration hostMigration)
	{
		_lastHostMigrationInfo = hostMigration;
	}

	internal void StartHostMigration(Snapshot snapshot = null)
	{
		Assert.Always(_lastHostMigrationInfo != null, "Invalid Host Migration info");
		GameMode gameMode = GameMode.Client;
		switch (_lastHostMigrationInfo.PeerMode)
		{
		case PeerMode.Server:
			gameMode = GameMode.Host;
			break;
		case PeerMode.Client:
			gameMode = GameMode.Client;
			break;
		default:
			Assert.Fail("Invalid New Game Mode on Host Migration.");
			break;
		}
		CloudCommunicator cloudCommunicator = _cloudServices.ExtractCommunicator();
		HostMigrationToken migrationToken = new HostMigrationToken(snapshot, cloudCommunicator, gameMode);
		InvokeHostMigration(migrationToken);
	}

	internal unsafe void Service_HostSnapshot()
	{
		if (IsServer && _buildHostSnapshotHandler.HasValue && _buildHostSnapshotHandler.Value.IsCompleted)
		{
			byte[] array = new byte[_buildHostSnapshotJob.Value.ResultLength * 4];
			fixed (byte* destination = array)
			{
				Native.MemCpy(destination, _buildHostSnapshotJob.Value.ResultData, array.Length);
			}
			Log.Debug(this, $"[Host Migration] Host Snapshot computed: Total Size={array.Length} bytes");
			_buildHostSnapshotTask.TrySetResult((true, GetHostSnapshotBufferSize(), _buildHostSnapshotJob.Value.ResultTick, _buildHostSnapshotJob.Value.LastID, array));
			_buildHostSnapshotHandler = null;
			_buildHostSnapshotJob = null;
		}
	}

	internal Task<(bool, int, int, uint, byte[])> GetServerSnapshot()
	{
		if (_buildHostSnapshotJob.HasValue || _buildHostSnapshotHandler.HasValue)
		{
			return Task.FromResult<(bool, int, int, uint, byte[])>((false, -1, -1, 0u, null));
		}
		_buildHostSnapshotJob = BuildCompressHostSnapshotJob();
		_buildHostSnapshotHandler = _buildHostSnapshotJob.Value.Schedule();
		return (_buildHostSnapshotTask = new TaskCompletionSource<(bool, int, int, uint, byte[])>()).Task;
	}

	internal unsafe int GetHostSnapshotBufferSize()
	{
		return Simulation.State.Allocator->ReplicateByteLength;
	}

	internal unsafe HostSnapshotCompressionJob BuildCompressHostSnapshotJob()
	{
		Assert.Check(IsServer);
		if (_hostSnapshotData0 == null)
		{
			Assert.Check(_hostSnapshotData1 == null);
			Assert.Check(_hostSnapshotDelta == null);
			_hostSnapshotData0 = (byte*)Native.MallocAndClear(Simulation.State.Allocator->ReplicateByteLength);
			_hostSnapshotData1 = (byte*)Native.MallocAndClear(Simulation.State.Allocator->ReplicateByteLength);
			_hostSnapshotDelta = (byte*)Native.MallocAndClear(Simulation.State.Allocator->ReplicateByteLength * 2 + 4 + 4);
		}
		else
		{
			Assert.Check(_hostSnapshotData1);
			Assert.Check(_hostSnapshotDelta);
			byte* hostSnapshotData = _hostSnapshotData1;
			_hostSnapshotData1 = _hostSnapshotData0;
			_hostSnapshotData0 = hostSnapshotData;
		}
		Native.MemCpy(_hostSnapshotData0, Simulation.State.Allocator->Replicate, Simulation.State.Allocator->ReplicateByteLength);
		HostSnapshotCompressionJob result = default;
		result.Length = Simulation.State.Allocator->ReplicateByteLength / 4;
		result.CurrentTick = Simulation.State.Tick;
		result.LastID = GetResumeNetworkId();
		result.Current = (int*)_hostSnapshotData0;
		result.Previous = (int*)_hostSnapshotData1;
		result.Result = (int*)_hostSnapshotDelta;
		return result;
	}

	internal void InvokeHostMigration(HostMigrationToken migrationToken)
	{
		try
		{
			for (int i = 0; i < _callbacks.Count; i++)
			{
				_callbacks[i].OnHostMigration(this, migrationToken);
			}
		}
		catch (Exception exn)
		{
			Log.Exception(exn);
		}
	}

	private void AddInactiveObjectGuard(NetworkObject obj)
	{
		NetworkObjectInactivityGuard networkObjectInactivityGuard;
		if (_inactivityGuardPool.Count > 0)
		{
			networkObjectInactivityGuard = _inactivityGuardPool.Pop();
			Assert.Check(networkObjectInactivityGuard);
		}
		else
		{
			GameObject gameObject = new GameObject("NetworkObjectInactivityGuard");
			networkObjectInactivityGuard = gameObject.AddComponent<NetworkObjectInactivityGuard>();
			gameObject.hideFlags = (Config.HideNetworkObjectInactivityGuard ? HideFlags.HideAndDontSave : (HideFlags.DontSave | HideFlags.NotEditable));
		}
		Assert.Check(networkObjectInactivityGuard.Object == null);
		networkObjectInactivityGuard.Object = obj;
		networkObjectInactivityGuard.transform.SetParent(obj.transform);
	}

	public static List<NetworkRunner>.Enumerator GetInstancesEnumerator()
	{
		return _instances.GetEnumerator();
	}

	private static bool AddInstance(NetworkRunner runner)
	{
		if (!_instances.Contains(runner))
		{
			_instances.Add(runner);
			return true;
		}
		return false;
	}

	private static bool RemoveInstance(NetworkRunner runner)
	{
		return _instances.Remove(runner);
	}

	internal static void InvokeUpdate(float delta)
	{
		if (_instancesSnapshot.Length < _instances.Count)
		{
			Array.Resize(ref _instancesSnapshot, _instances.Capacity);
		}
		_instances.CopyTo(_instancesSnapshot);
		_instancesSnapshotCount = _instances.Count;
		int i = 0;
		for (int instancesSnapshotCount = _instancesSnapshotCount; i < instancesSnapshotCount; i++)
		{
			NetworkRunner networkRunner = _instancesSnapshot[i];
			if (BehaviourUtils.IsAlive(networkRunner))
			{
				networkRunner.UpdateInternal(delta);
			}
		}
	}

	internal static void InvokeRender()
	{
		try
		{
			int i = 0;
			for (int instancesSnapshotCount = _instancesSnapshotCount; i < instancesSnapshotCount; i++)
			{
				NetworkRunner networkRunner = _instancesSnapshot[i];
				if (BehaviourUtils.IsAlive(networkRunner))
				{
					networkRunner.RenderInternal();
				}
			}
		}
		finally
		{
			Array.Clear(_instancesSnapshot, 0, _instancesSnapshotCount);
		}
	}

	public async Task<StartGameResult> JoinSessionLobby(SessionLobby sessionLobby, string lobbyID = null, AuthenticationValues authentication = null, AppSettings customAppSettings = null, bool? useDefaultCloudPorts = false, bool useCachedRegions = false)
	{
		Log.Debug(this, $"Joining Lobby {sessionLobby} {lobbyID}");
		try
		{
			await ConnectToCloud(authentication, customAppSettings, null, useDefaultCloudPorts, useCachedRegions);
			if (!IsCloudReady)
			{
				throw new StartGameException(ShutdownReason.Error, "Unable to connect to Photon Cloud");
			}
			if (_cloudServices.IsInRoom)
			{
				throw new StartGameException(ShutdownReason.Error, "Unable to join the Lobby " + lobbyID + ", already connected to a Game Session");
			}
			short result = await _cloudServices.JoinSessionLobby(sessionLobby, lobbyID);
			if (result != 0)
			{
				throw new StartGameException(ErrorCodeExt.ConvertToShutdownReason(result));
			}
			_simulationShutdown = (ShutdownFlags)0;
		}
		catch (Exception e)
		{
			return await ShutdownAndBuildResult(e);
		}
		return new StartGameResult();
	}

	public Task<StartGameResult> StartGame(StartGameArgs args)
	{
		if (_cloudServices?.IsInLobby != true && (IsStarting || IsRunning))
		{
			return Task.FromResult(new StartGameResult(ShutdownReason.AlreadyRunning));
		}
		InitFusionLogSystem();
		args.GameMode = ((args.HostMigrationToken != null) ? args.HostMigrationToken.GameMode : args.GameMode);
		Log.Debug(this, $"Starting in Game Mode {args.GameMode}");
		args.DisableNATPunchthrough |= RuntimeUnityFlagsSetup.IsUNITY_WEBGL;
		GameMode = args.GameMode;
		_simulationShutdown = (ShutdownFlags)0;
		args.Config = (args.Config ?? NetworkProjectConfig.Global).Copy();
		Log.Debug(args);
		switch (args.GameMode)
		{
		case GameMode.Single:
			return StartGameModeSinglePlayer(args);
		case GameMode.Shared:
		case GameMode.Server:
		case GameMode.Host:
		case GameMode.Client:
		case GameMode.AutoHostOrClient:
			return StartGameModeCloud(args);
		default:
			GameMode = (GameMode)0;
			return Task.FromResult(new StartGameResult(ShutdownReason.IncompatibleConfiguration));
		}
	}

	internal async Task ConnectToCloud(AuthenticationValues authentication = null, AppSettings customAppSettings = null, CloudCommunicator externalCommunicator = null, bool? useDefaultCloudPorts = false, bool useCachedRegions = false)
	{
		AppSettings appSettings = customAppSettings ?? PhotonAppSettings.Instance.AppSettings;
		if (appSettings == null)
		{
			throw new InvalidOperationException("Photon Application Settings not found.");
		}
		if (useCachedRegions && !string.IsNullOrEmpty(_cachedRegionSummary))
		{
			appSettings.BestRegionSummaryFromStorage = _cachedRegionSummary;
		}
		if (_cloudServices == null)
		{
			Log.Debug(this, "Connecting to Photon Cloud.");
			_cloudServices = new CloudServices(this, externalCommunicator);
			SessionInfo = new SessionInfo(this);
			if (!_cloudServices.IsCloudReady)
			{
				await _cloudServices.ConnectToCloud(authentication, appSettings, useDefaultCloudPorts);
			}
		}
	}

	internal async Task DisconnectFromCloud()
	{
		if (_cloudServices != null)
		{
			Log.Debug(this, "Disconnecting from Photon Cloud.");
			await _cloudServices.DisconnectFromCloud();
			_cloudServices.Dispose();
			_cloudServices = null;
		}
	}

	private async Task<StartGameResult> StartGameModeSinglePlayer(StartGameArgs args)
	{
		NetworkRunnerInitializeArgs runnerArgs = new NetworkRunnerInitializeArgs
		{
			Scene = args.Scene,
			SimulationMode = SimulationModes.Host,
			Address = null,
			PlayerCount = 1,
			Config = args.Config,
			Initialized = args.Initialized,
			ObjectPool = args.ObjectPool,
			SceneManager = args.SceneManager,
			CustomCallbackInterfaces = args.CustomCallbackInterfaces
		};
		await Initialize(runnerArgs);
		return new StartGameResult();
	}

	private async Task<StartGameResult> StartGameModeCloud(StartGameArgs args)
	{
		try
		{
			SimulationModes? simulationMode = null;
			args.Config.Simulation.Topology = SimulationConfig.Topologies.ClientServer;
			switch (args.GameMode)
			{
			case GameMode.Server:
				simulationMode = SimulationModes.Server;
				break;
			case GameMode.Host:
				simulationMode = SimulationModes.Host;
				break;
			case GameMode.Client:
				simulationMode = SimulationModes.Client;
				break;
			case GameMode.Shared:
				simulationMode = SimulationModes.Client;
				args.Config.Simulation.Topology = SimulationConfig.Topologies.Shared;
				args.Config.Simulation.ReplicationMode = SimulationConfig.StateReplicationModes.EventualConsistency;
				break;
			default:
				throw new StartGameException(ShutdownReason.InvalidArguments, string.Format("{0} set to {1}, which is invalid in this context", "GameMode", GameMode));
			case GameMode.AutoHostOrClient:
				break;
			}
			if (args.SessionProperties != null)
			{
				if (args.SessionProperties.Count > 10)
				{
					throw new StartGameException(ShutdownReason.InvalidArguments, "Max number of Custom Session Properties reached, only 10 properties are allowed.");
				}
				int customPropertiesSize = RealtimeExtensions_DictionaryProperties.CalculateTotalSize(args.SessionProperties);
				if (customPropertiesSize > 500)
				{
					throw new StartGameException(ShutdownReason.InvalidArguments, $"Max size of Custom Session Properties reached, current size of {customPropertiesSize} bytes, max 500 bytes are allowed.");
				}
			}
			_cloudOperation = new TaskCompletionSource<(ShutdownReason, string)>();
			await ConnectToCloud(args.AuthValues, args.CustomPhotonAppSettings, args.HostMigrationToken?.CloudCommunicator, args.UseDefaultPhotonCloudPorts, args.UseCachedRegions);
			if (!IsCloudReady)
			{
				throw new StartGameException(ShutdownReason.Error, "Unable to connect to Photon Cloud");
			}
			short resultCode = await _cloudServices.EnterRoom(args);
			if (resultCode != 0)
			{
				throw new StartGameException(ErrorCodeExt.ConvertToShutdownReason(resultCode));
			}
			_cloudServices.OnRoomChanged();
			if (GameMode == GameMode.AutoHostOrClient && !simulationMode.HasValue)
			{
				if (!_cloudServices.IsMasterClient)
				{
					SimulationModes? simulationModes = SimulationModes.Client;
					simulationMode = simulationModes;
					GameMode = GameMode.Client;
				}
				else
				{
					SimulationModes? simulationModes = SimulationModes.Host;
					simulationMode = simulationModes;
					GameMode = GameMode.Host;
				}
			}
			if (!simulationMode.HasValue)
			{
				throw new StartGameException(ShutdownReason.Error, "Invalid SimulationMode");
			}
			_cloudServices.UpdateInitializeArgs(new NetworkRunnerInitializeArgs
			{
				SimulationMode = simulationMode,
				Scene = args.Scene,
				Address = args.Address.GetValueOrDefault(NetAddress.Any(0)),
				PublicAddress = args.CustomPublicAddress,
				Config = args.Config,
				PlayerCount = args.PlayerCount,
				Initialized = args.Initialized,
				ObjectPool = args.ObjectPool,
				SceneManager = args.SceneManager,
				CustomCallbackInterfaces = args.CustomCallbackInterfaces,
				ConnectionToken = args.ConnectionToken,
				ResumeState = args.HostMigrationToken?.ResumeState,
				ResumeTick = args.HostMigrationToken?.ResumeTick,
				ResumeId = args.HostMigrationToken?.ResumeId,
				HostMigrationResume = args.HostMigrationResume
			});
			_cloudServices.IsNATPunchthroughEnabled = !args.DisableNATPunchthrough;
			_cloudServices.CustomSTUNServer = args.CustomSTUNServer;
			_cloudServices.SendJoinMessage();
			var (shutdownCode, errorMessage) = await _cloudOperation.Task;
			if (shutdownCode != ShutdownReason.Ok)
			{
				throw new StartGameException(shutdownCode, errorMessage);
			}
			while (IsStarting)
			{
				await TaskManager.Delay(10);
			}
			if (IsShutdown)
			{
				throw new StartGameException(ShutdownReason.Error, "Error while starting up NetworkRunner. State set to Shutdown, should Running.");
			}
			if (args.SessionProperties != null)
			{
				SessionInfo.UpdateCustomProperties(args.SessionProperties);
			}
		}
		catch (Exception e)
		{
			return await ShutdownAndBuildResult(e);
		}
		finally
		{
			_cloudOperation = null;
		}
		return new StartGameResult();
	}

	private async Task<StartGameResult> ShutdownAndBuildResult(Exception e)
	{
		StartGameResult result = StartGameResult.BuildStartGameResultFromException(e);
		Log.DebugWarn(this, $"StartGame Failed: {result}");
		await Shutdown(destroyGameObject: true, result.ShutdownReason);
		return result;
	}

	internal void InvokeSessionListUpdated(List<SessionInfo> sessionList)
	{
		try
		{
			for (int i = 0; i < _callbacks.Count; i++)
			{
				_callbacks[i].OnSessionListUpdated(this, sessionList);
			}
		}
		catch (Exception exn)
		{
			Log.Exception(this, exn);
		}
	}

	internal void InvokeCustomAuthenticationResponse(Dictionary<string, object> data)
	{
		try
		{
			for (int i = 0; i < _callbacks.Count; i++)
			{
				_callbacks[i].OnCustomAuthenticationResponse(this, data);
			}
		}
		catch (Exception exn)
		{
			Log.Exception(this, exn);
		}
	}

	public void SetActiveScene(SceneRef scene)
	{
		_simulation?.SetActiveScene(scene);
	}

	public void InvokeSceneLoadStart()
	{
		try
		{
			for (int i = 0; i < _callbacks.Count; i++)
			{
				_callbacks[i].OnSceneLoadStart(this);
			}
		}
		catch (Exception exn)
		{
			Log.Exception(this, exn);
		}
		CallbackInterfaceInvoker.ISceneLoadStart(_behaviourUpdater);
	}

	public void InvokeSceneLoadDone()
	{
		try
		{
			for (int i = 0; i < _callbacks.Count; i++)
			{
				_callbacks[i].OnSceneLoadDone(this);
			}
		}
		catch (Exception exn)
		{
			Log.Exception(this, exn);
		}
		CallbackInterfaceInvoker.ISceneLoadDone(_behaviourUpdater);
	}

	public static NetworkRunner GetRunnerForGameObject(GameObject gameObject)
	{
		return GetRunnerForScene(gameObject.scene);
	}

	public static NetworkRunner GetRunnerForScene(Scene scene)
	{
		return GetRunnerForScene(scene.handle);
	}

	public bool TryMultiplePeerAssignTempScene()
	{
		if (_isMultiplePeerUnitySceneTemp)
		{
			return false;
		}
		MultiplePeerUnityScene = UnityEngine.SceneManagement.SceneManager.CreateScene(base.gameObject.name + " Temp Scene", new CreateSceneParameters(NetworkProjectConfig.ConvertPhysicsMode(_config.PhysicsEngine)));
		_isMultiplePeerUnitySceneTemp = true;
		return true;
	}

	public PhysicsScene GetPhysicsScene()
	{
		if (IsRunning && Config.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			if (Config.PhysicsEngine == NetworkProjectConfig.PhysicsEngines.Physics3D)
			{
				return MultiplePeerUnityScene.GetPhysicsScene();
			}
			return default;
		}
		return Physics.defaultPhysicsScene;
	}

	public PhysicsScene2D GetPhysicsScene2D()
	{
		if (IsRunning && Config.PeerMode == NetworkProjectConfig.PeerModes.Multiple)
		{
			if (Config.PhysicsEngine == NetworkProjectConfig.PhysicsEngines.Physics2D)
			{
				return MultiplePeerUnityScene.GetPhysicsScene2D();
			}
			return default;
		}
		return Physics2D.defaultPhysicsScene;
	}

	public GameObject InstantiateInRunnerScene(GameObject original, Vector3 position, Quaternion rotation)
	{
		bool flag = EnsureRunnerSceneIsActive(out var previousActiveScene);
		GameObject gameObject = UnityEngine.Object.Instantiate(original, position, rotation);
		MoveToRunnerScene(gameObject);
		if (flag)
		{
			UnityEngine.SceneManagement.SceneManager.SetActiveScene(previousActiveScene);
		}
		return gameObject;
	}

	public GameObject InstantiateInRunnerScene(GameObject original)
	{
		bool flag = EnsureRunnerSceneIsActive(out var previousActiveScene);
		GameObject gameObject = UnityEngine.Object.Instantiate(original);
		MoveToRunnerScene(gameObject);
		if (flag)
		{
			UnityEngine.SceneManagement.SceneManager.SetActiveScene(previousActiveScene);
		}
		return gameObject;
	}

	public T InstantiateInRunnerScene<T>(T original) where T : Component
	{
		bool flag = EnsureRunnerSceneIsActive(out var previousActiveScene);
		T val = UnityEngine.Object.Instantiate(original);
		MoveToRunnerScene(val);
		if (flag)
		{
			UnityEngine.SceneManagement.SceneManager.SetActiveScene(previousActiveScene);
		}
		return val;
	}

	public T InstantiateInRunnerScene<T>(T original, Vector3 position, Quaternion rotation) where T : Component
	{
		bool flag = EnsureRunnerSceneIsActive(out var previousActiveScene);
		T val = UnityEngine.Object.Instantiate(original, position, rotation);
		MoveToRunnerScene(val);
		if (flag)
		{
			UnityEngine.SceneManagement.SceneManager.SetActiveScene(previousActiveScene);
		}
		return val;
	}

	public void MoveToRunnerScene<T>(T component) where T : Component
	{
		MoveToRunnerScene(component.gameObject);
	}

	public void MoveToRunnerScene(GameObject go)
	{
		if (Config.PeerMode != NetworkProjectConfig.PeerModes.Single)
		{
			NetworkObject[] componentsInChildren = go.GetComponentsInChildren<NetworkObject>();
			foreach (NetworkObject networkObject in componentsInChildren)
			{
				RunnerVisibilityNode.AddVisibilityNodes(networkObject.gameObject, this);
			}
			RunnerVisibilityNode.AddVisibilityNodes(go, this);
			if (go.scene != MultiplePeerUnityScene)
			{
				UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, MultiplePeerUnityScene);
			}
		}
	}

	public bool EnsureRunnerSceneIsActive(out Scene previousActiveScene)
	{
		if (Config.PeerMode == NetworkProjectConfig.PeerModes.Single)
		{
			previousActiveScene = default;
			return false;
		}
		Scene activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
		if (activeScene == MultiplePeerUnityScene)
		{
			previousActiveScene = default;
			return false;
		}
		previousActiveScene = activeScene;
		UnityEngine.SceneManagement.SceneManager.SetActiveScene(MultiplePeerUnityScene);
		return true;
	}

	internal static NetworkRunner GetRunnerForScene(int sceneHandle)
	{
		if (NetworkProjectConfig.Global.PeerMode == NetworkProjectConfig.PeerModes.Single)
		{
			if (_instances.Count > 1)
			{
				Log.Warn("Found several NetworkRunner instances when instance mode is set to Single");
			}
			return (_instances.Count == 0) ? null : _instances[0];
		}
		if (_instancesByMultiPeerScene.TryGetValue(sceneHandle, out var value))
		{
			return value;
		}
		return null;
	}

	string[] Simulation.ICallbacks.GetDefaultInterestGroups(NetworkId id)
	{
		if (_objects.TryGet(id, out var value))
		{
			return value.DefaultInterestGroups;
		}
		Assert.AlwaysFail($"Interest group not found: {id}");
		return null;
	}

	void Simulation.ICallbacks.ObjectReceivedUpdate(NetworkId id, int tick)
	{
		if (_objects.TryGet(id, out var value))
		{
			value.LastReceiveTick = tick;
		}
	}

	void Simulation.ICallbacks.ObjectStateAuthorityChanged(NetworkId id)
	{
		if (!_objects.TryGet(id, out var value))
		{
			return;
		}
		for (int i = 0; i < value.SimulationBehaviours.Length; i++)
		{
			try
			{
				if (value.SimulationBehaviours[i] is IStateAuthorityChanged stateAuthorityChanged)
				{
					stateAuthorityChanged.StateAuthorityChanged();
				}
			}
			catch (Exception exn)
			{
				Log.Exception(this, exn);
			}
		}
		for (int j = 0; j < value.NetworkedBehaviours.Length; j++)
		{
			try
			{
				if (value.NetworkedBehaviours[j] is IStateAuthorityChanged stateAuthorityChanged2)
				{
					stateAuthorityChanged2.StateAuthorityChanged();
				}
			}
			catch (Exception exn2)
			{
				Log.Exception(this, exn2);
			}
		}
	}

	private void ObjectLeaveSimulation(NetworkId id)
	{
		if (!_objects.TryGet(id, out var value) || !value.InSimulation)
		{
			return;
		}
		value.InSimulation = false;
		for (int i = 0; i < value.NetworkedBehaviours.Length; i++)
		{
			if (value.NetworkedBehaviours[i] is ISimulationExit simulationExit)
			{
				simulationExit.SimulationExit();
			}
		}
		for (int j = 0; j < value.SimulationBehaviours.Length; j++)
		{
			if (value.SimulationBehaviours[j] is ISimulationExit simulationExit2)
			{
				simulationExit2.SimulationExit();
			}
		}
	}

	private void ObjectJoinSimulation(NetworkId id)
	{
		if (!_objects.TryGet(id, out var value) || value.InSimulation)
		{
			return;
		}
		value.InSimulation = true;
		for (int i = 0; i < value.NetworkedBehaviours.Length; i++)
		{
			if (value.NetworkedBehaviours[i] is ISimulationEnter simulationEnter)
			{
				simulationEnter.SimulationEnter();
			}
		}
		for (int j = 0; j < value.SimulationBehaviours.Length; j++)
		{
			if (value.SimulationBehaviours[j] is ISimulationEnter simulationEnter2)
			{
				simulationEnter2.SimulationEnter();
			}
		}
	}

	unsafe void Simulation.ICallbacks.OnAfterSimulation()
	{
		_objects.GetIterateBufferStartCount(out var entries, out var start, out var count);
		_tempWords = null;
		_tempWordsCapacity = 0;
		try
		{
			for (int i = start; i < count; i++)
			{
				NetworkObject value = entries[i].Value;
				if (!BehaviourUtils.IsNotNull(value) || value.CallbackBehaviours.Count <= 0)
				{
					continue;
				}
				bool flag;
				do
				{
					NetworkBehaviour[] items = value.CallbackBehaviours.Items;
					int count2 = value.CallbackBehaviours.Count;
					int localAuthorityMask = value.GetLocalAuthorityMask();
					flag = false;
					for (int j = 0; j < count2; j++)
					{
						NetworkBehaviour networkBehaviour = items[j];
						NetworkBehaviourCallbacks dynamicCallbacks = networkBehaviour.DynamicCallbacks;
						NetworkBehaviourCallbacks staticCallbacks = NetworkBehaviourUtils.GetStaticCallbacks(networkBehaviour.GetType());
						int* ptr = value.Changed + networkBehaviour.WordOffset;
						int* ptr2 = networkBehaviour.Ptr;
						int wordCount = networkBehaviour.WordCount;
						ulong num = 0uL;
						ulong num2 = 0uL;
						bool flag2 = false;
						for (int k = 0; k < wordCount; k++)
						{
							if (ptr2[k] == ptr[k])
							{
								continue;
							}
							if (!flag2)
							{
								if (wordCount > _tempWordsCapacity)
								{
									if (_tempWords != null)
									{
										Simulation.TempFree(_tempWords);
										_tempWords = null;
									}
									_tempWordsCapacity = wordCount;
									_tempWords = (int*)Simulation.TempAllocNoClear(wordCount * 4);
								}
								Native.MemCpy(_tempWords, ptr2, wordCount * 4);
							}
							flag2 = true;
							flag |= staticCallbacks?.Invoke(k, networkBehaviour, localAuthorityMask, ptr, &num) == true;
							flag |= dynamicCallbacks?.Invoke(k, networkBehaviour, localAuthorityMask, ptr, &num2) == true;
							if (BehaviourUtils.IsNull(networkBehaviour.Object))
							{
								return;
							}
						}
						if (flag2)
						{
							Native.MemCpy(ptr, _tempWords, wordCount * 4);
						}
					}
				}
				while (flag);
			}
		}
		finally
		{
			if (_tempWords != null)
			{
				Simulation.TempFree(_tempWords);
			}
			_tempWords = null;
			_tempWordsCapacity = 0;
		}
	}

	void Simulation.ICallbacks.OnBeforeSimulation()
	{
	}

	unsafe bool Simulation.ICallbacks.TryBeginUpdateRemotePrefabs()
	{
		INetworkSceneManager sceneManager = _sceneManager;
		if (sceneManager != null && !sceneManager.IsReady(this))
		{
			return false;
		}
		if (IsClient && _simulation.LatestServerState.GlobalState->Scene != _simulation.State.GlobalState->Scene)
		{
			return false;
		}
		Assert.Check(_remotePrefabsWaitingForSpawnedCallback.Count == 0);
		CallbackInterfaceInvoker.IBeforeUpdateRemotePrefabs(_behaviourUpdater);
		return true;
	}

	void Simulation.ICallbacks.EndUpdateRemotePrefabs()
	{
		try
		{
			for (int i = 0; i < _remotePrefabsWaitingForSpawnedCallback.Count; i++)
			{
				NetworkObject networkObject = _remotePrefabsWaitingForSpawnedCallback[i];
				Assert.Check(BehaviourUtils.IsAlive(networkObject), "Remote prefab destroyed before having a chance to invoke Spawned");
				if (!networkObject.Id.IsValid)
				{
					Log.Warn(networkObject, "This object has been spawned and despawned in the same tick");
					_remotePrefabsWaitingForSpawnedCallback[i] = null;
				}
				else if (IsAwakeAtInitialization(networkObject))
				{
					InitializeNetworkObjectState(networkObject, (AttachOptions)0);
				}
			}
			foreach (NetworkObject item in _remotePrefabsWaitingForSpawnedCallback)
			{
				if (!BehaviourUtils.IsNull(item) && IsAwakeAtInitialization(item))
				{
					InvokeBeforeSpawnedCallbacks(item, (AttachOptions)0, null);
				}
			}
			bool flag = false;
			foreach (NetworkObject item2 in _remotePrefabsWaitingForSpawnedCallback)
			{
				if (BehaviourUtils.IsNull(item2))
				{
					continue;
				}
				Assert.Check(BehaviourUtils.IsAlive(item2), "Remote prefab destroyed before having a chance to invoke Spawned");
				if (IsAwakeAtInitialization(item2))
				{
					InvokeSpawnedCallback(item2);
					if ((item2.Flags & NetworkObjectFlags.PredictedSpawn) == NetworkObjectFlags.PredictedSpawn)
					{
						flag = true;
					}
				}
			}
			foreach (NetworkObject item3 in _remotePrefabsWaitingForSpawnedCallback)
			{
				if (!BehaviourUtils.IsNull(item3) && IsAwakeAtInitialization(item3))
				{
					InvokeAfterSpawnedCallback(item3);
				}
			}
			if (flag)
			{
				foreach (NetworkObject item4 in _remotePrefabsWaitingForSpawnedCallback)
				{
					if (!BehaviourUtils.IsNull(item4) && IsAwakeAtInitialization(item4) && (item4.Flags & NetworkObjectFlags.PredictedSpawn) == NetworkObjectFlags.PredictedSpawn)
					{
						InvokeMethodOnPredictedSpawnedObject(item4, (NetworkObject o, IPredictedSpawnBehaviour b) =>
						{
							b.PredictedSpawnSuccess();
						});
					}
				}
			}
		}
		finally
		{
			_remotePrefabsWaitingForSpawnedCallback.Clear();
		}
		CallbackInterfaceInvoker.IAfterUpdateRemotePrefabs(_behaviourUpdater);
	}

	unsafe bool Simulation.ICallbacks.CreateRemotePrefab(NetworkObjectHeader* header)
	{
		if (_objects.TryGet(header->Id, out var value))
		{
			return true;
		}
		if ((header->Flags & NetworkObjectHeaderFlags.NoPrefab) == NetworkObjectHeaderFlags.NoPrefab)
		{
			return true;
		}
		bool flag = false;
		if (header->SceneGuid != Guid.Empty)
		{
			if (_sceneObjectResolver != null)
			{
				if (!_sceneObjectResolver.TryResolveSceneObject(this, header->SceneGuid, out value))
				{
					return false;
				}
			}
			else if (!_sceneObjectLoopkup.TryGetValue(header->SceneGuid, out value))
			{
				return false;
			}
			if (value.Id.IsValid)
			{
				Log.Warn(value, $"Scene object is already attached to, won't allow this be attached: {*header}");
				return false;
			}
			Assert.Always(BehaviourUtils.IsNotNull(value), "Scene object was was resolved, but the instance is null", header->SceneGuid, header->Id);
			Assert.Check(!header->PredictionKey);
			Assert.Check(!value.PredictedSpawn.Key);
		}
		else
		{
			if (!TryResolvePrefabInstance(header, out value))
			{
				return false;
			}
			Assert.Always(BehaviourUtils.IsNotNull(value), "TryResolvePrefabInstance returned true, but the instance is null", header->Type, header->Id);
			Assert.Always(!value.Id.IsValid, "Expected instance id not valid", value.Id);
			flag = (bool)header->PredictionKey && header->PredictionKey == value.PredictedSpawn.Key;
		}
		Assert.Check(BehaviourUtils.IsAlive(value), "The instance has been destroyed", header->SceneGuid, header->Type, header->Id);
		if (flag)
		{
			value.PredictedSpawn = default;
		}
		InitializeNetworkObjectAssignRunner(value);
		InitializeNetworkObjectInstance(header, value, null, (AttachOptions)0, null);
		if (flag)
		{
			value.Flags |= NetworkObjectFlags.PredictedSpawn;
		}
		_remotePrefabsWaitingForSpawnedCallback.Add(value);
		return BehaviourUtils.IsAlive(value);
	}

	bool Simulation.ICallbacks.DestroyRemotePrefab(NetworkId id, bool exists)
	{
		if (_objects.TryGet(id, out var value))
		{
			if (exists)
			{
				Assert.Check(BehaviourUtils.IsAlive(value));
				Assert.Check(value.Id == id, "Object seem to have been attached to a different id already", BehaviourUtils.GetName(value), value.Id, id);
				Destroy(value, NetworkObjectDestroyFlags.DestroyedByReplicator);
			}
			else if (BehaviourUtils.IsAlive(value))
			{
				Assert.Check(value.Id == id, "Object seem to have been attached to a different id already", BehaviourUtils.GetName(value), value.Id, id);
				DestroyOrphaned(value, destroyedByEngine: false);
			}
			else
			{
				_objects.Remove(id);
			}
			return true;
		}
		return false;
	}

	private void ProcessSpawnQueue()
	{
		if (!_sceneManager.IsReady(this) || (Topology == SimulationConfig.Topologies.Shared && !LocalPlayer.IsValid))
		{
			return;
		}
		while (true)
		{
			Queue<SpawnQueueEntry> spawnQueue = _spawnQueue;
			if (spawnQueue != null && spawnQueue.Count > 0)
			{
				SpawnQueueEntry spawnQueueEntry = _spawnQueue.Dequeue();
				SpawnInternal(spawnQueueEntry.PrefabId, spawnQueueEntry.Position, spawnQueueEntry.Rotation, spawnQueueEntry.InputAuthority, spawnQueueEntry.OnBeforeSpawned, spawnQueueEntry.PredictionKey, spawnQueueEntry.SyncPhysics);
				continue;
			}
			break;
		}
	}

	void Simulation.ICallbacks.OnBeforeCopyPreviousState()
	{
		CallbackInterfaceInvoker.IBeforeCopyPreviousState(_behaviourUpdater);
	}

	void Simulation.ICallbacks.OnTick()
	{
		float fixedDeltaTime = Time.fixedDeltaTime;
		try
		{
			Time.fixedDeltaTime = _simulation.DeltaTime;
			_behaviourUpdater.InvokeFixedUpdateNetwork(_simulation.Stage, _simulation.Mode);
			if (!IsClient || _simulation.Stage != SimulationStages.Forward || _simulation.LatestServerState == null)
			{
				return;
			}
			foreach (NetworkObject predictionSpawn in _predictionSpawns)
			{
				if (predictionSpawn.PredictedSpawn.Tick > _simulation.LatestServerState.Tick && predictionSpawn.PredictedSpawn.Tick < _simulation.Tick)
				{
					InvokeMethodOnPredictedSpawnedObject(predictionSpawn, (NetworkObject o, IPredictedSpawnBehaviour b) =>
					{
						b.PredictedSpawnUpdate();
					});
				}
			}
		}
		finally
		{
			Time.fixedDeltaTime = fixedDeltaTime;
		}
	}

	unsafe void Simulation.ICallbacks.OnServerStart()
	{
		Simulation.State.GlobalState->ServerMode = _simulation.Mode;
		Simulation.State.GlobalState->MaxPlayers = _simulation.Config.DefaultPlayers;
	}

	unsafe void Simulation.ICallbacks.OnInputMissing(SimulationInput input)
	{
		for (int i = 0; i < _callbacks.Count; i++)
		{
			_callbacks[i].OnInputMissing(this, input.Player, new NetworkInput(input.Data, Simulation.Config.InputDataWordCount));
		}
	}

	unsafe void Simulation.ICallbacks.OnInput(SimulationInput input)
	{
		if (ProvideInput)
		{
			for (int i = 0; i < _callbacks.Count; i++)
			{
				_callbacks[i].OnInput(this, new NetworkInput(input.Data, Simulation.Config.InputDataWordCount));
			}
		}
	}

	private unsafe void OnMessageUser(SimulationMessage* message)
	{
		SimulationMessagePtr message2 = default;
		message2.Message = message;
		try
		{
			for (int i = 0; i < _callbacks.Count; i++)
			{
				_callbacks[i].OnUserSimulationMessage(this, message2);
			}
		}
		catch (Exception exn)
		{
			Log.Exception(this, exn);
		}
	}

	unsafe void Simulation.ICallbacks.OnMessage(SimulationMessage* message)
	{
		try
		{
			if (message->GetFlag(1))
			{
				OnMessageUser(message);
				return;
			}
			if (message->GetFlag(256))
			{
				Log.DebugWarn(message, "Dummy message received; likely the sender tried to send a message that was too large to be serialized.");
				return;
			}
			byte* data = SimulationMessage.GetData(message);
			RpcHeader rpcHeader = RpcHeader.Read(data, out var _);
			bool flag = message->IsTargeted();
			PlayerRef playerRef = PlayerRef.None;
			bool flag2 = false;
			if (flag)
			{
				playerRef = message->Target;
				flag2 = playerRef == LocalPlayer || (playerRef.IsNone && Simulation.IsServer);
			}
			if (message->GetFlag(4))
			{
				if (flag2 || !flag)
				{
					if (NetworkBehaviourUtils.TryGetRpcStaticInvokeDelegate(rpcHeader.Method, out var del))
					{
						del(this, message);
					}
					else
					{
						Log.Error(this, $"Could not find static RPC invoke delegate for index: {rpcHeader.Method}.");
					}
				}
				if (flag2 || !IsServer)
				{
					return;
				}
				if (flag)
				{
					if (playerRef == message->Source)
					{
						Log.DebugError(message, $"Target player {playerRef} same as the source, not forwarding (static).");
					}
					else
					{
						Simulation.ForwardMessage(message, playerRef, required: true);
					}
					return;
				}
				for (int i = 0; i < _simulation.MaxConnections; i++)
				{
					PlayerRef playerRef2 = i;
					if (playerRef2 != message->Source)
					{
						Simulation.ForwardMessage(message, playerRef2, required: false);
					}
				}
				return;
			}
			if (!TryFindObject(rpcHeader.Object, out var obj))
			{
				Log.DebugWarn(message, $"Simulation message target object not found: {rpcHeader.Object}");
				return;
			}
			NetworkBehaviour networkBehaviour = obj.NetworkedBehaviours[rpcHeader.Behaviour];
			if (BehaviourUtils.IsNotAlive(networkBehaviour))
			{
				Log.DebugWarn(message, $"Behaviour {rpcHeader.Behaviour} not found on {rpcHeader.Object}");
				return;
			}
			if (networkBehaviour.RpcCache == null && !NetworkBehaviourUtils.TryGetRpcInvokeDelegateArray(networkBehaviour.GetType(), out networkBehaviour.RpcCache))
			{
				Log.Error(this, $"Could not find RPC invoke array for {networkBehaviour.GetType()} on {obj.Name}.");
				return;
			}
			Assert.Check(rpcHeader.Method >= 0 && rpcHeader.Method < networkBehaviour.RpcCache.Length, rpcHeader.Method, networkBehaviour.RpcCache.Length, rpcHeader.Behaviour, networkBehaviour);
			RpcInvokeData rpcInvokeData = networkBehaviour.RpcCache[rpcHeader.Method];
			int rpcSourceAuthorityMask = obj.GetRpcSourceAuthorityMask(message->Source);
			if ((rpcInvokeData.Sources & rpcSourceAuthorityMask) == 0)
			{
				Log.DebugError(this, $"{message->Source} sent rpc {rpcInvokeData.Delegate.Method} to {obj.Name} but is not allowed.");
				return;
			}
			int localAuthorityMask = obj.GetLocalAuthorityMask();
			if ((rpcInvokeData.Targets & localAuthorityMask) != 0)
			{
				if (flag2 || !flag)
				{
					Assert.Check(!networkBehaviour.InvokeRpc);
					rpcInvokeData.Delegate(networkBehaviour, message);
					Assert.Check(!networkBehaviour.InvokeRpc);
				}
			}
			else if (flag2)
			{
				Log.DebugError(message, $"Not invoked locally because masks don't match: {rpcInvokeData.Targets} vs {localAuthorityMask}");
			}
			int num = rpcInvokeData.Targets & ~localAuthorityMask;
			if ((rpcInvokeData.Targets & 4) == 4)
			{
				num |= 4;
			}
			Assert.Check((num & 1) == 0 || Simulation.IsClient);
			if (!flag2 && IsServer && num != 0)
			{
				if (flag)
				{
					if (((num & 2) != 0 && obj.InputAuthority == playerRef) || ((num & 4) != 0 && obj.InputAuthority != playerRef))
					{
						if (playerRef == message->Source)
						{
							Log.DebugError(message, $"Target player {playerRef} same as the source, not forwarding ({obj.InputAuthority} {num}).");
						}
						else
						{
							Simulation.ForwardMessage(message, playerRef, required: true);
						}
					}
					else
					{
						Log.DebugError(message, $"Can't be forwarded to {playerRef} - excluded with authority masks");
					}
				}
				else
				{
					if ((num & 2) != 0 && obj.InputAuthority != default(PlayerRef))
					{
						Assert.Check(obj.InputAuthority != _simulation.LocalPlayer);
						if (obj.InputAuthority == message->Source)
						{
							if (rpcInvokeData.Targets == 2)
							{
								Log.DebugError(message, $"InputAuthority is same as the sender {obj.InputAuthority}, not forwarding.");
							}
						}
						else
						{
							Simulation.ForwardMessage(message, obj.InputAuthority, required: true);
						}
					}
					if ((num & 4) != 0)
					{
						for (int j = 0; j < _simulation.MaxConnections; j++)
						{
							PlayerRef playerRef3 = j;
							if (playerRef3 != obj.InputAuthority && playerRef3 != message->Source)
							{
								Simulation.ForwardMessage(message, playerRef3, required: false);
							}
						}
					}
				}
			}
			Simulation.Statistics.TickSample<Simulation.Statistics.RPCSample> item = new Simulation.Statistics.TickSample<Simulation.Statistics.RPCSample>(message->Tick, (float)Simulation.Stats.Timer.ElapsedInSeconds, new Simulation.Statistics.RPCSample
			{
				Behaviour = rpcHeader.Behaviour,
				Method = rpcHeader.Method
			});
			Simulation.Stats.GetObjectRpcBuffer(rpcHeader.Object, createIfMissing: true)?.Push(item);
		}
		catch (Exception exn)
		{
			Log.Exception(this, exn);
		}
	}

	void Simulation.ICallbacks.OnBeforeClientSidePredictionReset()
	{
		CallbackInterfaceInvoker.IBeforeClientPredictionReset(_behaviourUpdater);
	}

	void Simulation.ICallbacks.OnAfterClientSidePredictionReset()
	{
		CallbackInterfaceInvoker.IAfterClientPredictionReset(_behaviourUpdater);
	}

	void Simulation.ICallbacks.OnAfterTick()
	{
		CallbackInterfaceInvoker.IAfterTick(_behaviourUpdater);
	}

	void Simulation.ICallbacks.OnBeforeTick()
	{
		CallbackInterfaceInvoker.IBeforeTick(_behaviourUpdater);
	}

	void Simulation.ICallbacks.OnBeforeAllTicks(bool resimulation, int tickCount)
	{
		CallbackInterfaceInvoker.IBeforeAllTicks(_behaviourUpdater, resimulation, tickCount);
	}

	void Simulation.ICallbacks.OnConnectedToServer()
	{
		_cloudOperation?.TrySetResult((ShutdownReason.Ok, null));
		for (int i = 0; i < _callbacks.Count; i++)
		{
			_callbacks[i].OnConnectedToServer(this);
		}
		if (_sharedModeStartSceneRef.HasValue && IsSharedModeMasterClient)
		{
			SetActiveScene(_sharedModeStartSceneRef.Value);
		}
	}

	void Simulation.ICallbacks.OnDisconnectedFromServer()
	{
		for (int i = 0; i < _callbacks.Count; i++)
		{
			_callbacks[i].OnDisconnectedFromServer(this);
		}
	}

	void Simulation.ICallbacks.OnAfterAllTicks(bool resimulation, int tickCount)
	{
		CallbackInterfaceInvoker.IAfterAllTicks(_behaviourUpdater, resimulation, tickCount);
		ExpirePredictedSpawns();
	}

	void Simulation.ICallbacks.OnConnectionFailed(NetAddress remoteAddress, NetConnectFailedReason reason)
	{
		ShutdownReason item = ShutdownReason.Error;
		string item2 = "Connection Failed";
		switch (reason)
		{
		case NetConnectFailedReason.Timeout:
			item = ShutdownReason.ConnectionTimeout;
			item2 = "Connection Timeout";
			break;
		case NetConnectFailedReason.ServerFull:
			item = ShutdownReason.GameIsFull;
			item2 = "Game Is Full";
			break;
		case NetConnectFailedReason.ServerRefused:
			item = ShutdownReason.ConnectionRefused;
			item2 = "Connection Refused";
			break;
		}
		_cloudOperation?.TrySetResult((item, item2));
		for (int i = 0; i < _callbacks.Count; i++)
		{
			_callbacks[i].OnConnectFailed(this, remoteAddress, reason);
		}
	}

	void Simulation.ICallbacks.OnReliableData(PlayerRef player, byte[] dataArray)
	{
		for (int i = 0; i < _callbacks.Count; i++)
		{
			_callbacks[i].OnReliableDataReceived(this, player, new ArraySegment<byte>(dataArray));
		}
	}

	void Simulation.ICallbacks.PlayerJoined(PlayerRef player)
	{
		Assert.Check(_sceneManager.IsReady(this));
		for (int i = 0; i < _callbacks.Count; i++)
		{
			_callbacks[i].OnPlayerJoined(this, player);
		}
		CallbackInterfaceInvoker.IPlayerJoined(_behaviourUpdater, player);
	}

	void Simulation.ICallbacks.PlayerLeft(PlayerRef player)
	{
		Assert.Check(_sceneManager.IsReady(this));
		CallbackInterfaceInvoker.IPlayerLeft(_behaviourUpdater, player);
		for (int i = 0; i < _callbacks.Count; i++)
		{
			_callbacks[i].OnPlayerLeft(this, player);
		}
	}

	bool Simulation.ICallbacks.OnConnectionRequest(NetAddress remoteAddress, byte[] token)
	{
		if (_callbacks.Count > 0)
		{
			NetworkRunnerCallbackArgs.ConnectRequest connectRequest = new NetworkRunnerCallbackArgs.ConnectRequest();
			connectRequest.RemoteAddress = remoteAddress;
			for (int i = 0; i < _callbacks.Count; i++)
			{
				_callbacks[i].OnConnectRequest(this, connectRequest, token);
			}
			if (connectRequest.Accepted.HasValue)
			{
				return connectRequest.Accepted.Value;
			}
		}
		return true;
	}

	void Simulation.ICallbacks.OnInternalConnectionAttempt(int attempt, int totalConnectionAttempts, out bool shouldChange, out NetAddress newAddress)
	{
		shouldChange = false;
		newAddress = default;
		if (IsCloudReady)
		{
			_cloudServices.OnInternalConnectionAttempt(attempt, totalConnectionAttempts, out shouldChange, out newAddress);
		}
	}
}
