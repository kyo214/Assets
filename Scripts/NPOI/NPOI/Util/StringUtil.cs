using System;
using System.Collections.Generic;
using System.Text;

namespace NPOI.Util;

public class StringUtil
{
	private static Encoding ISO_8859_1 = Encoding.GetEncoding("ISO-8859-1");

	private static Encoding UTF16LE = Encoding.Unicode;

	private static Dictionary<int, int> msCodepointToUnicode;

	public static char MIN_HIGH_SURROGATE = '\ud800';

	public static char MAX_HIGH_SURROGATE = '\udbff';

	public static char MIN_LOW_SURROGATE = '\udc00';

	public static char MAX_LOW_SURROGATE = '\udfff';

	public const int MIN_SUPPLEMENTARY_CODE_POINT = 65536;

	private static int[] symbolMap_f020 = new int[96]
	{
		32, 33, 8704, 35, 8707, 37, 38, 8717, 40, 41,
		8727, 43, 44, 8722, 46, 47, 48, 49, 50, 51,
		52, 53, 54, 55, 56, 57, 58, 59, 60, 61,
		62, 63, 8773, 913, 914, 935, 916, 917, 934, 915,
		919, 921, 977, 922, 923, 924, 925, 927, 928, 920,
		929, 931, 932, 933, 962, 937, 926, 936, 918, 91,
		8765, 93, 8869, 95, 32, 945, 946, 967, 948, 949,
		966, 947, 951, 953, 981, 954, 955, 956, 957, 959,
		960, 952, 961, 963, 964, 965, 982, 969, 958, 968,
		950, 123, 124, 125, 8764, 32
	};

	private static int[] symbolMap_f0a0 = new int[96]
	{
		8364, 978, 8242, 8804, 8260, 8734, 402, 9827, 9830, 9829,
		9824, 8596, 8591, 8593, 8594, 8595, 176, 177, 8243, 8805,
		215, 181, 8706, 8729, 247, 8800, 8801, 8776, 8230, 9168,
		9135, 8629, 8501, 8475, 8476, 8472, 8855, 8853, 8709, 8745,
		8746, 8835, 8839, 8836, 8834, 8838, 8712, 8713, 8736, 8711,
		174, 169, 8482, 8719, 8730, 8901, 172, 8743, 8744, 8660,
		8656, 8657, 8658, 8659, 9674, 9001, 174, 169, 8482, 8721,
		9115, 9116, 9117, 9121, 9122, 9123, 9127, 9128, 9129, 9130,
		32, 9002, 8747, 8992, 9134, 8993, 9118, 9119, 9120, 9124,
		9125, 9126, 9131, 9132, 9133, 32
	};

	private StringUtil()
	{
	}

	public static string GetFromUnicodeLE(byte[] str, int offset, int len)
	{
		if (offset < 0 || offset >= str.Length)
		{
			throw new IndexOutOfRangeException("Illegal offset");
		}
		if (len < 0 || (str.Length - offset) / 2 < len)
		{
			throw new ArgumentException("Illegal Length");
		}
		return UTF16LE.GetString(str, offset, len * 2);
	}

	public static string GetFromUnicodeLE(byte[] str)
	{
		if (str.Length == 0)
		{
			return "";
		}
		return GetFromUnicodeLE(str, 0, str.Length / 2);
	}

	public static byte[] GetToUnicodeLE(string string1)
	{
		return UTF16LE.GetBytes(string1);
	}

	public static string GetFromUnicodeBE(byte[] str, int offset, int len)
	{
		if (offset < 0 || offset >= str.Length)
		{
			throw new IndexOutOfRangeException("Illegal offset");
		}
		if (len < 0 || (str.Length - offset) / 2 < len)
		{
			throw new ArgumentException("Illegal Length");
		}
		try
		{
			return Encoding.GetEncoding("UTF-16BE").GetString(str, offset, len * 2);
		}
		catch
		{
			throw new InvalidOperationException();
		}
	}

	public static string GetFromUnicodeBE(byte[] str)
	{
		if (str.Length == 0)
		{
			return "";
		}
		return GetFromUnicodeBE(str, 0, str.Length / 2);
	}

	public static string GetFromCompressedUnicode(byte[] str, int offset, int len)
	{
		int count = Math.Min(len, str.Length - offset);
		return ISO_8859_1.GetString(str, offset, count);
	}

	public static void PutCompressedUnicode(string input, byte[] output, int offset)
	{
		byte[] bytes = ISO_8859_1.GetBytes(input);
		Array.Copy(bytes, 0, output, offset, bytes.Length);
	}

	public static void PutCompressedUnicode(string input, ILittleEndianOutput out1)
	{
		byte[] bytes = ISO_8859_1.GetBytes(input);
		out1.Write(bytes);
	}

	public static void PutUnicodeLE(string input, byte[] output, int offset)
	{
		byte[] bytes = UTF16LE.GetBytes(input);
		Array.Copy(bytes, 0, output, offset, bytes.Length);
	}

	public static void PutUnicodeLE(string input, ILittleEndianOutput out1)
	{
		byte[] bytes = UTF16LE.GetBytes(input);
		out1.Write(bytes);
	}

	public static void PutUnicodeBE(string input, byte[] output, int offset)
	{
		try
		{
			byte[] bytes = Encoding.GetEncoding("UTF-16BE").GetBytes(input);
			Array.Copy(bytes, 0, output, offset, bytes.Length);
		}
		catch
		{
			throw new InvalidOperationException();
		}
	}

	public static string GetPreferredEncoding()
	{
		return ISO_8859_1.WebName;
	}

	public static bool HasMultibyte(string value)
	{
		if (value == null)
		{
			return false;
		}
		for (int i = 0; i < value.Length; i++)
		{
			if (value[i] > 'ÿ')
			{
				return true;
			}
		}
		return false;
	}

	public static string ReadCompressedUnicode(ILittleEndianInput in1, int nChars)
	{
		byte[] array = new byte[nChars];
		in1.ReadFully(array);
		return ISO_8859_1.GetString(array);
	}

	public static string ReadUnicodeLE(ILittleEndianInput in1, int nChars)
	{
		byte[] array = new byte[nChars * 2];
		in1.ReadFully(array);
		return UTF16LE.GetString(array);
	}

	public static string ReadUnicodeString(ILittleEndianInput in1)
	{
		int nChars = in1.ReadUShort();
		if (((byte)in1.ReadByte() & 1) == 0)
		{
			return ReadCompressedUnicode(in1, nChars);
		}
		return ReadUnicodeLE(in1, nChars);
	}

	public static string ReadUnicodeString(ILittleEndianInput in1, int nChars)
	{
		if (((byte)in1.ReadByte() & 1) == 0)
		{
			return ReadCompressedUnicode(in1, nChars);
		}
		return ReadUnicodeLE(in1, nChars);
	}

	public static void WriteUnicodeString(ILittleEndianOutput out1, string value)
	{
		int length = value.Length;
		out1.WriteShort(length);
		bool flag = HasMultibyte(value);
		out1.WriteByte(flag ? 1 : 0);
		if (flag)
		{
			PutUnicodeLE(value, out1);
		}
		else
		{
			PutCompressedUnicode(value, out1);
		}
	}

	public static void WriteUnicodeStringFlagAndData(ILittleEndianOutput out1, string value)
	{
		bool flag = HasMultibyte(value);
		out1.WriteByte(flag ? 1 : 0);
		if (flag)
		{
			PutUnicodeLE(value, out1);
		}
		else
		{
			PutCompressedUnicode(value, out1);
		}
	}

	public static int GetEncodedSize(string value)
	{
		return 3 + value.Length * ((!HasMultibyte(value)) ? 1 : 2);
	}

	public static bool IsUnicodeString(string value)
	{
		return !value.Equals(ISO_8859_1.GetString(ISO_8859_1.GetBytes(value)));
	}

	public static string ToHexString(string s)
	{
		char[] array = s.ToCharArray();
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			if (NeedToEncode(array[i]))
			{
				string value = ToHexString(array[i]);
				stringBuilder.Append(value);
			}
			else
			{
				stringBuilder.Append(array[i]);
			}
		}
		return stringBuilder.ToString();
	}

	public static string ToHexString(char chr)
	{
		return Convert.ToString(chr, 16);
	}

	public static string ToHexString(short chr)
	{
		return ToHexString((char)chr);
	}

	public static string ToHexString(int chr)
	{
		return ToHexString((char)chr);
	}

	public static string ToHexString(long chr)
	{
		return ToHexString((char)chr);
	}

	private static bool NeedToEncode(char chr)
	{
		string text = "$-_.+!*'(),@=&";
		if (chr > '\u007f')
		{
			return true;
		}
		if (char.IsLetterOrDigit(chr) || text.IndexOf(chr) >= 0)
		{
			return false;
		}
		return true;
	}

	public static string MapMsCodepointString(string string1)
	{
		if (string1 == null || "".Equals(string1))
		{
			return string1;
		}
		InitMsCodepointMap();
		StringBuilder stringBuilder = new StringBuilder();
		int length = string1.Length;
		int num;
		for (int i = 0; i < length; i += CharCount(num))
		{
			num = char.ConvertToUtf32(string1, i);
			int utf = msCodepointToUnicode[num];
			stringBuilder.Append(char.ConvertFromUtf32(utf));
		}
		return stringBuilder.ToString();
	}

	public static int toCodePoint(char high, char low)
	{
		return (int)(((uint)high << 10) + low + (65536 - ((uint)MIN_HIGH_SURROGATE << 10) - MIN_LOW_SURROGATE));
	}

	private static int codePointAt(char[] a, int index, int limit)
	{
		char c = a[index];
		if (char.IsHighSurrogate(c) && ++index < limit)
		{
			char c2 = a[index];
			if (char.IsLowSurrogate(c2))
			{
				return toCodePoint(c, c2);
			}
		}
		return c;
	}

	public static int CharCount(int codePoint)
	{
		if (codePoint < 65536)
		{
			return 1;
		}
		return 2;
	}

	public static void mapMsCodepoint(int msCodepoint, int unicodeCodepoint)
	{
		InitMsCodepointMap();
		msCodepointToUnicode.Add(msCodepoint, unicodeCodepoint);
	}

	private static void InitMsCodepointMap()
	{
		if (msCodepointToUnicode == null)
		{
			msCodepointToUnicode = new Dictionary<int, int>();
			int num = 61472;
			int[] array = symbolMap_f020;
			foreach (int value in array)
			{
				msCodepointToUnicode.Add(num++, value);
			}
			num = 61600;
			array = symbolMap_f0a0;
			foreach (int value2 in array)
			{
				msCodepointToUnicode.Add(num++, value2);
			}
		}
	}

	public static string Join(object[] array, string separator)
	{
		if (array.Length == 0)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(array[0]);
		for (int i = 1; i < array.Length; i++)
		{
			stringBuilder.Append(separator).Append(array[i]);
		}
		return stringBuilder.ToString();
	}

	public static string Join(string separator, params object[] array)
	{
		return Join(array, separator);
	}
}
