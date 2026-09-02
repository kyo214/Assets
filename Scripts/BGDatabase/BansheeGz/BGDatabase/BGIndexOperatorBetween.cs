using System;

namespace BansheeGz.BGDatabase;

public class BGIndexOperatorBetween<T> : BGIndexOperatorRange<T> where T : IComparable<T>
{
	public T From;

	public T To;

	public bool LowBoundInclusive;

	public bool UpperBoundInclusive;

	public BGIndexOperatorBetween(T from, T to, bool lowBoundInclusive, bool upperBoundInclusive)
	{
		From = from;
		To = to;
		LowBoundInclusive = lowBoundInclusive;
		UpperBoundInclusive = upperBoundInclusive;
	}

	protected override BGIndexStorageItem<T> GetKeyFrom(out bool pooled, out bool inclusive)
	{
		pooled = true;
		inclusive = LowBoundInclusive;
		BGIndexStorageItem<T> bGIndexStorageItem = BGIndexStorageItem<T>.Pool.Get();
		bGIndexStorageItem.key = From;
		return bGIndexStorageItem;
	}

	protected override BGIndexStorageItem<T> GetKeyTo(out bool pooled, out bool inclusive)
	{
		pooled = true;
		inclusive = UpperBoundInclusive;
		BGIndexStorageItem<T> bGIndexStorageItem = BGIndexStorageItem<T>.Pool.Get();
		bGIndexStorageItem.key = To;
		return bGIndexStorageItem;
	}
}
