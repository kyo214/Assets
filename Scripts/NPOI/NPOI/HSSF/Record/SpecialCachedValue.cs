using System;
using System.Text;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.HSSF.Record;

internal class SpecialCachedValue
{
	private const long BIT_MARKER = -281474976710656L;

	private const int VARIABLE_DATA_LENGTH = 6;

	private const int DATA_INDEX = 2;

	public const int STRING = 0;

	public const int BOOLEAN = 1;

	public const int ERROR_CODE = 2;

	public const int EMPTY = 3;

	private byte[] _variableData;

	public string FormatDebugString => FormatValue + " " + HexDump.ToHex(_variableData);

	private string FormatValue
	{
		get
		{
			int typeCode = GetTypeCode();
			switch (typeCode)
			{
			case 0:
				return "<string>";
			case 1:
				if (DataValue != 0)
				{
					return "TRUE";
				}
				return "FALSE";
			case 2:
				return ErrorEval.GetText(DataValue);
			case 3:
				return "<empty>";
			default:
				return "#error(type=" + typeCode + ")#";
			}
		}
	}

	private int DataValue => _variableData[2];

	private SpecialCachedValue(byte[] data)
	{
		_variableData = data;
	}

	public int GetTypeCode()
	{
		return _variableData[0];
	}

	public static SpecialCachedValue Create(long valueLongBits)
	{
		if ((-281474976710656L & valueLongBits) != -281474976710656L)
		{
			return null;
		}
		byte[] array = new byte[6];
		long num = valueLongBits;
		for (int i = 0; i < 6; i++)
		{
			array[i] = (byte)num;
			num >>= 8;
		}
		byte b = array[0];
		if ((uint)b > 3u)
		{
			throw new RecordFormatException("Bad special value code (" + array[0] + ")");
		}
		return new SpecialCachedValue(array);
	}

	public void Serialize(ILittleEndianOutput out1)
	{
		out1.Write(_variableData);
		out1.WriteShort(65535);
	}

	public static SpecialCachedValue CreateCachedEmptyValue()
	{
		return Create(3, 0);
	}

	public static SpecialCachedValue CreateForString()
	{
		return Create(0, 0);
	}

	public static SpecialCachedValue CreateCachedBoolean(bool b)
	{
		return Create(1, b ? 1 : 0);
	}

	public static SpecialCachedValue CreateCachedErrorCode(int errorCode)
	{
		return Create(2, errorCode);
	}

	private static SpecialCachedValue Create(int code, int data)
	{
		return new SpecialCachedValue(new byte[6]
		{
			(byte)code,
			0,
			(byte)data,
			0,
			0,
			0
		});
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		stringBuilder.Append(GetType().Name);
		stringBuilder.Append('[').Append(FormatValue).Append(']');
		return stringBuilder.ToString();
	}

	public CellType GetValueType()
	{
		int typeCode = GetTypeCode();
		return typeCode switch
		{
			0 => CellType.String, 
			1 => CellType.Boolean, 
			2 => CellType.Error, 
			3 => CellType.String, 
			_ => throw new InvalidOperationException("Unexpected type id (" + typeCode + ")"), 
		};
	}

	public bool GetBooleanValue()
	{
		if (GetTypeCode() != 1)
		{
			throw new InvalidOperationException("Not a bool cached value - " + FormatValue);
		}
		return DataValue != 0;
	}

	public int GetErrorValue()
	{
		if (GetTypeCode() != 2)
		{
			throw new InvalidOperationException("Not an error cached value - " + FormatValue);
		}
		return DataValue;
	}
}
