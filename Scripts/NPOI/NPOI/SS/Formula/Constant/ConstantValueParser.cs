using System;
using NPOI.Util;

namespace NPOI.SS.Formula.Constant;

public class ConstantValueParser
{
	private const int TYPE_EMPTY = 0;

	private const int TYPE_NUMBER = 1;

	private const int TYPE_STRING = 2;

	private const int TYPE_BOOLEAN = 4;

	private const int TYPE_ERROR_CODE = 16;

	private const int TRUE_ENCODING = 1;

	private const int FALSE_ENCODING = 0;

	private const object EMPTY_REPRESENTATION = null;

	private ConstantValueParser()
	{
	}

	public static object[] Parse(ILittleEndianInput in1, int nValues)
	{
		object[] array = new object[nValues];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = ReadAConstantValue(in1);
		}
		return array;
	}

	private static object ReadAConstantValue(ILittleEndianInput in1)
	{
		byte b = (byte)in1.ReadByte();
		switch (b)
		{
		case 0:
			in1.ReadLong();
			return null;
		case 1:
			return in1.ReadDouble();
		case 2:
			return StringUtil.ReadUnicodeString(in1);
		case 4:
			return ReadBoolean(in1);
		case 16:
		{
			int errorCode = in1.ReadUShort();
			in1.ReadUShort();
			in1.ReadInt();
			return ErrorConstant.ValueOf(errorCode);
		}
		default:
			throw new Exception("Unknown grbit value (" + b + ")");
		}
	}

	private static object ReadBoolean(ILittleEndianInput in1)
	{
		byte b = (byte)in1.ReadLong();
		return b switch
		{
			0 => false, 
			1 => true, 
			_ => throw new Exception("unexpected bool encoding (" + b + ")"), 
		};
	}

	public static int GetEncodedSize(Array values)
	{
		int num = values.Length;
		for (int i = 0; i < values.Length; i++)
		{
			num += GetEncodedSize(values.GetValue(i));
		}
		return num;
	}

	private static int GetEncodedSize(object obj)
	{
		if (obj == null)
		{
			return 8;
		}
		Type type = obj.GetType();
		if (type == typeof(bool) || type == typeof(double) || type == typeof(ErrorConstant))
		{
			return 8;
		}
		return StringUtil.GetEncodedSize((string)obj);
	}

	public static void Encode(ILittleEndianOutput out1, Array values)
	{
		for (int i = 0; i < values.Length; i++)
		{
			EncodeSingleValue(out1, values.GetValue(i));
		}
	}

	private static void EncodeSingleValue(ILittleEndianOutput out1, object value)
	{
		if (value == null)
		{
			out1.WriteByte(0);
			out1.WriteLong(0L);
			return;
		}
		if (value is int num)
		{
			out1.WriteByte(4);
			long v = ((num != 0) ? 1 : 0);
			out1.WriteLong(v);
			return;
		}
		if (value is double v2)
		{
			out1.WriteByte(1);
			out1.WriteDouble(v2);
			return;
		}
		if (value is string)
		{
			string value2 = (string)value;
			out1.WriteByte(2);
			StringUtil.WriteUnicodeString(out1, value2);
			return;
		}
		if (value is ErrorConstant)
		{
			ErrorConstant obj = (ErrorConstant)value;
			out1.WriteByte(16);
			long v3 = obj.ErrorCode;
			out1.WriteLong(v3);
			return;
		}
		throw new Exception("Unexpected value type (" + value.GetType().Name + "'");
	}
}
