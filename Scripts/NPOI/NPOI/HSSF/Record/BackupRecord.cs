using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class BackupRecord : StandardRecord
{
	public const short sid = 64;

	private short field_1_backup;

	public short Backup
	{
		get
		{
			return field_1_backup;
		}
		set
		{
			field_1_backup = value;
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 64;

	public BackupRecord()
	{
	}

	public BackupRecord(RecordInputStream in1)
	{
		field_1_backup = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[BACKUP]\n");
		stringBuilder.Append("    .backup          = ").Append(StringUtil.ToHexString(Backup)).Append("\n");
		stringBuilder.Append("[/BACKUP]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(Backup);
	}
}
