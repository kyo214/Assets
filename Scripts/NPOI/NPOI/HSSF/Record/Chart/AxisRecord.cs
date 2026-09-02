using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class AxisRecord : StandardRecord, ICloneable
{
	public const short sid = 4125;

	private short field_1_axisType;

	public const short AXIS_TYPE_CATEGORY_OR_X_AXIS = 0;

	public const short AXIS_TYPE_VALUE_AXIS = 1;

	public const short AXIS_TYPE_SERIES_AXIS = 2;

	private int field_2_reserved1;

	private int field_3_reserved2;

	private int field_4_reserved3;

	private int field_5_reserved4;

	protected override int DataSize => 18;

	public override short Sid => 4125;

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

	public int Reserved1
	{
		get
		{
			return field_2_reserved1;
		}
		set
		{
			field_2_reserved1 = value;
		}
	}

	public int Reserved2
	{
		get
		{
			return field_3_reserved2;
		}
		set
		{
			field_3_reserved2 = value;
		}
	}

	public int Reserved3
	{
		get
		{
			return field_4_reserved3;
		}
		set
		{
			field_4_reserved3 = value;
		}
	}

	public int Reserved4
	{
		get
		{
			return field_5_reserved4;
		}
		set
		{
			field_5_reserved4 = value;
		}
	}

	public AxisRecord()
	{
	}

	public AxisRecord(RecordInputStream in1)
	{
		field_1_axisType = in1.ReadShort();
		field_2_reserved1 = in1.ReadInt();
		field_3_reserved2 = in1.ReadInt();
		field_4_reserved3 = in1.ReadInt();
		field_5_reserved4 = in1.ReadInt();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[AXIS]\n");
		stringBuilder.Append("    .axisType             = ").Append("0x").Append(HexDump.ToHex(AxisType))
			.Append(" (")
			.Append(AxisType)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .reserved1            = ").Append("0x").Append(HexDump.ToHex(Reserved1))
			.Append(" (")
			.Append(Reserved1)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .reserved2            = ").Append("0x").Append(HexDump.ToHex(Reserved2))
			.Append(" (")
			.Append(Reserved2)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .reserved3            = ").Append("0x").Append(HexDump.ToHex(Reserved3))
			.Append(" (")
			.Append(Reserved3)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .reserved4            = ").Append("0x").Append(HexDump.ToHex(Reserved4))
			.Append(" (")
			.Append(Reserved4)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/AXIS]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_axisType);
		out1.WriteInt(field_2_reserved1);
		out1.WriteInt(field_3_reserved2);
		out1.WriteInt(field_4_reserved3);
		out1.WriteInt(field_5_reserved4);
	}

	public override object Clone()
	{
		return new AxisRecord
		{
			field_1_axisType = field_1_axisType,
			field_2_reserved1 = field_2_reserved1,
			field_3_reserved2 = field_3_reserved2,
			field_4_reserved3 = field_4_reserved3,
			field_5_reserved4 = field_5_reserved4
		};
	}
}
