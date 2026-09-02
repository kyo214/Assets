using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class BottomMarginRecord : StandardRecord, IMargin, ICloneable
{
	public const short sid = 41;

	private double field_1_margin;

	protected override int DataSize => 8;

	public override short Sid => 41;

	public double Margin
	{
		get
		{
			return field_1_margin;
		}
		set
		{
			field_1_margin = value;
		}
	}

	public BottomMarginRecord()
	{
	}

	public BottomMarginRecord(RecordInputStream in1)
	{
		field_1_margin = in1.ReadDouble();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[BottomMargin]\n");
		stringBuilder.Append("    .margin               = ").Append(" (").Append(Margin)
			.Append(" )\n");
		stringBuilder.Append("[/BottomMargin]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteDouble(field_1_margin);
	}

	public override object Clone()
	{
		return new BottomMarginRecord
		{
			field_1_margin = field_1_margin
		};
	}
}
