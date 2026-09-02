using System;

namespace BansheeGz.BGDatabase;

public class BGSharedObjectValue<T> : IDisposable
{
	private readonly BGObjectPool<T> pool;

	public readonly T Value;

	public BGSharedObjectValue(BGObjectPool<T> pool)
	{
		this.pool = pool;
		Value = pool.Get();
	}

	public void Dispose()
	{
		pool.Return(Value);
	}
}
