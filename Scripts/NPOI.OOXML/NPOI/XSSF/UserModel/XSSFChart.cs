using System;
using System.Collections.Generic;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Dml;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.SS.UserModel;
using NPOI.SS.UserModel.Charts;
using NPOI.XSSF.UserModel.Charts;

namespace NPOI.XSSF.UserModel;

public class XSSFChart : POIXMLDocumentPart, IChart, ManuallyPositionable, IChartAxisFactory
{
	private XSSFGraphicFrame frame;

	private ChartSpaceDocument chartSpaceDocument;

	private CT_Chart chart;

	private List<IChartAxis> axis = new List<IChartAxis>();

	public IChartDataFactory ChartDataFactory => XSSFChartDataFactory.GetInstance();

	public IChartAxisFactory ChartAxisFactory => this;

	public XSSFRichTextString Title
	{
		get
		{
			if (!chart.IsSetTitle())
			{
				return null;
			}
			CT_Title title = chart.title;
			if (title.tx == null)
			{
				return null;
			}
			if (title.tx.rich == null)
			{
				return null;
			}
			return new XSSFRichTextString(title.tx.rich.ToString());
		}
	}

	public XSSFChart()
	{
		CreateChart();
	}

	protected XSSFChart(PackagePart part)
		: base(part)
	{
		XmlDocument xmldoc = POIXMLDocumentPart.ConvertStreamToXml(part.GetInputStream());
		chartSpaceDocument = ChartSpaceDocument.Parse(xmldoc, POIXMLDocumentPart.NamespaceManager);
		chart = chartSpaceDocument.GetChartSpace().chart;
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	protected XSSFChart(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	private void CreateChart()
	{
		chartSpaceDocument = new ChartSpaceDocument();
		chart = chartSpaceDocument.GetChartSpace().AddNewChart();
		chart.AddNewPlotArea().AddNewLayout();
		chart.AddNewPlotVisOnly().val = 1;
		CT_PrintSettings cT_PrintSettings = chartSpaceDocument.GetChartSpace().AddNewPrintSettings();
		cT_PrintSettings.AddNewHeaderFooter();
		CT_PageMargins cT_PageMargins = cT_PrintSettings.AddNewPageMargins();
		cT_PageMargins.b = 0.75;
		cT_PageMargins.l = 0.7;
		cT_PageMargins.r = 0.7;
		cT_PageMargins.t = 0.75;
		cT_PageMargins.header = 0.3;
		cT_PageMargins.footer = 0.3;
		cT_PrintSettings.AddNewPageSetup();
	}

	public CT_ChartSpace GetCTChartSpace()
	{
		return chartSpaceDocument.GetChartSpace();
	}

	public CT_Chart GetCTChart()
	{
		return chart;
	}

	protected internal override void Commit()
	{
		PackagePart packagePart = GetPackagePart();
		chartSpaceDocument.Save(packagePart.GetOutputStream());
	}

	public XSSFGraphicFrame GetGraphicFrame()
	{
		return frame;
	}

	internal void SetGraphicFrame(XSSFGraphicFrame frame)
	{
		this.frame = frame;
	}

	public void Plot(IChartData data, params IChartAxis[] axis)
	{
		data.FillChart(this, axis);
	}

	public IValueAxis CreateValueAxis(AxisPosition pos)
	{
		long id = axis.Count + 1;
		XSSFValueAxis xSSFValueAxis = new XSSFValueAxis(this, id, pos);
		if (axis.Count == 1)
		{
			IChartAxis chartAxis = axis[0];
			chartAxis.CrossAxis(xSSFValueAxis);
			xSSFValueAxis.CrossAxis(chartAxis);
		}
		axis.Add(xSSFValueAxis);
		return xSSFValueAxis;
	}

	public IChartAxis CreateCategoryAxis(AxisPosition pos)
	{
		long id = axis.Count + 1;
		XSSFCategoryAxis xSSFCategoryAxis = new XSSFCategoryAxis(this, id, pos);
		if (axis.Count == 1)
		{
			IChartAxis chartAxis = axis[0];
			chartAxis.CrossAxis(xSSFCategoryAxis);
			xSSFCategoryAxis.CrossAxis(chartAxis);
		}
		axis.Add(xSSFCategoryAxis);
		return xSSFCategoryAxis;
	}

	public IChartAxis CreateDateAxis(AxisPosition pos)
	{
		long id = axis.Count + 1;
		XSSFDateAxis xSSFDateAxis = new XSSFDateAxis(this, id, pos);
		if (axis.Count == 1)
		{
			IChartAxis chartAxis = axis[0];
			chartAxis.CrossAxis(xSSFDateAxis);
			xSSFDateAxis.CrossAxis(chartAxis);
		}
		axis.Add(xSSFDateAxis);
		return xSSFDateAxis;
	}

	public List<IChartAxis> GetAxis()
	{
		if (axis.Count == 0 && HasAxis())
		{
			ParseAxis();
		}
		return axis;
	}

	public IManualLayout GetManualLayout()
	{
		return new XSSFManualLayout(this);
	}

	public bool IsPlotOnlyVisibleCells()
	{
		if (chart.plotVisOnly.val != 1)
		{
			return false;
		}
		return true;
	}

	public void SetPlotOnlyVisibleCells(bool plotVisOnly)
	{
		chart.plotVisOnly.val = (plotVisOnly ? 1 : 0);
	}

	public void SetTitle(string newTitle)
	{
		CT_Title cT_Title = ((!chart.IsSetTitle()) ? chart.AddNewTitle() : chart.title);
		CT_Tx cT_Tx = ((!cT_Title.IsSetTx()) ? cT_Title.AddNewTx() : cT_Title.tx);
		if (cT_Tx.IsSetStrRef())
		{
			cT_Tx.UnsetStrRef();
		}
		NPOI.OpenXmlFormats.Dml.Chart.CT_TextBody cT_TextBody;
		if (cT_Tx.IsSetRich())
		{
			cT_TextBody = cT_Tx.rich;
		}
		else
		{
			cT_TextBody = cT_Tx.AddNewRich();
			cT_TextBody.AddNewBodyPr();
		}
		CT_TextParagraph cT_TextParagraph = ((cT_TextBody.SizeOfPArray() <= 0) ? cT_TextBody.AddNewP() : cT_TextBody.GetPArray(0));
		if (cT_TextParagraph.SizeOfRArray() > 0)
		{
			cT_TextParagraph.GetRArray(0).t = newTitle;
		}
		else if (cT_TextParagraph.SizeOfFldArray() > 0)
		{
			cT_TextParagraph.GetFldArray(0).t = newTitle;
		}
		else
		{
			cT_TextParagraph.AddNewR().t = newTitle;
		}
	}

	public IChartLegend GetOrCreateLegend()
	{
		return new XSSFChartLegend(this);
	}

	public void DeleteLegend()
	{
		if (chart.IsSetLegend())
		{
			chart.unsetLegend();
		}
	}

	public void SetCTDispBlanksAs(CT_DispBlanksAs disp)
	{
		chart.dispBlanksAs = disp;
	}

	private bool HasAxis()
	{
		CT_PlotArea plotArea = chart.plotArea;
		return ((plotArea.valAx != null) ? plotArea.valAx.Count : 0) + ((plotArea.catAx != null) ? plotArea.catAx.Count : 0) + ((plotArea.dateAx != null) ? plotArea.dateAx.Count : 0) + ((plotArea.serAx != null) ? plotArea.serAx.Count : 0) > 0;
	}

	private void ParseAxis()
	{
		ParseCategoryAxis();
		ParseValueAxis();
	}

	private void ParseCategoryAxis()
	{
		if (chart.plotArea.catAx == null)
		{
			return;
		}
		foreach (CT_CatAx item in chart.plotArea.catAx)
		{
			axis.Add(new XSSFCategoryAxis(this, item));
		}
	}

	private void ParseValueAxis()
	{
		if (chart.plotArea.valAx == null)
		{
			return;
		}
		foreach (CT_ValAx item in chart.plotArea.valAx)
		{
			axis.Add(new XSSFValueAxis(this, item));
		}
	}
}
