using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dissonance.Integrations.PhotonFusion.Demo;

public class FusionDemoControl : MonoBehaviour, INetworkRunnerCallbacks
{
	[SerializeField]
	private NetworkPrefabRef _playerPrefab;

	private readonly Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

	[SerializeField]
	private GameObject _world;

	private NetworkRunner _runner;

	public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		if (runner.IsServer)
		{
			NetworkObject value = runner.Spawn(position: new Vector3(player.RawEncoded % runner.Config.Simulation.DefaultPlayers * 3, 1f, 0f), prefabRef: _playerPrefab, rotation: Quaternion.identity, inputAuthority: player);
			_spawnedCharacters.Add(player, value);
		}
	}

	public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		if (_spawnedCharacters.TryGetValue(player, out var value))
		{
			runner.Despawn(value);
			_spawnedCharacters.Remove(player);
		}
	}

	public void OnInput(NetworkRunner runner, NetworkInput input)
	{
		NetworkInputData value = default;
		if (Input.GetKey(KeyCode.W))
		{
			value.direction += Vector3.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			value.direction += Vector3.back;
		}
		if (Input.GetKey(KeyCode.A))
		{
			value.direction += Vector3.left;
		}
		if (Input.GetKey(KeyCode.D))
		{
			value.direction += Vector3.right;
		}
		input.Set(value);
	}

	public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
	{
	}

	public void OnConnectedToServer(NetworkRunner runner)
	{
	}

	public void OnDisconnectedFromServer(NetworkRunner runner)
	{
	}

	public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
	{
	}

	public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
	{
	}

	public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
	{
	}

	public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
	{
	}

	public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
	{
	}

	public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
	{
	}

	public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
	{
	}

	public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
	{
	}

	public void OnSceneLoadStart(NetworkRunner runner)
	{
	}

	public void OnSceneLoadDone(NetworkRunner runner)
	{
		_world.SetActive(value: true);
	}

	private async void StartGame(GameMode mode)
	{
		_runner = base.gameObject.AddComponent<NetworkRunner>();
		_runner.ProvideInput = true;
		await _runner.StartGame(new StartGameArgs
		{
			GameMode = mode,
			SessionName = "TestRoom",
			Scene = SceneManager.GetActiveScene().buildIndex,
			SceneManager = base.gameObject.AddComponent<NetworkSceneManagerDefault>()
		});
	}

	private void OnGUI()
	{
		if (_runner == null)
		{
			if (GUI.Button(new Rect(50f, 50f, 200f, 40f), "Host"))
			{
				StartGame(GameMode.Host);
			}
			if (GUI.Button(new Rect(50f, 110f, 200f, 40f), "Join"))
			{
				StartGame(GameMode.Client);
			}
		}
	}
}
