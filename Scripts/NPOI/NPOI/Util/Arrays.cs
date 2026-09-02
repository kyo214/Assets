using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NPOI.Util.Collections;

namespace NPOI.Util;

public class Arrays
{
	public static void Fill(byte[] array, byte defaultValue)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = defaultValue;
		}
	}

	public static void Fill(char[] array, char defaultValue)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = defaultValue;
		}
	}

	public static void Fill<T>(T[] array, T defaultValue)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = defaultValue;
		}
	}

	public static void Fill(byte[] a, int fromIndex, int toIndex, byte val)
	{
		RangeCheck(a.Length, fromIndex, toIndex);
		for (int i = fromIndex; i < toIndex; i++)
		{
			a[i] = val;
		}
	}

	public static void Fill(char[] a, int fromIndex, int toIndex, char val)
	{
		RangeCheck(a.Length, fromIndex, toIndex);
		for (int i = fromIndex; i < toIndex; i++)
		{
			a[i] = val;
		}
	}

	private static void RangeCheck(int length, int fromIndex, int toIndex)
	{
		if (fromIndex > toIndex)
		{
			throw new ArgumentException("fromIndex(" + fromIndex + ") > toIndex(" + toIndex + ")");
		}
		if (fromIndex < 0)
		{
			throw new IndexOutOfRangeException("fromIndex(" + fromIndex + ")");
		}
		if (toIndex > length)
		{
			throw new IndexOutOfRangeException("toIndex(" + toIndex + ")");
		}
	}

	public static ArrayList AsList(Array arr)
	{
		if (arr.Length <= 0)
		{
			return new ArrayList();
		}
		ArrayList arrayList = new ArrayList(arr.Length);
		for (int i = 0; i < arr.Length; i++)
		{
			arrayList.Add(arr.GetValue(i));
		}
		return arrayList;
	}

	public static ArrayList AsArrayList<T>(params T[] arr)
	{
		if (arr.Length == 0)
		{
			return new ArrayList();
		}
		ArrayList arrayList = new ArrayList(arr.Length);
		arrayList.AddRange(arr);
		return arrayList;
	}

	public static List<T> AsList<T>(params T[] arr)
	{
		if (arr.Length == 0)
		{
			return new List<T>();
		}
		List<T> list = new List<T>(arr.Length);
		list.AddRange(arr);
		return list;
	}

	public static void Fill(int[] array, byte defaultValue)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = defaultValue;
		}
	}

	public new static bool Equals(object a1, object b1)
	{
		if (a1 == null || b1 == null)
		{
			return false;
		}
		Array array = a1 as Array;
		Array array2 = b1 as Array;
		if (array.Length != array2.Length)
		{
			return false;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (!array.GetValue(i).Equals(array2.GetValue(i)))
			{
				return false;
			}
		}
		return true;
	}

	public static bool Equals(object[] a, object[] a2)
	{
		if (a == a2)
		{
			return true;
		}
		if (a == null || a2 == null)
		{
			return false;
		}
		int num = a.Length;
		if (a2.Length != num)
		{
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			object obj = a[i];
			object obj2 = a2[i];
			if (!(obj?.Equals(obj2) ?? (obj2 == null)))
			{
				return false;
			}
		}
		return true;
	}

	public static void ArrayMoveWithin(object[] array, int moveFrom, int moveTo, int numToMove)
	{
		if (numToMove > 0 && moveFrom != moveTo)
		{
			if (moveFrom < 0 || moveFrom >= array.Length)
			{
				throw new ArgumentException("The moveFrom must be a valid array index");
			}
			if (moveTo < 0 || moveTo >= array.Length)
			{
				throw new ArgumentException("The moveTo must be a valid array index");
			}
			if (moveFrom + numToMove > array.Length)
			{
				throw new ArgumentException("Asked to move more entries than the array has");
			}
			if (moveTo + numToMove > array.Length)
			{
				throw new ArgumentException("Asked to move to a position that doesn't have enough space");
			}
			object[] array2 = new object[numToMove];
			Array.Copy(array, moveFrom, array2, 0, numToMove);
			object[] array3;
			int destinationIndex;
			if (moveFrom > moveTo)
			{
				array3 = new object[moveFrom - moveTo];
				Array.Copy(array, moveTo, array3, 0, array3.Length);
				destinationIndex = moveTo + numToMove;
			}
			else
			{
				array3 = new object[moveTo - moveFrom];
				Array.Copy(array, moveFrom + numToMove, array3, 0, array3.Length);
				destinationIndex = moveFrom;
			}
			Array.Copy(array2, 0, array, moveTo, array2.Length);
			Array.Copy(array3, 0, array, destinationIndex, array3.Length);
		}
	}

	public static byte[] CopyOf(byte[] source, int newLength)
	{
		byte[] array = new byte[newLength];
		Array.Copy(source, 0, array, 0, Math.Min(source.Length, newLength));
		return array;
	}

	internal static int[] CopyOfRange(int[] original, int from, int to)
	{
		int num = to - from;
		if (num < 0)
		{
			throw new ArgumentException(from + " > " + to);
		}
		int[] array = new int[num];
		Array.Copy(original, from, array, 0, Math.Min(original.Length - from, num));
		return array;
	}

	internal static byte[] CopyOfRange(byte[] original, int from, int to)
	{
		int num = to - from;
		if (num < 0)
		{
			throw new ArgumentException(from + " > " + to);
		}
		byte[] array = new byte[num];
		Array.Copy(original, from, array, 0, Math.Min(original.Length - from, num));
		return array;
	}

	public static int HashCode(long[] a)
	{
		if (a == null)
		{
			return 0;
		}
		int num = 1;
		foreach (long num2 in a)
		{
			int num3 = (int)(num2 ^ Operator.UnsignedRightShift(num2, 32));
			num = 31 * num + num3;
		}
		return num;
	}

	public static int HashCode(int[] a)
	{
		if (a == null)
		{
			return 0;
		}
		int num = 1;
		foreach (int num2 in a)
		{
			num = 31 * num + num2;
		}
		return num;
	}

	public static int HashCode(short[] a)
	{
		if (a == null)
		{
			return 0;
		}
		int num = 1;
		foreach (short num2 in a)
		{
			num = 31 * num + num2;
		}
		return num;
	}

	public static int HashCode(char[] a)
	{
		if (a == null)
		{
			return 0;
		}
		int num = 1;
		foreach (char c in a)
		{
			num = 31 * num + c;
		}
		return num;
	}

	public static int HashCode(byte[] a)
	{
		if (a == null)
		{
			return 0;
		}
		int num = 1;
		foreach (byte b in a)
		{
			num = 31 * num + b;
		}
		return num;
	}

	public static int HashCode(bool[] a)
	{
		if (a == null)
		{
			return 0;
		}
		int num = 1;
		foreach (bool flag in a)
		{
			num = 31 * num + (flag ? 1231 : 1237);
		}
		return num;
	}

	public static int HashCode(float[] a)
	{
		if (a == null)
		{
			return 0;
		}
		int num = 1;
		foreach (float value in a)
		{
			num = 31 * num + BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
		}
		return num;
	}

	public static int HashCode(double[] a)
	{
		if (a == null)
		{
			return 0;
		}
		int num = 1;
		for (int i = 0; i < a.Length; i++)
		{
			long num2 = BitConverter.DoubleToInt64Bits(a[i]);
			num = 31 * num + (int)(num2 ^ Operator.UnsignedRightShift(num2, 32));
		}
		return num;
	}

	public static int HashCode(object[] a)
	{
		if (a == null)
		{
			return 0;
		}
		int num = 1;
		for (int i = 0; i < a.Length; i++)
		{
			num = 31 * num + (a[i]?.GetHashCode() ?? 0);
		}
		return num;
	}

	public static int DeepHashCode(object[] a)
	{
		if (a == null)
		{
			return 0;
		}
		int num = 1;
		foreach (object obj in a)
		{
			int num2 = 0;
			if (obj is object[])
			{
				num2 = DeepHashCode((object[])obj);
			}
			else if (obj is byte[])
			{
				num2 = HashCode((byte[])obj);
			}
			else if (obj is short[])
			{
				num2 = HashCode((short[])obj);
			}
			else if (obj is int[])
			{
				num2 = HashCode((int[])obj);
			}
			else if (obj is long[])
			{
				num2 = HashCode((long[])obj);
			}
			else if (obj is char[])
			{
				num2 = HashCode((char[])obj);
			}
			else if (obj is float[])
			{
				num2 = HashCode((float[])obj);
			}
			else if (obj is double[])
			{
				num2 = HashCode((double[])obj);
			}
			else if (obj is bool[])
			{
				num2 = HashCode((bool[])obj);
			}
			else if (obj != null)
			{
				num2 = obj.GetHashCode();
			}
			num = 31 * num + num2;
		}
		return num;
	}

	public static bool DeepEquals(object[] a1, object[] a2)
	{
		if (a1 == a2)
		{
			return true;
		}
		if (a1 == null || a2 == null)
		{
			return false;
		}
		int num = a1.Length;
		if (a2.Length != num)
		{
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			object obj = a1[i];
			object obj2 = a2[i];
			if (obj != obj2)
			{
				if (obj == null)
				{
					return false;
				}
				if (!DeepEquals0(obj, obj2))
				{
					return false;
				}
			}
		}
		return true;
	}

	private static bool DeepEquals0(object e1, object e2)
	{
		if (e1 is object[] && e2 is object[])
		{
			return DeepEquals((object[])e1, (object[])e2);
		}
		if (e1 is byte[] && e2 is byte[])
		{
			return Equals((byte[])e1, (byte[])e2);
		}
		if (e1 is short[] && e2 is short[])
		{
			return Equals((short[])e1, (short[])e2);
		}
		if (e1 is int[] && e2 is int[])
		{
			return Equals((int[])e1, (int[])e2);
		}
		if (e1 is long[] && e2 is long[])
		{
			return Equals((long[])e1, (long[])e2);
		}
		if (e1 is char[] && e2 is char[])
		{
			return Equals((char[])e1, (char[])e2);
		}
		if (e1 is float[] && e2 is float[])
		{
			return Equals((float[])e1, (float[])e2);
		}
		if (e1 is double[] && e2 is double[])
		{
			return Equals((double[])e1, (double[])e2);
		}
		if (e1 is bool[] && e2 is bool[])
		{
			return Equals((bool[])e1, (bool[])e2);
		}
		return e1.Equals(e2);
	}

	public static string ToString(long[] a)
	{
		if (a == null)
		{
			return "null";
		}
		int num = a.Length - 1;
		if (num == -1)
		{
			return "[]";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('[');
		int num2 = 0;
		while (true)
		{
			stringBuilder.Append(a[num2]);
			if (num2 == num)
			{
				break;
			}
			stringBuilder.Append(", ");
			num2++;
		}
		return stringBuilder.Append(']').ToString();
	}

	public static string ToString(int[] a)
	{
		if (a == null)
		{
			return "null";
		}
		int num = a.Length - 1;
		if (num == -1)
		{
			return "[]";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('[');
		int num2 = 0;
		while (true)
		{
			stringBuilder.Append(a[num2]);
			if (num2 == num)
			{
				break;
			}
			stringBuilder.Append(", ");
			num2++;
		}
		return stringBuilder.Append(']').ToString();
	}

	public static string ToString(short[] a)
	{
		if (a == null)
		{
			return "null";
		}
		int num = a.Length - 1;
		if (num == -1)
		{
			return "[]";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('[');
		int num2 = 0;
		while (true)
		{
			stringBuilder.Append(a[num2]);
			if (num2 == num)
			{
				break;
			}
			stringBuilder.Append(", ");
			num2++;
		}
		return stringBuilder.Append(']').ToString();
	}

	public static string ToString(char[] a)
	{
		if (a == null)
		{
			return "null";
		}
		int num = a.Length - 1;
		if (num == -1)
		{
			return "[]";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('[');
		int num2 = 0;
		while (true)
		{
			stringBuilder.Append(a[num2]);
			if (num2 == num)
			{
				break;
			}
			stringBuilder.Append(", ");
			num2++;
		}
		return stringBuilder.Append(']').ToString();
	}

	public static string ToString(byte[] a)
	{
		if (a == null)
		{
			return "null";
		}
		int num = a.Length - 1;
		if (num == -1)
		{
			return "[]";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('[');
		int num2 = 0;
		while (true)
		{
			stringBuilder.Append(a[num2]);
			if (num2 == num)
			{
				break;
			}
			stringBuilder.Append(", ");
			num2++;
		}
		return stringBuilder.Append(']').ToString();
	}

	public static string ToString(bool[] a)
	{
		if (a == null)
		{
			return "null";
		}
		int num = a.Length - 1;
		if (num == -1)
		{
			return "[]";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('[');
		int num2 = 0;
		while (true)
		{
			stringBuilder.Append(a[num2]);
			if (num2 == num)
			{
				break;
			}
			stringBuilder.Append(", ");
			num2++;
		}
		return stringBuilder.Append(']').ToString();
	}

	public static string ToString(float[] a)
	{
		if (a == null)
		{
			return "null";
		}
		int num = a.Length - 1;
		if (num == -1)
		{
			return "[]";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('[');
		int num2 = 0;
		while (true)
		{
			stringBuilder.Append(a[num2]);
			if (num2 == num)
			{
				break;
			}
			stringBuilder.Append(", ");
			num2++;
		}
		return stringBuilder.Append(']').ToString();
	}

	public static string ToString(double[] a)
	{
		if (a == null)
		{
			return "null";
		}
		int num = a.Length - 1;
		if (num == -1)
		{
			return "[]";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('[');
		int num2 = 0;
		while (true)
		{
			stringBuilder.Append(a[num2]);
			if (num2 == num)
			{
				break;
			}
			stringBuilder.Append(", ");
			num2++;
		}
		return stringBuilder.Append(']').ToString();
	}

	public static string DeepToString(object[] a)
	{
		if (a == null)
		{
			return "null";
		}
		int num = 20 * a.Length;
		if (a.Length != 0 && num <= 0)
		{
			num = int.MaxValue;
		}
		StringBuilder stringBuilder = new StringBuilder(num);
		DeepToString(a, stringBuilder, new NPOI.Util.Collections.HashSet<object[]>());
		return stringBuilder.ToString();
	}

	private static void DeepToString(object[] a, StringBuilder buf, NPOI.Util.Collections.HashSet<object[]> dejaVu)
	{
		if (a == null)
		{
			buf.Append("null");
			return;
		}
		int num = a.Length - 1;
		if (num == -1)
		{
			buf.Append("[]");
			return;
		}
		dejaVu.Add(a);
		buf.Append('[');
		int num2 = 0;
		while (true)
		{
			object obj = a[num2];
			if (obj == null)
			{
				buf.Append("null");
			}
			else
			{
				Type type = obj.GetType();
				if (type.IsArray)
				{
					if (type == typeof(byte[]))
					{
						buf.Append(ToString((byte[])obj));
					}
					else if (type == typeof(short[]))
					{
						buf.Append(ToString((short[])obj));
					}
					else if (type == typeof(int[]))
					{
						buf.Append(ToString((int[])obj));
					}
					else if (type == typeof(long[]))
					{
						buf.Append(ToString((long[])obj));
					}
					else if (type == typeof(char[]))
					{
						buf.Append(ToString((char[])obj));
					}
					else if (type == typeof(float[]))
					{
						buf.Append(ToString((float[])obj));
					}
					else if (type == typeof(double[]))
					{
						buf.Append(ToString((double[])obj));
					}
					else if (type == typeof(bool[]))
					{
						buf.Append(ToString((bool[])obj));
					}
					else if (dejaVu.Contains(obj as object[]))
					{
						buf.Append("[...]");
					}
					else
					{
						DeepToString((object[])obj, buf, dejaVu);
					}
				}
				else
				{
					buf.Append(obj.ToString());
				}
			}
			if (num2 == num)
			{
				break;
			}
			buf.Append(", ");
			num2++;
		}
		buf.Append(']');
		dejaVu.Remove(a);
	}

	public static string ToString(object[] a)
	{
		if (a == null)
		{
			return "null";
		}
		int num = a.Length - 1;
		if (num == -1)
		{
			return "[]";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('[');
		int num2 = 0;
		while (true)
		{
			stringBuilder.Append(a[num2].ToString());
			if (num2 == num)
			{
				break;
			}
			stringBuilder.Append(", ");
			num2++;
		}
		return stringBuilder.Append(']').ToString();
	}
}
