using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class MMSRecord : StandardRecord
{
	public const short sid = 193;

	private byte field_1_AddMenuCount;

	private byte field_2_delMenuCount;

	public byte AddMenuCount
	{
		get
		{
			return field_1_AddMenuCount;
		}
		set
		{
			field_1_AddMenuCount = value;
		}
	}

	public byte DelMenuCount
	{
		get
		{
			return field_2_delMenuCount;
		}
		set
		{
			field_2_delMenuCount = value;
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 193;

	public MMSRecord()
	{
	}

	public MMSRecord(RecordInputStream in1)
	{
		if (in1.Remaining != 0)
		{
			field_1_AddMenuCount = (byte)in1.ReadByte();
			field_2_delMenuCount = (byte)in1.ReadByte();
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[MMS]\n");
		stringBuilder.Append("    .addMenu        = ").Append(StringUtil.ToHexString(AddMenuCount)).Append("\n");
		stringBuilder.Append("    .delMenu        = ").Append(StringUtil.ToHexString(DelMenuCount)).Append("\n");
		stringBuilder.Append("[/MMS]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteByte(AddMenuCount);
		out1.WriteByte(DelMenuCount);
	}
}
