using System;

namespace BansheeGz.BGDatabase;

public abstract class BGEventArgsA : EventArgs, IDisposable
{
	protected abstract BGObjectPool Pool { get; }

	public abstract void Clear();

	public void Dispose()
	{
		Clear();
		Pool.Return(this);
	}
}
