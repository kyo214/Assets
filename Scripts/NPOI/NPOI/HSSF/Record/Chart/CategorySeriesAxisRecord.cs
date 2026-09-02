using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class CategorySeriesAxisRecord : StandardRecord, ICloneable
{
	public static short sid = 4128;

	private static BitField valueAxisCrossing = BitFieldFactory.GetInstance(1);

	private static BitField crossesFarRight = BitFieldFactory.GetInstance(2);

	private static BitField reversed = BitFieldFactory.GetInstance(4);

	private short field_1_crossingPoint;

	private short field_2_labelFrequency;

	private short field_3_tickMarkFrequency;

	private short field_4_options;

	protected override int DataSize => 8;

	public override short Sid => sid;

	public short CrossingPoint
	{
		get
		{
			return field_1_crossingPoint;
		}
		set
		{
			field_1_crossingPoint = value;
		}
	}

	public short LabelFrequency
	{
		get
		{
			return field_2_labelFrequency;
		}
		set
		{
			field_2_labelFrequency = value;
		}
	}

	public short TickMarkFrequency
	{
		get
		{
			return field_3_tickMarkFrequency;
		}
		set
		{
			field_3_tickMarkFrequency = value;
		}
	}

	public short Options
	{
		get
		{
			return field_4_options;
		}
		set
		{
			field_4_options = value;
		}
	}

	public bool IsValueAxisCrossing
	{
		get
		{
			return valueAxisCrossing.IsSet(field_4_options);
		}
		set
		{
			field_4_options = valueAxisCrossing.SetShortBoolean(field_4_options, value);
		}
	}

	public bool IsCrossesFarRight
	{
		get
		{
			return crossesFarRight.IsSet(field_4_options);
		}
		set
		{
			field_4_options = crossesFarRight.SetShortBoolean(field_4_options, value);
		}
	}

	public bool IsReversed
	{
		get
		{
			return reversed.IsSet(field_4_options);
		}
		set
		{
			field_4_options = reversed.SetShortBoolean(field_4_options, value);
		}
	}

	public CategorySeriesAxisRecord()
	{
	}

	public CategorySeriesAxisRecord(RecordInputStream in1)
	{
		field_1_crossingPoint = in1.ReadShort();
		field_2_labelFrequency = in1.ReadShort();
		field_3_tickMarkFrequency = in1.ReadShort();
		field_4_options = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[CATSERRANGE]\n");
		stringBuilder.Append("    .crossingPoint        = ").Append("0x").Append(HexDump.ToHex(CrossingPoint))
			.Append(" (")
			.Append(CrossingPoint)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .labelFrequency       = ").Append("0x").Append(HexDump.ToHex(LabelFrequency))
			.Append(" (")
			.Append(LabelFrequency)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .tickMarkFrequency    = ").Append("0x").Append(HexDump.ToHex(TickMarkFrequency))
			.Append(" (")
			.Append(TickMarkFrequency)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .options              = ").Append("0x").Append(HexDump.ToHex(Options))
			.Append(" (")
			.Append(Options)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("         .valueAxisCrossing        = ").Append(IsValueAxisCrossing).Append('\n');
		stringBuilder.Append("         .crossesFarRight          = ").Append(IsCrossesFarRight).Append('\n');
		stringBuilder.Append("         .reversed                 = ").Append(IsReversed).Append('\n');
		stringBuilder.Append("[/CATSERRANGE]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_crossingPoint);
		out1.WriteShort(field_2_labelFrequency);
		out1.WriteShort(field_3_tickMarkFrequency);
		out1.WriteShort(field_4_options);
	}

	public override object Clone()
	{
		return new CategorySeriesAxisRecord
		{
			field_1_crossingPoint = field_1_crossingPoint,
			field_2_labelFrequency = field_2_labelFrequency,
			field_3_tickMarkFrequency = field_3_tickMarkFrequency,
			field_4_options = field_4_options
		};
	}
}
