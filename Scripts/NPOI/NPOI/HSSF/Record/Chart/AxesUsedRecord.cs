using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class AxesUsedRecord : StandardRecord
{
	public const short sid = 4166;

	private short field_1_numAxis;

	protected override int DataSize => 2;

	public override short Sid => 4166;

	public short NumAxis
	{
		get
		{
			return field_1_numAxis;
		}
		set
		{
			field_1_numAxis = value;
		}
	}

	public AxesUsedRecord()
	{
	}

	public AxesUsedRecord(RecordInputStream in1)
	{
		field_1_numAxis = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[AXISUSED]\n");
		stringBuilder.Append("    .numAxis              = ").Append("0x").Append(HexDump.ToHex(NumAxis))
			.Append(" (")
			.Append(NumAxis)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/AXISUSED]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_numAxis);
	}

	public override object Clone()
	{
		return new AxesUsedRecord
		{
			field_1_numAxis = field_1_numAxis
		};
	}
}
