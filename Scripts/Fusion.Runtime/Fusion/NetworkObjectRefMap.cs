#define DEBUG
using System;

namespace Fusion;

public class NetworkObjectRefMap<T>
{
	public struct Entry
	{
		public NetworkId Id;

		public T Value;

		public int Next;
	}

	private const int INVALID_ENTRY = 0;

	private const int ENTRY_START_INDEX = 1;

	private int[] _buckets;

	private Entry[] _entries;

	private int _free;

	private int _usedCount;

	private int _freeCount;

	public int Count => _usedCount - _freeCount - 1;

	public uint Capacity => (uint)_entries.Length;

	public NetworkObjectRefMap(uint capacity = 256u)
	{
		capacity = Primes.GetNextPrime(capacity);
		_free = 0;
		_usedCount = 1;
		_freeCount = 0;
		_buckets = new int[capacity];
		_entries = new Entry[capacity];
	}

	public void Clear()
	{
		_free = 0;
		_usedCount = 1;
		_freeCount = 0;
		Array.Clear(_buckets, 0, _buckets.Length);
		Array.Clear(_entries, 0, _entries.Length);
	}

	public void GetIterateBufferStartCount(out Entry[] entries, out int start, out int count)
	{
		entries = _entries;
		start = 1;
		count = _usedCount;
	}

	public bool Contains(NetworkId id)
	{
		return Find(id) != 0;
	}

	public bool Remove(NetworkId id)
	{
		uint num = id.Raw % Capacity;
		int num2 = _buckets[num];
		int num3 = 0;
		while (num2 != 0)
		{
			if (_entries[num2].Id == id)
			{
				if (num3 == 0)
				{
					_buckets[num] = _entries[num2].Next;
				}
				else
				{
					_entries[num3].Next = _entries[num2].Next;
				}
				_entries[num2].Id = default;
				_entries[num2].Value = default;
				_entries[num2].Next = _free;
				_free = num2;
				_freeCount++;
				return true;
			}
			num3 = num2;
			num2 = _entries[num2].Next;
		}
		return false;
	}

	public bool TryGet(NetworkId id, out T value)
	{
		int num = Find(id);
		if (num == 0)
		{
			value = default;
			return false;
		}
		value = _entries[num].Value;
		return true;
	}

	public void Add(NetworkId id, T value)
	{
		if (Find(id) != 0)
		{
			throw new InvalidOperationException();
		}
		Insert(id, value);
	}

	public void AddOrUpdate(NetworkId id, T value)
	{
		int num = Find(id);
		if (num == 0)
		{
			Insert(id, value);
		}
		else
		{
			_entries[num].Value = value;
		}
	}

	private void Insert(NetworkId id, T value)
	{
		int num;
		if (_free != 0)
		{
			Assert.Check(_freeCount > 0);
			num = _free;
			_free = _entries[num].Next;
			_freeCount--;
		}
		else
		{
			if (_usedCount == _entries.Length)
			{
				Expand();
			}
			Assert.Check(_usedCount < _entries.Length);
			num = _usedCount++;
		}
		uint num2 = id.Raw % Capacity;
		_entries[num].Next = _buckets[num2];
		_entries[num].Id = id;
		_entries[num].Value = value;
		_buckets[num2] = num;
	}

	private void Expand()
	{
		uint capacity = Capacity;
		Entry[] entries = _entries;
		uint nextPrime = Primes.GetNextPrime(capacity);
		Entry[] array = new Entry[nextPrime];
		int[] array2 = new int[nextPrime];
		Array.Copy(entries, 0, array, 0, entries.Length);
		for (int i = 1; i < _usedCount; i++)
		{
			uint num = array[i].Id.Raw % nextPrime;
			array[i].Next = array2[num];
			array2[num] = i;
		}
		_buckets = array2;
		_entries = array;
	}

	private int Find(NetworkId id)
	{
		uint num = id.Raw % Capacity;
		for (int num2 = _buckets[num]; num2 != 0; num2 = _entries[num2].Next)
		{
			if (_entries[num2].Id == id)
			{
				return num2;
			}
		}
		return 0;
	}
}
