using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.SS.UserModel.Charts;

namespace NPOI.XSSF.UserModel.Charts;

public class XSSFCategoryAxis : XSSFChartAxis
{
	private CT_CatAx ctCatAx;

	public override long Id => ctCatAx.axId.val;

	public XSSFCategoryAxis(XSSFChart chart, long id, AxisPosition pos)
		: base(chart)
	{
		createAxis(id, pos);
	}

	public XSSFCategoryAxis(XSSFChart chart, CT_CatAx ctCatAx)
		: base(chart)
	{
		this.ctCatAx = ctCatAx;
	}

	protected override CT_AxPos GetCTAxPos()
	{
		return ctCatAx.axPos;
	}

	protected override CT_NumFmt GetCTNumFmt()
	{
		if (ctCatAx.IsSetNumFmt())
		{
			return ctCatAx.numFmt;
		}
		return ctCatAx.AddNewNumFmt();
	}

	protected override CT_Scaling GetCTScaling()
	{
		return ctCatAx.scaling;
	}

	protected override CT_Crosses GetCTCrosses()
	{
		return ctCatAx.crosses;
	}

	protected override CT_Boolean GetDelete()
	{
		return ctCatAx.delete;
	}

	protected override CT_TickMark GetMajorCTTickMark()
	{
		return ctCatAx.majorTickMark;
	}

	public void SetMajorCTTickMark(CT_TickMark tm)
	{
		ctCatAx.majorTickMark = tm;
	}

	protected override CT_TickMark GetMinorCTTickMark()
	{
		return ctCatAx.minorTickMark;
	}

	public override void CrossAxis(IChartAxis axis)
	{
		ctCatAx.crossAx.val = (uint)axis.Id;
	}

	public void SetAuto(CT_Boolean au)
	{
		ctCatAx.auto = au;
	}

	private void createAxis(long id, AxisPosition pos)
	{
		ctCatAx = chart.GetCTChart().plotArea.AddNewCatAx();
		ctCatAx.AddNewAxId().val = (uint)id;
		ctCatAx.AddNewAxPos();
		ctCatAx.AddNewScaling();
		ctCatAx.AddNewCrosses();
		ctCatAx.AddNewCrossAx();
		ctCatAx.AddNewTickLblPos().val = ST_TickLblPos.nextTo;
		ctCatAx.AddNewDelete();
		ctCatAx.AddNewMajorTickMark();
		ctCatAx.AddNewMinorTickMark();
		base.Position = pos;
		base.Orientation = AxisOrientation.MinToMax;
		base.Crosses = AxisCrosses.AutoZero;
		base.IsVisible = true;
		base.MajorTickMark = AxisTickMark.Cross;
		base.MinorTickMark = AxisTickMark.None;
	}
}
