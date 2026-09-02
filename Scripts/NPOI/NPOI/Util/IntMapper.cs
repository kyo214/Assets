using System.Collections.Generic;

namespace NPOI.Util;

public class IntMapper<T>
{
	private List<T> elements;

	private Dictionary<T, int> valueKeyMap;

	private static int _default_size = 10;

	public int Size => elements.Count;

	public T this[int index] => elements[index];

	public IntMapper()
		: this(_default_size)
	{
	}

	public IntMapper(int InitialCapacity)
	{
		elements = new List<T>(InitialCapacity);
		valueKeyMap = new Dictionary<T, int>(InitialCapacity);
	}

	public bool Add(T value)
	{
		int count = elements.Count;
		elements.Add(value);
		if (valueKeyMap.ContainsKey(value))
		{
			valueKeyMap[value] = count;
		}
		else
		{
			valueKeyMap.Add(value, count);
		}
		return true;
	}

	public int GetIndex(T o)
	{
		if (!valueKeyMap.ContainsKey(o))
		{
			return -1;
		}
		return valueKeyMap[o];
	}

	public IEnumerator<T> GetEnumerator()
	{
		return elements.GetEnumerator();
	}

	public void Clear()
	{
		elements.Clear();
		valueKeyMap.Clear();
	}
}
