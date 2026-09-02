using System;

namespace BansheeGz.BGDatabase;

public struct BGStoreFieldAdapter<T, TStoreType> : BGStoreFieldI<TStoreType>
{
	private readonly BGFieldCachedA<T, TStoreType> field;

	public TStoreType this[int index]
	{
		get
		{
			return field.StoreGet(index);
		}
		set
		{
			field.StoreSet(index, value);
		}
	}

	public int Count => this.field.StoreCount;

	public int MinSize
	{
		set
		{
			this.field.StoreMinSize = value;
		}
	}

	public int MinCapacity
	{
		set
		{
			this.field.StoreMinCapacity = value;
		}
	}

	public BGStoreFieldAdapter(BGFieldCachedA<T, TStoreType> field)
	{
		this = default;
		this.field = field;
	}

	public void ForEachKey(Action<int> action)
	{
		field.StoreForEachKey(action);
	}

	public void ForEachKeyValue(Action<int, TStoreType> action)
	{
		field.StoreForEachKeyValue(action);
	}

	public TStoreType[] CopyRawValues()
	{
		return field.StoreCopyRawValues();
	}

	public TStoreType Get(int index)
	{
		return this[index];
	}

	public void DeleteAt(int index)
	{
		field.StoreDeleteAt(index);
	}

	public void Clear()
	{
		field.StoreClear();
	}

	public void Add(TStoreType item)
	{
		field.StoreAdd(item);
	}

	public void Swap(int index1, int index2)
	{
		field.StoreSwap(index1, index2);
	}

	public void MoveValues(int fromIndex, int toIndex, int numberOfElements)
	{
		field.StoreMoveValues(fromIndex, toIndex, numberOfElements);
	}
}
