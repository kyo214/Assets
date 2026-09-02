using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class PaneRecord : StandardRecord
{
	public const short sid = 65;

	private short field_1_x;

	private short field_2_y;

	private short field_3_topRow;

	private short field_4_leftColumn;

	private short field_5_activePane;

	public const short ACTIVE_PANE_LOWER_RIGHT = 0;

	public const short ACTIVE_PANE_UPPER_RIGHT = 1;

	public const short ACTIVE_PANE_LOWER_LEFT = 2;

	public const short ACTIVE_PANE_UPPER_LEFT = 3;

	protected override int DataSize => 10;

	public override short Sid => 65;

	public short X
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

	public short Y
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

	public short TopRow
	{
		get
		{
			return field_3_topRow;
		}
		set
		{
			field_3_topRow = value;
		}
	}

	public short LeftColumn
	{
		get
		{
			return field_4_leftColumn;
		}
		set
		{
			field_4_leftColumn = value;
		}
	}

	public short ActivePane
	{
		get
		{
			return field_5_activePane;
		}
		set
		{
			field_5_activePane = value;
		}
	}

	public PaneRecord()
	{
	}

	public PaneRecord(RecordInputStream in1)
	{
		field_1_x = in1.ReadShort();
		field_2_y = in1.ReadShort();
		field_3_topRow = in1.ReadShort();
		field_4_leftColumn = in1.ReadShort();
		field_5_activePane = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[PANE]\n");
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
		stringBuilder.Append("    .topRow               = ").Append("0x").Append(HexDump.ToHex(TopRow))
			.Append(" (")
			.Append(TopRow)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .leftColumn           = ").Append("0x").Append(HexDump.ToHex(LeftColumn))
			.Append(" (")
			.Append(LeftColumn)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .activePane           = ").Append("0x").Append(HexDump.ToHex(ActivePane))
			.Append(" (")
			.Append(ActivePane)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/PANE]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_x);
		out1.WriteShort(field_2_y);
		out1.WriteShort(field_3_topRow);
		out1.WriteShort(field_4_leftColumn);
		out1.WriteShort(field_5_activePane);
	}

	public override object Clone()
	{
		return new PaneRecord
		{
			field_1_x = field_1_x,
			field_2_y = field_2_y,
			field_3_topRow = field_3_topRow,
			field_4_leftColumn = field_4_leftColumn,
			field_5_activePane = field_5_activePane
		};
	}
}
