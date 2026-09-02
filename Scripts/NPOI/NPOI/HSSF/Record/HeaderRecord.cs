using System;
using System.Text;

namespace NPOI.HSSF.Record;

public class HeaderRecord : HeaderFooterBase, ICloneable
{
	public const short sid = 20;

	public override short Sid => 20;

	public HeaderRecord(string text)
		: base(text)
	{
	}

	public HeaderRecord(RecordInputStream in1)
		: base(in1)
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[HEADER]\n");
		stringBuilder.Append("    .header = ").Append(base.Text).Append("\n");
		stringBuilder.Append("[/HEADER]\n");
		return stringBuilder.ToString();
	}

	public override object Clone()
	{
		return new HeaderRecord(base.Text);
	}
}
