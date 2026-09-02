#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fusion;

public static class ArrayUtils
{
	public static T[] Concat<T>(params T[][] arrays)
	{
		int num = arrays.Sum((T[] x) => (x != null) ? x.Length : 0);
		T[] array = new T[num];
		int num2 = 0;
		foreach (T[] array2 in arrays)
		{
			if (array2 != null)
			{
				for (int num4 = 0; num4 < array2.Length; num4++)
				{
					array[num2++] = array2[num4];
				}
			}
		}
		Assert.Check(num == num2);
		return array;
	}

	public static T[][] CreateGrid<T>(int rows, int cols)
	{
		T[][] array = new T[rows][];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new T[cols];
		}
		return array;
	}

	public static void Add<T>(ref T[] array, T item)
	{
		Array.Resize(ref array, array.Length + 1);
		array[array.Length - 1] = item;
	}

	public static T[] Clone<T>(this T[] array)
	{
		T[] array2 = new T[array.Length];
		Array.Copy(array, array2, array.Length);
		return array2;
	}

	public static T[] AddAtStart<T>(T[] array, T item)
	{
		T[] array2 = new T[array.Length + 1];
		array2[0] = item;
		Array.Copy(array, 0, array2, 1, array.Length);
		return array2;
	}

	public static T[] RemoveAtStart<T>(T[] array)
	{
		T[] array2 = new T[array.Length - 1];
		Array.Copy(array, 1, array2, 0, array2.Length);
		return array2;
	}

	public static B[] Map<A, B>(this A[] array, Func<A, B> map)
	{
		if (array == null)
		{
			return null;
		}
		B[] array2 = new B[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = map(array[i]);
		}
		return array2;
	}

	public static B[] Map<A, B>(this List<A> array, Func<A, B> map)
	{
		if (array == null)
		{
			return null;
		}
		B[] array2 = new B[array.Count];
		for (int i = 0; i < array.Count; i++)
		{
			array2[i] = map(array[i]);
		}
		return array2;
	}

	public static T[] Slice<T>(this T[] array, int slice)
	{
		T[] array2 = new T[slice];
		Array.Copy(array, array2, slice);
		return array2;
	}

	public static B[] MapRef<A, B>(this A[] array, Func<A, B> map) where A : class where B : class
	{
		if (array == null)
		{
			return null;
		}
		B[] array2 = new B[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == null)
			{
				array2[i] = null;
			}
			else
			{
				array2[i] = map(array[i]);
			}
		}
		return array2;
	}
}
