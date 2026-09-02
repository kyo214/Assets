using System.IO;
using System.Xml;

namespace NPOI.OpenXmlFormats.Spreadsheet;

public class ChartsheetDocument
{
	private CT_Chartsheet sheet;

	public ChartsheetDocument()
	{
	}

	public ChartsheetDocument(CT_Chartsheet sheet)
	{
		this.sheet = sheet;
	}

	public static ChartsheetDocument Parse(XmlDocument xmldoc, XmlNamespaceManager nsmgr)
	{
		return new ChartsheetDocument(CT_Chartsheet.Parse(xmldoc.DocumentElement, nsmgr));
	}

	public CT_Chartsheet GetChartsheet()
	{
		return sheet;
	}

	public void SetChartsheet(CT_Chartsheet sheet)
	{
		this.sheet = sheet;
	}

	public void Save(Stream stream)
	{
		sheet.Write(stream);
	}
}
