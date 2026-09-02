using System;
using System.Collections.Generic;
using Dissonance.Extensions;
using Dissonance.Networking;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace Dissonance.Integrations.PhotonFusion;

public class FusionCommsNetwork : BaseCommsNetwork<FusionServer, FusionClient, FusionPeer, Unit, Unit>, INetworkRunnerCallbacks
{
	private bool _sentNetworkRunnerWarning;

	private NetworkRunner _runner;

	private NetworkRunner _addedToCallbacks;

	private readonly Queue<byte[]> _byteArrayPool = new Queue<byte[]>();

	private readonly Queue<(FusionPeer, ArraySegment<byte>)> _serverMessageQueue = new Queue<(FusionPeer, ArraySegment<byte>)>();

	private readonly Queue<ArraySegment<byte>> _clientMessageQueue = new Queue<ArraySegment<byte>>();

	protected override FusionServer CreateServer(Unit connectionParameters)
	{
		return new FusionServer(this);
	}

	protected override FusionClient CreateClient(Unit connectionParameters)
	{
		return new FusionClient(this);
	}

	protected override void Update()
	{
		if (base.IsInitialized)
		{
			if (!_runner)
			{
				_runner = null;
			}
			if (_runner == null)
			{
				_runner = GetComponentInParent<NetworkRunner>();
				if (_runner == null)
				{
					if (!_sentNetworkRunnerWarning)
					{
						_sentNetworkRunnerWarning = true;
						Log.Warn("`FusionCommsNetwork` not attached to `NetworkRunner`");
					}
					_runner = UnityEngine.Object.FindObjectOfType<NetworkRunner>();
				}
			}
			if (_runner != null)
			{
				if (_addedToCallbacks != _runner)
				{
					if (_addedToCallbacks != null)
					{
						_addedToCallbacks.RemoveCallbacks(this);
					}
					_runner.AddCallbacks(this);
					_addedToCallbacks = _runner;
				}
				if (!_runner.IsRunning)
				{
					if (base.Mode != NetworkMode.None)
					{
						Stop();
					}
				}
				else if (_runner.Mode == SimulationModes.Host)
				{
					if (base.Mode != NetworkMode.Host)
					{
						RpcHandler.Initialize(this, _runner);
						RunAsHost(Unit.None, Unit.None);
					}
				}
				else if (_runner.Mode == SimulationModes.Client)
				{
					if (base.Mode != NetworkMode.Client)
					{
						RpcHandler.Initialize(this, _runner);
						RunAsClient(Unit.None);
					}
				}
				else if (base.Mode != NetworkMode.DedicatedServer)
				{
					RpcHandler.Initialize(this, _runner);
					RunAsDedicatedServer(Unit.None);
				}
			}
		}
		base.Update();
	}

	internal void ReadClientMessages(FusionClient client)
	{
		while (_clientMessageQueue.Count > 0)
		{
			ArraySegment<byte> data = _clientMessageQueue.Dequeue();
			client.NetworkReceivedPacket(data);
			RecycleBuffer(data.Array);
		}
	}

	internal void ReadServerMessages(FusionServer server)
	{
		while (_serverMessageQueue.Count > 0)
		{
			var (source, data) = _serverMessageQueue.Dequeue();
			server.NetworkReceivedPacket(source, data);
			RecycleBuffer(data.Array);
		}
	}

	internal void SendToServer(ArraySegment<byte> packet, bool reliable)
	{
		if (base.Server != null)
		{
			_serverMessageQueue.Enqueue((new FusionPeer(_runner.LocalPlayer, loopback: true), CopyForLoopback(packet)));
		}
		else
		{
			RpcHandler.SendToServer(this, packet, reliable);
		}
	}

	internal void SendToClient(FusionPeer dest, ArraySegment<byte> packet, bool reliable)
	{
		if (base.Client != null && dest.IsLoopback)
		{
			_clientMessageQueue.Enqueue(CopyForLoopback(packet));
		}
		else if (!RpcHandler.SendToClient(this, dest, packet, reliable))
		{
			base.Server?.ClientDisconnected(dest);
		}
	}

	private void RecycleBuffer(byte[] buffer)
	{
		if (buffer.Length == 1024)
		{
			_byteArrayPool.Enqueue(buffer);
		}
	}

	private ArraySegment<byte> CopyForLoopback(ArraySegment<byte> packet)
	{
		byte[] destination = ((_byteArrayPool.Count == 0) ? new byte[1024] : _byteArrayPool.Dequeue());
		return packet.CopyToSegment(destination);
	}

	internal void DeliverMessageToServer(byte[] data, PlayerRef source)
	{
		_serverMessageQueue.Enqueue((new FusionPeer(source, loopback: false), new ArraySegment<byte>(data)));
	}

	internal void DeliverMessageToClient(byte[] data)
	{
		_clientMessageQueue.Enqueue(new ArraySegment<byte>(data));
	}

	void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		base.Server?.ClientDisconnected(new FusionPeer(player, loopback: false));
	}

	void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
	{
	}

	void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
	}

	void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
	{
	}

	void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner)
	{
	}

	void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
	}

	void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
	{
	}

	void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
	{
	}

	void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
	{
	}
}
