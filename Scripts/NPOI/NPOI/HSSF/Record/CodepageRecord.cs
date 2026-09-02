using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class CodepageRecord : StandardRecord
{
	public const short sid = 66;

	private short field_1_codepage;

	public const short CODEPAGE = 1200;

	public short Codepage
	{
		get
		{
			return field_1_codepage;
		}
		set
		{
			field_1_codepage = value;
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 66;

	public CodepageRecord()
	{
	}

	public CodepageRecord(RecordInputStream in1)
	{
		field_1_codepage = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[CODEPAGE]\n");
		stringBuilder.Append("    .codepage        = ").Append(StringUtil.ToHexString(Codepage)).Append("\n");
		stringBuilder.Append("[/CODEPAGE]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(Codepage);
	}
}
