using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class DatRecord : StandardRecord, ICloneable
{
	public const short sid = 4195;

	private short field_1_options;

	private BitField horizontalBorder = BitFieldFactory.GetInstance(1);

	private BitField verticalBorder = BitFieldFactory.GetInstance(2);

	private BitField border = BitFieldFactory.GetInstance(4);

	private BitField showSeriesKey = BitFieldFactory.GetInstance(8);

	protected override int DataSize => 2;

	public override short Sid => 4195;

	public short Options
	{
		get
		{
			return field_1_options;
		}
		set
		{
			field_1_options = value;
		}
	}

	public DatRecord()
	{
	}

	public DatRecord(RecordInputStream in1)
	{
		field_1_options = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[DAT]\n");
		stringBuilder.Append("    .options              = ").Append("0x").Append(HexDump.ToHex(Options))
			.Append(" (")
			.Append(Options)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("         .horizontalBorder         = ").Append(IsHorizontalBorder()).Append('\n');
		stringBuilder.Append("         .verticalBorder           = ").Append(IsVerticalBorder()).Append('\n');
		stringBuilder.Append("         .border                   = ").Append(IsBorder()).Append('\n');
		stringBuilder.Append("         .showSeriesKey            = ").Append(IsShowSeriesKey()).Append('\n');
		stringBuilder.Append("[/DAT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_options);
	}

	public override object Clone()
	{
		return new DatRecord
		{
			field_1_options = field_1_options
		};
	}

	public void SetHorizontalBorder(bool value)
	{
		field_1_options = horizontalBorder.SetShortBoolean(field_1_options, value);
	}

	public bool IsHorizontalBorder()
	{
		return horizontalBorder.IsSet(field_1_options);
	}

	public void SetVerticalBorder(bool value)
	{
		field_1_options = verticalBorder.SetShortBoolean(field_1_options, value);
	}

	public bool IsVerticalBorder()
	{
		return verticalBorder.IsSet(field_1_options);
	}

	public void SetBorder(bool value)
	{
		field_1_options = border.SetShortBoolean(field_1_options, value);
	}

	public bool IsBorder()
	{
		return border.IsSet(field_1_options);
	}

	public void SetShowSeriesKey(bool value)
	{
		field_1_options = showSeriesKey.SetShortBoolean(field_1_options, value);
	}

	public bool IsShowSeriesKey()
	{
		return showSeriesKey.IsSet(field_1_options);
	}
}
