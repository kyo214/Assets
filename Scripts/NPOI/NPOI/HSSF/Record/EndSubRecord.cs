using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class EndSubRecord : SubRecord, ICloneable
{
	public const short sid = 0;

	private const int ENCODED_SIZE = 0;

	public override bool IsTerminating => true;

	public override int DataSize => 0;

	public override short Sid => 0;

	public EndSubRecord()
	{
	}

	public EndSubRecord(ILittleEndianInput in1, int size)
	{
		if ((size & 0xFF) != 0)
		{
			throw new RecordFormatException("Unexpected size (" + size + ")");
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[ftEnd]\n");
		stringBuilder.Append("[/ftEnd]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(0);
		out1.WriteShort(0);
	}

	public override object Clone()
	{
		return new EndSubRecord();
	}
}
