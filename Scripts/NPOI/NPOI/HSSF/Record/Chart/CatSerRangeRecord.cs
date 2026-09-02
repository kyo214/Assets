using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class CatSerRangeRecord : StandardRecord
{
	public const short sid = 4128;

	private short field_1_catCross;

	private short field_2_catLabel;

	private short field_3_catMark;

	private short field_4_options;

	private BitField fBetween = BitFieldFactory.GetInstance(1);

	private BitField fMaxCross = BitFieldFactory.GetInstance(2);

	private BitField fReverse = BitFieldFactory.GetInstance(4);

	protected override int DataSize => 8;

	public override short Sid => 4128;

	public short CrossPoint
	{
		get
		{
			return field_1_catCross;
		}
		set
		{
			field_1_catCross = value;
		}
	}

	public short LabelInterval
	{
		get
		{
			return field_2_catLabel;
		}
		set
		{
			field_2_catLabel = value;
		}
	}

	public short MarkInterval
	{
		get
		{
			return field_3_catMark;
		}
		set
		{
			field_3_catMark = value;
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

	public bool IsBetween
	{
		get
		{
			return fBetween.IsSet(field_4_options);
		}
		set
		{
			field_4_options = fBetween.SetShortBoolean(field_4_options, value);
		}
	}

	public bool IsMaxCross
	{
		get
		{
			return fMaxCross.IsSet(field_4_options);
		}
		set
		{
			field_4_options = fMaxCross.SetShortBoolean(field_4_options, value);
		}
	}

	public bool IsReverse
	{
		get
		{
			return fReverse.IsSet(field_4_options);
		}
		set
		{
			field_4_options = fReverse.SetShortBoolean(field_4_options, value);
		}
	}

	public CatSerRangeRecord()
	{
	}

	public CatSerRangeRecord(RecordInputStream in1)
	{
		field_1_catCross = in1.ReadShort();
		field_2_catLabel = in1.ReadShort();
		field_3_catMark = in1.ReadShort();
		field_4_options = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[CATSERRANGE]\n");
		stringBuilder.Append("    .catCross        = ").Append("0x").Append(HexDump.ToHex(CrossPoint))
			.Append(" (")
			.Append(CrossPoint)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .catLabel       = ").Append("0x").Append(HexDump.ToHex(LabelInterval))
			.Append(" (")
			.Append(LabelInterval)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .catMark    = ").Append("0x").Append(HexDump.ToHex(MarkInterval))
			.Append(" (")
			.Append(MarkInterval)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .options              = ").Append("0x").Append(HexDump.ToHex(Options))
			.Append(" (")
			.Append(Options)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("         .fBetween        = ").Append(IsBetween).Append('\n');
		stringBuilder.Append("         .fMaxCross       = ").Append(IsMaxCross).Append('\n');
		stringBuilder.Append("         .fReverse        = ").Append(IsReverse).Append('\n');
		stringBuilder.Append("[/CATSERRANGE]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_catCross);
		out1.WriteShort(field_2_catLabel);
		out1.WriteShort(field_3_catMark);
		out1.WriteShort(field_4_options);
	}

	public override object Clone()
	{
		return new CatSerRangeRecord
		{
			field_1_catCross = field_1_catCross,
			field_2_catLabel = field_2_catLabel,
			field_3_catMark = field_3_catMark,
			field_4_options = field_4_options
		};
	}
}
