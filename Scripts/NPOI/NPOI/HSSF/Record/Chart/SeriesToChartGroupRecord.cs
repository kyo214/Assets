using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

[Obsolete("duplication record, see SerToCrtRecord")]
public class SeriesToChartGroupRecord : StandardRecord
{
	public const short sid = 4165;

	private short field_1_chartGroupIndex;

	protected override int DataSize => 2;

	public override short Sid => 4165;

	public short ChartGroupIndex
	{
		get
		{
			return field_1_chartGroupIndex;
		}
		set
		{
			field_1_chartGroupIndex = value;
		}
	}

	public SeriesToChartGroupRecord()
	{
	}

	public SeriesToChartGroupRecord(RecordInputStream in1)
	{
		field_1_chartGroupIndex = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[SeriesToChartGroup]\n");
		stringBuilder.Append("    .chartGroupIndex      = ").Append("0x").Append(HexDump.ToHex(ChartGroupIndex))
			.Append(" (")
			.Append(ChartGroupIndex)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/SeriesToChartGroup]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_chartGroupIndex);
	}

	public override object Clone()
	{
		return new SeriesToChartGroupRecord
		{
			field_1_chartGroupIndex = field_1_chartGroupIndex
		};
	}
}
