using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class FrameRecord : StandardRecord, ICloneable
{
	public const short sid = 4146;

	private short field_1_borderType;

	public const short BORDER_TYPE_REGULAR = 0;

	public const short BORDER_TYPE_SHADOW = 1;

	private short field_2_options;

	private BitField autoSize = BitFieldFactory.GetInstance(1);

	private BitField autoPosition = BitFieldFactory.GetInstance(2);

	protected override int DataSize => 4;

	public override short Sid => 4146;

	public short BorderType
	{
		get
		{
			return field_1_borderType;
		}
		set
		{
			field_1_borderType = value;
		}
	}

	public short Options
	{
		get
		{
			return field_2_options;
		}
		set
		{
			field_2_options = value;
		}
	}

	public bool IsAutoSize
	{
		get
		{
			return autoSize.IsSet(field_2_options);
		}
		set
		{
			field_2_options = autoSize.SetShortBoolean(field_2_options, value);
		}
	}

	public bool IsAutoPosition
	{
		get
		{
			return autoPosition.IsSet(field_2_options);
		}
		set
		{
			field_2_options = autoPosition.SetShortBoolean(field_2_options, value);
		}
	}

	public FrameRecord()
	{
	}

	public FrameRecord(RecordInputStream in1)
	{
		field_1_borderType = in1.ReadShort();
		field_2_options = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[FRAME]\n");
		stringBuilder.Append("    .borderType           = ").Append("0x").Append(HexDump.ToHex(BorderType))
			.Append(" (")
			.Append(BorderType)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("    .options              = ").Append("0x").Append(HexDump.ToHex(Options))
			.Append(" (")
			.Append(Options)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("         .autoSize                 = ").Append(IsAutoSize).Append('\n');
		stringBuilder.Append("         .autoPosition             = ").Append(IsAutoPosition).Append('\n');
		stringBuilder.Append("[/FRAME]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_borderType);
		out1.WriteShort(field_2_options);
	}

	public override object Clone()
	{
		return new FrameRecord
		{
			field_1_borderType = field_1_borderType,
			field_2_options = field_2_options
		};
	}
}
