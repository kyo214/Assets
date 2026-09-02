using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class PasswordRev4Record : StandardRecord
{
	public const short sid = 444;

	private int field_1_password;

	protected override int DataSize => 2;

	public override short Sid => 444;

	public PasswordRev4Record(int pw)
	{
		field_1_password = pw;
	}

	public PasswordRev4Record(RecordInputStream in1)
	{
		field_1_password = in1.ReadShort();
	}

	public void SetPassword(short pw)
	{
		field_1_password = pw;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[PROT4REVPASSWORD]\n");
		stringBuilder.Append("    .password       = ").Append(StringUtil.ToHexString(field_1_password)).Append("\n");
		stringBuilder.Append("[/PROT4REVPASSWORD]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_password);
	}
}
