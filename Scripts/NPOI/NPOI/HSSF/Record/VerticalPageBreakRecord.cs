using System.Collections.Generic;

namespace NPOI.HSSF.Record;

public class VerticalPageBreakRecord : PageBreakRecord
{
	public new const short sid = 26;

	public override short Sid => 26;

	public VerticalPageBreakRecord()
	{
	}

	public VerticalPageBreakRecord(RecordInputStream in1)
		: base(in1)
	{
	}

	public override object Clone()
	{
		PageBreakRecord pageBreakRecord = new VerticalPageBreakRecord();
		IEnumerator<Break> breaksEnumerator = GetBreaksEnumerator();
		while (breaksEnumerator.MoveNext())
		{
			Break current = breaksEnumerator.Current;
			pageBreakRecord.AddBreak(current.main, current.subFrom, current.subTo);
		}
		return pageBreakRecord;
	}
}
