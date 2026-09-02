using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class WriteProtectRecord : StandardRecord
{
	public const short sid = 134;

	protected override int DataSize => 0;

	public override short Sid => 134;

	public WriteProtectRecord()
	{
	}

	public WriteProtectRecord(RecordInputStream in1)
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[WritePROTECT]\n");
		stringBuilder.Append("[/WritePROTECT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
	}
}
