using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class EOFRecord : StandardRecord, ICloneable
{
	public const short sid = 10;

	public const int ENCODED_SIZE = 4;

	public static readonly EOFRecord instance = new EOFRecord();

	protected override int DataSize => 0;

	public override short Sid => 10;

	public EOFRecord()
	{
	}

	public EOFRecord(RecordInputStream in1)
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[EOF]\n");
		stringBuilder.Append("[/EOF]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
	}

	public override object Clone()
	{
		return new EOFRecord();
	}
}
