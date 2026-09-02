using System;
using System.Text;

namespace NPOI.HSSF.Record;

public class FooterRecord : HeaderFooterBase, ICloneable
{
	public const short sid = 21;

	public override short Sid => 21;

	public FooterRecord(string text)
		: base(text)
	{
	}

	public FooterRecord(RecordInputStream in1)
		: base(in1)
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[FOOTER]\n");
		stringBuilder.Append("    .footer         = ").Append(base.Text).Append("\n");
		stringBuilder.Append("[/FOOTER]\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		return new FooterRecord(base.Text);
	}
}
