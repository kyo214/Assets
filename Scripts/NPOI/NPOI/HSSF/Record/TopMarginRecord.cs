using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class TopMarginRecord : StandardRecord, IMargin
{
	public const short sid = 40;

	private double field_1_margin;

	protected override int DataSize => 8;

	public override short Sid => 40;

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

	public TopMarginRecord()
	{
	}

	public TopMarginRecord(RecordInputStream in1)
	{
		field_1_margin = in1.ReadDouble();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[TopMargin]\n");
		stringBuilder.Append("    .margin               = ").Append(" (").Append(Margin)
			.Append(" )\n");
		stringBuilder.Append("[/TopMargin]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteDouble(field_1_margin);
	}

	public override object Clone()
	{
		return new TopMarginRecord
		{
			field_1_margin = field_1_margin
		};
	}
}
