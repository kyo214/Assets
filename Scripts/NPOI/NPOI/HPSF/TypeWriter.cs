using System;
using System.IO;
using NPOI.Util;

namespace NPOI.HPSF;

public class TypeWriter
{
	public static int WriteToStream(Stream out1, short n)
	{
		LittleEndian.PutShort(out1, n);
		return 2;
	}

	public static int WriteToStream(Stream out1, int n)
	{
		LittleEndian.PutInt(n, out1);
		return 4;
	}

	[Obsolete]
	public static int WriteToStream(Stream out1, uint n)
	{
		int num = 4;
		byte[] array = new byte[num];
		LittleEndian.PutUInt(array, 0, n);
		out1.Write(array, 0, num);
		return num;
	}

	public static int WriteToStream(Stream out1, long n)
	{
		LittleEndian.PutLong(n, out1);
		return 8;
	}

	public static void WriteUShortToStream(Stream out1, int n)
	{
		if ((n & -65536) != 0)
		{
			throw new IllegalPropertySetDataException("Value " + n + " cannot be represented by 2 bytes.");
		}
		LittleEndian.PutUShort(n, out1);
	}

	public static int WriteUIntToStream(Stream out1, uint n)
	{
		ulong num = (ulong)(n & -4294967296L);
		if (num != 0L && num != 18446744069414584320uL)
		{
			throw new IllegalPropertySetDataException("Value " + n + " cannot be represented by 4 bytes.");
		}
		LittleEndian.PutUInt(n, out1);
		return 4;
	}

	public static int WriteToStream(Stream out1, ClassID n)
	{
		byte[] array = new byte[16];
		n.Write(array, 0);
		out1.Write(array, 0, array.Length);
		return array.Length;
	}

	public static void WriteToStream(Stream out1, Property[] properties, int codepage)
	{
		if (properties != null)
		{
			foreach (Property property in properties)
			{
				WriteUIntToStream(out1, (uint)property.ID);
				WriteUIntToStream(out1, (uint)property.Count);
			}
			foreach (Property property2 in properties)
			{
				long type = property2.Type;
				WriteUIntToStream(out1, (uint)type);
				VariantSupport.Write(out1, (int)type, property2.Value, codepage);
			}
		}
	}

	public static int WriteToStream(Stream out1, double n)
	{
		LittleEndian.PutDouble(n, out1);
		return 8;
	}
}
