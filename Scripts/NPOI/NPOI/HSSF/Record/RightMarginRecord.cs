using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class RightMarginRecord : StandardRecord, IMargin
{
	public const short sid = 39;

	private double field_1_margin;

	public override short Sid => 39;

	protected override int DataSize => 8;

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

	public RightMarginRecord()
	{
	}

	public RightMarginRecord(RecordInputStream in1)
	{
		field_1_margin = in1.ReadDouble();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[RightMargin]\n");
		stringBuilder.Append("    .margin               = ").Append(" (").Append(Margin)
			.Append(" )\n");
		stringBuilder.Append("[/RightMargin]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteDouble(field_1_margin);
	}

	public override object Clone()
	{
		return new RightMarginRecord
		{
			field_1_margin = field_1_margin
		};
	}
}
