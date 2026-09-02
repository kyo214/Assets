using System;
using System.Collections;

namespace NPOI.HPSF;

public class Util
{
	public static readonly long EPOCH_DIFF = new DateTime(1970, 1, 1).Ticks;

	public static void Copy(byte[] src, int srcOffSet, int Length, byte[] dst, int dstOffSet)
	{
		for (int i = 0; i < Length; i++)
		{
			dst[dstOffSet + i] = src[srcOffSet + i];
		}
	}

	public static byte[] Cat(byte[][] byteArrays)
	{
		int num = 0;
		for (int i = 0; i < byteArrays.Length; i++)
		{
			num += byteArrays[i].Length;
		}
		byte[] array = new byte[num];
		int num2 = 0;
		for (int j = 0; j < byteArrays.Length; j++)
		{
			for (int k = 0; k < byteArrays[j].Length; k++)
			{
				array[num2++] = byteArrays[j][k];
			}
		}
		return array;
	}

	public static byte[] Copy(byte[] src, int offset, int Length)
	{
		byte[] array = new byte[Length];
		Copy(src, offset, Length, array, 0);
		return array;
	}

	public static DateTime FiletimeToDate(int high, int low)
	{
		return FiletimeToDate(((long)high << 32) | (low & 0xFFFFFFFFu));
	}

	public static DateTime FiletimeToDate(long filetime)
	{
		return DateTime.FromFileTime(filetime);
	}

	public static long DateToFileTime(DateTime dateTime)
	{
		return dateTime.ToFileTime();
	}

	public static bool AreEqual(IList c1, IList c2)
	{
		return internalEquals(c1, c2);
	}

	private static bool internalEquals(IList c1, IList c2)
	{
		IEnumerator enumerator = c1.GetEnumerator();
		while (enumerator.MoveNext())
		{
			object current = enumerator.Current;
			bool flag = false;
			IEnumerator enumerator2 = c2.GetEnumerator();
			while (!flag && enumerator2.MoveNext())
			{
				object current2 = enumerator2.Current;
				if (current.Equals(current2))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	public static byte[] Pad4(byte[] ba)
	{
		int num = 4;
		int num2 = ba.Length % num;
		byte[] array;
		if (num2 == 0)
		{
			array = ba;
		}
		else
		{
			num2 = num - num2;
			array = new byte[ba.Length + num2];
			System.Array.Copy(ba, array, ba.Length);
		}
		return array;
	}

	public static char[] Pad4(char[] ca)
	{
		int num = 4;
		int num2 = ca.Length % num;
		char[] array;
		if (num2 == 0)
		{
			array = ca;
		}
		else
		{
			num2 = num - num2;
			array = new char[ca.Length + num2];
			System.Array.Copy(ca, array, ca.Length);
		}
		return array;
	}

	public static char[] Pad4(string s)
	{
		return Pad4(s.ToCharArray());
	}
}
