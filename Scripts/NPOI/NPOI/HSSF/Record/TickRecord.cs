using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class TickRecord : StandardRecord
{
	public const short sid = 4126;

	private byte field_1_majorTickType;

	private byte field_2_minorTickType;

	private byte field_3_labelPosition;

	private byte field_4_background;

	private int field_5_labelColorRgb;

	private int field_6_zero1;

	private int field_7_zero2;

	private int field_8_zero3;

	private int field_9_zero4;

	private short field_10_options;

	private BitField autoTextColor = BitFieldFactory.GetInstance(1);

	private BitField autoTextBackground = BitFieldFactory.GetInstance(2);

	private BitField rotation = BitFieldFactory.GetInstance(28);

	private BitField autorotate = BitFieldFactory.GetInstance(32);

	private short field_11_tickColor;

	private short field_12_zero5;

	protected override int DataSize => 30;

	public override short Sid => 4126;

	public byte MajorTickType
	{
		get
		{
			return field_1_majorTickType;
		}
		set
		{
			field_1_majorTickType = value;
		}
	}

	public byte MinorTickType
	{
		get
		{
			return field_2_minorTickType;
		}
		set
		{
			field_2_minorTickType = value;
		}
	}

	public byte LabelPosition
	{
		get
		{
			return field_3_labelPosition;
		}
		set
		{
			field_3_labelPosition = value;
		}
	}

	public byte Background
	{
		get
		{
			return field_4_background;
		}
		set
		{
			field_4_background = value;
		}
	}

	public int LabelColorRgb
	{
		get
		{
			return field_5_labelColorRgb;
		}
		set
		{
			field_5_labelColorRgb = value;
		}
	}

	public int Zero1
	{
		get
		{
			return field_6_zero1;
		}
		set
		{
			field_6_zero1 = value;
		}
	}

	public int Zero2
	{
		get
		{
			return field_7_zero2;
		}
		set
		{
			field_7_zero2 = value;
		}
	}

	public short Options
	{
		get
		{
			return field_10_options;
		}
		set
		{
			field_10_options = value;
		}
	}

	public short TickColor
	{
		get
		{
			return field_11_tickColor;
		}
		set
		{
			field_11_tickColor = value;
		}
	}

	public short Zero3
	{
		get
		{
			return field_12_zero5;
		}
		set
		{
			field_12_zero5 = value;
		}
	}

	public bool IsAutoTextColor
	{
		get
		{
			return autoTextColor.IsSet(field_10_options);
		}
		set
		{
			field_10_options = autoTextColor.SetShortBoolean(field_10_options, value);
		}
	}

	public bool IsAutoTextBackground
	{
		get
		{
			return autoTextBackground.IsSet(field_10_options);
		}
		set
		{
			field_10_options = autoTextBackground.SetShortBoolean(field_10_options, value);
		}
	}

	public short Rotation
	{
		get
		{
			return rotation.GetShortValue(field_10_options);
		}
		set
		{
			field_10_options = rotation.SetShortValue(field_10_options, value);
		}
	}

	public bool IsAutorotate
	{
		get
		{
			return autorotate.IsSet(field_10_options);
		}
		set
		{
			field_10_options = autorotate.SetShortBoolean(field_10_options, value);
		}
	}

	public TickRecord()
	{
	}

	public TickRecord(RecordInputStream in1)
	{
		field_1_majorTickType = (byte)in1.ReadByte();
		field_2_minorTickType = (byte)in1.ReadByte();
		field_3_labelPosition = (byte)in1.ReadByte();
		field_4_background = (byte)in1.ReadByte();
		field_5_labelColorRgb = (byte)in1.ReadInt();
		field_6_zero1 = in1.ReadInt();
		field_7_zero2 = in1.ReadInt();
		field_8_zero3 = in1.ReadInt();
		field_9_zero4 = in1.ReadInt();
		field_10_options = in1.ReadShort();
		field_11_tickColor = in1.ReadShort();
		field_12_zero5 = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[TICK]\n");
		stringBuilder.Append("    .majorTickType        = ").Append("0x").Append(HexDump.ToHex(MajorTickType))
			.Append(" (")
			.Append(MajorTickType)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .minorTickType        = ").Append("0x").Append(HexDump.ToHex(MinorTickType))
			.Append(" (")
			.Append(MinorTickType)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .labelPosition        = ").Append("0x").Append(HexDump.ToHex(LabelPosition))
			.Append(" (")
			.Append(LabelPosition)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .background           = ").Append("0x").Append(HexDump.ToHex(Background))
			.Append(" (")
			.Append(Background)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .labelColorRgb        = ").Append("0x").Append(HexDump.ToHex(LabelColorRgb))
			.Append(" (")
			.Append(LabelColorRgb)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .zero1                = ").Append("0x").Append(HexDump.ToHex(Zero1))
			.Append(" (")
			.Append(Zero1)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .zero2                = ").Append("0x").Append(HexDump.ToHex(Zero2))
			.Append(" (")
			.Append(Zero2)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .options              = ").Append("0x").Append(HexDump.ToHex(Options))
			.Append(" (")
			.Append(Options)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("         .autoTextColor            = ").Append(IsAutoTextColor).Append('\n');
		stringBuilder.Append("         .autoTextBackground       = ").Append(IsAutoTextBackground).Append('\n');
		stringBuilder.Append("         .rotation                 = ").Append(Rotation).Append('\n');
		stringBuilder.Append("         .autorotate               = ").Append(IsAutorotate).Append('\n');
		stringBuilder.Append("    .tickColor            = ").Append("0x").Append(HexDump.ToHex(TickColor))
			.Append(" (")
			.Append(TickColor)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .zero3                = ").Append("0x").Append(HexDump.ToHex(Zero3))
			.Append(" (")
			.Append(Zero3)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/TICK]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteByte(field_1_majorTickType);
		out1.WriteByte(field_2_minorTickType);
		out1.WriteByte(field_3_labelPosition);
		out1.WriteByte(field_4_background);
		out1.WriteInt(field_5_labelColorRgb);
		out1.WriteInt(field_6_zero1);
		out1.WriteInt(field_7_zero2);
		out1.WriteInt(field_8_zero3);
		out1.WriteInt(field_9_zero4);
		out1.WriteShort(field_10_options);
		out1.WriteShort(field_11_tickColor);
		out1.WriteShort(field_12_zero5);
	}

	public override object Clone()
	{
		return new TickRecord
		{
			field_1_majorTickType = field_1_majorTickType,
			field_2_minorTickType = field_2_minorTickType,
			field_3_labelPosition = field_3_labelPosition,
			field_4_background = field_4_background,
			field_5_labelColorRgb = field_5_labelColorRgb,
			field_6_zero1 = field_6_zero1,
			field_7_zero2 = field_7_zero2,
			field_8_zero3 = field_8_zero3,
			field_9_zero4 = field_9_zero4,
			field_10_options = field_10_options,
			field_11_tickColor = field_11_tickColor,
			field_12_zero5 = field_12_zero5
		};
	}
}
