using System;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Spreadsheet;

namespace NPOI.XSSF.UserModel;

public class XSSFPivotCacheRecords : POIXMLDocumentPart
{
	private CT_PivotCacheRecords ctPivotCacheRecords;

	public XSSFPivotCacheRecords()
	{
		ctPivotCacheRecords = new CT_PivotCacheRecords();
	}

	protected XSSFPivotCacheRecords(PackagePart part)
		: base(part)
	{
		ReadFrom(part.GetInputStream());
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	protected XSSFPivotCacheRecords(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	protected void ReadFrom(Stream is1)
	{
		try
		{
			XmlDocument xmlDocument = POIXMLDocumentPart.ConvertStreamToXml(is1);
			ctPivotCacheRecords = CT_PivotCacheRecords.Parse(xmlDocument.DocumentElement, POIXMLDocumentPart.NamespaceManager);
		}
		catch (XmlException ex)
		{
			throw new IOException(ex.Message);
		}
	}

	public CT_PivotCacheRecords GetCtPivotCacheRecords()
	{
		return ctPivotCacheRecords;
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		ctPivotCacheRecords.Save(outputStream);
		outputStream.Close();
	}
}
