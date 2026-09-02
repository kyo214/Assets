using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGSharedListValue<T> : IDisposable
{
	private readonly BGListPoolDefault<T> pool;

	public readonly List<T> Value;

	public BGSharedListValue(BGListPoolDefault<T> pool)
	{
		this.pool = pool;
		Value = pool.Get();
		Value.Clear();
	}

	public void Dispose()
	{
		Value.Clear();
		pool.Return(Value);
	}
}
