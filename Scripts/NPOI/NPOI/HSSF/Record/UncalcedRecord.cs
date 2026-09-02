using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class UncalcedRecord : StandardRecord
{
	public const short sid = 94;

	private short _reserved;

	public override short Sid => 94;

	protected override int DataSize => 2;

	public static int StaticRecordSize => 6;

	public UncalcedRecord()
	{
		_reserved = 0;
	}

	public UncalcedRecord(RecordInputStream in1)
	{
		_reserved = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[UNCALCED]\n");
		stringBuilder.Append("[/UNCALCED]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(_reserved);
	}
}
