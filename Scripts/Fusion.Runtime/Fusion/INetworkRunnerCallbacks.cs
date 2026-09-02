using System;
using System.Collections.Generic;
using Fusion.Sockets;

namespace Fusion;

public interface INetworkRunnerCallbacks
{
	void OnPlayerJoined(NetworkRunner runner, PlayerRef player);

	void OnPlayerLeft(NetworkRunner runner, PlayerRef player);

	void OnInput(NetworkRunner runner, NetworkInput input);

	void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input);

	void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason);

	void OnConnectedToServer(NetworkRunner runner);

	void OnDisconnectedFromServer(NetworkRunner runner);

	void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token);

	void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason);

	void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message);

	void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList);

	void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data);

	void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken);

	void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data);

	void OnSceneLoadDone(NetworkRunner runner);

	void OnSceneLoadStart(NetworkRunner runner);
}
