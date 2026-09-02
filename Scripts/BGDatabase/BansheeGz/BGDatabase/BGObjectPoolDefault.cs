using System;

namespace BansheeGz.BGDatabase;

public class BGObjectPoolDefault<T> : BGObjectPool<T> where T : new()
{
	private static readonly BGObjectPoolDefault<T> I = new BGObjectPoolDefault<T>();

	private BGObjectPoolDefault()
		: base((Func<T>)(() => new T()), (Action<T>)null)
	{
	}
}
