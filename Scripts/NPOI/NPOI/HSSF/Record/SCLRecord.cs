using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class SCLRecord : StandardRecord
{
	public const short sid = 160;

	private short field_1_numerator;

	private short field_2_denominator;

	protected override int DataSize => 4;

	public override short Sid => 160;

	public short Numerator
	{
		get
		{
			return field_1_numerator;
		}
		set
		{
			field_1_numerator = value;
		}
	}

	public short Denominator
	{
		get
		{
			return field_2_denominator;
		}
		set
		{
			field_2_denominator = value;
		}
	}

	public SCLRecord()
	{
	}

	public SCLRecord(RecordInputStream in1)
	{
		field_1_numerator = in1.ReadShort();
		field_2_denominator = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[SCL]\n");
		stringBuilder.Append("    .numerator            = ").Append("0x").Append(HexDump.ToHex(Numerator))
			.Append(" (")
			.Append(Numerator)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .denominator          = ").Append("0x").Append(HexDump.ToHex(Denominator))
			.Append(" (")
			.Append(Denominator)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/SCL]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_numerator);
		out1.WriteShort(field_2_denominator);
	}

	public override object Clone()
	{
		return new SCLRecord
		{
			field_1_numerator = field_1_numerator,
			field_2_denominator = field_2_denominator
		};
	}
}
