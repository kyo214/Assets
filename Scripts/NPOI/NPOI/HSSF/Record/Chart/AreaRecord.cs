using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class AreaRecord : StandardRecord, ICloneable
{
	public const short sid = 4122;

	private short field_1_formatFlags;

	private BitField stacked = BitFieldFactory.GetInstance(1);

	private BitField DisplayAsPercentage = BitFieldFactory.GetInstance(2);

	private BitField shadow = BitFieldFactory.GetInstance(4);

	protected override int DataSize => 2;

	public override short Sid => 4122;

	public short FormatFlags
	{
		get
		{
			return field_1_formatFlags;
		}
		set
		{
			field_1_formatFlags = value;
		}
	}

	public bool IsStacked
	{
		get
		{
			return stacked.IsSet(field_1_formatFlags);
		}
		set
		{
			field_1_formatFlags = stacked.SetShortBoolean(field_1_formatFlags, value);
		}
	}

	public bool IsDisplayAsPercentage
	{
		get
		{
			return DisplayAsPercentage.IsSet(field_1_formatFlags);
		}
		set
		{
			field_1_formatFlags = DisplayAsPercentage.SetShortBoolean(field_1_formatFlags, value);
		}
	}

	public bool IsShadow
	{
		get
		{
			return shadow.IsSet(field_1_formatFlags);
		}
		set
		{
			field_1_formatFlags = shadow.SetShortBoolean(field_1_formatFlags, value);
		}
	}

	public AreaRecord()
	{
	}

	public AreaRecord(RecordInputStream in1)
	{
		field_1_formatFlags = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[AREA]\n");
		stringBuilder.Append("    .formatFlags          = ").Append("0x").Append(HexDump.ToHex(FormatFlags))
			.Append(" (")
			.Append(FormatFlags)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("         .stacked                  = ").Append(IsStacked).Append('\n');
		stringBuilder.Append("         .DisplayAsPercentage      = ").Append(IsDisplayAsPercentage).Append('\n');
		stringBuilder.Append("         .shadow                   = ").Append(IsShadow).Append('\n');
		stringBuilder.Append("[/AREA]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_formatFlags);
	}

	public override object Clone()
	{
		return new AreaRecord
		{
			field_1_formatFlags = field_1_formatFlags
		};
	}
}
