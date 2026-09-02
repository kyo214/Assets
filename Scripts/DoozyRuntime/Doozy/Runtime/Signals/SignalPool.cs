using System.Collections.Generic;
using Doozy.Runtime.Common.Attributes;

namespace Doozy.Runtime.Signals;

internal static class SignalPool
{
	private static HashSet<Signal> pool { get; set; }

	[ClearOnReload(false)]
	private static bool initialized { get; set; }

	[ExecuteOnReload]
	private static void Initialize()
	{
		if (!initialized)
		{
			pool = new HashSet<Signal>();
			initialized = true;
		}
	}

	public static T Get<T>() where T : Signal, new()
	{
		Initialize();
		pool.Remove(null);
		T val = null;
		foreach (Signal item in pool)
		{
			if (item is T val2)
			{
				val = val2;
				pool.Remove(val2);
				break;
			}
		}
		if (val == null)
		{
			val = new T();
		}
		return val;
	}

	public static void AddToPool<T>(this T signal) where T : Signal
	{
		Initialize();
		signal.Reset();
		pool.Add(signal);
	}
}
