using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGStoreField<T> : BGArrayStore<T>, BGStoreFieldI<T>
{
	public T this[int index]
	{
		get
		{
			if (index >= base.Count)
			{
				throw new Exception("Index is out of bounds, greater or equal to maxIndex, " + index + ">=" + base.Count);
			}
			return Items[index];
		}
		set
		{
			if (index >= base.Count)
			{
				throw new Exception("Index is out of bounds, greater or equal to maxIndex, " + index + ">=" + base.Count);
			}
			Items[index] = value;
		}
	}

	public void ForEachKey(Action<int> action)
	{
		int count = base.Count;
		T y = default;
		EqualityComparer<T> equalityComparer = EqualityComparer<T>.Default;
		for (int i = 0; i < count; i++)
		{
			T x = Items[i];
			if (!equalityComparer.Equals(x, y))
			{
				action(i);
			}
		}
	}

	public void ForEachKeyValue(Action<int, T> action)
	{
		int count = base.Count;
		for (int i = 0; i < count; i++)
		{
			action(i, Items[i]);
		}
	}

	public T[] CopyRawValues()
	{
		T[] array = new T[base.Count];
		Array.Copy(Items, array, base.Count);
		return array;
	}
}
