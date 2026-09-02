using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class PlotGrowthRecord : StandardRecord
{
	public const short sid = 4196;

	private int field_1_horizontalScale;

	private int field_2_verticalScale;

	protected override int DataSize => 8;

	public override short Sid => 4196;

	public int HorizontalScale
	{
		get
		{
			return field_1_horizontalScale;
		}
		set
		{
			field_1_horizontalScale = value;
		}
	}

	public int VerticalScale
	{
		get
		{
			return field_2_verticalScale;
		}
		set
		{
			field_2_verticalScale = value;
		}
	}

	public PlotGrowthRecord()
	{
	}

	public PlotGrowthRecord(RecordInputStream in1)
	{
		field_1_horizontalScale = in1.ReadInt();
		field_2_verticalScale = in1.ReadInt();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[PLOTGROWTH]\n");
		stringBuilder.Append("    .horizontalScale      = ").Append("0x").Append(HexDump.ToHex(HorizontalScale))
			.Append(" (")
			.Append(HorizontalScale)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .verticalScale        = ").Append("0x").Append(HexDump.ToHex(VerticalScale))
			.Append(" (")
			.Append(VerticalScale)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/PLOTGROWTH]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteInt(field_1_horizontalScale);
		out1.WriteInt(field_2_verticalScale);
	}

	public override object Clone()
	{
		return new PlotGrowthRecord
		{
			field_1_horizontalScale = field_1_horizontalScale,
			field_2_verticalScale = field_2_verticalScale
		};
	}
}
