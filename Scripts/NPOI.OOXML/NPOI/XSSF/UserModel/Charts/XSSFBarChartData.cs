using System;
using System.Collections.Generic;
using System.Drawing;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.SS.UserModel;
using NPOI.SS.UserModel.Charts;

namespace NPOI.XSSF.UserModel.Charts;

public class XSSFBarChartData<Tx, Ty> : IBarChartData<Tx, Ty>, IChartData
{
	public class Series : AbstractXSSFChartSeries, IBarChartSeries<Tx, Ty>, IChartSeries
	{
		private int id;

		private int order;

		private byte[] fillColor;

		private IChartDataSource<Tx> categories;

		private IChartDataSource<Ty> values;

		internal Series(int id, int order, IChartDataSource<Tx> categories, IChartDataSource<Ty> values)
		{
			this.id = id;
			this.order = order;
			this.categories = categories;
			this.values = values;
		}

		public void SetId(int id)
		{
			this.id = id;
		}

		public void SetOrder(int order)
		{
			this.order = order;
		}

		public void SetFillColor(Color color)
		{
			fillColor = new byte[3];
			fillColor[0] = color.R;
			fillColor[1] = color.G;
			fillColor[2] = color.B;
		}

		public IChartDataSource<Tx> GetCategoryAxisData()
		{
			return categories;
		}

		public IChartDataSource<Ty> GetValues()
		{
			return values;
		}

		internal void AddToChart(CT_BarChart ctBarChart)
		{
			CT_BarSer cT_BarSer = ctBarChart.AddNewSer();
			ctBarChart.AddNewGrouping().val = ST_BarGrouping.clustered;
			cT_BarSer.AddNewIdx().val = (uint)id;
			cT_BarSer.AddNewOrder().val = (uint)order;
			CT_Boolean cT_Boolean = new CT_Boolean();
			cT_Boolean.val = 0;
			cT_BarSer.invertIfNegative = cT_Boolean;
			ctBarChart.AddNewBarDir().val = ST_BarDir.col;
			XSSFChartUtil.BuildAxDataSource(cT_BarSer.AddNewCat(), categories);
			XSSFChartUtil.BuildNumDataSource(cT_BarSer.AddNewVal(), values);
			if (base.IsTitleSet)
			{
				cT_BarSer.tx = GetCTSerTx();
			}
			if (fillColor != null)
			{
				cT_BarSer.spPr = new CT_ShapeProperties();
				cT_BarSer.spPr.AddNewSolidFill().AddNewSrgbClr().val = fillColor;
			}
		}
	}

	private List<IBarChartSeries<Tx, Ty>> series;

	public XSSFBarChartData()
	{
		series = new List<IBarChartSeries<Tx, Ty>>();
	}

	public IBarChartSeries<Tx, Ty> AddSeries(IChartDataSource<Tx> categoryAxisData, IChartDataSource<Ty> values)
	{
		if (!values.IsNumeric)
		{
			throw new ArgumentException("Value data source must be numeric.");
		}
		int count = this.series.Count;
		Series series = new Series(count, count, categoryAxisData, values);
		this.series.Add(series);
		return series;
	}

	public List<IBarChartSeries<Tx, Ty>> GetSeries()
	{
		return series;
	}

	public void FillChart(IChart chart, params IChartAxis[] axis)
	{
		if (!(chart is XSSFChart))
		{
			throw new ArgumentException("Chart must be instance of XSSFChart");
		}
		CT_PlotArea plotArea = ((XSSFChart)chart).GetCTChart().plotArea;
		int allSeriesCount = plotArea.GetAllSeriesCount();
		CT_BarChart cT_BarChart = plotArea.AddNewBarChart();
		cT_BarChart.AddNewVaryColors().val = 0;
		for (int i = 0; i < series.Count; i++)
		{
			Series obj = (Series)series[i];
			obj.SetId(allSeriesCount + i);
			obj.SetOrder(allSeriesCount + i);
			obj.AddToChart(cT_BarChart);
		}
		foreach (IChartAxis chartAxis in axis)
		{
			cT_BarChart.AddNewAxId().val = (uint)chartAxis.Id;
		}
	}
}
