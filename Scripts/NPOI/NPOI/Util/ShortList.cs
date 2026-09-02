using System;

namespace NPOI.Util;

public class ShortList
{
	private short[] _array;

	private int _limit;

	private static int _default_size = 128;

	public int Count => _limit;

	public ShortList()
		: this(_default_size)
	{
	}

	public ShortList(ShortList list)
		: this(list._array.Length)
	{
		Array.Copy(list._array, 0, _array, 0, _array.Length);
		_limit = list._limit;
	}

	public ShortList(int InitialCapacity)
	{
		_array = new short[InitialCapacity];
		_limit = 0;
	}

	public void Add(int index, short value)
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
			GrowArray(_limit * 2);
		}
		Array.Copy(_array, index, _array, index + 1, _limit - index);
		_array[index] = value;
		_limit++;
	}

	public bool Add(short value)
	{
		if (_limit == _array.Length)
		{
			GrowArray(_limit * 2);
		}
		_array[_limit++] = value;
		return true;
	}

	public bool AddAll(ShortList c)
	{
		if (c._limit != 0)
		{
			if (_limit + c._limit > _array.Length)
			{
				GrowArray(_limit + c._limit);
			}
			Array.Copy(c._array, 0, _array, _limit, c._limit);
			_limit += c._limit;
		}
		return true;
	}

	public bool AddAll(int index, ShortList c)
	{
		if (index > _limit)
		{
			throw new IndexOutOfRangeException();
		}
		if (c._limit != 0)
		{
			if (_limit + c._limit > _array.Length)
			{
				GrowArray(_limit + c._limit);
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

	public bool Contains(short o)
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

	public bool ContainsAll(ShortList c)
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
			ShortList shortList = (ShortList)o;
			if (shortList._limit == _limit)
			{
				flag = true;
				int num = 0;
				while (flag && num < _limit)
				{
					flag = _array[num] == shortList._array[num];
					num++;
				}
			}
		}
		return flag;
	}

	public short Get(int index)
	{
		if (index >= _limit)
		{
			throw new IndexOutOfRangeException();
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

	public int IndexOf(short o)
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

	public int LastIndexOf(short o)
	{
		int num = _limit - 1;
		while (num >= 0 && o != _array[num])
		{
			num--;
		}
		return num;
	}

	public short Remove(int index)
	{
		if (index >= _limit)
		{
			throw new IndexOutOfRangeException();
		}
		short result = _array[index];
		Array.Copy(_array, index + 1, _array, index, _limit - index);
		_limit--;
		return result;
	}

	public bool RemoveValue(short o)
	{
		bool flag = false;
		int num = 0;
		while (!flag && num < _limit)
		{
			if (o == _array[num])
			{
				Array.Copy(_array, num + 1, _array, num, _limit - num);
				_limit--;
				flag = true;
			}
			num++;
		}
		return flag;
	}

	public bool RemoveAll(ShortList c)
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

	public bool RetainAll(ShortList c)
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

	public short Set(int index, short element)
	{
		if (index >= _limit)
		{
			throw new IndexOutOfRangeException();
		}
		short result = _array[index];
		_array[index] = element;
		return result;
	}

	public int Size()
	{
		return _limit;
	}

	public short[] ToArray()
	{
		short[] array = new short[_limit];
		Array.Copy(_array, 0, array, 0, _limit);
		return array;
	}

	public short[] ToArray(short[] a)
	{
		if (a.Length == _limit)
		{
			Array.Copy(_array, 0, a, 0, _limit);
			return a;
		}
		return ToArray();
	}

	private void GrowArray(int new_size)
	{
		short[] array = new short[(new_size == _array.Length) ? (new_size + 1) : new_size];
		Array.Copy(_array, 0, array, 0, _limit);
		_array = array;
	}
}
