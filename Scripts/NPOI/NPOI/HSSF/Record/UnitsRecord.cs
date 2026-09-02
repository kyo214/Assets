using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class UnitsRecord : StandardRecord
{
	public const short sid = 4097;

	private short field_1_units;

	protected override int DataSize => 2;

	public override short Sid => 4097;

	public short Units
	{
		get
		{
			return field_1_units;
		}
		set
		{
			field_1_units = value;
		}
	}

	public UnitsRecord()
	{
	}

	public UnitsRecord(RecordInputStream in1)
	{
		field_1_units = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[UNITS]\n");
		stringBuilder.Append("    .units                = ").Append("0x").Append(HexDump.ToHex(Units))
			.Append(" (")
			.Append(Units)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/UNITS]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_units);
	}

	public override object Clone()
	{
		return new UnitsRecord
		{
			field_1_units = field_1_units
		};
	}
}
