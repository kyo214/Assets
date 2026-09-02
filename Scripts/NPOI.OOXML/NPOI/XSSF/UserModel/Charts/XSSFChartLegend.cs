using System;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.SS.UserModel.Charts;

namespace NPOI.XSSF.UserModel.Charts;

public class XSSFChartLegend : IChartLegend, ManuallyPositionable
{
	private CT_Legend legend;

	public LegendPosition Position
	{
		get
		{
			if (legend.IsSetLegendPos())
			{
				return ToLegendPosition(legend.legendPos);
			}
			return LegendPosition.Right;
		}
		set
		{
			if (!legend.IsSetLegendPos())
			{
				legend.AddNewLegendPos();
			}
			legend.legendPos.val = FromLegendPosition(value);
			legend.legendPosSpecified = true;
		}
	}

	public bool IsOverlay
	{
		get
		{
			return legend.overlay.val != 0;
		}
		set
		{
			legend.overlay.val = (value ? 1 : 0);
		}
	}

	public XSSFChartLegend(XSSFChart chart)
	{
		CT_Chart cTChart = chart.GetCTChart();
		legend = (cTChart.IsSetLegend() ? cTChart.legend : cTChart.AddNewLegend());
		SetDefaults();
	}

	private void SetDefaults()
	{
		if (!legend.IsSetOverlay())
		{
			legend.AddNewOverlay();
		}
		legend.overlay.val = 0;
	}

	internal CT_Legend GetCTLegend()
	{
		return legend;
	}

	public IManualLayout GetManualLayout()
	{
		if (!legend.IsSetLayout())
		{
			legend.AddNewLayout();
		}
		return new XSSFManualLayout(legend.layout);
	}

	private ST_LegendPos FromLegendPosition(LegendPosition position)
	{
		return position switch
		{
			LegendPosition.Bottom => ST_LegendPos.b, 
			LegendPosition.Left => ST_LegendPos.l, 
			LegendPosition.Right => ST_LegendPos.r, 
			LegendPosition.Top => ST_LegendPos.t, 
			LegendPosition.TopRight => ST_LegendPos.tr, 
			_ => throw new ArgumentException(), 
		};
	}

	private LegendPosition ToLegendPosition(CT_LegendPos ctLegendPos)
	{
		return ctLegendPos.val switch
		{
			ST_LegendPos.b => LegendPosition.Bottom, 
			ST_LegendPos.l => LegendPosition.Left, 
			ST_LegendPos.r => LegendPosition.Right, 
			ST_LegendPos.t => LegendPosition.Top, 
			ST_LegendPos.tr => LegendPosition.TopRight, 
			_ => throw new ArgumentException(), 
		};
	}
}
