using System.Collections.Generic;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record.Chart;

namespace NPOI.HSSF.Record.Aggregates.Chart;

public class AXSAggregate : ChartRecordAggregate
{
	private IFmtRecordRecord ifmt;

	private TickRecord tick;

	private FontIndexRecord fontx;

	private List<AxisLineFormatRecord> axisLines = new List<AxisLineFormatRecord>();

	private List<LineFormatRecord> lineFormats = new List<LineFormatRecord>();

	private AreaFormatRecord areaFormat;

	private GelFrameAggregate gelFrame;

	private List<ShapePropsAggregate> shapes = new List<ShapePropsAggregate>();

	private TextPropsStreamRecord textProps;

	private List<ContinueFrt12Record> continues = new List<ContinueFrt12Record>();

	public AXSAggregate(RecordStream rs, ChartRecordAggregate container)
		: base("AXS", container)
	{
		if (rs.PeekNextChartSid() == 4174)
		{
			ifmt = (IFmtRecordRecord)rs.GetNext();
		}
		if (rs.PeekNextChartSid() == 4126)
		{
			tick = (TickRecord)rs.GetNext();
		}
		if (rs.PeekNextChartSid() == 4134)
		{
			fontx = (FontIndexRecord)rs.GetNext();
		}
		if (rs.PeekNextChartSid() == 4129)
		{
			while (rs.PeekNextChartSid() == 4129)
			{
				axisLines.Add((AxisLineFormatRecord)rs.GetNext());
				lineFormats.Add((LineFormatRecord)rs.GetNext());
			}
		}
		if (rs.PeekNextChartSid() == 4106)
		{
			areaFormat = (AreaFormatRecord)rs.GetNext();
		}
		if (rs.PeekNextChartSid() == 4198)
		{
			gelFrame = new GelFrameAggregate(rs, this);
		}
		if (rs.PeekNextChartSid() == 2212)
		{
			while (rs.PeekNextChartSid() == 2212)
			{
				shapes.Add(new ShapePropsAggregate(rs, this));
			}
		}
		if (rs.PeekNextChartSid() == 2213)
		{
			textProps = (TextPropsStreamRecord)rs.GetNext();
			while (rs.PeekNextChartSid() == 2175)
			{
				continues.Add((ContinueFrt12Record)rs.GetNext());
			}
		}
	}

	public override void VisitContainedRecords(RecordVisitor rv)
	{
		if (ifmt != null)
		{
			rv.VisitRecord(ifmt);
		}
		if (tick != null)
		{
			rv.VisitRecord(tick);
		}
		if (fontx != null)
		{
			rv.VisitRecord(fontx);
		}
		for (int i = 0; i < axisLines.Count; i++)
		{
			rv.VisitRecord(axisLines[i]);
			rv.VisitRecord(lineFormats[i]);
		}
		if (areaFormat != null)
		{
			rv.VisitRecord(areaFormat);
		}
		if (gelFrame != null)
		{
			gelFrame.VisitContainedRecords(rv);
		}
		foreach (ShapePropsAggregate shape in shapes)
		{
			shape.VisitContainedRecords(rv);
		}
		if (textProps == null)
		{
			return;
		}
		rv.VisitRecord(textProps);
		foreach (ContinueFrt12Record @continue in continues)
		{
			rv.VisitRecord(@continue);
		}
	}
}
