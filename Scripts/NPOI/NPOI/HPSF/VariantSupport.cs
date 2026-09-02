using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NPOI.Util;

namespace NPOI.HPSF;

public class VariantSupport : Variant
{
	private static bool logUnsupportedTypes = false;

	protected static List<long> unsupportedMessage;

	public static int[] SUPPORTED_TYPES = new int[10] { 0, 2, 3, 20, 5, 64, 30, 31, 71, 11 };

	public static bool IsLogUnsupportedTypes
	{
		get
		{
			return logUnsupportedTypes;
		}
		set
		{
			logUnsupportedTypes = value;
		}
	}

	public static void WriteUnsupportedTypeMessage(UnsupportedVariantTypeException ex)
	{
		if (IsLogUnsupportedTypes)
		{
			if (unsupportedMessage == null)
			{
				unsupportedMessage = new List<long>();
			}
			long variantType = ex.VariantType;
			if (!unsupportedMessage.Contains(variantType))
			{
				Console.Error.WriteLine(ex.Message);
				unsupportedMessage.Add(variantType);
			}
		}
	}

	public bool IsSupportedType(int variantType)
	{
		for (int i = 0; i < SUPPORTED_TYPES.Length; i++)
		{
			if (variantType == SUPPORTED_TYPES[i])
			{
				return true;
			}
		}
		return false;
	}

	public static object Read(byte[] src, int offset, int length, long type, int codepage)
	{
		TypedPropertyValue typedPropertyValue = new TypedPropertyValue((int)type, null);
		int num;
		try
		{
			num = typedPropertyValue.ReadValue(src, offset);
		}
		catch (InvalidOperationException)
		{
			int num2 = Math.Min(length, src.Length - offset);
			byte[] array = new byte[num2];
			System.Array.Copy(src, offset, array, 0, num2);
			throw new ReadingNotSupportedException(type, array);
		}
		switch ((int)type)
		{
		case 0:
		case 3:
		case 5:
		case 20:
			return typedPropertyValue.Value;
		case 2:
			return (short)typedPropertyValue.Value;
		case 64:
		{
			Filetime filetime = (Filetime)typedPropertyValue.Value;
			return Util.FiletimeToDate((int)filetime.High, (int)filetime.Low);
		}
		case 30:
			return ((CodePageString)typedPropertyValue.Value).GetJavaValue(codepage);
		case 31:
			return ((UnicodeString)typedPropertyValue.Value).ToJavaString();
		case 71:
			return ((ClipboardData)typedPropertyValue.Value).ToByteArray();
		case 11:
			return ((VariantBool)typedPropertyValue.Value).Value;
		default:
		{
			byte[] array2 = new byte[num];
			System.Array.Copy(src, offset, array2, 0, num);
			throw new ReadingNotSupportedException(type, array2);
		}
		}
	}

	public static string CodepageToEncoding(int codepage)
	{
		return CodePageUtil.CodepageToEncoding(codepage);
	}

	public static int Write(Stream out1, long type, object value, int codepage)
	{
		int num = 0;
		switch ((int)type)
		{
		case 11:
			_ = new byte[2];
			if ((bool)value)
			{
				out1.WriteByte(byte.MaxValue);
				out1.WriteByte(byte.MaxValue);
			}
			else
			{
				out1.WriteByte(0);
				out1.WriteByte(0);
			}
			num += 2;
			break;
		case 30:
		{
			CodePageString codePageString = new CodePageString((string)value, codepage);
			num += codePageString.Write(out1);
			break;
		}
		case 31:
		{
			int n2 = ((string)value).Length + 1;
			num += TypeWriter.WriteUIntToStream(out1, (uint)n2);
			char[] array3 = ((string)value).ToCharArray();
			for (int i = 0; i < array3.Length; i++)
			{
				int num3 = (array3[i] & 0xFF00) >> 8;
				int num4 = array3[i] & 0xFF;
				byte value2 = (byte)num3;
				byte value3 = (byte)num4;
				out1.WriteByte(value3);
				out1.WriteByte(value2);
				num += 2;
			}
			out1.WriteByte(0);
			out1.WriteByte(0);
			num += 2;
			break;
		}
		case 71:
		{
			byte[] array2 = (byte[])value;
			out1.Write(array2, 0, array2.Length);
			num = array2.Length;
			break;
		}
		case 0:
			num += TypeWriter.WriteUIntToStream(out1, 0u);
			break;
		case 2:
		{
			short n;
			try
			{
				n = Convert.ToInt16(value, CultureInfo.InvariantCulture);
			}
			catch (OverflowException)
			{
				n = (short)(int)value;
			}
			num += TypeWriter.WriteToStream(out1, n);
			break;
		}
		case 3:
			if (!(value is int))
			{
				throw new Exception("Could not cast an object To int: " + value.GetType().Name + ", " + value.ToString());
			}
			num += TypeWriter.WriteToStream(out1, (int)value);
			break;
		case 20:
			num += TypeWriter.WriteToStream(out1, Convert.ToInt64(value, CultureInfo.CurrentCulture));
			break;
		case 5:
			num += TypeWriter.WriteToStream(out1, (double)value);
			break;
		case 64:
		{
			long num2 = ((value == null) ? 0 : Util.DateToFileTime((DateTime)value));
			int high = (int)((num2 >> 32) & 0xFFFFFFFFu);
			Filetime filetime = new Filetime((int)(num2 & 0xFFFFFFFFu), high);
			num += filetime.Write(out1);
			break;
		}
		default:
			if (value is byte[])
			{
				byte[] array = (byte[])value;
				out1.Write(array, 0, array.Length);
				num = array.Length;
				WriteUnsupportedTypeMessage(new WritingNotSupportedException(type, value));
				break;
			}
			throw new WritingNotSupportedException(type, value);
		}
		for (; (num & 3) != 0; num++)
		{
			out1.WriteByte(0);
		}
		return num;
	}
}
