using System;

namespace DestroyIt;

public static class ArrayExtensions
{
	public static T[] RemoveAllAt<T>(this T[] array, int[] removeIndices)
	{
		T[] result = new T[0];
		if (removeIndices.Length == 0)
		{
			return array;
		}
		if (removeIndices.Length >= array.Length)
		{
			return result;
		}
		result = new T[array.Length];
		int i = 0;
		int num = 0;
		int num2 = 0;
		for (; i < array.Length; i++)
		{
			bool flag = true;
			for (int j = 0; j < removeIndices.Length; j++)
			{
				if (i == removeIndices[j])
				{
					flag = false;
				}
			}
			if (flag)
			{
				num2++;
				result[num] = array[i];
				num++;
			}
		}
		Array.Resize(ref result, num2);
		return result;
	}
}
