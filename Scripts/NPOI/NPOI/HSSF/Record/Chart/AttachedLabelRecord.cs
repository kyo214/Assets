using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class AttachedLabelRecord : StandardRecord
{
	public const short sid = 4108;

	private short field_1_formatFlags;

	private BitField showActual = BitFieldFactory.GetInstance(1);

	private BitField showPercent = BitFieldFactory.GetInstance(2);

	private BitField labelAsPercentage = BitFieldFactory.GetInstance(4);

	private BitField smoothedLine = BitFieldFactory.GetInstance(8);

	private BitField showLabel = BitFieldFactory.GetInstance(16);

	private BitField showBubbleSizes = BitFieldFactory.GetInstance(32);

	protected override int DataSize => 2;

	public override short Sid => 4108;

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

	public bool IsShowActual
	{
		get
		{
			return showActual.IsSet(field_1_formatFlags);
		}
		set
		{
			field_1_formatFlags = showActual.SetShortBoolean(field_1_formatFlags, value);
		}
	}

	public bool IsShowPercent
	{
		get
		{
			return showPercent.IsSet(field_1_formatFlags);
		}
		set
		{
			field_1_formatFlags = showPercent.SetShortBoolean(field_1_formatFlags, value);
		}
	}

	public bool IsLabelAsPercentage
	{
		get
		{
			return labelAsPercentage.IsSet(field_1_formatFlags);
		}
		set
		{
			field_1_formatFlags = labelAsPercentage.SetShortBoolean(field_1_formatFlags, value);
		}
	}

	public bool IsSmoothedLine
	{
		get
		{
			return smoothedLine.IsSet(field_1_formatFlags);
		}
		set
		{
			field_1_formatFlags = smoothedLine.SetShortBoolean(field_1_formatFlags, value);
		}
	}

	public bool IsShowLabel
	{
		get
		{
			return showLabel.IsSet(field_1_formatFlags);
		}
		set
		{
			field_1_formatFlags = showLabel.SetShortBoolean(field_1_formatFlags, value);
		}
	}

	public bool IsShowBubbleSizes
	{
		get
		{
			return showBubbleSizes.IsSet(field_1_formatFlags);
		}
		set
		{
			field_1_formatFlags = showBubbleSizes.SetShortBoolean(field_1_formatFlags, value);
		}
	}

	public AttachedLabelRecord()
	{
	}

	public AttachedLabelRecord(RecordInputStream in1)
	{
		field_1_formatFlags = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[ATTACHEDLABEL]\n");
		stringBuilder.Append("    .formatFlags          = ").Append("0x").Append(HexDump.ToHex(FormatFlags))
			.Append(" (")
			.Append(FormatFlags)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("         .showActual               = ").Append(IsShowActual).Append('\n');
		stringBuilder.Append("         .showPercent              = ").Append(IsShowPercent).Append('\n');
		stringBuilder.Append("         .labelAsPercentage        = ").Append(IsLabelAsPercentage).Append('\n');
		stringBuilder.Append("         .smoothedLine             = ").Append(IsSmoothedLine).Append('\n');
		stringBuilder.Append("         .showLabel                = ").Append(IsShowLabel).Append('\n');
		stringBuilder.Append("         .showBubbleSizes          = ").Append(IsShowBubbleSizes).Append('\n');
		stringBuilder.Append("[/ATTACHEDLABEL]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_formatFlags);
	}

	public override object Clone()
	{
		return new AttachedLabelRecord
		{
			field_1_formatFlags = field_1_formatFlags
		};
	}
}
