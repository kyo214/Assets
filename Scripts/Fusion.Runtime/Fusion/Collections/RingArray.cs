using System;
using System.Collections;
using System.Collections.Generic;

namespace Fusion.Collections;

public class RingArray<T> : IEnumerable<T>, IEnumerable where T : struct
{
	public struct _RangeIterator : IEnumerable<T>, IEnumerable
	{
		private RingArray<T> ra;

		public IEnumerator<T> GetEnumerator()
		{
			int head = ra._head;
			int tail = ra._tail;
			T[] array = ra._array;
			if (head == tail)
			{
				yield return array[head];
				yield break;
			}
			int modmask = ra.modMask;
			int increment = 1;
			int i = head;
			int end = tail + 1;
			if (end < i)
			{
				end += ra.Length;
			}
			for (; i != end; i += increment)
			{
				yield return array[modmask & i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public _RangeIterator(RingArray<T> ringArray)
		{
			ra = ringArray;
		}
	}

	public struct _ReverseRangeIterator : IEnumerable<T>, IEnumerable
	{
		private RingArray<T> ra;

		public IEnumerator<T> GetEnumerator()
		{
			int head = ra._head;
			int tail = ra._tail;
			T[] array = ra._array;
			if (head == tail)
			{
				yield return array[head];
				yield break;
			}
			int modmask = ra.modMask;
			int increment = -1;
			int i = tail;
			int end = head - 1;
			if (i < end)
			{
				i += ra.Length;
			}
			for (; i != end; i += increment)
			{
				yield return array[modmask & i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public _ReverseRangeIterator(RingArray<T> ringArray)
		{
			ra = ringArray;
		}
	}

	public readonly T[] _array;

	public readonly int modMask;

	private int _head;

	private int _tail;

	private int _current;

	public IEnumerator<T> OffsetEnumerator;

	public int Length { get; private set; }

	public int Head
	{
		get
		{
			return _head;
		}
		set
		{
			_head = modMask & value;
		}
	}

	public int Tail
	{
		get
		{
			return _tail;
		}
		set
		{
			_tail = modMask & value;
		}
	}

	public int Current
	{
		get
		{
			return _current;
		}
		set
		{
			_current = modMask & value;
		}
	}

	public _RangeIterator RangeIterator => new _RangeIterator(this);

	public _ReverseRangeIterator ReverseRangeIterator => new _ReverseRangeIterator(this);

	public ref T this[int frameId]
	{
		get
		{
			frameId &= modMask;
			return ref _array[frameId];
		}
	}

	public RingArray(int countBits)
	{
		if (countBits > 8)
		{
			Log.Error("RingArray will not accept countBits > 8. Length is 2^countBits. You may have entered the intended size, rather than the power value. Using value of 8 as fallback.");
			countBits = 8;
		}
		int num = (Length = 1 << countBits);
		modMask = num - 1;
		Head = 0;
		Tail = num - 1;
		_array = new T[num];
	}

	public void Clear()
	{
		Array.Clear(_array, 0, _array.Length);
	}

	public IEnumerator<T> GetEnumerator()
	{
		_ = Current;
		_ = Current + Length;
		int i = Current;
		while (i < Length + Current)
		{
			yield return _array[i];
			int num = i + 1;
			i = num;
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
