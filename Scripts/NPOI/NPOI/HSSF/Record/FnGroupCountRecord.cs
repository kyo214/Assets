using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class FnGroupCountRecord : StandardRecord
{
	public const short sid = 156;

	public const short COUNT = 14;

	private short field_1_count;

	public short Count
	{
		get
		{
			return field_1_count;
		}
		set
		{
			field_1_count = value;
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 156;

	public FnGroupCountRecord()
	{
	}

	public FnGroupCountRecord(RecordInputStream in1)
	{
		field_1_count = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[FNGROUPCOUNT]\n");
		stringBuilder.Append("    .count            = ").Append(Count).Append("\n");
		stringBuilder.Append("[/FNGROUPCOUNT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(Count);
	}
}
