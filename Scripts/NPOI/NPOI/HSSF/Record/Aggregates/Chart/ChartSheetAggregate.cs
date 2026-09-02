using System;
using System.Collections.Generic;
using System.IO;
using NPOI.HSSF.Model;

namespace NPOI.HSSF.Record.Aggregates.Chart;

public class ChartSheetAggregate : ChartRecordAggregate
{
	private BOFRecord _bofRec;

	private List<RecordBase> _recs;

	private PageSettingsBlock _psBlock;

	private ChartFormatsAggregate chartFormats;

	private SeriesDataAggregate seriesData;

	internal int AttachLabelCount;

	public ChartSheetAggregate(RecordStream rs, ChartRecordAggregate container)
		: base("CHARTSHEET", container)
	{
		_bofRec = (BOFRecord)rs.GetNext();
		List<RecordBase> list = new List<RecordBase>();
		while (rs.PeekNextClass() != typeof(EOFRecord))
		{
			rs.PeekNextClass();
			if (PageSettingsBlock.IsComponentRecord(rs.PeekNextChartSid()))
			{
				if (_psBlock != null)
				{
					if (rs.PeekNextChartSid() != 2204)
					{
						throw new InvalidDataException("Found more than one PageSettingsBlock in chart sub-stream");
					}
					_psBlock.AddLateHeaderFooter((HeaderFooterRecord)rs.GetNext());
				}
				else
				{
					_psBlock = new PageSettingsBlock(rs);
					list.Add(_psBlock);
				}
			}
			else if (rs.PeekNextChartSid() == 4098)
			{
				chartFormats = new ChartFormatsAggregate(rs, this);
				list.Add(chartFormats);
			}
			else if (rs.PeekNextChartSid() == 512)
			{
				seriesData = new SeriesDataAggregate(rs);
				list.Add(seriesData);
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
