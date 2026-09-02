using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
public abstract class SpawnPointManagerPrototype<T> : Fusion.Behaviour, ISpawnPointManagerPrototype<T> where T : Component, ISpawnPointPrototype
{
	public enum SpawnSequence
	{
		PlayerId = 0,
		RoundRobin = 1,
		Random = 2
	}

	[InlineHelp]
	public SpawnSequence Sequence;

	[InlineHelp]
	public LayerMask BlockingLayers;

	[InlineHelp]
	public float BlockedCheckRadius = 2f;

	[NonSerialized]
	internal List<Component> _spawnPoints = new List<Component>();

	[NonSerialized]
	public int LastSpawnIndex = -1;

	private NetworkRNG rng;

	protected static Collider[] blocked3D;

	private void Awake()
	{
		rng = new NetworkRNG(0);
	}

	public void CollectSpawnPoints(NetworkRunner runner)
	{
		_spawnPoints.Clear();
		_spawnPoints.AddRange(runner.SimulationUnityScene.FindObjectsOfTypeInOrder<T, Component>());
	}

	public virtual Transform GetNextSpawnPoint(NetworkRunner runner, PlayerRef player, bool skipIfBlocked = true)
	{
		CollectSpawnPoints(runner);
		int count = _spawnPoints.Count;
		if (_spawnPoints == null || count == 0)
		{
			return null;
		}
		int num;
		Component component;
		if (Sequence == SpawnSequence.PlayerId)
		{
			num = (int)player % count;
			component = _spawnPoints[num];
		}
		else if (Sequence == SpawnSequence.RoundRobin)
		{
			num = (LastSpawnIndex + 1) % count;
			component = _spawnPoints[num];
		}
		else
		{
			num = rng.RangeInclusive(0, count);
			component = _spawnPoints[num];
		}
		if (skipIfBlocked && BlockingLayers.value != 0 && IsBlocked(component))
		{
			(int, Component) nextUnblocked = GetNextUnblocked(num);
			if (nextUnblocked.Item1 > -1)
			{
				(LastSpawnIndex, _) = nextUnblocked;
				return nextUnblocked.Item2.transform;
			}
			component = nextUnblocked.Item2;
			return AllSpawnPointsBlockedFallback();
		}
		LastSpawnIndex = num;
		return component.transform;
	}

	public virtual Transform AllSpawnPointsBlockedFallback()
	{
		return base.transform;
	}

	public virtual (int, Component) GetNextUnblocked(int failedIndex)
	{
		int i = 1;
		for (int count = _spawnPoints.Count; i < count; i++)
		{
			Component component = _spawnPoints[i % count];
			if (!IsBlocked(component))
			{
				return (i, component);
			}
		}
		return (-1, null);
	}

	public virtual bool IsBlocked(Component spawnPoint)
	{
		PhysicsScene physicsScene = spawnPoint.gameObject.scene.GetPhysicsScene();
		if (blocked3D == null)
		{
			blocked3D = new Collider[1];
		}
		int num = physicsScene.OverlapSphere(spawnPoint.transform.position, BlockedCheckRadius, blocked3D, BlockingLayers.value, QueryTriggerInteraction.UseGlobal);
		if (num > 0)
		{
			Debug.LogWarning(blocked3D[0].name + " is blocking " + spawnPoint.name);
		}
		return num > 0;
	}
}
