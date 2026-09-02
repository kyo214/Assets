using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class SeriesRecord : StandardRecord
{
	public const short sid = 4099;

	private short field_1_categoryDataType;

	public const short CATEGORY_DATA_TYPE_DATES = 0;

	public const short CATEGORY_DATA_TYPE_NUMERIC = 1;

	public const short CATEGORY_DATA_TYPE_SEQUENCE = 2;

	public const short CATEGORY_DATA_TYPE_TEXT = 3;

	private short field_2_valuesDataType;

	public const short VALUES_DATA_TYPE_DATES = 0;

	public const short VALUES_DATA_TYPE_NUMERIC = 1;

	public const short VALUES_DATA_TYPE_SEQUENCE = 2;

	public const short VALUES_DATA_TYPE_TEXT = 3;

	private short field_3_numCategories;

	private short field_4_numValues;

	private short field_5_bubbleSeriesType;

	public const short BUBBLE_SERIES_TYPE_DATES = 0;

	public const short BUBBLE_SERIES_TYPE_NUMERIC = 1;

	public const short BUBBLE_SERIES_TYPE_SEQUENCE = 2;

	public const short BUBBLE_SERIES_TYPE_TEXT = 3;

	private short field_6_numBubbleValues;

	protected override int DataSize => 12;

	public override short Sid => 4099;

	public short CategoryDataType
	{
		get
		{
			return field_1_categoryDataType;
		}
		set
		{
			field_1_categoryDataType = value;
		}
	}

	public short ValuesDataType
	{
		get
		{
			return field_2_valuesDataType;
		}
		set
		{
			field_2_valuesDataType = value;
		}
	}

	public short NumCategories
	{
		get
		{
			return field_3_numCategories;
		}
		set
		{
			field_3_numCategories = value;
		}
	}

	public short NumValues
	{
		get
		{
			return field_4_numValues;
		}
		set
		{
			field_4_numValues = value;
		}
	}

	public short BubbleSeriesType
	{
		get
		{
			return field_5_bubbleSeriesType;
		}
		set
		{
			field_5_bubbleSeriesType = value;
		}
	}

	public short NumBubbleValues
	{
		get
		{
			return field_6_numBubbleValues;
		}
		set
		{
			field_6_numBubbleValues = value;
		}
	}

	public SeriesRecord()
	{
	}

	public SeriesRecord(RecordInputStream in1)
	{
		field_1_categoryDataType = in1.ReadShort();
		field_2_valuesDataType = in1.ReadShort();
		field_3_numCategories = in1.ReadShort();
		field_4_numValues = in1.ReadShort();
		field_5_bubbleSeriesType = in1.ReadShort();
		field_6_numBubbleValues = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[SERIES]\n");
		stringBuilder.Append("    .categoryDataType     = ").Append("0x").Append(HexDump.ToHex(CategoryDataType))
			.Append(" (")
			.Append(CategoryDataType)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .valuesDataType       = ").Append("0x").Append(HexDump.ToHex(ValuesDataType))
			.Append(" (")
			.Append(ValuesDataType)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .numCategories        = ").Append("0x").Append(HexDump.ToHex(NumCategories))
			.Append(" (")
			.Append(NumCategories)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .numValues            = ").Append("0x").Append(HexDump.ToHex(NumValues))
			.Append(" (")
			.Append(NumValues)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .bubbleSeriesType     = ").Append("0x").Append(HexDump.ToHex(BubbleSeriesType))
			.Append(" (")
			.Append(BubbleSeriesType)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .numBubbleValues      = ").Append("0x").Append(HexDump.ToHex(NumBubbleValues))
			.Append(" (")
			.Append(NumBubbleValues)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/SERIES]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_categoryDataType);
		out1.WriteShort(field_2_valuesDataType);
		out1.WriteShort(field_3_numCategories);
		out1.WriteShort(field_4_numValues);
		out1.WriteShort(field_5_bubbleSeriesType);
		out1.WriteShort(field_6_numBubbleValues);
	}

	public override object Clone()
	{
		return new SeriesRecord
		{
			field_1_categoryDataType = field_1_categoryDataType,
			field_2_valuesDataType = field_2_valuesDataType,
			field_3_numCategories = field_3_numCategories,
			field_4_numValues = field_4_numValues,
			field_5_bubbleSeriesType = field_5_bubbleSeriesType,
			field_6_numBubbleValues = field_6_numBubbleValues
		};
	}
}
