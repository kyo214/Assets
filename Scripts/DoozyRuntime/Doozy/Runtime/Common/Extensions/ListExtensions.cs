using System.Collections.Generic;
using UnityEngine;

namespace Doozy.Runtime.Common.Extensions;

public static class ListExtensions
{
	public static T GetRandomItem<T>(this List<T> target)
	{
		return target[Random.Range(0, target.Count)];
	}

	public static List<T> Shuffle<T>(this List<T> target)
	{
		for (int num = target.Count - 1; num > 1; num--)
		{
			int num2 = Random.Range(0, num + 1);
			int index = num2;
			int index2 = num;
			T val = target[num];
			T val2 = target[num2];
			T val3 = (target[index] = val);
			val3 = (target[index2] = val2);
		}
		return target;
	}

	public static List<T> RemoveNulls<T>(this List<T> target)
	{
		for (int num = target.Count - 1; num >= 0; num--)
		{
			if (target[num] == null)
			{
				target.RemoveAt(num);
			}
		}
		return target;
	}
}
