using System;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.Util;

namespace NPOI.XSSF.UserModel;

public class XSSFChartSheet : XSSFSheet
{
	private static byte[] BLANK_WORKSHEET = blankWorksheet();

	protected CT_Chartsheet chartsheet;

	protected XSSFChartSheet(PackagePart part)
		: base(part)
	{
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	protected XSSFChartSheet(PackagePart part, PackageRelationship rel)
		: base(part)
	{
	}

	internal override void Read(Stream is1)
	{
		base.Read(new MemoryStream(BLANK_WORKSHEET));
		try
		{
			XmlDocument xmldoc = POIXMLDocumentPart.ConvertStreamToXml(is1);
			chartsheet = ChartsheetDocument.Parse(xmldoc, POIXMLDocumentPart.NamespaceManager).GetChartsheet();
		}
		catch (XmlException ex)
		{
			throw new POIXMLException(ex);
		}
	}

	public CT_Chartsheet GetCTChartsheet()
	{
		return chartsheet;
	}

	protected override CT_Drawing GetCTDrawing()
	{
		return chartsheet.drawing;
	}

	protected override CT_LegacyDrawing GetCTLegacyDrawing()
	{
		return chartsheet.legacyDrawing;
	}

	internal override void Write(Stream out1)
	{
		new ChartsheetDocument(chartsheet).Save(out1);
	}

	private static byte[] blankWorksheet()
	{
		MemoryStream memoryStream = new MemoryStream();
		try
		{
			new XSSFSheet().Write(memoryStream);
		}
		catch (IOException e)
		{
			throw new RuntimeException(e);
		}
		return memoryStream.ToArray();
	}
}
