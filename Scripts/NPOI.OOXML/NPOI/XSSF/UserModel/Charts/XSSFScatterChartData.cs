using System;
using System.Collections.Generic;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.SS.UserModel;
using NPOI.SS.UserModel.Charts;

namespace NPOI.XSSF.UserModel.Charts;

public class XSSFScatterChartData<Tx, Ty> : IScatterChartData<Tx, Ty>, IChartData
{
	internal class Series : AbstractXSSFChartSeries, IScatterChartSeries<Tx, Ty>, IChartSeries
	{
		private int id;

		private int order;

		private bool useCache;

		private IChartDataSource<Tx> xs;

		private IChartDataSource<Ty> ys;

		internal Series(int id, int order, IChartDataSource<Tx> xs, IChartDataSource<Ty> ys)
		{
			this.id = id;
			this.order = order;
			this.xs = xs;
			this.ys = ys;
		}

		public void SetId(int id)
		{
			this.id = id;
		}

		public void SetOrder(int order)
		{
			this.order = order;
		}

		public IChartDataSource<Tx> GetXValues()
		{
			return xs;
		}

		public IChartDataSource<Ty> GetYValues()
		{
			return ys;
		}

		public void SetUseCache(bool useCache)
		{
			this.useCache = useCache;
		}

		internal void AddToChart(CT_ScatterChart ctScatterChart)
		{
			CT_ScatterSer cT_ScatterSer = ctScatterChart.AddNewSer();
			cT_ScatterSer.AddNewIdx().val = (uint)id;
			cT_ScatterSer.AddNewOrder().val = (uint)order;
			XSSFChartUtil.BuildAxDataSource(cT_ScatterSer.AddNewXVal(), xs);
			XSSFChartUtil.BuildNumDataSource(cT_ScatterSer.AddNewYVal(), ys);
			if (base.IsTitleSet)
			{
				cT_ScatterSer.tx = GetCTSerTx();
			}
		}
	}

	private List<IScatterChartSeries<Tx, Ty>> series;

	public XSSFScatterChartData()
	{
		series = new List<IScatterChartSeries<Tx, Ty>>();
	}

	public IScatterChartSeries<Tx, Ty> AddSeries(IChartDataSource<Tx> xs, IChartDataSource<Ty> ys)
	{
		if (!ys.IsNumeric)
		{
			throw new ArgumentException("Y axis data source must be numeric.");
		}
		int count = this.series.Count;
		Series series = new Series(count, count, xs, ys);
		this.series.Add(series);
		return series;
	}

	public void FillChart(IChart chart, IChartAxis[] axis)
	{
		if (!(chart is XSSFChart))
		{
			throw new ArgumentException("Chart must be instance of XSSFChart");
		}
		CT_PlotArea plotArea = ((XSSFChart)chart).GetCTChart().plotArea;
		int allSeriesCount = plotArea.GetAllSeriesCount();
		CT_ScatterChart cT_ScatterChart = plotArea.AddNewScatterChart();
		AddStyle(cT_ScatterChart);
		for (int i = 0; i < series.Count; i++)
		{
			Series obj = (Series)series[i];
			obj.SetId(allSeriesCount + i);
			obj.SetOrder(allSeriesCount + i);
			obj.AddToChart(cT_ScatterChart);
		}
		foreach (IChartAxis chartAxis in axis)
		{
			cT_ScatterChart.AddNewAxId().val = (uint)chartAxis.Id;
		}
	}

	public List<IScatterChartSeries<Tx, Ty>> GetSeries()
	{
		return series;
	}

	private void AddStyle(CT_ScatterChart ctScatterChart)
	{
		ctScatterChart.AddNewScatterStyle().val = ST_ScatterStyle.lineMarker;
	}
}
