using System.Collections.Generic;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.XSSF.Model;
using NPOI.XSSF.UserModel.Helpers;

namespace NPOI.XSSF.UserModel;

public class XSSFMap
{
	private CT_Map ctMap;

	private MapInfo mapInfo;

	public XSSFMap(CT_Map ctMap, MapInfo mapInfo)
	{
		this.ctMap = ctMap;
		this.mapInfo = mapInfo;
	}

	public CT_Map GetCTMap()
	{
		return ctMap;
	}

	public CT_Schema GetCTSchema()
	{
		string schemaID = ctMap.SchemaID;
		return mapInfo.GetCTSchemaById(schemaID);
	}

	public string GetSchema()
	{
		return GetCTSchema().InnerXml;
	}

	public List<XSSFSingleXmlCell> GetRelatedSingleXMLCell()
	{
		List<XSSFSingleXmlCell> list = new List<XSSFSingleXmlCell>();
		int numberOfSheets = mapInfo.Workbook.NumberOfSheets;
		for (int i = 0; i < numberOfSheets; i++)
		{
			foreach (POIXMLDocumentPart relation in ((XSSFSheet)mapInfo.Workbook.GetSheetAt(i)).GetRelations())
			{
				if (!(relation is SingleXmlCells))
				{
					continue;
				}
				foreach (XSSFSingleXmlCell item in ((SingleXmlCells)relation).GetAllSimpleXmlCell())
				{
					if (item.GetMapId() == ctMap.ID)
					{
						list.Add(item);
					}
				}
			}
		}
		return list;
	}

	public List<XSSFTable> GetRelatedTables()
	{
		List<XSSFTable> list = new List<XSSFTable>();
		_ = mapInfo.Workbook.NumberOfSheets;
		foreach (XSSFSheet item in mapInfo.Workbook)
		{
			foreach (POIXMLDocumentPart.RelationPart relationPart in item.RelationParts)
			{
				if (relationPart.Relationship.RelationshipType.Equals(XSSFRelation.TABLE.Relation))
				{
					XSSFTable xSSFTable = relationPart.DocumentPart as XSSFTable;
					if (xSSFTable.MapsTo(ctMap.ID))
					{
						list.Add(xSSFTable);
					}
				}
			}
		}
		return list;
	}
}
