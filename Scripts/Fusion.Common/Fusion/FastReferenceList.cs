#define DEBUG
using System;

namespace Fusion;

internal class FastReferenceList<T> where T : class
{
	public int Count;

	public T[] Items;

	public FastReferenceList(int capacity = 4)
	{
		Assert.Check(capacity > 0);
		Items = new T[capacity];
		Count = 0;
	}

	public void Add(T item)
	{
		if (Count == Items.Length)
		{
			Array.Resize(ref Items, Items.Length * 2);
		}
		Items[Count++] = item;
	}

	public bool Contains(T item)
	{
		for (int i = 0; i < Count; i++)
		{
			if (item == Items[i])
			{
				return true;
			}
		}
		return false;
	}

	public bool RemoveUnordered(T item)
	{
		for (int i = 0; i < Count; i++)
		{
			if (item == Items[i])
			{
				Items[i] = null;
				Count--;
				if (i < Count)
				{
					Items[i] = Items[Count];
				}
				return true;
			}
		}
		return false;
	}
}
