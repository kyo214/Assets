using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class InterfaceHdrRecord : StandardRecord
{
	public const short sid = 225;

	private int _codepage;

	public const short CODEPAGE = 1200;

	protected override int DataSize => 2;

	public override short Sid => 225;

	public InterfaceHdrRecord(int codePage)
	{
		_codepage = codePage;
	}

	public InterfaceHdrRecord(RecordInputStream in1)
	{
		_codepage = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[INTERFACEHDR]\n");
		stringBuilder.Append("    .codepage        = ").Append(StringUtil.ToHexString(_codepage)).Append("\n");
		stringBuilder.Append("[/INTERFACEHDR]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(_codepage);
	}
}
