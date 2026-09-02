using System;
using System.Collections.Generic;

namespace NPOI.HSSF.Record;

public class HorizontalPageBreakRecord : PageBreakRecord, ICloneable
{
	public new const short sid = 27;

	public override short Sid => 27;

	public HorizontalPageBreakRecord()
	{
	}

	public HorizontalPageBreakRecord(RecordInputStream in1)
		: base(in1)
	{
	}

	public override object Clone()
	{
		PageBreakRecord pageBreakRecord = new HorizontalPageBreakRecord();
		IEnumerator<Break> breaksEnumerator = GetBreaksEnumerator();
		while (breaksEnumerator.MoveNext())
		{
			Break current = breaksEnumerator.Current;
			pageBreakRecord.AddBreak(current.main, current.subFrom, current.subTo);
		}
		return pageBreakRecord;
	}
}
