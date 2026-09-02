#define DEBUG
using System.Runtime.InteropServices;

namespace Fusion;

public static class Radix
{
	[StructLayout(LayoutKind.Explicit, Size = 8)]
	public struct SortTarget
	{
		public const int SIZE = 8;

		public const int ALIGNMENT = 4;

		[FieldOffset(0)]
		public byte ValueByte0;

		[FieldOffset(1)]
		public byte ValueByte1;

		[FieldOffset(2)]
		public byte ValueByte2;

		[FieldOffset(3)]
		public byte ValueByte3;

		[FieldOffset(0)]
		public int SortData;

		[FieldOffset(4)]
		public int UserData;
	}

	public const int FLAG_START = 1073741824;

	public const int FLAG_END = int.MinValue;

	public const int RADIX = 8;

	public unsafe static void Sort(SortTarget* a, SortTarget* t, int aLength, int* p, int* c)
	{
		Assert.Check(sizeof(SortTarget) == 8);
		Native.MemClear(c, 4096);
		for (int i = 0; i < aLength; i++)
		{
			c[(int)a[i].ValueByte0]++;
		}
		*p = 0;
		for (int j = 1; j < 256; j++)
		{
			p[j] = p[j - 1] + c[j - 1];
		}
		for (int k = 0; k < aLength; k++)
		{
			t[p[(int)a[k].ValueByte0]++] = a[k];
		}
		SortTarget* ptr = a;
		a = t;
		t = ptr;
		c += 256;
		for (int l = 0; l < aLength; l++)
		{
			c[(int)a[l].ValueByte1]++;
		}
		*p = 0;
		for (int m = 1; m < 256; m++)
		{
			p[m] = p[m - 1] + c[m - 1];
		}
		for (int n = 0; n < aLength; n++)
		{
			t[p[(int)a[n].ValueByte1]++] = a[n];
		}
		ptr = a;
		a = t;
		t = ptr;
		c += 256;
		for (int num = 0; num < aLength; num++)
		{
			c[(int)a[num].ValueByte2]++;
		}
		*p = 0;
		for (int num2 = 1; num2 < 256; num2++)
		{
			p[num2] = p[num2 - 1] + c[num2 - 1];
		}
		for (int num3 = 0; num3 < aLength; num3++)
		{
			t[p[(int)a[num3].ValueByte2]++] = a[num3];
		}
		ptr = a;
		a = t;
		t = ptr;
		c += 256;
		for (int num4 = 0; num4 < aLength; num4++)
		{
			c[(int)a[num4].ValueByte3]++;
		}
		*p = 0;
		for (int num5 = 1; num5 < 256; num5++)
		{
			p[num5] = p[num5 - 1] + c[num5 - 1];
		}
		for (int num6 = 0; num6 < aLength; num6++)
		{
			t[p[(int)a[num6].ValueByte3]++] = a[num6];
		}
	}
}
