using System;
using System.Collections.Generic;

namespace NPOI.HSSF.Util;

public class LazilyConcatenatedByteArray
{
	private List<byte[]> arrays = new List<byte[]>(1);

	public void Clear()
	{
		arrays.Clear();
	}

	public void Concatenate(byte[] array)
	{
		if (array == null)
		{
			throw new ArgumentException("array cannot be null");
		}
		arrays.Add(array);
	}

	public byte[] ToArray()
	{
		if (arrays.Count == 0)
		{
			return null;
		}
		if (arrays.Count > 1)
		{
			int num = 0;
			foreach (byte[] array2 in arrays)
			{
				num += array2.Length;
			}
			byte[] array = new byte[num];
			int num2 = 0;
			foreach (byte[] array3 in arrays)
			{
				Array.Copy(array3, 0, array, num2, array3.Length);
				num2 += array3.Length;
			}
			arrays.Clear();
			arrays.Add(array);
		}
		return arrays[0];
	}
}
