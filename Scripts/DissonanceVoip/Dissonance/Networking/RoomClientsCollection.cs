using System.Collections.Generic;
using Dissonance.Datastructures;
using JetBrains.Annotations;

namespace Dissonance.Networking;

internal class RoomClientsCollection<T>
{
	private class ClientIdComparer : IComparer<ClientInfo<T>>
	{
		public int Compare(ClientInfo<T> x, ClientInfo<T> y)
		{
			if (x == null)
			{
				if (y == null)
				{
					return 0;
				}
				return -1;
			}
			if (y == null)
			{
				return 1;
			}
			return x.PlayerId.CompareTo(y.PlayerId);
		}
	}

	private static readonly IComparer<ClientInfo<T>> ClientComparer = new ClientIdComparer();

	private readonly Dictionary<string, List<ClientInfo<T>>> _clientByRoomName = new Dictionary<string, List<ClientInfo<T>>>();

	private readonly Dictionary<ushort, List<string>> _roomNamesByHash = new Dictionary<ushort, List<string>>();

	private readonly Pool<List<string>> _listStringPool = new Pool<List<string>>(16, () => new List<string>());

	private void AddToHashCache(string name)
	{
		ushort key = new RoomName(name, suppress: true).ToRoomId();
		if (!_roomNamesByHash.TryGetValue(key, out var value))
		{
			value = _listStringPool.Get();
			_roomNamesByHash.Add(key, value);
			value.Clear();
			value.Add(name);
		}
		else if (!value.Contains(name))
		{
			value.Add(name);
		}
	}

	private void RemoveFromHashCache(string name)
	{
		ushort key = new RoomName(name, suppress: true).ToRoomId();
		if (_roomNamesByHash.TryGetValue(key, out var value))
		{
			value.Remove(name);
			if (value.Count == 0)
			{
				_roomNamesByHash.Remove(key);
				_listStringPool.Put(value);
			}
		}
	}

	public void Add(string room, [NotNull] ClientInfo<T> client)
	{
		AddToHashCache(room);
		if (!_clientByRoomName.TryGetValue(room, out var value))
		{
			value = new List<ClientInfo<T>>();
			_clientByRoomName.Add(room, value);
		}
		int num = value.BinarySearch(client, ClientComparer);
		if (num < 0)
		{
			value.Insert(~num, client);
		}
	}

	public bool Remove(string room, [NotNull] ClientInfo<T> client)
	{
		if (!_clientByRoomName.TryGetValue(room, out var value))
		{
			return false;
		}
		int num = value.BinarySearch(client, ClientComparer);
		if (num < 0)
		{
			return false;
		}
		value.RemoveAt(num);
		if (value.Count == 0)
		{
			RemoveFromHashCache(room);
		}
		return true;
	}

	public void Clear()
	{
		_clientByRoomName.Clear();
	}

	public bool TryGetClientsInRoom(string room, [CanBeNull] List<ClientInfo<T>> output)
	{
		if (_clientByRoomName.TryGetValue(room, out var value))
		{
			output?.AddRange(value);
			return true;
		}
		return false;
	}

	public bool TryGetClientsInRoom(ushort roomId, [CanBeNull] List<ClientInfo<T>> output)
	{
		if (!_roomNamesByHash.TryGetValue(roomId, out var value))
		{
			return false;
		}
		for (int i = 0; i < value.Count; i++)
		{
			if (_clientByRoomName.TryGetValue(value[i], out var value2))
			{
				output?.AddRange(value2);
			}
		}
		return true;
	}

	public int ClientCount()
	{
		int num = 0;
		foreach (KeyValuePair<string, List<ClientInfo<T>>> item in _clientByRoomName)
		{
			num += item.Value.Count;
		}
		return num;
	}
}
