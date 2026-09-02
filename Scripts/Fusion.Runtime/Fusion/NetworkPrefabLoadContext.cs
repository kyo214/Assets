using System;
using UnityEngine;

namespace Fusion;

public readonly struct NetworkPrefabLoadContext
{
	public const int FLAGS_RESERVED_BITS = 8;

	public const int FLAGS_RESERVED = 255;

	public const int FLAGS_PREFER_ASYNC = 1;

	public readonly int Flags;

	public readonly NetworkPrefabId Id;

	public readonly INetworkPrefabSource Prefab;

	private readonly NetworkPrefabTable.PrefabEntry Entry;

	internal NetworkPrefabLoadContext(NetworkPrefabTable.PrefabEntry entry, int flags, INetworkPrefabSource prefab, NetworkPrefabId id)
	{
		Flags = flags;
		Id = id;
		Prefab = prefab;
		Entry = entry;
	}

	public bool HasFlag(int flag)
	{
		return (Flags & flag) == flag;
	}

	public void Loaded(GameObject prefab)
	{
		if (prefab == null)
		{
			Entry.LoadFinished(this, (NetworkObject)null);
			return;
		}
		NetworkObject component = prefab.GetComponent<NetworkObject>();
		if (BehaviourUtils.IsAlive(component))
		{
			Entry.LoadFinished(this, component);
		}
		else
		{
			Entry.LoadFinished(this, new InvalidOperationException(string.Format("Prefab {0} does not have {1} component.", prefab, "NetworkObject")));
		}
	}

	public void Loaded(NetworkObject prefab)
	{
		Entry.LoadFinished(this, prefab);
	}

	public void Error(Exception error)
	{
		Entry.LoadFinished(this, error);
	}
}
