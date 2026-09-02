using System;
using System.IO;

namespace NPOI.Util;

public class LittleEndian : LittleEndianConsts
{
	private LittleEndian()
	{
	}

	public static short GetShort(byte[] data, int offset)
	{
		int num = data[offset] & 0xFF;
		return (short)(((data[offset + 1] & 0xFF) << 8) + num);
	}

	public static int GetUShort(byte[] data, int offset)
	{
		int num = data[offset] & 0xFF;
		return ((data[offset + 1] & 0xFF) << 8) + num;
	}

	public static short GetShort(byte[] data)
	{
		return GetShort(data, 0);
	}

	public static int GetUShort(byte[] data)
	{
		return GetUShort(data, 0);
	}

	public static int GetInt(byte[] data, int offset)
	{
		int num = offset;
		int num2 = data[num++] & 0xFF;
		int num3 = data[num++] & 0xFF;
		int num4 = data[num++] & 0xFF;
		return ((data[num++] & 0xFF) << 24) + (num4 << 16) + (num3 << 8) + num2;
	}

	public static int GetInt(byte[] data)
	{
		return GetInt(data, 0);
	}

	public static long GetUInt(byte[] data, int offset)
	{
		return GetInt(data, offset) & 0xFFFFFFFFu;
	}

	public static long GetUInt(byte[] data)
	{
		return GetUInt(data, 0);
	}

	public static long GetLong(byte[] data, int offset)
	{
		long num = 0L;
		for (int num2 = offset + 8 - 1; num2 >= offset; num2--)
		{
			num <<= 8;
			num |= (long)(0xFFuL & (ulong)data[num2]);
		}
		return num;
	}

	public static double GetDouble(byte[] data, int offset)
	{
		return BitConverter.Int64BitsToDouble(GetLong(data, offset));
	}

	public static void PutShort(byte[] data, int offset, short value)
	{
		int num = offset;
		data[num++] = (byte)(value & 0xFF);
		data[num++] = (byte)((value >> 8) & 0xFF);
	}

	public static void PutByte(byte[] data, int offset, int value)
	{
		data[offset] = (byte)value;
	}

	public static void PutUByte(byte[] data, int offset, short value)
	{
		data[offset] = (byte)(value & 0xFF);
	}

	public static void PutUShort(byte[] data, int offset, int value)
	{
		int num = offset;
		data[num++] = (byte)(value & 0xFF);
		data[num++] = (byte)((value >> 8) & 0xFF);
	}

	public static void PutShort(Stream outputStream, short value)
	{
		outputStream.WriteByte((byte)(value & 0xFF));
		outputStream.WriteByte((byte)((value >> 8) & 0xFF));
	}

	public static void PutInt(byte[] data, int offset, int value)
	{
		int num = offset;
		data[num++] = (byte)(value & 0xFF);
		data[num++] = (byte)((value >> 8) & 0xFF);
		data[num++] = (byte)((value >> 16) & 0xFF);
		data[num++] = (byte)((value >> 24) & 0xFF);
	}

	public static void PutInt(int value, Stream outputStream)
	{
		outputStream.WriteByte((byte)(value & 0xFF));
		outputStream.WriteByte((byte)((value >> 8) & 0xFF));
		outputStream.WriteByte((byte)((value >> 16) & 0xFF));
		outputStream.WriteByte((byte)((value >> 24) & 0xFF));
	}

	public static void PutLong(byte[] data, int offset, long value)
	{
		int num = 8 + offset;
		long num2 = value;
		for (int i = offset; i < num; i++)
		{
			data[i] = (byte)(num2 & 0xFF);
			num2 >>= 8;
		}
	}

	public static void PutDouble(byte[] data, int offset, double value)
	{
		long num = 0L;
		num = ((!double.IsNaN(value)) ? BitConverter.DoubleToInt64Bits(value) : (-276939487313920L));
		PutLong(data, offset, num);
	}

	public static short ReadShort(Stream stream)
	{
		return (short)ReadUShort(stream);
	}

	public static int ReadUShort(Stream stream)
	{
		int num = stream.ReadByte();
		int num2 = stream.ReadByte();
		if ((num | num2) < 0)
		{
			throw new BufferUnderrunException();
		}
		return (num2 << 8) + num;
	}

	public static int ReadInt(Stream stream)
	{
		int num = stream.ReadByte();
		int num2 = stream.ReadByte();
		int num3 = stream.ReadByte();
		int num4 = stream.ReadByte();
		if ((num | num2 | num3 | num4) < 0)
		{
			throw new BufferUnderrunException();
		}
		return (num4 << 24) + (num3 << 16) + (num2 << 8) + num;
	}

	public static long ReadLong(Stream stream)
	{
		int num = stream.ReadByte();
		int num2 = stream.ReadByte();
		int num3 = stream.ReadByte();
		int num4 = stream.ReadByte();
		int num5 = stream.ReadByte();
		int num6 = stream.ReadByte();
		int num7 = stream.ReadByte();
		int num8 = stream.ReadByte();
		if ((num | num2 | num3 | num4 | num5 | num6 | num7 | num8) < 0)
		{
			throw new BufferUnderrunException();
		}
		return ((long)num8 << 56) + ((long)num7 << 48) + ((long)num6 << 40) + ((long)num5 << 32) + ((long)num4 << 24) + (num3 << 16) + (num2 << 8) + num;
	}

	public static int UByteToInt(byte b)
	{
		if ((b & 0x80) == 0)
		{
			return b;
		}
		return (b & 0x7F) + 128;
	}

	public static byte[] GetByteArray(byte[] data, int offset, int size)
	{
		byte[] array = new byte[size];
		Array.Copy(data, offset, array, 0, size);
		return array;
	}

	[Obsolete]
	public static double GetDouble(byte[] data)
	{
		return GetDouble(data, 0);
	}

	[Obsolete]
	public static long GetLong(byte[] data)
	{
		return GetLong(data, 0);
	}

	[Obsolete]
	public static ulong GetULong(byte[] data)
	{
		return GetULong(data, 0);
	}

	[Obsolete]
	public static ulong GetULong(byte[] data, int offset)
	{
		return BitConverter.ToUInt64(data, offset);
	}

	private static long GetNumber(byte[] data, int offset, int size)
	{
		long num = 0L;
		for (int num2 = offset + size - 1; num2 >= offset; num2--)
		{
			num <<= 8;
			num |= (long)(0xFFuL & (ulong)data[num2]);
		}
		return num;
	}

	public static short GetUByte(byte[] data)
	{
		return (short)(data[0] & 0xFF);
	}

	public static short GetUByte(byte[] data, int offset)
	{
		return (short)(data[offset] & 0xFF);
	}

	[Obsolete]
	public static void PutDouble(byte[] data, double value)
	{
		PutDouble(data, 0, value);
	}

	public static void PutDouble(double value, Stream outputStream)
	{
		PutLong(BitConverter.DoubleToInt64Bits(value), outputStream);
	}

	public static void PutUInt(long value, Stream outputStream)
	{
		outputStream.WriteByte((byte)(value & 0xFF));
		outputStream.WriteByte((byte)((value >> 8) & 0xFF));
		outputStream.WriteByte((byte)((value >> 16) & 0xFF));
		outputStream.WriteByte((byte)((value >> 24) & 0xFF));
	}

	[Obsolete]
	public static void PutUInt(byte[] data, int offset, uint value)
	{
		PutNumber(data, offset, Convert.ToInt64(value), 4);
	}

	public static void PutUInt(byte[] data, int offset, long value)
	{
		int num = offset;
		data[num++] = (byte)(value & 0xFF);
		data[num++] = (byte)((value >> 8) & 0xFF);
		data[num++] = (byte)((value >> 16) & 0xFF);
		data[num++] = (byte)((value >> 24) & 0xFF);
	}

	[Obsolete]
	public static void PutLong(byte[] data, long value)
	{
		PutLong(data, 0, value);
	}

	public static void PutLong(long value, Stream outputStream)
	{
		outputStream.WriteByte((byte)(value & 0xFF));
		outputStream.WriteByte((byte)((value >> 8) & 0xFF));
		outputStream.WriteByte((byte)((value >> 16) & 0xFF));
		outputStream.WriteByte((byte)((value >> 24) & 0xFF));
		outputStream.WriteByte((byte)((value >> 32) & 0xFF));
		outputStream.WriteByte((byte)((value >> 40) & 0xFF));
		outputStream.WriteByte((byte)((value >> 48) & 0xFF));
		outputStream.WriteByte((byte)((value >> 56) & 0xFF));
	}

	[Obsolete]
	public static void PutULong(byte[] data, ulong value)
	{
		PutULong(data, 0, value);
	}

	[Obsolete]
	public static void PutULong(byte[] data, int offset, ulong value)
	{
		PutNumber(data, offset, value, 8);
	}

	private static void PutNumber(byte[] data, int offset, long value, int size)
	{
		int num = size + offset;
		long num2 = value;
		for (int i = offset; i < num; i++)
		{
			data[i] = (byte)(num2 & 0xFF);
			num2 >>= 8;
		}
	}

	private static void PutNumber(byte[] data, int offset, ulong value, int size)
	{
		int num = size + offset;
		ulong num2 = value;
		for (int i = offset; i < num; i++)
		{
			data[i] = (byte)(num2 & 0xFF);
			num2 >>= 8;
		}
	}

	[Obsolete]
	public static void PutShortArray(byte[] data, int offset, short[] value)
	{
		PutNumber(data, offset, Convert.ToInt64(value.Length), 2);
		for (int i = 0; i < value.Length; i++)
		{
			PutNumber(data, offset + 2 + i * 2, Convert.ToInt64(value[i]), 2);
		}
	}

	[Obsolete]
	public static void PutUShort(byte[] data, int value)
	{
		PutNumber(data, 0, Convert.ToInt64(value), 2);
	}

	public static void PutUShort(int value, Stream outputStream)
	{
		outputStream.WriteByte((byte)(value & 0xFF));
		outputStream.WriteByte((byte)((value >> 8) & 0xFF));
	}

	[Obsolete]
	public static byte[] ReadFromStream(Stream stream, int size)
	{
		byte[] array = new byte[size];
		int num = stream.Read(array, 0, array.Length);
		if (num == 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = 0;
			}
			return array;
		}
		if (num != size)
		{
			throw new BufferUnderrunException();
		}
		return array;
	}

	[Obsolete]
	public static ulong ReadULong(Stream stream)
	{
		return BitConverter.ToUInt64(ReadFromStream(stream, 8), 0);
	}
}
