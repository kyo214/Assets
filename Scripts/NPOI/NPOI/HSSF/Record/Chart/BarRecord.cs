using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class BarRecord : StandardRecord, ICloneable
{
	public const short sid = 4119;

	private short field_1_barSpace;

	private short field_2_categorySpace;

	private short field_3_formatFlags;

	private BitField horizontal = BitFieldFactory.GetInstance(1);

	private BitField stacked = BitFieldFactory.GetInstance(2);

	private BitField DisplayAsPercentage = BitFieldFactory.GetInstance(4);

	private BitField shadow = BitFieldFactory.GetInstance(8);

	protected override int DataSize => 6;

	public override short Sid => 4119;

	public short BarSpace
	{
		get
		{
			return field_1_barSpace;
		}
		set
		{
			field_1_barSpace = value;
		}
	}

	public short CategorySpace
	{
		get
		{
			return field_2_categorySpace;
		}
		set
		{
			field_2_categorySpace = value;
		}
	}

	public short FormatFlags
	{
		get
		{
			return field_3_formatFlags;
		}
		set
		{
			field_3_formatFlags = value;
		}
	}

	public bool IsHorizontal
	{
		get
		{
			return horizontal.IsSet(field_3_formatFlags);
		}
		set
		{
			field_3_formatFlags = horizontal.SetShortBoolean(field_3_formatFlags, value);
		}
	}

	public bool IsStacked
	{
		get
		{
			return stacked.IsSet(field_3_formatFlags);
		}
		set
		{
			field_3_formatFlags = stacked.SetShortBoolean(field_3_formatFlags, value);
		}
	}

	public bool IsDisplayAsPercentage
	{
		get
		{
			return DisplayAsPercentage.IsSet(field_3_formatFlags);
		}
		set
		{
			field_3_formatFlags = DisplayAsPercentage.SetShortBoolean(field_3_formatFlags, value);
		}
	}

	public bool IsShadow
	{
		get
		{
			return shadow.IsSet(field_3_formatFlags);
		}
		set
		{
			field_3_formatFlags = shadow.SetShortBoolean(field_3_formatFlags, value);
		}
	}

	public BarRecord()
	{
	}

	public BarRecord(RecordInputStream in1)
	{
		field_1_barSpace = in1.ReadShort();
		field_2_categorySpace = in1.ReadShort();
		field_3_formatFlags = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[BAR]\n");
		stringBuilder.Append("    .barSpace             = ").Append("0x").Append(HexDump.ToHex(BarSpace))
			.Append(" (")
			.Append(BarSpace)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .categorySpace        = ").Append("0x").Append(HexDump.ToHex(CategorySpace))
			.Append(" (")
			.Append(CategorySpace)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .formatFlags          = ").Append("0x").Append(HexDump.ToHex(FormatFlags))
			.Append(" (")
			.Append(FormatFlags)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("         .horizontal               = ").Append(IsHorizontal).Append('\n');
		stringBuilder.Append("         .stacked                  = ").Append(IsStacked).Append('\n');
		stringBuilder.Append("         .DisplayAsPercentage      = ").Append(IsDisplayAsPercentage).Append('\n');
		stringBuilder.Append("         .shadow                   = ").Append(IsShadow).Append('\n');
		stringBuilder.Append("[/BAR]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_barSpace);
		out1.WriteShort(field_2_categorySpace);
		out1.WriteShort(field_3_formatFlags);
	}

	public override object Clone()
	{
		return new BarRecord
		{
			field_1_barSpace = field_1_barSpace,
			field_2_categorySpace = field_2_categorySpace,
			field_3_formatFlags = field_3_formatFlags
		};
	}
}
