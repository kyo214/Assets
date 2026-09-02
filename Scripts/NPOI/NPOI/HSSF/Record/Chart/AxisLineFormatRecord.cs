using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class AxisLineFormatRecord : StandardRecord, ICloneable
{
	public const short sid = 4129;

	private short field_1_axisType;

	public static short AXIS_TYPE_AXIS_LINE = 0;

	public static short AXIS_TYPE_MAJOR_GRID_LINE = 1;

	public static short AXIS_TYPE_MINOR_GRID_LINE = 2;

	public static short AXIS_TYPE_WALLS_OR_FLOOR = 3;

	protected override int DataSize => 2;

	public override short Sid => 4129;

	public short AxisType
	{
		get
		{
			return field_1_axisType;
		}
		set
		{
			field_1_axisType = value;
		}
	}

	public AxisLineFormatRecord()
	{
	}

	public AxisLineFormatRecord(RecordInputStream in1)
	{
		field_1_axisType = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[AXISLINEFORMAT]\n");
		stringBuilder.Append("    .axisType             = ").Append("0x").Append(HexDump.ToHex(AxisType))
			.Append(" (")
			.Append(AxisType)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/AXISLINEFORMAT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_axisType);
	}

	public override object Clone()
	{
		return new AxisLineFormatRecord
		{
			field_1_axisType = field_1_axisType
		};
	}
}
