using System;
using System.Collections.Generic;
using System.IO;
using NPOI.HSSF.Model;

namespace NPOI.HSSF.Record.Aggregates;

public class ChartSubstreamRecordAggregate : RecordAggregate
{
	private BOFRecord _bofRec;

	private List<RecordBase> _recs;

	private PageSettingsBlock _psBlock;

	public ChartSubstreamRecordAggregate(RecordStream rs)
	{
		_bofRec = (BOFRecord)rs.GetNext();
		List<RecordBase> list = new List<RecordBase>();
		while (rs.PeekNextClass() != typeof(EOFRecord))
		{
			rs.PeekNextClass();
			if (PageSettingsBlock.IsComponentRecord(rs.PeekNextSid()))
			{
				if (_psBlock != null)
				{
					if (rs.PeekNextSid() != 2204)
					{
						throw new InvalidDataException("Found more than one PageSettingsBlock in chart sub-stream, had sid: " + rs.PeekNextSid());
					}
					_psBlock.AddLateHeaderFooter((HeaderFooterRecord)rs.GetNext());
				}
				else
				{
					_psBlock = new PageSettingsBlock(rs);
					list.Add(_psBlock);
				}
			}
			else
			{
				list.Add(rs.GetNext());
			}
		}
		_recs = list;
		if (!(rs.GetNext() is EOFRecord))
		{
			throw new InvalidOperationException("Bad chart EOF");
		}
	}

	public override void VisitContainedRecords(RecordVisitor rv)
	{
		if (_recs.Count == 0)
		{
			return;
		}
		rv.VisitRecord(_bofRec);
		for (int i = 0; i < _recs.Count; i++)
		{
			RecordBase recordBase = _recs[i];
			if (recordBase is RecordAggregate)
			{
				((RecordAggregate)recordBase).VisitContainedRecords(rv);
			}
			else
			{
				rv.VisitRecord((Record)recordBase);
			}
		}
		rv.VisitRecord(EOFRecord.instance);
	}
}
