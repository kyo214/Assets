#define DEBUG
using System.Collections.Generic;

namespace Fusion;

internal class SimulationConnectionObjectData
{
	private struct NetworkObjectConnectionData
	{
		public Tick Tick;

		public NetworkObjectConnectionDataStatus Status;

		public unsafe int* Groups;
	}

	private enum NetworkObjectConnectionDataStatus
	{
		CreatedUnconfirmed = 0,
		CreatedConfirmed = 1,
		DestroyUnconfirmed = 2
	}

	private const string LogPrefix = "[ECData] ";

	private Dictionary<NetworkId, NetworkObjectConnectionData> _objects;

	private Queue<NetworkId> _destroyed;

	public int DestroyedCount => _destroyed.Count;

	public SimulationConnectionObjectData()
	{
		_objects = new Dictionary<NetworkId, NetworkObjectConnectionData>(new NetworkId.EqualityComparer());
		_destroyed = new Queue<NetworkId>();
	}

	public NetworkId DestroyedNextId()
	{
		return _destroyed.Dequeue();
	}

	public void Clear()
	{
		_objects.Clear();
		_destroyed.Clear();
	}

	public void SetCreateConfirmed(NetworkId id)
	{
		if (!TryUpdate(id, NetworkObjectConnectionDataStatus.CreatedConfirmed))
		{
		}
	}

	public void SetCreateConfirmed(NetworkId id, Tick tick)
	{
		if (!TryUpdate(id, NetworkObjectConnectionDataStatus.CreatedConfirmed, tick))
		{
		}
	}

	public void SetDestroyConfirmed(NetworkId id)
	{
		if (!_objects.Remove(id))
		{
		}
	}

	public void SetDestroyed(NetworkId id)
	{
		if (TryUpdate(id, NetworkObjectConnectionDataStatus.DestroyUnconfirmed))
		{
			_destroyed.Enqueue(id);
		}
	}

	public void SetSentTick(NetworkId id, Tick tick)
	{
		Tick? tick2 = tick;
		if (!TryUpdate(id, null, tick2))
		{
			Assert.Fail("SetSentTick failed, no such object", id, tick);
		}
	}

	public unsafe int* GetOrAllocGroups(NetworkId id, Allocator* allocator)
	{
		NetworkObjectConnectionData orCreateData = GetOrCreateData(id);
		if (orCreateData.Groups == null)
		{
			orCreateData.Groups = Allocator.AllocAndClearArray<int>(allocator, Maths.IntsRequiredForBits(NetworkBehaviourUtils.InterestGroupKeysMax));
			_objects[id] = orCreateData;
		}
		return orCreateData.Groups;
	}

	public unsafe int* GetGroups(NetworkId id)
	{
		if (TryGetData(id, out var data))
		{
			return data.Groups;
		}
		return null;
	}

	public void EnsureExist(NetworkId id)
	{
		if (!_objects.ContainsKey(id))
		{
			_objects.Add(id, default);
		}
	}

	public unsafe void EnsureExist(NetworkId id, out Tick sentTick, out bool isCreateUnconfirmed, out int* interestGroups)
	{
		if (!_objects.TryGetValue(id, out var value))
		{
			value = default;
			_objects.Add(id, value);
		}
		sentTick = value.Tick;
		isCreateUnconfirmed = value.Status == NetworkObjectConnectionDataStatus.CreatedUnconfirmed;
		interestGroups = value.Groups;
		if (!isCreateUnconfirmed)
		{
		}
	}

	public bool? IsCreateUnconfirmed(NetworkId id)
	{
		if (TryGetData(id, out var data))
		{
			return data.Status == NetworkObjectConnectionDataStatus.CreatedUnconfirmed;
		}
		return null;
	}

	public bool? IsDestroyUnconfirmed(NetworkId id)
	{
		if (TryGetData(id, out var data))
		{
			return data.Status == NetworkObjectConnectionDataStatus.DestroyUnconfirmed;
		}
		return null;
	}

	public Tick GetSentTick(NetworkId id)
	{
		NetworkObjectConnectionData data;
		return TryGetData(id, out data) ? data.Tick : default(Tick);
	}

	private bool TryGetData(NetworkId id, out NetworkObjectConnectionData data)
	{
		return _objects.TryGetValue(id, out data);
	}

	private NetworkObjectConnectionData GetOrCreateData(NetworkId id)
	{
		if (_objects.TryGetValue(id, out var value))
		{
			return value;
		}
		_objects.Add(id, default);
		return default;
	}

	private bool TryUpdate(NetworkId id, NetworkObjectConnectionDataStatus? status = null, Tick? tick = null)
	{
		if (_objects.TryGetValue(id, out var value))
		{
			if (status.HasValue && value.Status != status && value.Status <= status.Value)
			{
				value.Status = status.Value;
			}
			if (tick.HasValue)
			{
				value.Tick = tick.Value;
			}
			_objects[id] = value;
			return true;
		}
		return false;
	}
}
