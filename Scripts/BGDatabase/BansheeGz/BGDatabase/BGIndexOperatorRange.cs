using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public abstract class BGIndexOperatorRange<T> : BGIndexOperator where T : IComparable<T>
{
	protected abstract BGIndexStorageItem<T> GetKeyFrom(out bool pooled, out bool inclusive);

	protected abstract BGIndexStorageItem<T> GetKeyTo(out bool pooled, out bool inclusive);

	internal override void GetResult<T1>(List<T1> result, BGIndexStorage storage)
	{
		BGIndexStorage<T> bGIndexStorage = (BGIndexStorage<T>)storage;
		BGIndexStorageItem<T> keyFrom = GetKeyFrom(out var pooled, out var inclusive);
		BGIndexStorageItem<T> keyTo = GetKeyTo(out var pooled2, out var inclusive2);
		try
		{
			bGIndexStorage.GetRange(result, keyFrom, keyTo, inclusive, inclusive2);
		}
		finally
		{
			if (pooled)
			{
				BGIndexStorageItem<T>.Pool.Return(keyFrom);
			}
			if (pooled2)
			{
				BGIndexStorageItem<T>.Pool.Return(keyTo);
			}
		}
	}
}
