using System;

namespace BansheeGz.BGDatabase;

public class BGArrayStore<T>
{
	protected T[] Items = Array.Empty<T>();

	public int Count { get; private set; }

	public int MinSize
	{
		set
		{
			if (Count < value)
			{
				MinCapacity = value;
				Count = value;
			}
		}
	}

	public int MinCapacity
	{
		set
		{
			if (Items.Length < value)
			{
				int num = ((Items.Length == 0) ? 4 : (Items.Length * 2));
				if (num < value)
				{
					num = value;
				}
				T[] array = new T[num];
				if (Count > 0)
				{
					Array.Copy(Items, 0, array, 0, Count);
				}
				Items = array;
			}
		}
	}

	public T Get(int index)
	{
		if (index >= Count)
		{
			throw new IndexOutOfRangeException("Index is out of bounds, greater or equal to maxIndex, " + index + ">=" + Count);
		}
		return Items[index];
	}

	public void DeleteAt(int index)
	{
		if (Count > index)
		{
			Count--;
			int num = Count - index;
			if (num > 0)
			{
				Array.Copy(Items, index + 1, Items, index, num);
			}
			Items[Count] = default;
		}
	}

	public void Clear()
	{
		Items = Array.Empty<T>();
		Count = 0;
	}

	public void Add(T item)
	{
		MinCapacity = Count + 1;
		Items[Count] = item;
		Count++;
	}

	public void Swap(int index1, int index2)
	{
		T[] items = Items;
		T[] items2 = Items;
		T val = Items[index2];
		T val2 = Items[index1];
		items[index1] = val;
		items2[index2] = val2;
	}

	public void MoveValues(int fromIndex, int toIndex, int numberOfElements)
	{
		T[] array = new T[numberOfElements];
		Array.Copy(Items, fromIndex, array, 0, numberOfElements);
		if (fromIndex > toIndex)
		{
			if (toIndex + numberOfElements < fromIndex)
			{
				Array.Copy(Items, toIndex, Items, toIndex + numberOfElements, fromIndex - toIndex);
			}
			else
			{
				int num = fromIndex - toIndex;
				Array.Copy(Items, toIndex, Items, fromIndex + numberOfElements - num, num);
			}
		}
		else if (fromIndex + numberOfElements <= toIndex)
		{
			Array.Copy(Items, fromIndex + numberOfElements, Items, fromIndex, toIndex - fromIndex);
		}
		else
		{
			Array.Copy(Items, fromIndex + numberOfElements, Items, fromIndex, toIndex - fromIndex);
		}
		Array.Copy(array, 0, Items, toIndex, numberOfElements);
	}
}
