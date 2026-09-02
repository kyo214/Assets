using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toked;

public static class IListExtensions
{
	public static void Shuffle<T>(this IList<T> ts)
	{
		int count = ts.Count;
		int num = count - 1;
		for (int i = 0; i < num; i++)
		{
			int num2 = UnityEngine.Random.Range(i, count);
			int index = i;
			int index2 = num2;
			T val = ts[num2];
			T val2 = ts[i];
			T val3 = (ts[index] = val);
			val3 = (ts[index2] = val2);
		}
	}

	public static void Shuffle<T>(this IList<T> ts, int seed)
	{
		UnityEngine.Random.InitState(seed);
		ts.Shuffle();
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}
}
