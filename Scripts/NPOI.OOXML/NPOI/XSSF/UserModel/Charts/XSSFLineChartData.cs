using System;
using System.Collections.Generic;
using System.Drawing;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.SS.UserModel;
using NPOI.SS.UserModel.Charts;

namespace NPOI.XSSF.UserModel.Charts;

public class XSSFLineChartData<Tx, Ty> : ILineChartData<Tx, Ty>, IChartData
{
	public class Series : AbstractXSSFChartSeries, ILineChartSeries<Tx, Ty>, IChartSeries
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

		internal void AddToChart(CT_LineChart ctLineChart)
		{
			CT_LineSer cT_LineSer = ctLineChart.AddNewSer();
			ctLineChart.AddNewGrouping().val = ST_Grouping.standard;
			cT_LineSer.AddNewIdx().val = (uint)id;
			cT_LineSer.AddNewOrder().val = (uint)order;
			cT_LineSer.AddNewMarker().AddNewSymbol().val = ST_MarkerStyle.none;
			XSSFChartUtil.BuildAxDataSource(cT_LineSer.AddNewCat(), categories);
			XSSFChartUtil.BuildNumDataSource(cT_LineSer.AddNewVal(), values);
			if (base.IsTitleSet)
			{
				cT_LineSer.tx = GetCTSerTx();
			}
			if (fillColor != null)
			{
				cT_LineSer.spPr = new CT_ShapeProperties();
				cT_LineSer.spPr.AddNewLn().AddNewSolidFill().AddNewSrgbClr()
					.val = fillColor;
			}
		}
	}

	private List<ILineChartSeries<Tx, Ty>> series;

	public XSSFLineChartData()
	{
		series = new List<ILineChartSeries<Tx, Ty>>();
	}

	public ILineChartSeries<Tx, Ty> AddSeries(IChartDataSource<Tx> categoryAxisData, IChartDataSource<Ty> values)
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

	public List<ILineChartSeries<Tx, Ty>> GetSeries()
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
		CT_LineChart cT_LineChart = plotArea.AddNewLineChart();
		cT_LineChart.AddNewVaryColors().val = 0;
		for (int i = 0; i < series.Count; i++)
		{
			Series obj = (Series)series[i];
			obj.SetId(allSeriesCount + i);
			obj.SetOrder(allSeriesCount + i);
			obj.AddToChart(cT_LineChart);
		}
		foreach (IChartAxis chartAxis in axis)
		{
			cT_LineChart.AddNewAxId().val = (uint)chartAxis.Id;
		}
	}
}
