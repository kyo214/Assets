using System;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Spreadsheet;

namespace NPOI.XSSF.UserModel;

public class XSSFPivotCache : POIXMLDocumentPart
{
	private CT_PivotCache ctPivotCache;

	public XSSFPivotCache()
	{
		ctPivotCache = new CT_PivotCache();
	}

	public XSSFPivotCache(CT_PivotCache ctPivotCache)
	{
		this.ctPivotCache = ctPivotCache;
	}

	protected XSSFPivotCache(PackagePart part)
		: base(part)
	{
		ReadFrom(part.GetInputStream());
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	protected XSSFPivotCache(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	protected void ReadFrom(Stream is1)
	{
		try
		{
			XmlDocument xmlDocument = POIXMLDocumentPart.ConvertStreamToXml(is1);
			ctPivotCache = CT_PivotCache.Parse(xmlDocument.DocumentElement, POIXMLDocumentPart.NamespaceManager);
		}
		catch (XmlException ex)
		{
			throw new IOException(ex.Message);
		}
	}

	public CT_PivotCache GetCTPivotCache()
	{
		return ctPivotCache;
	}
}
