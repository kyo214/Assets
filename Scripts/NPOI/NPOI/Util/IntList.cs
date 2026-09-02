using System;

namespace NPOI.Util;

public class IntList
{
	private int[] _array;

	private int _limit;

	private int fillval;

	private static int _default_size = 128;

	public int Count => _limit;

	public IntList()
		: this(_default_size)
	{
	}

	public IntList(int InitialCapacity)
		: this(InitialCapacity, 0)
	{
	}

	public IntList(IntList list)
		: this(list._array.Length)
	{
		Array.Copy(list._array, 0, _array, 0, _array.Length);
		_limit = list._limit;
	}

	public IntList(int initialCapacity, int fillvalue)
	{
		_array = new int[initialCapacity];
		if (fillval != 0)
		{
			fillval = fillvalue;
			FillArray(fillval, _array, 0);
		}
		_limit = 0;
	}

	private void FillArray(int val, int[] array, int index)
	{
		for (int i = index; i < array.Length; i++)
		{
			array[i] = val;
		}
	}

	public void Add(int index, int value)
	{
		if (index > _limit)
		{
			throw new IndexOutOfRangeException();
		}
		if (index == _limit)
		{
			Add(value);
			return;
		}
		if (_limit == _array.Length)
		{
			growArray(_limit * 2);
		}
		Array.Copy(_array, index, _array, index + 1, _limit - index);
		_array[index] = value;
		_limit++;
	}

	public bool Add(int value)
	{
		if (_limit == _array.Length)
		{
			growArray(_limit * 2);
		}
		_array[_limit++] = value;
		return true;
	}

	public bool AddAll(IntList c)
	{
		if (c._limit != 0)
		{
			if (_limit + c._limit > _array.Length)
			{
				growArray(_limit + c._limit);
			}
			Array.Copy(c._array, 0, _array, _limit, c._limit);
			_limit += c._limit;
		}
		return true;
	}

	public bool AddAll(int index, IntList c)
	{
		if (index > _limit)
		{
			throw new IndexOutOfRangeException();
		}
		if (c._limit != 0)
		{
			if (_limit + c._limit > _array.Length)
			{
				growArray(_limit + c._limit);
			}
			Array.Copy(_array, index, _array, index + c._limit, _limit - index);
			Array.Copy(c._array, 0, _array, index, c._limit);
			_limit += c._limit;
		}
		return true;
	}

	public void Clear()
	{
		_limit = 0;
	}

	public bool Contains(int o)
	{
		bool flag = false;
		int num = 0;
		while (!flag && num < _limit)
		{
			if (_array[num] == o)
			{
				flag = true;
			}
			num++;
		}
		return flag;
	}

	public bool ContainsAll(IntList c)
	{
		bool flag = true;
		if (this != c)
		{
			int num = 0;
			while (flag && num < c._limit)
			{
				if (!Contains(c._array[num]))
				{
					flag = false;
				}
				num++;
			}
		}
		return flag;
	}

	public override bool Equals(object o)
	{
		bool flag = this == o;
		if (!flag && o != null && o.GetType() == GetType())
		{
			IntList intList = (IntList)o;
			if (intList._limit == _limit)
			{
				flag = true;
				int num = 0;
				while (flag && num < _limit)
				{
					flag = _array[num] == intList._array[num];
					num++;
				}
			}
		}
		return flag;
	}

	public int Get(int index)
	{
		if (index >= _limit)
		{
			throw new IndexOutOfRangeException(index + " not accessible in a list of length " + _limit);
		}
		return _array[index];
	}

	public override int GetHashCode()
	{
		int num = 0;
		for (int i = 0; i < _limit; i++)
		{
			num = 31 * num + _array[i];
		}
		return num;
	}

	public int IndexOf(int o)
	{
		int i;
		for (i = 0; i < _limit && o != _array[i]; i++)
		{
		}
		if (i == _limit)
		{
			i = -1;
		}
		return i;
	}

	public bool IsEmpty()
	{
		return _limit == 0;
	}

	public int LastIndexOf(int o)
	{
		int num = _limit - 1;
		while (num >= 0 && o != _array[num])
		{
			num--;
		}
		return num;
	}

	public int Remove(int index)
	{
		if (index >= _limit)
		{
			throw new IndexOutOfRangeException();
		}
		int result = _array[index];
		Array.Copy(_array, index + 1, _array, index, _limit - index);
		_limit--;
		return result;
	}

	public bool RemoveValue(int o)
	{
		bool flag = false;
		int num = 0;
		while (!flag && num < _limit)
		{
			if (o == _array[num])
			{
				if (num + 1 < _limit)
				{
					Array.Copy(_array, num + 1, _array, num, _limit - num);
				}
				_limit--;
				flag = true;
			}
			num++;
		}
		return flag;
	}

	public bool RemoveAll(IntList c)
	{
		bool result = false;
		for (int i = 0; i < c._limit; i++)
		{
			if (RemoveValue(c._array[i]))
			{
				result = true;
			}
		}
		return result;
	}

	public bool RetainAll(IntList c)
	{
		bool result = false;
		int num = 0;
		while (num < _limit)
		{
			if (!c.Contains(_array[num]))
			{
				Remove(num);
				result = true;
			}
			else
			{
				num++;
			}
		}
		return result;
	}

	public int Set(int index, int element)
	{
		if (index >= _limit)
		{
			throw new IndexOutOfRangeException();
		}
		int result = _array[index];
		_array[index] = element;
		return result;
	}

	public int Size()
	{
		return _limit;
	}

	public int[] ToArray()
	{
		int[] array = new int[_limit];
		Array.Copy(_array, 0, array, 0, _limit);
		return array;
	}

	public int[] ToArray(int[] a)
	{
		if (a.Length == _limit)
		{
			Array.Copy(_array, 0, a, 0, _limit);
			return a;
		}
		return ToArray();
	}

	private void growArray(int new_size)
	{
		int[] array = new int[(new_size == _array.Length) ? (new_size + 1) : new_size];
		if (fillval != 0)
		{
			FillArray(fillval, array, _array.Length);
		}
		Array.Copy(_array, 0, array, 0, _limit);
		_array = array;
	}
}
