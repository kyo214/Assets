#define DEBUG
using System;
using System.Collections.Generic;

namespace Fusion;

internal class NetworkObjectPriorityHeap
{
	public struct Item
	{
		public float Priority;

		public NetworkId Value;
	}

	private int _count;

	private Item[] _heap;

	private HashSet<uint> _contains;

	public bool IsEmpty => _count == 1;

	public NetworkObjectPriorityHeap()
	{
		_heap = new Item[1024];
		_count = 1;
		_contains = new HashSet<uint>();
	}

	public bool Contains(NetworkId id)
	{
		return _contains.Contains(id.Raw);
	}

	public unsafe void BuildFromMap(NetworkObjectRefMapPtr* map)
	{
		Assert.Check(IsEmpty);
		NetworkObjectRefMapPtr.GetIterateBufferStartCount(map, out var entries, out var start, out var count);
		if (count > _heap.Length)
		{
			int num;
			for (num = _heap.Length * 2; num < count; num *= 2)
			{
			}
			_heap = new Item[num];
		}
		for (int i = start; i < count; i++)
		{
			if ((bool)entries[i].Id)
			{
				_contains.Add(entries[i].Id.Raw);
				_heap[_count].Priority = 1f;
				_heap[_count].Value = entries[i].Id;
				_count++;
			}
		}
	}

	public void Clear()
	{
		_count = 1;
		Array.Clear(_heap, 0, _heap.Length);
	}

	public void IncreasePriorities()
	{
		for (int i = 1; i < _count; i++)
		{
			_heap[i].Priority *= 2f;
		}
	}

	public void PushIfNotContains(NetworkId value, float priority)
	{
		if (!_contains.Contains(value.Raw))
		{
			Push(value, priority);
		}
	}

	public void Push(NetworkId value, float priority)
	{
		if (_count == _heap.Length)
		{
			ExpandHeap();
		}
		Assert.Always(_contains.Add(value.Raw), "network id already exists in priority heap");
		Item item = default;
		item.Priority = priority;
		item.Value = value;
		int num = _count;
		_heap[num] = item;
		while (num != 1)
		{
			int num2 = num / 2;
			if (_heap[num2].Priority < item.Priority)
			{
				_heap[num] = _heap[num2];
				_heap[num2] = item;
				num = num2;
				continue;
			}
			break;
		}
		_count++;
	}

	public Item Peek()
	{
		Assert.Check(_count > 1);
		return _heap[1];
	}

	public float PeekPriority()
	{
		Assert.Check(_count > 1);
		return _heap[1].Priority;
	}

	public NetworkId PeekValue()
	{
		Assert.Check(_count > 1);
		return _heap[1].Value;
	}

	public bool TryPop(out Item item)
	{
		if (_count > 1)
		{
			item = Pop();
			return true;
		}
		item = default;
		return false;
	}

	public Item Pop()
	{
		Assert.Check(_count > 1);
		_count--;
		Item result = _heap[1];
		_heap[1] = _heap[_count];
		int num = 1;
		int num2 = 1;
		do
		{
			num2 = num;
			if (2 * num2 + 1 <= _count)
			{
				if (_heap[num2].Priority <= _heap[2 * num2].Priority)
				{
					num = 2 * num2;
				}
				if (_heap[num].Priority <= _heap[2 * num2 + 1].Priority)
				{
					num = 2 * num2 + 1;
				}
			}
			else if (2 * num2 <= _count && _heap[num2].Priority <= _heap[2 * num2].Priority)
			{
				num = 2 * num2;
			}
			if (num2 != num)
			{
				Item item = _heap[num2];
				_heap[num2] = _heap[num];
				_heap[num] = item;
			}
		}
		while (num2 != num);
		Assert.Always(_contains.Remove(result.Value.Raw), "networkid wasn't in contains set");
		return result;
	}

	public NetworkId PopValue()
	{
		return Pop().Value;
	}

	private void ExpandHeap()
	{
		Item[] array = new Item[_heap.Length * 2];
		Array.Copy(_heap, array, _heap.Length);
		_heap = array;
	}
}
