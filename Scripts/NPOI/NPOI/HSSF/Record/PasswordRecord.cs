using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class PasswordRecord : StandardRecord
{
	public const short sid = 19;

	private int field_1_password;

	public int Password
	{
		get
		{
			return field_1_password;
		}
		set
		{
			field_1_password = value;
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 19;

	public PasswordRecord(int password)
	{
		field_1_password = password;
	}

	public PasswordRecord(RecordInputStream in1)
	{
		field_1_password = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[PASSWORD]\n");
		stringBuilder.Append("    .password       = ").Append(StringUtil.ToHexString(Password)).Append("\n");
		stringBuilder.Append("[/PASSWORD]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_password);
	}

	public override object Clone()
	{
		return new PasswordRecord(field_1_password);
	}
}
