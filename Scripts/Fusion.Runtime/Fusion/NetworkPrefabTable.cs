#define DEBUG
using System;
using System.Collections.Generic;

namespace Fusion;

public class NetworkPrefabTable
{
	internal class PrefabEntry
	{
		public INetworkPrefabSource Prefab;

		public object State;

		internal void LoadFinished(in NetworkPrefabLoadContext context, NetworkObject prefab)
		{
			if (BehaviourUtils.IsNotAlive(prefab))
			{
				LoadFinished(in context, new InvalidOperationException("Load returned null"));
			}
			else
			{
				State = prefab;
			}
		}

		internal void LoadFinished(in NetworkPrefabLoadContext context, Exception error)
		{
			if (error == null)
			{
				error = new InvalidOperationException("Unknown");
			}
			Log.Error($"Error loading {context.Id}: {error}");
			State = error;
		}
	}

	private Dictionary<NetworkObjectGuid, NetworkPrefabId> _guidToId = new Dictionary<NetworkObjectGuid, NetworkPrefabId>();

	private Dictionary<NetworkPrefabId, PrefabEntry> _typeToPrefab = new Dictionary<NetworkPrefabId, PrefabEntry>();

	private NetworkPrefabId _lastId;

	private object _beingLoadedState = new object();

	public int Count => _guidToId.Count;

	public NetworkPrefabId LastId => _lastId;

	public IEnumerable<(NetworkPrefabId, INetworkPrefabSource)> GetEntries()
	{
		foreach (KeyValuePair<NetworkPrefabId, PrefabEntry> kv in _typeToPrefab)
		{
			yield return (kv.Key, kv.Value.Prefab);
		}
	}

	internal bool TryGetPrefabEntry(NetworkObjectGuid guid, out INetworkPrefabSource prefab)
	{
		if (TryGetId(guid, out var id) && _typeToPrefab.TryGetValue(id, out var value))
		{
			prefab = value.Prefab;
			return true;
		}
		prefab = null;
		return false;
	}

	public bool TryGetPrefab(NetworkPrefabId typeId, out NetworkObject obj)
	{
		if (!_typeToPrefab.TryGetValue(typeId, out var value))
		{
			obj = null;
			return false;
		}
		if (value.State == null)
		{
			NetworkPrefabLoadContext context = new NetworkPrefabLoadContext(value, 0, value.Prefab, typeId);
			value.State = _beingLoadedState;
			try
			{
				value.Prefab.Load(in context);
			}
			catch (Exception error)
			{
				value.LoadFinished(in context, error);
			}
		}
		if (value.State is NetworkObject networkObject)
		{
			obj = networkObject;
			return true;
		}
		obj = null;
		return false;
	}

	internal void Clear()
	{
		UnloadAll();
		_guidToId.Clear();
		_typeToPrefab.Clear();
		_lastId = default;
	}

	public bool Unload(NetworkPrefabId id)
	{
		if (_typeToPrefab.TryGetValue(id, out var value))
		{
			return UnloadEntry(value);
		}
		return false;
	}

	public void UnloadAll()
	{
		foreach (PrefabEntry value in _typeToPrefab.Values)
		{
			UnloadEntry(value);
		}
	}

	public bool TryGetId(NetworkObjectGuid guid, out NetworkPrefabId id)
	{
		if (_guidToId.TryGetValue(guid, out id))
		{
			Assert.Check(id.IsValid);
			return true;
		}
		id = default;
		return false;
	}

	public bool TryAdd(NetworkObjectGuid guid, INetworkPrefabSource source, out NetworkPrefabId id)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (!guid.IsValid)
		{
			throw new ArgumentException($"Prefab's guid is not valid: {guid}", "source");
		}
		if (_guidToId.TryGetValue(guid, out id))
		{
			return false;
		}
		id = AddInternal(guid, source);
		return true;
	}

	private NetworkPrefabId AddInternal(NetworkObjectGuid guid, INetworkPrefabSource prefab)
	{
		Assert.Check(prefab != null);
		NetworkPrefabId lastId = _lastId;
		lastId.Value++;
		_typeToPrefab.Add(lastId, new PrefabEntry
		{
			Prefab = prefab
		});
		_guidToId.Add(guid, lastId);
		_lastId = lastId;
		return lastId;
	}

	private static bool UnloadEntry(PrefabEntry entry)
	{
		if (entry.State == null)
		{
			return false;
		}
		entry.State = null;
		entry.Prefab.Unload();
		return true;
	}
}
