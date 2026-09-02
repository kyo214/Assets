using System;
using System.Collections.Generic;

namespace Doozy.Runtime.Reactor.Internal;

public static class ReactionPool
{
	public static List<Reaction> pool { get; private set; } = new List<Reaction>();

	private static bool initialized { get; set; }

	private static void Initialize()
	{
		if (!initialized)
		{
			if (pool == null)
			{
				pool = new List<Reaction>();
			}
			initialized = true;
		}
	}

	public static T Get<T>() where T : Reaction
	{
		Initialize();
		pool.Remove(null);
		T val = null;
		foreach (Reaction item in pool)
		{
			if (item is T val2 && !val2.GetType().IsSubclassOf(typeof(T)))
			{
				val = val2;
				pool.Remove(val2);
				break;
			}
		}
		if (val == null)
		{
			val = Activator.CreateInstance<T>();
		}
		val.state = ReactionState.Idle;
		return val;
	}

	public static void AddToPool<T>(this T reaction) where T : Reaction
	{
		Initialize();
		reaction.Reset();
		reaction.state = ReactionState.Pooled;
		pool.Add(reaction);
	}
}
