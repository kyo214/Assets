#define DEBUG
using System.Collections.Generic;
using Fusion.Photon.Realtime;
using Fusion.Protocol;
using Fusion.Sockets.Stun;

namespace Fusion;

internal class CloudServicesMetadata
{
	public const int JOIN_PM_TIMEOUT = 15000;

	public static readonly TypedLobby LobbyClientServer = new TypedLobby("ClientServer", LobbyType.Default);

	public static readonly TypedLobby LobbyShared = new TypedLobby("Shared", LobbyType.Default);

	public ScheduledRequests ScheduledRequests = ScheduledRequests.None;

	public Disconnect LastDisconnectMsg = null;

	public Dictionary<long, ReflexiveInfo> UniqueIdToReflexiveInfoTable = new Dictionary<long, ReflexiveInfo>();

	private StunResult _localStunResult = null;

	public NetworkRunnerInitializeArgs RunnerInitializeArgs { get; set; } = default;

	public NATPunchStage CurrentPunchStage { get; set; } = NATPunchStage.None;

	public JoinProcessStage CurrentJoinStage { get; set; } = JoinProcessStage.Idle;

	public ProtocolMessageVersion CurrentProtocolMessageVersion { get; set; } = ProtocolMessageVersion.V1_6_0;

	public ReflexiveInfo RemoteReflexiveInfo { get; set; } = null;

	public StunResult LocalReflexiveInfo
	{
		get
		{
			return _localStunResult;
		}
		set
		{
			_localStunResult = value;
			if (_localStunResult != null)
			{
				Log.Debug($"Local ReflexiveInfo: {_localStunResult}");
			}
		}
	}

	public byte[] UniqueId { get; set; } = null;
}
