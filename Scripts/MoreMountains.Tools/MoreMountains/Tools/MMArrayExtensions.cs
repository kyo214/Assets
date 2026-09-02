using UnityEngine;

namespace MoreMountains.Tools;

public static class MMArrayExtensions
{
	public static T MMRandomValue<T>(this T[] array)
	{
		int num = Random.Range(0, array.Length);
		return array[num];
	}

	public static T[] MMShuffle<T>(this T[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			T val = array[i];
			int num = Random.Range(i, array.Length);
			array[i] = array[num];
			array[num] = val;
		}
		return array;
	}
}
