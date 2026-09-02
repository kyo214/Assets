using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class BeginRecord : StandardRecord, ICloneable
{
	public const short sid = 4147;

	public static BeginRecord instance = new BeginRecord();

	protected override int DataSize => 0;

	public override short Sid => 4147;

	public BeginRecord()
	{
	}

	public BeginRecord(RecordInputStream in1)
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[BEGIN]\n");
		stringBuilder.Append("[/BEGIN]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
	}

	public override object Clone()
	{
		return new BeginRecord();
	}
}
