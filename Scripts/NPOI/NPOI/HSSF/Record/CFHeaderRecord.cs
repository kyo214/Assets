using System;
using NPOI.SS.Util;

namespace NPOI.HSSF.Record;

public class CFHeaderRecord : CFHeaderBase, ICloneable
{
	public static short sid = 432;

	protected override string RecordName => "CFHEADER";

	public override short Sid => sid;

	public CFHeaderRecord()
	{
		CreateEmpty();
	}

	public CFHeaderRecord(CellRangeAddress[] regions, int nRules)
		: base(regions, nRules)
	{
	}

	public CFHeaderRecord(RecordInputStream in1)
	{
		Read(in1);
	}

	public override object Clone()
	{
		CFHeaderRecord result = new CFHeaderRecord();
		CopyTo(result);
		return result;
	}
}
