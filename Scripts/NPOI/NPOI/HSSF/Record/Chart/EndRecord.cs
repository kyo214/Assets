using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class EndRecord : StandardRecord, ICloneable
{
	public const short sid = 4148;

	public static EndRecord instance = new EndRecord();

	protected override int DataSize => 0;

	public override short Sid => 4148;

	public EndRecord()
	{
	}

	public EndRecord(RecordInputStream in1)
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[END]\n");
		stringBuilder.Append("[/END]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
	}

	public override object Clone()
	{
		return new EndRecord();
	}
}
