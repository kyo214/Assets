using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGListPoolDefault<T> : BGObjectPool<List<T>>
{
	public static readonly BGListPoolDefault<T> I = new BGListPoolDefault<T>();

	private BGListPoolDefault()
		: base((Func<List<T>>)(() => new List<T>()), (Action<List<T>>)((List<T> list) =>
		{
			list.Clear();
		}))
	{
	}
}
