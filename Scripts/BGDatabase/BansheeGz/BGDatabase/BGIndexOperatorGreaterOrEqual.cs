using System;

namespace BansheeGz.BGDatabase;

public class BGIndexOperatorGreaterOrEqual<T> : BGIndexOperatorRange<T> where T : IComparable<T>
{
	public T Value;

	public BGIndexOperatorGreaterOrEqual(T value)
	{
		Value = value;
	}

	protected override BGIndexStorageItem<T> GetKeyFrom(out bool pooled, out bool inclusive)
	{
		pooled = true;
		inclusive = true;
		BGIndexStorageItem<T> bGIndexStorageItem = BGIndexStorageItem<T>.Pool.Get();
		bGIndexStorageItem.key = Value;
		return bGIndexStorageItem;
	}

	protected override BGIndexStorageItem<T> GetKeyTo(out bool pooled, out bool inclusive)
	{
		pooled = false;
		inclusive = false;
		return BGIndexStorageItem<T>.Eternity;
	}
}
