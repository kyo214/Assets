using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class TextRecord : StandardRecord
{
	public const short sid = 4133;

	private byte field_1_horizontalAlignment;

	public const byte HORIZONTAL_ALIGNMENT_LEFT = 1;

	public const byte HORIZONTAL_ALIGNMENT_CENTER = 2;

	public const byte HORIZONTAL_ALIGNMENT_BOTTOM = 3;

	public const byte HORIZONTAL_ALIGNMENT_JUSTIFY = 4;

	public const byte HORIZONTAL_ALIGNMENT_DISTRIBUTED = 7;

	private byte field_2_verticalAlignment;

	public const byte VERTICAL_ALIGNMENT_TOP = 1;

	public const byte VERTICAL_ALIGNMENT_CENTER = 2;

	public const byte VERTICAL_ALIGNMENT_BOTTOM = 3;

	public const byte VERTICAL_ALIGNMENT_JUSTIFY = 4;

	public const byte VERTICAL_ALIGNMENT_DISTRIBUTED = 7;

	private short field_3_DisplayMode;

	public const short DISPLAY_MODE_TRANSPARENT = 1;

	public const short DISPLAY_MODE_OPAQUE = 2;

	private int field_4_rgbColor;

	private int field_5_x;

	private int field_6_y;

	private int field_7_width;

	private int field_8_height;

	private short field_9_options1;

	private BitField autoColor = BitFieldFactory.GetInstance(1);

	private BitField showKey = BitFieldFactory.GetInstance(2);

	private BitField showValue = BitFieldFactory.GetInstance(4);

	private BitField autoText = BitFieldFactory.GetInstance(16);

	private BitField generated = BitFieldFactory.GetInstance(32);

	private BitField autoLabelDeleted = BitFieldFactory.GetInstance(64);

	private BitField autoBackground = BitFieldFactory.GetInstance(128);

	public const short ROTATION_NONE = 0;

	public const short ROTATION_TOP_TO_BOTTOM = 1;

	public const short ROTATION_ROTATED_90_DEGREES = 2;

	public const short ROTATION_ROTATED_90_DEGREES_CLOCKWISE = 3;

	private BitField showCategoryLabelAsPercentage = BitFieldFactory.GetInstance(2048);

	private BitField showValueAsPercentage = BitFieldFactory.GetInstance(4096);

	private BitField showBubbleSizes = BitFieldFactory.GetInstance(8192);

	private BitField showLabel = BitFieldFactory.GetInstance(16384);

	private short field_10_IndexOfColorValue;

	private short field_11_options2;

	private BitField dataLabelPlacement = BitFieldFactory.GetInstance(15);

	public const short DATA_LABEL_PLACEMENT_CHART_DEPENDENT = 0;

	public const short DATA_LABEL_PLACEMENT_OUTSIDE = 1;

	public const short DATA_LABEL_PLACEMENT_INSIDE = 2;

	public const short DATA_LABEL_PLACEMENT_CENTER = 3;

	public const short DATA_LABEL_PLACEMENT_AXIS = 4;

	public const short DATA_LABEL_PLACEMENT_ABOVE = 5;

	public const short DATA_LABEL_PLACEMENT_BELOW = 6;

	public const short DATA_LABEL_PLACEMENT_LEFT = 7;

	public const short DATA_LABEL_PLACEMENT_RIGHT = 8;

	public const short DATA_LABEL_PLACEMENT_AUTO = 9;

	public const short DATA_LABEL_PLACEMENT_USER_MOVED = 10;

	private BitField readingOrder = BitFieldFactory.GetInstance(49152);

	public const short READING_ORDER_CONTEXT = 0;

	public const short READING_ORDER_LTR = 1;

	public const short READING_ORDER_RTL = 2;

	private short field_12_textRotation;

	protected override int DataSize => 32;

	public override short Sid => 4133;

	public byte HorizontalAlignment
	{
		get
		{
			return field_1_horizontalAlignment;
		}
		set
		{
			field_1_horizontalAlignment = value;
		}
	}

	public byte VerticalAlignment
	{
		get
		{
			return field_2_verticalAlignment;
		}
		set
		{
			field_2_verticalAlignment = value;
		}
	}

	public short DisplayMode
	{
		get
		{
			return field_3_DisplayMode;
		}
		set
		{
			field_3_DisplayMode = value;
		}
	}

	public int RgbColor
	{
		get
		{
			return field_4_rgbColor;
		}
		set
		{
			field_4_rgbColor = value;
		}
	}

	public int X
	{
		get
		{
			return field_5_x;
		}
		set
		{
			field_5_x = value;
		}
	}

	public int Y
	{
		get
		{
			return field_6_y;
		}
		set
		{
			field_6_y = value;
		}
	}

	public int Width
	{
		get
		{
			return field_7_width;
		}
		set
		{
			field_7_width = value;
		}
	}

	public int Height
	{
		get
		{
			return field_8_height;
		}
		set
		{
			field_8_height = value;
		}
	}

	public short Options1
	{
		get
		{
			return field_9_options1;
		}
		set
		{
			field_9_options1 = value;
		}
	}

	public short IndexOfColorValue
	{
		get
		{
			return field_10_IndexOfColorValue;
		}
		set
		{
			field_10_IndexOfColorValue = value;
		}
	}

	public short Options2
	{
		get
		{
			return field_11_options2;
		}
		set
		{
			field_11_options2 = value;
		}
	}

	public short TextRotation
	{
		get
		{
			return field_12_textRotation;
		}
		set
		{
			field_12_textRotation = value;
		}
	}

	public bool IsAutoColor
	{
		get
		{
			return autoColor.IsSet(field_9_options1);
		}
		set
		{
			field_9_options1 = autoColor.SetShortBoolean(field_9_options1, value);
		}
	}

	public bool ShowKey
	{
		get
		{
			return showKey.IsSet(field_9_options1);
		}
		set
		{
			field_9_options1 = showKey.SetShortBoolean(field_9_options1, value);
		}
	}

	public bool ShowValue
	{
		get
		{
			return showValue.IsSet(field_9_options1);
		}
		set
		{
			field_9_options1 = showValue.SetShortBoolean(field_9_options1, value);
		}
	}

	public bool IsAutoGeneratedText
	{
		get
		{
			return autoText.IsSet(field_9_options1);
		}
		set
		{
			field_9_options1 = autoText.SetShortBoolean(field_9_options1, value);
		}
	}

	public bool IsGenerated
	{
		get
		{
			return generated.IsSet(field_9_options1);
		}
		set
		{
			field_9_options1 = generated.SetShortBoolean(field_9_options1, value);
		}
	}

	public bool IsAutoLabelDeleted
	{
		get
		{
			return autoLabelDeleted.IsSet(field_9_options1);
		}
		set
		{
			field_9_options1 = autoLabelDeleted.SetShortBoolean(field_9_options1, value);
		}
	}

	public bool IsAutoBackground
	{
		get
		{
			return autoBackground.IsSet(field_9_options1);
		}
		set
		{
			field_9_options1 = autoBackground.SetShortBoolean(field_9_options1, value);
		}
	}

	public bool ShowCategoryLabelAsPercentage
	{
		get
		{
			return showCategoryLabelAsPercentage.IsSet(field_9_options1);
		}
		set
		{
			showCategoryLabelAsPercentage.SetShortBoolean(field_9_options1, value);
		}
	}

	public bool ShowValueAsPercentage
	{
		get
		{
			return showValueAsPercentage.IsSet(field_9_options1);
		}
		set
		{
			field_9_options1 = showValueAsPercentage.SetShortBoolean(field_9_options1, value);
		}
	}

	public bool ShowBubbleSizes
	{
		get
		{
			return showBubbleSizes.IsSet(field_9_options1);
		}
		set
		{
			field_9_options1 = showBubbleSizes.SetShortBoolean(field_9_options1, value);
		}
	}

	public bool ShowLabel
	{
		get
		{
			return showLabel.IsSet(field_9_options1);
		}
		set
		{
			field_9_options1 = showLabel.SetShortBoolean(field_9_options1, value);
		}
	}

	public short DataLabelPlacement
	{
		get
		{
			return dataLabelPlacement.GetShortValue(field_11_options2);
		}
		set
		{
			field_11_options2 = dataLabelPlacement.SetShortValue(field_11_options2, value);
		}
	}

	public short ReadingOrder
	{
		get
		{
			return readingOrder.GetShortValue(field_11_options2);
		}
		set
		{
			field_11_options2 = readingOrder.SetShortValue(field_11_options2, value);
		}
	}

	public TextRecord()
	{
	}

	public TextRecord(RecordInputStream in1)
	{
		field_1_horizontalAlignment = (byte)in1.ReadByte();
		field_2_verticalAlignment = (byte)in1.ReadByte();
		field_3_DisplayMode = in1.ReadShort();
		field_4_rgbColor = in1.ReadInt();
		field_5_x = in1.ReadInt();
		field_6_y = in1.ReadInt();
		field_7_width = in1.ReadInt();
		field_8_height = in1.ReadInt();
		field_9_options1 = in1.ReadShort();
		field_10_IndexOfColorValue = in1.ReadShort();
		field_11_options2 = in1.ReadShort();
		field_12_textRotation = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[TEXT]\n");
		stringBuilder.Append("    .horizontalAlignment  = ").Append("0x").Append(HexDump.ToHex(HorizontalAlignment))
			.Append(" (")
			.Append(HorizontalAlignment)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .verticalAlignment    = ").Append("0x").Append(HexDump.ToHex(VerticalAlignment))
			.Append(" (")
			.Append(VerticalAlignment)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .DisplayMode          = ").Append("0x").Append(HexDump.ToHex(DisplayMode))
			.Append(" (")
			.Append(DisplayMode)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .rgbColor             = ").Append("0x").Append(HexDump.ToHex(RgbColor))
			.Append(" (")
			.Append(RgbColor)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
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
		stringBuilder.Append("    .options1             = ").Append("0x").Append(HexDump.ToHex(Options1))
			.Append(" (")
			.Append(Options1)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("         .autoColor                = ").Append(IsAutoColor).Append('\n');
		stringBuilder.Append("         .showKey                  = ").Append(ShowKey).Append('\n');
		stringBuilder.Append("         .showValue                = ").Append(ShowValue).Append('\n');
		stringBuilder.Append("         .autoGeneratedText        = ").Append(IsAutoGeneratedText).Append('\n');
		stringBuilder.Append("         .generated                = ").Append(IsGenerated).Append('\n');
		stringBuilder.Append("         .autoLabelDeleted         = ").Append(IsAutoLabelDeleted).Append('\n');
		stringBuilder.Append("         .autoBackground           = ").Append(IsAutoBackground).Append('\n');
		stringBuilder.Append("         .showCategoryLabelAsPercentage     = ").Append(ShowCategoryLabelAsPercentage).Append('\n');
		stringBuilder.Append("         .showValueAsPercentage     = ").Append(ShowValueAsPercentage).Append('\n');
		stringBuilder.Append("         .showBubbleSizes          = ").Append(ShowBubbleSizes).Append('\n');
		stringBuilder.Append("         .showLabel                = ").Append(ShowLabel).Append('\n');
		stringBuilder.Append("    .IndexOfColorValue    = ").Append("0x").Append(HexDump.ToHex(IndexOfColorValue))
			.Append(" (")
			.Append(IndexOfColorValue)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .options2             = ").Append("0x").Append(HexDump.ToHex(Options2))
			.Append(" (")
			.Append(Options2)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("         .dataLabelPlacement       = ").Append(DataLabelPlacement).Append('\n');
		stringBuilder.Append("         .readingOrder             = ").Append(ReadingOrder).Append('\n');
		stringBuilder.Append("    .textRotation         = ").Append("0x").Append(HexDump.ToHex(TextRotation))
			.Append(" (")
			.Append(TextRotation)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/TEXT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteByte(field_1_horizontalAlignment);
		out1.WriteByte(field_2_verticalAlignment);
		out1.WriteShort(field_3_DisplayMode);
		out1.WriteInt(field_4_rgbColor);
		out1.WriteInt(field_5_x);
		out1.WriteInt(field_6_y);
		out1.WriteInt(field_7_width);
		out1.WriteInt(field_8_height);
		out1.WriteShort(field_9_options1);
		out1.WriteShort(field_10_IndexOfColorValue);
		out1.WriteShort(field_11_options2);
		out1.WriteShort(field_12_textRotation);
	}

	public override object Clone()
	{
		return new TextRecord
		{
			field_1_horizontalAlignment = field_1_horizontalAlignment,
			field_2_verticalAlignment = field_2_verticalAlignment,
			field_3_DisplayMode = field_3_DisplayMode,
			field_4_rgbColor = field_4_rgbColor,
			field_5_x = field_5_x,
			field_6_y = field_6_y,
			field_7_width = field_7_width,
			field_8_height = field_8_height,
			field_9_options1 = field_9_options1,
			field_10_IndexOfColorValue = field_10_IndexOfColorValue,
			field_11_options2 = field_11_options2,
			field_12_textRotation = field_12_textRotation
		};
	}
}
