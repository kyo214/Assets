using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class ChartRecord : StandardRecord, ICloneable
{
	public const short sid = 4098;

	private int field_1_x;

	private int field_2_y;

	private int field_3_width;

	private int field_4_height;

	protected override int DataSize => 16;

	public override short Sid => 4098;

	public int X
	{
		get
		{
			return field_1_x;
		}
		set
		{
			field_1_x = value;
		}
	}

	public int Y
	{
		get
		{
			return field_2_y;
		}
		set
		{
			field_2_y = value;
		}
	}

	public int Width
	{
		get
		{
			return field_3_width;
		}
		set
		{
			field_3_width = value;
		}
	}

	public int Height
	{
		get
		{
			return field_4_height;
		}
		set
		{
			field_4_height = value;
		}
	}

	public ChartRecord()
	{
	}

	public ChartRecord(RecordInputStream in1)
	{
		field_1_x = in1.ReadInt();
		field_2_y = in1.ReadInt();
		field_3_width = in1.ReadInt();
		field_4_height = in1.ReadInt();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[CHART]\n");
		stringBuilder.Append("    .x                    = ").Append("0x").Append(HexDump.ToHex(X))
			.Append(" (")
			.Append(X)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .y                    = ").Append("0x").Append(HexDump.ToHex(Y))
			.Append(" (")
			.Append(Y)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .width                = ").Append("0x").Append(HexDump.ToHex(Width))
			.Append(" (")
			.Append(Width)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .height               = ").Append("0x").Append(HexDump.ToHex(Height))
			.Append(" (")
			.Append(Height)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/CHART]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteInt(field_1_x);
		out1.WriteInt(field_2_y);
		out1.WriteInt(field_3_width);
		out1.WriteInt(field_4_height);
	}

	public override object Clone()
	{
		return new ChartRecord
		{
			field_1_x = field_1_x,
			field_2_y = field_2_y,
			field_3_width = field_3_width,
			field_4_height = field_4_height
		};
	}
}
