using System;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class BoolErrRecord : CellRecord, ICloneable
{
	public const short sid = 517;

	private int _value;

	private bool _isError;

	public bool BooleanValue => _value != 0;

	public byte ErrorValue => (byte)_value;

	public bool IsBoolean => !_isError;

	public bool IsError => _isError;

	protected override string RecordName => "BOOLERR";

	protected override int ValueDataSize => 2;

	public override short Sid => 517;

	public BoolErrRecord()
	{
	}

	public BoolErrRecord(RecordInputStream in1)
		: base(in1)
	{
		switch (in1.Remaining)
		{
		case 2:
			_value = in1.ReadByte();
			break;
		case 3:
			_value = in1.ReadUShort();
			break;
		default:
			throw new RecordFormatException("Unexpected size (" + in1.Remaining + ") for BOOLERR record.");
		}
		int num = in1.ReadUByte();
		switch (num)
		{
		case 0:
			_isError = false;
			break;
		case 1:
			_isError = true;
			break;
		default:
			throw new RecordFormatException("Unexpected isError flag (" + num + ") for BOOLERR record.");
		}
	}

	public void SetValue(bool value)
	{
		_value = (value ? 1 : 0);
		_isError = false;
	}

	public void SetValue(byte value)
	{
		SetValue(FormulaError.ForInt(value));
	}

	public void SetValue(FormulaError value)
	{
		switch ((FormulaErrorEnum)value.Code)
		{
		case FormulaErrorEnum.NULL:
		case FormulaErrorEnum.DIV_0:
		case FormulaErrorEnum.VALUE:
		case FormulaErrorEnum.REF:
		case FormulaErrorEnum.NAME:
		case FormulaErrorEnum.NUM:
		case FormulaErrorEnum.NA:
			_value = value.Code;
			_isError = true;
			break;
		default:
			throw new ArgumentException("Error Value can only be 0,7,15,23,29,36 or 42. It cannot be " + value);
		}
	}

	protected override void AppendValueText(StringBuilder buffer)
	{
		if (IsBoolean)
		{
			buffer.Append("    .boolValue   = ").Append(BooleanValue).Append("\n");
		}
		else
		{
			buffer.Append("    .errCode     = ").Append(FormulaError.ForInt(ErrorValue).String).Append("\n");
		}
	}

	protected override void SerializeValue(ILittleEndianOutput out1)
	{
		out1.WriteByte(_value);
		out1.WriteByte(_isError ? 1 : 0);
	}

	public override object Clone()
	{
		BoolErrRecord boolErrRecord = new BoolErrRecord();
		CopyBaseFields(boolErrRecord);
		boolErrRecord._value = _value;
		boolErrRecord._isError = _isError;
		return boolErrRecord;
	}
}
