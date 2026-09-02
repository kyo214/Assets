using System;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.SS.UserModel.Charts;

namespace NPOI.XSSF.UserModel.Charts;

public class XSSFValueAxis : XSSFChartAxis, IValueAxis, IChartAxis
{
	private CT_ValAx ctValAx;

	public override long Id => ctValAx.axId.val;

	public XSSFValueAxis(XSSFChart chart, long id, AxisPosition pos)
		: base(chart)
	{
		CreateAxis(id, pos);
	}

	public XSSFValueAxis(XSSFChart chart, CT_ValAx ctValAx)
		: base(chart)
	{
		this.ctValAx = ctValAx;
	}

	public void SetCrossBetween(AxisCrossBetween crossBetween)
	{
		ctValAx.crossBetween.val = fromCrossBetween(crossBetween);
	}

	public AxisCrossBetween GetCrossBetween()
	{
		return ToCrossBetween(ctValAx.crossBetween.val);
	}

	protected override CT_Boolean GetDelete()
	{
		return ctValAx.delete;
	}

	protected override CT_TickMark GetMajorCTTickMark()
	{
		return ctValAx.majorTickMark;
	}

	public void SetMajorCTTickMark(CT_TickMark tm)
	{
		ctValAx.majorTickMark = tm;
	}

	protected override CT_TickMark GetMinorCTTickMark()
	{
		return ctValAx.minorTickMark;
	}

	protected override CT_AxPos GetCTAxPos()
	{
		return ctValAx.axPos;
	}

	protected override CT_NumFmt GetCTNumFmt()
	{
		if (ctValAx.IsSetNumFmt())
		{
			return ctValAx.numFmt;
		}
		return ctValAx.AddNewNumFmt();
	}

	protected override CT_Scaling GetCTScaling()
	{
		return ctValAx.scaling;
	}

	protected override CT_Crosses GetCTCrosses()
	{
		return ctValAx.crosses;
	}

	public override void CrossAxis(IChartAxis axis)
	{
		ctValAx.crossAx.val = (uint)axis.Id;
	}

	private void CreateAxis(long id, AxisPosition pos)
	{
		ctValAx = chart.GetCTChart().plotArea.AddNewValAx();
		ctValAx.AddNewAxId().val = (uint)id;
		ctValAx.AddNewAxPos();
		ctValAx.AddNewScaling();
		ctValAx.AddNewCrossBetween();
		ctValAx.AddNewCrosses();
		ctValAx.AddNewCrossAx();
		ctValAx.AddNewTickLblPos().val = ST_TickLblPos.nextTo;
		ctValAx.AddNewDelete();
		ctValAx.AddNewMajorTickMark();
		ctValAx.AddNewMinorTickMark();
		base.Position = pos;
		base.Orientation = AxisOrientation.MinToMax;
		SetCrossBetween(AxisCrossBetween.MidpointCategory);
		base.Crosses = AxisCrosses.AutoZero;
		base.IsVisible = true;
		base.MajorTickMark = AxisTickMark.Cross;
		base.MinorTickMark = AxisTickMark.None;
	}

	private static ST_CrossBetween fromCrossBetween(AxisCrossBetween crossBetween)
	{
		return crossBetween switch
		{
			AxisCrossBetween.Between => ST_CrossBetween.between, 
			AxisCrossBetween.MidpointCategory => ST_CrossBetween.midCat, 
			_ => throw new ArgumentException(), 
		};
	}

	private static AxisCrossBetween ToCrossBetween(ST_CrossBetween ctCrossBetween)
	{
		return ctCrossBetween switch
		{
			ST_CrossBetween.between => AxisCrossBetween.Between, 
			ST_CrossBetween.midCat => AxisCrossBetween.MidpointCategory, 
			_ => throw new ArgumentException(), 
		};
	}
}
