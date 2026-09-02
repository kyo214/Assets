#define DEBUG
#define FUSION_UNITY
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Fusion.Async;
using Fusion.Photon.Realtime;
using Fusion.Photon.Realtime.Async;
using Fusion.Photon.Realtime.Extension;
using Fusion.Protocol;
using Fusion.Sockets;
using Fusion.Sockets.Stun;

namespace Fusion;

internal class CloudServices : IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks, IDisposable
{
	private bool _tryingToReconnect = false;

	private readonly CloudServicesMetadata _metadata;

	private readonly NetworkRunner _runner;

	private CloudCommunicator _communicator;

	private readonly Stopwatch _watch = new Stopwatch();

	private readonly Dictionary<string, SessionInfo> cachedSessionList = new Dictionary<string, SessionInfo>();

	private readonly Stopwatch _snapshotWatch = new Stopwatch();

	private volatile int _lastSnapshotTick = -1;

	private volatile int _lastConfirmedSnapshotTick = -1;

	private const string MSG_START_BEFORE_JOIN = "Received Start Message, but never a Join Confirmation. Shutdown.";

	private const string MSG_RUNNER_FAIL_INIT = "Runner failed to Initialize. Shutdown.";

	private const string MSG_JOIN_TIMEOUT = "Join Confirmation timeout. Shutdown.";

	private byte[] _dummyData;

	private CancellationTokenSource _dummyTrafficCts;

	private CancellationTokenSource _dummyTrafficLinkCts;

	public bool CloudServerDisconnected { get; private set; } = false;

	public bool IsCloudReady => _communicator?.Client != null && _communicator.Client.IsConnectedAndReady;

	public string UserId => IsCloudReady ? _communicator.Client.UserId : null;

	public bool IsInRoom => IsCloudReady && _communicator.Client.IsReadyAndInRoom;

	public bool IsInLobby => IsCloudReady && _communicator.Client.InLobby;

	public int SessionSlots => IsInRoom ? _communicator.Client.CurrentRoom.MaxPlayers : (-1);

	public bool IsMasterClient => IsInRoom && _communicator.Client.LocalPlayer.IsMasterClient;

	public AuthenticationValues AuthenticationValues => IsCloudReady ? _communicator.Client.AuthValues : null;

	public ICommunicator Communicator => _communicator;

	public string CachedRegionSummary => _communicator.Client.SummaryToCache;

	public bool IsNATPunchthroughEnabled { get; internal set; } = true;

	public string CustomSTUNServer { get; internal set; } = null;

	public NATType NATType => (_metadata?.LocalReflexiveInfo != null) ? _metadata.LocalReflexiveInfo.NatType : NATType.Invalid;

	private bool IsServerOrMasterClient => _runner != null && (_runner.IsServer || _runner.IsSharedModeMasterClient);

	public void OnConnected()
	{
	}

	public void OnConnectedToMaster()
	{
	}

	public void OnCustomAuthenticationFailed(string debugMessage)
	{
		OperationFailHandler(32755, debugMessage);
	}

	public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
	{
		_runner?.InvokeCustomAuthenticationResponse(data);
	}

	public void OnDisconnected(DisconnectCause cause)
	{
		ShutdownReason shutdownReason = DisconnectCauseExt.ConvertToShutdownReason(cause);
		if (!HandlePhotonCloudDisconnect(shutdownReason))
		{
			if (shutdownReason != ShutdownReason.Ok)
			{
				Log.Debug(_runner, $"Disconnected from Photon Cloud: {cause}/{shutdownReason}");
			}
			string text = null;
			if (_metadata.LastDisconnectMsg != null)
			{
				shutdownReason = DisconnectReasonExt.ConvertToShutdownReason(_metadata.LastDisconnectMsg.DisconnectReason);
				text = _metadata.LastDisconnectMsg.CustomData;
				Log.DebugWarn(_runner, string.Format("Fusion Disconnect: {0}={1}, Message={2}", "DisconnectReason", _metadata.LastDisconnectMsg.DisconnectReason, text));
			}
			if (_runner._cloudOperation != null)
			{
				_runner._cloudOperation.TrySetResult((shutdownReason, text));
			}
			else
			{
				_runner.Shutdown(destroyGameObject: true, shutdownReason, forceShutdownProcedure: true);
			}
		}
	}

	public void OnRegionListReceived(RegionHandler regionHandler)
	{
		string text = string.Join(", ", from region in regionHandler.EnabledRegions.AsEnumerable()
			select region.Code + "[" + region.HostAndPort + "]");
		Log.Debug("OnRegionListReceived: EnabledRegions=" + text);
	}

	public void OnCreatedRoom()
	{
		Log.Debug(_runner, "Created Session: " + _communicator.Client.CurrentRoom.Name);
	}

	public void OnJoinedRoom()
	{
		Log.Debug(_runner, "Joined Session: " + _communicator.Client.CurrentRoom.Name);
		_runner.LobbyInfo.Reset();
	}

	public void OnLeftRoom()
	{
		Log.Debug(_runner, "Left Session");
		_runner.LobbyInfo.Reset();
	}

	public void OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	public void OnCreateRoomFailed(short returnCode, string message)
	{
		OperationFailHandler(returnCode, message);
	}

	public void OnJoinRandomFailed(short returnCode, string message)
	{
		OperationFailHandler(returnCode, message);
	}

	public void OnJoinRoomFailed(short returnCode, string message)
	{
		OperationFailHandler(returnCode, message);
	}

	public void OnJoinedLobby()
	{
		_runner.LobbyInfo.IsValid = true;
		_runner.LobbyInfo.Name = _communicator.Client.CurrentLobby.Name;
		_runner.LobbyInfo.Region = _communicator.Client.CloudRegion.Replace("/*", "");
		Log.Debug(_runner, "Joined Lobby: " + _runner.LobbyInfo.Name + ", Region=" + _runner.LobbyInfo.Region);
	}

	public void OnLeftLobby()
	{
		_runner.LobbyInfo.Reset();
		Log.Debug(_runner, "Left Lobby");
	}

	public void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		OnRoomListChanged(roomList);
	}

	public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
	{
	}

	private void OperationFailHandler(short returnCode, string message)
	{
		Log.DebugWarn(_runner, $"Photon Cloud Operation failed [{returnCode}]: '{message}'");
		ShutdownReason shutdownReason = ErrorCodeExt.ConvertToShutdownReason(returnCode);
		if (_runner._cloudOperation != null)
		{
			_runner._cloudOperation.TrySetResult((shutdownReason, message));
		}
		else if (!HandlePhotonCloudDisconnect(shutdownReason))
		{
			_runner.Shutdown(destroyGameObject: true, shutdownReason, forceShutdownProcedure: true);
		}
	}

	private bool HandlePhotonCloudDisconnect(ShutdownReason shutdownReason)
	{
		if (NetworkRunner.CloudConnectionLost != null && _runner._cloudOperation == null && (shutdownReason == ShutdownReason.PhotonCloudTimeout || (_tryingToReconnect && shutdownReason == ShutdownReason.GameNotFound)) && (_runner.IsServer || (_runner.IsClient && _runner.CurrentConnectionType == ConnectionType.Direct)) && _runner.GameMode >= GameMode.Server)
		{
			_tryingToReconnect = shutdownReason == ShutdownReason.PhotonCloudTimeout && _communicator.Client.ReconnectAndRejoin();
			if (_tryingToReconnect)
			{
				Log.Debug(_runner, $"Attempting to reconnect to Photon Cloud. Previous disconnect: {shutdownReason}");
			}
			else
			{
				CloudServerDisconnected = true;
			}
			try
			{
				Log.Debug(_runner, string.Format("Cloud Connection Lost: {0}={1}, TryingToReconnect={2}", "ShutdownReason", shutdownReason, _tryingToReconnect));
				NetworkRunner.CloudConnectionLost(_runner, shutdownReason, _tryingToReconnect);
			}
			catch (Exception exn)
			{
				Log.Exception(exn);
			}
			return true;
		}
		return false;
	}

	public CloudServices(NetworkRunner runner, CloudCommunicator communicator = null)
	{
		_runner = runner;
		_runner.InitFusionLogSystem();
		InitRelayLogs();
		TaskManager.Setup();
		_communicator = communicator ?? new CloudCommunicator();
		_communicator.Client.AddCallbackTarget(this);
		_communicator.Client.OnRoomChanged += OnRoomChanged;
		_communicator.Client.AddressRewriter = runner.CloudAddressRewriter;
		_communicator.WasExtracted = false;
		if (_communicator.Client.IsConnected)
		{
			_communicator.Client.StartFallbackSendAck();
		}
		_communicator.RegisterPackageCallback<Join>(HandleJoinMessage);
		_communicator.RegisterPackageCallback<Start>(HandleStartMessage);
		_communicator.RegisterPackageCallback<Disconnect>(HandleDisconnectMessage);
		_communicator.RegisterPackageCallback<ReflexiveInfo>(HandleReflexiveInfoMessage);
		_communicator.RegisterPackageCallback<NetworkConfigSync>(HandleNetworkConfigMessage);
		_communicator.RegisterPackageCallback<HostMigration>(HandleHostMigrationMessage);
		_communicator.RegisterPackageCallback<Snapshot>(HandleSnapshotMessage);
		_communicator.RegisterPackageCallback<DummyTrafficSync>(HandleDummyTrafficSync);
		_metadata = new CloudServicesMetadata();
	}

	public CloudCommunicator ExtractCommunicator()
	{
		_communicator.Client.RemoveCallbackTarget(this);
		_communicator.Client.OnRoomChanged -= OnRoomChanged;
		_communicator.Client.StopFallbackSendAck();
		_communicator.Reset();
		_communicator.WasExtracted = true;
		return _communicator;
	}

	public void Update()
	{
		if (_communicator != null && !_communicator.WasExtracted && !CloudServerDisconnected)
		{
			_communicator.Service();
			Service_CheckScheduledRequests();
		}
	}

	public async Task ConnectToCloud(AuthenticationValues authentication = null, AppSettings customAppSettings = null, bool? useDefaultCloudPorts = null)
	{
		AppSettings appSettings = customAppSettings ?? PhotonAppSettings.Instance.AppSettings;
		if (appSettings == null)
		{
			throw new InvalidOperationException("Photon Application Settings not found.");
		}
		Log.Debug(appSettings.ToStringFull());
		_communicator.Client.AuthValues = authentication;
		if (_communicator.Client.AuthValues != null)
		{
			Log.Debug(_runner, $"Connecting using Authentication: {_communicator.Client.AuthValues}");
		}
		_communicator.Client.UseDefaultPorts = useDefaultCloudPorts == true;
		await _communicator.Client.ConnectToMasterAsync(appSettings);
		Log.Debug(_runner, "Connected to Photon Cloud.");
	}

	public Task<short> JoinSessionLobby(SessionLobby sessionLobby, string lobbyID = null, LobbyType lobbyType = LobbyType.Default)
	{
		if (!IsCloudReady)
		{
			return Task.FromException<short>(new InvalidOperationException("Fusion Relay Client is not ready. Make sure the call ConnectToCloud before start with StartGame"));
		}
		TypedLobby lobby;
		switch (sessionLobby)
		{
		case SessionLobby.ClientServer:
			lobby = CloudServicesMetadata.LobbyClientServer;
			break;
		case SessionLobby.Shared:
			lobby = CloudServicesMetadata.LobbyShared;
			break;
		case SessionLobby.Custom:
			if (string.IsNullOrEmpty(lobbyID?.Trim()))
			{
				return Task.FromException<short>(new InvalidOperationException("Invalid Lobby Name: Empty or Null"));
			}
			lobby = new TypedLobby(lobbyID.Trim(), lobbyType);
			break;
		default:
			return Task.FromException<short>(new InvalidOperationException("Invalid Lobby Type"));
		}
		return _communicator.Client.JoinLobbyAsync(lobby);
	}

	public Task<short> EnterRoom(StartGameArgs args)
	{
		if (!IsCloudReady)
		{
			return Task.FromException<short>(new InvalidOperationException("Fusion Relay Client is not ready. Make sure the call ConnectToCloud before start with StartGame"));
		}
		if (IsInRoom)
		{
			return Task.FromResult((short)0);
		}
		bool flag = NetworkRunner.CloudConnectionLost != null && (args.GameMode == GameMode.Client || args.GameMode == GameMode.Host || args.GameMode == GameMode.Server || args.GameMode == GameMode.AutoHostOrClient);
		if (flag)
		{
			_communicator.Client.DisconnectTimeout = 5000;
		}
		bool flag2 = args.GameMode == GameMode.Host || args.GameMode == GameMode.Server;
		bool flag3 = (args.GameMode == GameMode.Shared || args.GameMode == GameMode.AutoHostOrClient || args.GameMode == GameMode.Client) && !args.DisableClientSessionCreation;
		bool flag4 = args.GameMode == GameMode.Server;
		TypedLobby typedLobby = ((!_communicator.Client.InLobby) ? ((!string.IsNullOrEmpty(args.CustomLobbyName?.Trim())) ? new TypedLobby(args.CustomLobbyName.Trim(), LobbyType.Default) : ((args.GameMode == GameMode.Shared) ? CloudServicesMetadata.LobbyShared : CloudServicesMetadata.LobbyClientServer)) : _communicator.Client.CurrentLobby);
		string text = args.SessionName?.Trim();
		bool flag5 = string.IsNullOrEmpty(text);
		text = (flag5 ? Guid.NewGuid().ToString() : text);
		int maxPlayers = (args.PlayerCount ?? NetworkProjectConfig.Global.Simulation.DefaultPlayers) + (flag4 ? 1 : 0);
		EnterRoomParams enterRoomParams = FusionRelayClient.BuildEnterRoomParams(typedLobby, text, maxPlayers, args.SessionProperties, args.IsOpen ?? true, args.IsVisible ?? true, flag);
		OpJoinRandomRoomParams opJoinRandomRoomParams = FusionRelayClient.BuildJoinParams(typedLobby, args.SessionProperties, args.MatchmakingMode.GetValueOrDefault());
		string text2 = (flag5 ? ("Random Session (" + text + ")") : enterRoomParams.RoomName);
		Log.Debug(_runner, "Joining Session: [" + text2 + "], Lobby=[" + opJoinRandomRoomParams.TypedLobby.Name + "], Region=[" + _communicator.Client.CloudRegion?.Replace("/*", "") + "]");
		if (flag2)
		{
			return _communicator.Client.CreateOrJoinRoomAsync(enterRoomParams);
		}
		if (flag5)
		{
			if (flag3)
			{
				return _communicator.Client.JoinRandomOrCreateRoomAsync(opJoinRandomRoomParams, enterRoomParams);
			}
			return _communicator.Client.JoinRandomRoomAsync(opJoinRandomRoomParams);
		}
		if (flag3)
		{
			return _communicator.Client.CreateOrJoinRoomAsync(enterRoomParams);
		}
		return _communicator.Client.JoinRoomAsync(enterRoomParams);
	}

	public async Task DisconnectFromCloud()
	{
		if (_communicator != null)
		{
			if (_communicator.WasExtracted)
			{
				return;
			}
			await _communicator.Client.DisconnectAsync();
		}
		Log.Debug(_runner, "Disconnected from Photon Cloud.");
	}

	public string GetActorUserID(int actorID)
	{
		if (IsInRoom && _communicator.Client.CurrentRoom.Players.TryGetValue(actorID, out var value))
		{
			return value.UserId;
		}
		return null;
	}

	public bool TryGetActorIdByUniqueId(long uniqueId, out int actorId)
	{
		if (_metadata.UniqueIdToReflexiveInfoTable.TryGetValue(uniqueId, out var value))
		{
			actorId = value.ActorNr;
			return true;
		}
		actorId = -1;
		return false;
	}

	internal void OnInternalConnectionAttempt(int attempt, int totalConnectionAttempts, out bool shouldChange, out NetAddress newAddress)
	{
		shouldChange = false;
		newAddress = default;
		if (_runner.GameMode != GameMode.Client)
		{
			return;
		}
		switch (_metadata.CurrentPunchStage)
		{
		case NATPunchStage.None:
			Assert.AlwaysFail($"CloudServices should not be in Stage {_metadata.CurrentPunchStage}");
			break;
		case NATPunchStage.Local:
			if (attempt > 2)
			{
				shouldChange = true;
				newAddress = _metadata.RemoteReflexiveInfo.PublicAddr;
				_metadata.CurrentPunchStage = NATPunchStage.Public;
			}
			break;
		case NATPunchStage.Public:
			if ((float)attempt >= (float)totalConnectionAttempts * 2f / 3f)
			{
				shouldChange = true;
				newAddress = NetAddress.FromActorId(_metadata.RemoteReflexiveInfo.ActorNr);
				_metadata.CurrentPunchStage = NATPunchStage.Relay;
			}
			break;
		case NATPunchStage.Relay:
			break;
		}
	}

	private void Connect(NATPunchStage punchStage, NetAddress endPoint)
	{
		Log.Debug(_runner, $"Connecting to {endPoint}");
		_metadata.CurrentPunchStage = punchStage;
		_runner.Connect(endPoint, _metadata.RunnerInitializeArgs.ConnectionToken, _metadata.UniqueId);
	}

	public void Dispose()
	{
		_communicator?.Dispose();
		_communicator = null;
	}

	internal void OnRoomChanged()
	{
		if (IsInRoom && _runner.SessionInfo != null)
		{
			UpdateSessionInfo(_runner.SessionInfo, _communicator.Client.CurrentRoom, _communicator.Client.CloudRegion);
			Log.Debug($"SessionInfo Update: {_runner.SessionInfo}");
		}
	}

	internal bool UpdateRoomProperties(Dictionary<string, SessionProperty> customProperties)
	{
		return IsServerOrMasterClient && IsInRoom && _communicator.Client.UpdateRoomProperties(customProperties);
	}

	internal bool UpdateRoomIsOpen(bool status)
	{
		return IsServerOrMasterClient && IsInRoom && _communicator.Client.UpdateRoomIsOpen(status);
	}

	internal bool UpdateRoomIsVisible(bool status)
	{
		return IsServerOrMasterClient && IsInRoom && _communicator.Client.UpdateRoomIsVisible(status);
	}

	private void OnRoomListChanged(List<RoomInfo> roomList)
	{
		foreach (RoomInfo room in roomList)
		{
			if (room.RemovedFromList)
			{
				cachedSessionList.Remove(room.Name);
				continue;
			}
			if (!cachedSessionList.ContainsKey(room.Name))
			{
				cachedSessionList[room.Name] = new SessionInfo();
			}
			UpdateSessionInfo(cachedSessionList[room.Name], room, _communicator.Client.CloudRegion);
		}
		_runner.InvokeSessionListUpdated(new List<SessionInfo>(cachedSessionList.Values));
	}

	public void SendJoinMessage()
	{
		PeerMode peerMode;
		PluginGameMode pluginGameMode;
		switch (_runner.GameMode)
		{
		case GameMode.Shared:
			peerMode = PeerMode.Client;
			pluginGameMode = PluginGameMode.Shared;
			break;
		case GameMode.Server:
		case GameMode.Host:
			peerMode = PeerMode.Server;
			pluginGameMode = PluginGameMode.ClientServer;
			break;
		case GameMode.Client:
			peerMode = PeerMode.Client;
			pluginGameMode = PluginGameMode.ClientServer;
			break;
		default:
			throw new InvalidOperationException($"Invalid Game Mode {_runner.GameMode}");
		}
		if (pluginGameMode != PluginGameMode.Invalid && peerMode != PeerMode.None)
		{
			Join obj = new Join(JoinMessageType.Request, pluginGameMode, peerMode);
			if (!IsNATPunchthroughEnabled)
			{
				obj.JoinRequests |= JoinRequests.DisableNATPunch;
			}
			_communicator.SendMessage(0, obj);
			_watch.Restart();
			_metadata.CurrentJoinStage = JoinProcessStage.Joining;
		}
	}

	public void SendNetworkSyncMessage(NetworkProjectConfig projectConfig)
	{
		string text = NetworkProjectConfig.SerializeMinimal(projectConfig);
		Log.Debug(_runner, "Sending serialized NetworkProjectConfig:\n" + text);
		NetworkConfigSync message = new NetworkConfigSync(SyncType.Response, text, _metadata.CurrentProtocolMessageVersion);
		_communicator.SendMessage(0, message);
	}

	public void SendReflexiveInfo(StunResult stunResult)
	{
		if (!stunResult.IsValid)
		{
		}
		ReflexiveInfo message = new ReflexiveInfo(_communicator.CommunicatorID, stunResult.PublicEndPoint, stunResult.PrivateEndPoint, stunResult.NatType, null, _metadata.CurrentProtocolMessageVersion);
		_communicator.SendMessage(0, message);
	}

	public void SendStateSnapshot(int snapshotSize, int tick, uint lastId, byte[] data)
	{
		try
		{
			Snapshot snapshot = new Snapshot(tick, lastId, SnapshotType.Data, snapshotSize, data, _metadata.CurrentProtocolMessageVersion);
			Assert.Check(snapshot.IsValid);
			_communicator.SendMessage(0, snapshot);
		}
		catch (Exception msg)
		{
			Log.DebugError(msg);
		}
	}

	private void HandleJoinMessage(int sender, Join join)
	{
		Assert.Check(sender == 0, "Invalid Sender of Join Confirmation", sender);
		Assert.Check(join.Type == JoinMessageType.Confirmation, "Invalid Join Message, it should be a Confirmation");
		Assert.Check(_metadata.CurrentJoinStage == JoinProcessStage.Joining, string.Format("Invalid {0}={1}, it should be {2}", "CurrentJoinStage", _metadata.CurrentJoinStage, "Joining"));
		_watch.Reset();
		_metadata.CurrentJoinStage = JoinProcessStage.Done;
		_metadata.CurrentProtocolMessageVersion = join.ProtocolVersion;
		_metadata.UniqueId = join.UniqueId;
		if (join.JoinRequests.HasFlag(JoinRequests.NetworkConfig))
		{
			SendNetworkSyncMessage(NetworkRunner.SetupNetworkProjectConfig(_metadata.RunnerInitializeArgs));
		}
		if (join.JoinRequests.HasFlag(JoinRequests.ReflexiveInfo))
		{
			_metadata.ScheduledRequests.Set(ScheduledRequests.ReflexiveInfo);
		}
	}

	private async void HandleStartMessage(int sender, Start start)
	{
		Assert.Check(sender == 0, string.Format("Invalid Sender of {0} Message: {1}", "Start", sender));
		if (!(await ConfirmJoin()))
		{
			Log.DebugWarn(_runner, "Received Start Message, but never a Join Confirmation. Shutdown.");
			_runner._cloudOperation?.TrySetResult((ShutdownReason.Error, "Received Start Message, but never a Join Confirmation. Shutdown."));
			return;
		}
		try
		{
			if (_metadata.RunnerInitializeArgs.SimulationMode == SimulationModes.Client)
			{
				NetworkRunnerInitializeArgs initArgs = _metadata.RunnerInitializeArgs;
				initArgs.PlayerCount = SessionSlots;
				_metadata.RunnerInitializeArgs = initArgs;
			}
			if (!(await _runner.Initialize(_metadata.RunnerInitializeArgs)))
			{
				Log.DebugWarn(_runner, "Runner failed to Initialize. Shutdown.");
				_runner._cloudOperation?.TrySetResult((ShutdownReason.Error, "Runner failed to Initialize. Shutdown."));
				return;
			}
			switch (_runner.GameMode)
			{
			case GameMode.Shared:
				if (start.StartRequests.HasFlag(StartRequests.ConnectToShared))
				{
					Connect(NATPunchStage.Relay, NetAddress.FromActorId(0));
				}
				break;
			case GameMode.Server:
			case GameMode.Host:
			case GameMode.Client:
				if (_metadata.ScheduledRequests.IsSet(ScheduledRequests.ReflexiveInfo))
				{
					CloudServicesMetadata metadata = _metadata;
					metadata.LocalReflexiveInfo = await QueryReflexiveInfo().ConfigureAwait(continueOnCapturedContext: false);
					SendReflexiveInfo(_metadata.LocalReflexiveInfo);
					_metadata.ScheduledRequests.Clear(ScheduledRequests.ReflexiveInfo);
				}
				if (_runner.IsClient)
				{
					_metadata.RemoteReflexiveInfo = new ReflexiveInfo(start.RemoteServerID, default, default, NATType.UdpBlocked);
					IsNATPunchthroughEnabled &= start.StartRequests.HasFlag(StartRequests.WaitForReflexiveInfo);
				}
				break;
			}
			Log.Debug(_runner, "Fusion Simulation Startup Done.");
			switch (_runner.GameMode)
			{
			case GameMode.Server:
			case GameMode.Host:
				_runner._cloudOperation?.TrySetResult((ShutdownReason.Ok, null));
				break;
			case GameMode.Shared:
			case GameMode.Client:
				break;
			}
		}
		catch (Exception exception)
		{
			_runner._cloudOperation?.TrySetException(exception);
		}
	}

	private void HandleDisconnectMessage(int sender, Disconnect disconnect)
	{
		Assert.Check(sender == 0, string.Format("Invalid Sender of {0} Message: {1}", "Disconnect", sender));
		_metadata.LastDisconnectMsg = disconnect;
	}

	private void HandleNetworkConfigMessage(int sender, NetworkConfigSync configSync)
	{
	}

	private async void HandleReflexiveInfoMessage(int sender, ReflexiveInfo reflexiveInfo)
	{
		Assert.Check(sender == 0, $"Invalid Sender of Reflexive Info Message: {sender}");
		if (!(await ConfirmJoin()))
		{
			Log.DebugWarn(_runner, "Received ReflexiveInfo Message, but never a Join Confirmation. Ignore.");
			return;
		}
		switch (_runner.GameMode)
		{
		case GameMode.Client:
			if (_metadata.RemoteReflexiveInfo.ActorNr == reflexiveInfo.ActorNr)
			{
				_metadata.RemoteReflexiveInfo = reflexiveInfo;
				if (IsNATPunchthroughEnabled && CheckSubnet(_metadata.RemoteReflexiveInfo.PrivateAddr))
				{
					Connect(NATPunchStage.Local, _metadata.RemoteReflexiveInfo.PrivateAddr);
				}
				else if (IsNATPunchthroughEnabled && _metadata.RemoteReflexiveInfo.PublicAddr.IsValid && _metadata.RemoteReflexiveInfo.NatType.IsValid())
				{
					Connect(NATPunchStage.Public, _metadata.RemoteReflexiveInfo.PublicAddr);
				}
				else
				{
					Connect(NATPunchStage.Relay, NetAddress.FromActorId(_metadata.RemoteReflexiveInfo.ActorNr));
				}
			}
			break;
		case GameMode.Server:
		case GameMode.Host:
			if (reflexiveInfo.UniqueId != null && reflexiveInfo.UniqueId.Length == 8)
			{
				long uniqueId = BitConverter.ToInt64(reflexiveInfo.UniqueId, 0);
				if (uniqueId != 0)
				{
					_metadata.UniqueIdToReflexiveInfoTable[uniqueId] = reflexiveInfo;
				}
				else
				{
					Log.Warn($"Received Invalid UniqueId from Actor {reflexiveInfo.ActorNr}");
				}
			}
			Run_ReversePing(reflexiveInfo.PrivateAddr);
			if (IsNATPunchthroughEnabled)
			{
				Run_ReversePing(reflexiveInfo.PublicAddr);
			}
			break;
		}
	}

	private void HandleHostMigrationMessage(int sender, HostMigration hostMigration)
	{
		Assert.Check(sender == 0, string.Format("Invalid Sender of {0}: {1}", "HostMigration", sender));
		_runner.SetupHostMigration(hostMigration);
		if (!hostMigration.WaitForSnapshot)
		{
			_runner.StartHostMigration();
		}
	}

	private void HandleSnapshotMessage(int sender, Snapshot snapshot)
	{
		Assert.Check(sender == 0, string.Format("Invalid Sender of {0}: {1}", "Snapshot", sender));
		switch (snapshot.SnapshotType)
		{
		case SnapshotType.Data:
			_runner.StartHostMigration(snapshot);
			break;
		case SnapshotType.Confirmation:
			if (_lastSnapshotTick != snapshot.Tick)
			{
				Log.DebugWarn($"Expecting Snapshot: {_lastSnapshotTick}");
			}
			if (_lastConfirmedSnapshotTick < snapshot.Tick)
			{
				Interlocked.Exchange(ref _lastConfirmedSnapshotTick, snapshot.Tick);
				Log.Debug($"Host Snapshot for Tick {_lastConfirmedSnapshotTick} confirmed.");
			}
			break;
		}
	}

	private void HandleDummyTrafficSync(int sender, DummyTrafficSync dummyTrafficSync)
	{
		Assert.Check(sender == 0, string.Format("Invalid Sender of {0}: {1}", "DummyTrafficSync", sender));
		SetupDummyTraffic(dummyTrafficSync);
	}

	private void Service_CheckScheduledRequests()
	{
		switch (_metadata.CurrentJoinStage)
		{
		case JoinProcessStage.Idle:
			break;
		case JoinProcessStage.Joining:
			if (_watch.ElapsedMilliseconds > 15000)
			{
				_metadata.CurrentJoinStage = JoinProcessStage.Fail;
				_watch.Stop();
			}
			break;
		case JoinProcessStage.Done:
			break;
		case JoinProcessStage.Fail:
			Log.DebugWarn(_runner, "Join Confirmation timeout. Shutdown.");
			_runner._cloudOperation?.TrySetResult((ShutdownReason.PhotonCloudTimeout, "Join Confirmation timeout. Shutdown."));
			break;
		}
	}

	private async Task<bool> ConfirmJoin()
	{
		while (true)
		{
			switch (_metadata.CurrentJoinStage)
			{
			case JoinProcessStage.Idle:
				Assert.AlwaysFail("Received a Protocol Message without sending Join Message.");
				break;
			case JoinProcessStage.Joining:
				while (_metadata.CurrentJoinStage == JoinProcessStage.Joining)
				{
					await TaskManager.Delay(10);
				}
				continue;
			case JoinProcessStage.Done:
				return true;
			}
			break;
		}
		return false;
	}

	internal void StartBackgroundCloudServices()
	{
		if (_runner.IsHostMigrationEnabled)
		{
			TaskManager.Service((Func<Task<bool>>)Service_SendHostMigrationSnapshots, _runner.OperationsCancellationToken, (int)_runner.HostMigrationSnapshotDelay, "SendHostMigrationSnapshots");
		}
	}

	private async Task<bool> Service_SendHostMigrationSnapshots()
	{
		if (_runner.IsRunning && _runner.IsInitialized)
		{
			if (!_runner.IsServer || !_runner.IsHostMigrationEnabled)
			{
				return false;
			}
			switch (_metadata.CurrentJoinStage)
			{
			case JoinProcessStage.Idle:
			case JoinProcessStage.Joining:
				return true;
			case JoinProcessStage.Fail:
				return false;
			}
			if ((int)_metadata.CurrentProtocolMessageVersion < 5)
			{
				return false;
			}
			await SendHostMigrationSnapshot();
		}
		return true;
	}

	internal async Task<bool> SendHostMigrationSnapshot()
	{
		if (_lastSnapshotTick > _lastConfirmedSnapshotTick)
		{
			Log.Warn($"Host Snapshot Confirmed for Tick {_lastSnapshotTick} was not confirmed yet. Ignore.");
			return false;
		}
		int lastStoredTick = _lastSnapshotTick;
		var (valid, snapshotSize, tick, lastId, data) = await _runner.GetServerSnapshot();
		if (valid && lastStoredTick == Interlocked.CompareExchange(ref _lastSnapshotTick, tick, lastStoredTick))
		{
			SendStateSnapshot(snapshotSize, tick, lastId, data);
			return true;
		}
		return false;
	}

	private void Run_ReversePing(NetAddress remoteAddr)
	{
		if (!remoteAddr.IsValid)
		{
			return;
		}
		TaskManager.Run(async (CancellationToken token) =>
		{
			Log.Debug(_runner, $"Reverse NAT Punch: {remoteAddr}");
			for (int i = 0; i < 10; i++)
			{
				token.ThrowIfCancellationRequested();
				if (!SendPing(remoteAddr))
				{
					break;
				}
				await TaskManager.Delay(100, token);
			}
		}, _runner.OperationsCancellationToken);
		unsafe bool SendPing(NetAddress netAddress)
		{
			return _runner?.Simulation?.NetworkSendPing(netAddress, null, 0) == true;
		}
	}

	private void SetupDummyTraffic(DummyTrafficSync dummyTrafficSyncMessage)
	{
		if (dummyTrafficSyncMessage == null || !dummyTrafficSyncMessage.IsValid)
		{
			return;
		}
		if (_dummyTrafficCts != null)
		{
			_dummyTrafficCts.Cancel();
			_dummyTrafficCts.Dispose();
		}
		_dummyTrafficCts = new CancellationTokenSource();
		_dummyTrafficLinkCts = CancellationTokenSource.CreateLinkedTokenSource(_dummyTrafficCts.Token, _runner.OperationsCancellationToken);
		if (_dummyData == null || _dummyData.Length != dummyTrafficSyncMessage.Size)
		{
			_dummyData = new byte[dummyTrafficSyncMessage.Size];
			new Random().NextBytes(_dummyData);
		}
		TaskManager.Service(() =>
		{
			if (_runner.IsRunning && _runner.Topology != SimulationConfig.Topologies.ClientServer)
			{
				return Task.FromResult(result: false);
			}
			SendDummyTraffic(_dummyData);
			return Task.FromResult(result: true);
		}, _dummyTrafficLinkCts.Token, dummyTrafficSyncMessage.SendInterval, "DummyTraffic");
		unsafe void SendDummyTraffic(byte[] buffer)
		{
			if (_runner.IsRunning && _communicator.Client.IsConnectedAndReady)
			{
				fixed (byte* buffer2 = buffer)
				{
					_communicator.SendPackage(102, _communicator.CommunicatorID, reliable: false, buffer2, buffer.Length);
				}
			}
		}
	}

	private unsafe async Task<StunResult> QueryReflexiveInfo()
	{
		if (!IsNATPunchthroughEnabled)
		{
			return StunResult.Invalid;
		}
		await Task.Yield();
		NetAddress publicAddr1 = NetAddress.AnyIPv4Addr;
		NetAddress publicAddr2 = NetAddress.AnyIPv4Addr;
		if (StunClient.QueryLocalAddress(_runner._simulation._netPeer, _runner._simulation._netSocket, out var targetFamily, out var address))
		{
			AddressFamily family = targetFamily;
			NetAddress localAddr = address;
			if (_metadata.RunnerInitializeArgs.PublicAddress.HasValue)
			{
				NetAddress value;
				publicAddr2 = (value = _metadata.RunnerInitializeArgs.PublicAddress.Value);
				publicAddr1 = value;
			}
			else
			{
				await StunServers.SetupStunServers(CustomSTUNServer);
				int debugMultiplier = ((_runner.Config.PeerMode != NetworkProjectConfig.PeerModes.Multiple) ? 1 : 10);
				int attemptDelay = 150;
				int stunTimeout = attemptDelay * 10 * debugMultiplier;
				Guid requestID = Guid.Empty;
				Stopwatch watch = Stopwatch.StartNew();
				Stopwatch attemptWatch = new Stopwatch();
				while (!_runner.IsShutdown && watch.ElapsedMilliseconds < stunTimeout && (publicAddr1.Equals(NetAddress.AnyIPv4Addr) || publicAddr2.Equals(NetAddress.AnyIPv4Addr)))
				{
					bool validRequest = false;
					if (StunClient.QueryPublicAddress(_runner._simulation._netPeer, _runner._simulation._netSocket, family, ref requestID, out var skipNATDiscovery))
					{
						StunClient.PendingRequests.TryAdd(requestID, new ConcurrentDictionary<int, NetAddress>());
						validRequest = true;
					}
					attemptWatch.Restart();
					while (validRequest && attemptWatch.ElapsedMilliseconds < attemptDelay)
					{
						await TaskManager.Delay(attemptDelay / 10);
						if (StunClient.PendingRequests.TryGetValue(requestID, out var addresses) && addresses.Count > 0)
						{
							KeyValuePair<int, NetAddress>[] publicAddresses = addresses.ToArray();
							if (publicAddresses.Length >= 1 && publicAddr1.Equals(NetAddress.AnyIPv4Addr))
							{
								publicAddr1 = publicAddresses[0].Value;
								if (skipNATDiscovery)
								{
									publicAddr2 = publicAddr1;
								}
							}
							if (publicAddresses.Length >= 2 && publicAddr2.Equals(NetAddress.AnyIPv4Addr))
							{
								publicAddr2 = publicAddresses[1].Value;
							}
							if (!publicAddr1.Equals(NetAddress.AnyIPv4Addr) && !publicAddr2.Equals(NetAddress.AnyIPv4Addr))
							{
								break;
							}
						}
						addresses = null;
					}
				}
				StunClient.PendingRequests.TryRemove(requestID, out var _);
			}
			StunResult stunResult = new StunResult(publicAddr1, localAddr);
			if (publicAddr1.Equals(NetAddress.AnyIPv4Addr) && publicAddr2.Equals(NetAddress.AnyIPv4Addr))
			{
				stunResult.NatType = NATType.UdpBlocked;
			}
			else if (publicAddr1.Equals(localAddr))
			{
				stunResult.NatType = NATType.OpenInternet;
			}
			else if (publicAddr1.Equals(publicAddr2))
			{
				stunResult.NatType = NATType.FullCone;
			}
			else
			{
				stunResult.NatType = NATType.Symmetric;
			}
			return stunResult;
		}
		return StunResult.Invalid;
	}

	public void UpdateInitializeArgs(NetworkRunnerInitializeArgs newArgs)
	{
		_metadata.RunnerInitializeArgs = newArgs;
	}

	private bool CheckSubnet(NetAddress remotePrivateEndPoint)
	{
		return _metadata.LocalReflexiveInfo != null && (remotePrivateEndPoint.IsIPv6 || NetAddress.SubnetMask.IsSameSubNet(_metadata.LocalReflexiveInfo.PrivateEndPoint, remotePrivateEndPoint));
	}

	private void InitRelayLogs()
	{
		Fusion.Photon.Realtime.Async.Log.Init((string info) =>
		{
			Log.InfoRealtime(info);
		}, (string warn) =>
		{
			Log.WarnRealtime(warn);
		}, (string error) =>
		{
			Log.ErrorRealtime(error);
		}, (Exception exn) =>
		{
			Log.ExceptionRealtime(exn);
		});
	}

	private void UpdateSessionInfo(SessionInfo sessionInfo, RoomInfo roomInfo, string region)
	{
		if (roomInfo is Room room)
		{
			sessionInfo.Name = room.Name;
			sessionInfo._isOpen = room.IsOpen;
			sessionInfo._isVisible = room.IsVisible;
			sessionInfo.MaxPlayers = room.MaxPlayers;
			sessionInfo.PlayerCount = room.PlayerCount;
		}
		else
		{
			sessionInfo.Name = roomInfo.Name;
			sessionInfo._isOpen = roomInfo.IsOpen;
			sessionInfo._isVisible = roomInfo.IsVisible;
			sessionInfo.MaxPlayers = roomInfo.MaxPlayers;
			sessionInfo.PlayerCount = roomInfo.PlayerCount;
		}
		sessionInfo.Region = region;
		sessionInfo.Properties = new ReadOnlyDictionary<string, SessionProperty>(roomInfo.GetCustomProperties());
		sessionInfo._isValid = true;
	}
}
