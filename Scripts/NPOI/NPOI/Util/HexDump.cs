using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace NPOI.Util;

public class HexDump
{
	private static readonly char[] _hexcodes = new char[18]
	{
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'A', 'B', 'C', 'D', 'E', 'F', '\0', '\0'
	};

	private static readonly int[] _shifts = new int[16]
	{
		60, 56, 52, 48, 44, 40, 36, 32, 28, 24,
		20, 16, 12, 8, 4, 0
	};

	public static readonly string EOL = Environment.NewLine;

	private static char[] DIGITS_LOWER = new char[16]
	{
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'a', 'b', 'c', 'd', 'e', 'f'
	};

	private static char[] DIGITS_UPPER = new char[16]
	{
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'A', 'B', 'C', 'D', 'E', 'F'
	};

	private HexDump()
	{
	}

	private static string Dump(byte value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Length = 0;
		for (int i = 0; i < 2; i++)
		{
			stringBuilder.Append(_hexcodes[(value >> _shifts[i + 6]) & 0xF]);
		}
		return stringBuilder.ToString();
	}

	private static string Dump(long value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Length = 0;
		for (int i = 0; i < 8; i++)
		{
			stringBuilder.Append(_hexcodes[(int)(value >> _shifts[i + _shifts.Length - 8]) & 0xF]);
		}
		return stringBuilder.ToString();
	}

	public static string Dump(byte[] data, long offset, int index)
	{
		return Dump(data, offset, index, int.MaxValue);
	}

	public static string Dump(byte[] data, long offset, int index, int length)
	{
		if (data == null || data.Length == 0)
		{
			return "No Data" + EOL;
		}
		int num = ((length == int.MaxValue || length < 0 || index + length < 0) ? data.Length : Math.Min(data.Length, index + length));
		if (index < 0 || index >= data.Length)
		{
			throw new IndexOutOfRangeException("illegal index: " + index + " into array of length " + data.Length);
		}
		long num2 = offset + index;
		StringBuilder stringBuilder = new StringBuilder(74);
		for (int i = index; i < num; i += 16)
		{
			int num3 = num - i;
			if (num3 > 16)
			{
				num3 = 16;
			}
			stringBuilder.Append(Dump(num2)).Append(' ');
			for (int j = 0; j < 16; j++)
			{
				if (j < num3)
				{
					stringBuilder.Append(Dump(data[j + i])).Append(' ');
				}
				else
				{
					stringBuilder.Append("   ");
				}
			}
			for (int k = 0; k < num3; k++)
			{
				stringBuilder.Append(ToAscii(data[k + i]));
			}
			stringBuilder.Append(EOL);
			num2 += num3;
		}
		return stringBuilder.ToString();
	}

	public static char ToAscii(int dataB)
	{
		char c = (char)(dataB & 0xFF);
		if (char.IsControl(c))
		{
			return '.';
		}
		byte b = (byte)c;
		if (b == 221 || b == byte.MaxValue)
		{
			c = '.';
		}
		return c;
	}

	public static void Dump(byte[] data, long offset, Stream stream, int index)
	{
		Dump(data, offset, stream, index, data.Length - index);
	}

	public static void Dump(Stream inStream, int start, int bytesToDump)
	{
		using MemoryStream memoryStream = new MemoryStream();
		if (bytesToDump == -1)
		{
			for (int num = inStream.ReadByte(); num != -1; num = inStream.ReadByte())
			{
				memoryStream.WriteByte((byte)num);
			}
		}
		else
		{
			int num2 = bytesToDump;
			while (num2-- > 0)
			{
				int num3 = inStream.ReadByte();
				if (num3 == -1)
				{
					break;
				}
				memoryStream.WriteByte((byte)num3);
			}
		}
		byte[] array = memoryStream.ToArray();
		Dump(array, 0L, null, start, array.Length);
	}

	public static void Dump(byte[] data, long offset, Stream stream, int index, int length)
	{
		if (data.Length == 0)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "No Data{0}", new object[1] { EOL }));
			if (stream != null)
			{
				stream.Write(bytes, 0, bytes.Length);
				stream.Flush();
			}
			return;
		}
		if (index < 0 || index >= data.Length)
		{
			throw new IndexOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "illegal index: {0} into array of length {1}", new object[2] { index, data.Length }));
		}
		if (data.Length == 0)
		{
			return;
		}
		long num = offset + index;
		StringBuilder stringBuilder = new StringBuilder(74);
		int num2 = Math.Min(data.Length, index + length);
		for (int i = index; i < num2; i += 16)
		{
			int num3 = num2 - i;
			if (num3 > 16)
			{
				num3 = 16;
			}
			stringBuilder.Append(Dump(num)).Append(' ');
			for (int j = 0; j < 16; j++)
			{
				if (j < num3)
				{
					stringBuilder.Append(Dump(data[j + i]));
				}
				else
				{
					stringBuilder.Append("  ");
				}
				stringBuilder.Append(' ');
			}
			for (int k = 0; k < num3; k++)
			{
				if (data[k + i] >= 32 && data[k + i] < 127)
				{
					stringBuilder.Append((char)data[k + i]);
				}
				else
				{
					stringBuilder.Append('.');
				}
			}
			stringBuilder.Append(EOL);
			byte[] bytes2 = Encoding.UTF8.GetBytes(stringBuilder.ToString());
			if (stream != null)
			{
				stream.Write(bytes2, 0, bytes2.Length);
				stream.Flush();
			}
			stringBuilder.Length = 0;
			num += num3;
		}
	}

	public static void Dump(Stream in1, Stream out1, int start, int bytesToDump)
	{
		MemoryStream memoryStream = new MemoryStream();
		if (bytesToDump == -1)
		{
			for (int num = in1.ReadByte(); num != -1; num = in1.ReadByte())
			{
				memoryStream.WriteByte((byte)num);
			}
		}
		else
		{
			int num2 = bytesToDump;
			while (num2-- > 0)
			{
				int num3 = in1.ReadByte();
				if (num3 == -1)
				{
					break;
				}
				memoryStream.WriteByte((byte)num3);
			}
		}
		byte[] array = memoryStream.ToArray();
		Dump(array, 0L, out1, start, array.Length);
	}

	public static char[] ShortToHex(int value)
	{
		return ToHexChars(value, 2);
	}

	public static char[] ByteToHex(int value)
	{
		return ToHexChars(value, 1);
	}

	public static char[] IntToHex(int value)
	{
		return ToHexChars(value, 4);
	}

	public static char[] LongToHex(long value)
	{
		return ToHexChars(value, 8);
	}

	private static char[] ToHexChars(long pValue, int nBytes)
	{
		int num = 2 + nBytes * 2;
		char[] array = new char[num];
		long num2 = pValue;
		do
		{
			array[--num] = _hexcodes[(int)(num2 & 0xF)];
			num2 >>= 4;
		}
		while (num > 1);
		array[0] = '0';
		array[1] = 'x';
		return array;
	}

	public static string ToHex(byte value)
	{
		return ToHex(value, 2);
	}

	public static string ToHex(short value)
	{
		return ToHex(value, 4);
	}

	public static string ToHex(int value)
	{
		return ToHex(value, 8);
	}

	public static string ToHex(long value)
	{
		return ToHex(value, 16);
	}

	public static string ToHex(byte[] value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('[');
		if (value != null && value.Length != 0)
		{
			for (int i = 0; i < value.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(ToHex(value[i]));
			}
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	public static string ToHex(short[] value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append('[');
		for (int i = 0; i < value.Length; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(ToHex(value[i]));
		}
		stringBuilder.Append(']');
		return stringBuilder.ToString();
	}

	private static string ToHex(long value, int digits)
	{
		StringBuilder stringBuilder = new StringBuilder(digits);
		for (int i = 0; i < digits; i++)
		{
			stringBuilder.Append(_hexcodes[(int)((value >> _shifts[i + (16 - digits)]) & 0xF)]);
		}
		return stringBuilder.ToString();
	}

	public static string ToHex(byte[] value, int bytesPerLine)
	{
		int num = ((value.Length != 0) ? ((int)Math.Round(Math.Log(value.Length) / Math.Log(10.0) + 0.50000001)) : 0);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < num; i++)
		{
			stringBuilder.Append('0');
		}
		stringBuilder.Append(": ");
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.Append(0.0.ToString(stringBuilder.ToString(), CultureInfo.InvariantCulture));
		if (value.Length == 0)
		{
			stringBuilder2.Append("0");
		}
		int num2 = -1;
		for (int j = 0; j < value.Length; j++)
		{
			if (++num2 == bytesPerLine)
			{
				stringBuilder2.Append('\n');
				stringBuilder2.Append(((double)j).ToString(stringBuilder.ToString(), CultureInfo.InvariantCulture));
				num2 = 0;
			}
			else if (j > 0)
			{
				stringBuilder2.Append(", ");
			}
			stringBuilder2.Append(ToHex(value[j]));
		}
		return stringBuilder2.ToString();
	}

	public static string EncodeHexString(byte[] data)
	{
		return new string(EncodeHex(data));
	}

	public static char[] EncodeHex(byte[] data)
	{
		return EncodeHex(data, toLowerCase: true);
	}

	public static char[] EncodeHex(byte[] data, bool toLowerCase)
	{
		return EncodeHex(data, toLowerCase ? DIGITS_LOWER : DIGITS_UPPER);
	}

	protected static char[] EncodeHex(byte[] data, char[] toDigits)
	{
		int num = data.Length;
		char[] array = new char[num << 1];
		int i = 0;
		int num2 = 0;
		for (; i < num; i++)
		{
			array[num2++] = toDigits[(0xF0 & data[i]) >> 4];
			array[num2++] = toDigits[0xF & data[i]];
		}
		return array;
	}
}
