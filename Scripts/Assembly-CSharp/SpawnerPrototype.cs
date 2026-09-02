using System.Collections.Generic;
using Fusion;
using UnityEngine;

[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
public class SpawnerPrototype<T> : SimulationBehaviour, IPlayerJoined, IPlayerLeft, ISpawned, ISceneLoadDone where T : Component, ISpawnPointPrototype
{
	public enum SpawnMethods
	{
		AutoOnNetworkStart = 0,
		ByScriptOnly = 1
	}

	public enum AuthorityOptions
	{
		Auto = 0,
		Server = 1,
		Player = 2
	}

	protected Dictionary<PlayerRef, List<NetworkObject>> _spawnedLookup = new Dictionary<PlayerRef, List<NetworkObject>>();

	[InlineHelp]
	public NetworkObject PlayerPrefab;

	[InlineHelp]
	public SpawnMethods SpawnMethod;

	[InlineHelp]
	[DrawIf("_AllowClientObjects", Hide = true)]
	[MultiPropertyDrawersFix]
	public AuthorityOptions StateAuthority;

	protected ISpawnPointManagerPrototype<T> spawnManager;

	protected bool _AllowClientObjects => (((bool)Runner && Runner.IsRunning) ? Runner.Config : NetworkProjectConfig.Global).Simulation.Topology == SimulationConfig.Topologies.Shared;

	protected virtual void Awake()
	{
		spawnManager = GetComponent<ISpawnPointManagerPrototype<T>>();
	}

	public void Spawned()
	{
		if (SpawnMethod == SpawnMethods.AutoOnNetworkStart && (bool)Object && _AllowClientObjects && StateAuthority != AuthorityOptions.Server)
		{
			NetworkObject playerObject = TrySpawn(Runner, Runner.LocalPlayer);
			RegisterPlayerAndObject(Runner.LocalPlayer, playerObject);
		}
	}

	public void SceneLoadDone()
	{
		if (SpawnMethod == SpawnMethods.AutoOnNetworkStart && !Object && _AllowClientObjects && StateAuthority != AuthorityOptions.Server)
		{
			NetworkObject playerObject = TrySpawn(Runner, Runner.LocalPlayer);
			RegisterPlayerAndObject(Runner.LocalPlayer, playerObject);
		}
	}

	public void PlayerJoined(PlayerRef player)
	{
		PlayerJoined(Runner, player);
	}

	public void PlayerLeft(PlayerRef player)
	{
		PlayerLeft(Runner, player);
	}

	private void PlayerJoined(NetworkRunner runner, PlayerRef player)
	{
		if (SpawnMethod == SpawnMethods.AutoOnNetworkStart && (!_AllowClientObjects || StateAuthority == AuthorityOptions.Server))
		{
			NetworkObject playerObject = TrySpawn(runner, player);
			RegisterPlayerAndObject(player, playerObject);
		}
	}

	private void PlayerLeft(NetworkRunner runner, PlayerRef player)
	{
		DespawnPlayersObjects(runner, player);
		UnregisterPlayer(player);
	}

	public NetworkObject TrySpawn(NetworkRunner runner, PlayerRef player)
	{
		if (!PlayerPrefab || !player.IsValid)
		{
			return null;
		}
		Transform transform = ((spawnManager != null) ? spawnManager.GetNextSpawnPoint(runner, player) : null);
		if (transform == null)
		{
			transform = base.transform;
		}
		Vector3 position = transform.position;
		Quaternion rotation = transform.rotation;
		return runner.Spawn(PlayerPrefab, position, rotation, player);
	}

	[BehaviourButtonAction("Spawn For All Players On Server", true, false, null)]
	public void TrySpawnAll()
	{
		List<NetworkRunner>.Enumerator instancesEnumerator = NetworkRunner.GetInstancesEnumerator();
		while (instancesEnumerator.MoveNext())
		{
			NetworkRunner current = instancesEnumerator.Current;
			if (!current.IsRunning || !current.IsServer)
			{
				continue;
			}
			foreach (PlayerRef activePlayer in current.ActivePlayers)
			{
				NetworkObject playerObject = TrySpawn(current, activePlayer);
				RegisterPlayerAndObject(activePlayer, playerObject);
			}
		}
	}

	protected virtual void RegisterPlayerAndObject(PlayerRef player, NetworkObject playerObject)
	{
		if (!_spawnedLookup.TryGetValue(player, out var value))
		{
			value = new List<NetworkObject>();
			_spawnedLookup.Add(player, value);
		}
		if ((bool)playerObject)
		{
			value.Add(playerObject);
		}
		Runner.SetPlayerAlwaysInterested(player, playerObject, alwaysInterested: true);
	}

	protected void DespawnPlayersObjects(NetworkRunner runner, PlayerRef player)
	{
		if (!_spawnedLookup.ContainsKey(player))
		{
			return;
		}
		List<NetworkObject> list = _spawnedLookup[player];
		if (list.Count > 0)
		{
			foreach (NetworkObject item in list)
			{
				runner.Despawn(item);
			}
		}
		UnregisterPlayer(player);
	}

	protected void UnregisterPlayer(PlayerRef player)
	{
		if (_spawnedLookup.ContainsKey(player))
		{
			_spawnedLookup.Remove(player);
		}
	}
}
