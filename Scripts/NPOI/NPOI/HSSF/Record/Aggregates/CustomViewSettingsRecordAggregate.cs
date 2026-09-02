using System;
using System.Collections.Generic;
using NPOI.HSSF.Model;

namespace NPOI.HSSF.Record.Aggregates;

public class CustomViewSettingsRecordAggregate : RecordAggregate
{
	private Record _begin;

	private Record _end;

	private List<RecordBase> _recs;

	private PageSettingsBlock _psBlock;

	public CustomViewSettingsRecordAggregate(RecordStream rs)
	{
		_begin = rs.GetNext();
		if (_begin.Sid != 426)
		{
			throw new InvalidOperationException("Bad begin record");
		}
		List<RecordBase> list = new List<RecordBase>();
		while (rs.PeekNextSid() != 427)
		{
			if (PageSettingsBlock.IsComponentRecord(rs.PeekNextSid()))
			{
				if (_psBlock != null)
				{
					if (rs.PeekNextSid() != 2204)
					{
						throw new InvalidOperationException("Found more than one PageSettingsBlock in chart sub-stream, had sid: " + rs.PeekNextSid());
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
		_end = rs.GetNext();
		if (_end.Sid != 427)
		{
			throw new InvalidOperationException("Bad custom view Settings end record");
		}
	}

	public override void VisitContainedRecords(RecordVisitor rv)
	{
		if (_recs.Count == 0)
		{
			return;
		}
		rv.VisitRecord(_begin);
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
		rv.VisitRecord(_end);
	}

	public static bool IsBeginRecord(int sid)
	{
		return sid == 426;
	}

	public void Append(RecordBase r)
	{
		_recs.Add(r);
	}
}
