using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.XSSF.UserModel;

namespace NPOI.XSSF.Model;

public class MapInfo : POIXMLDocumentPart
{
	private CT_MapInfo mapInfo;

	private Dictionary<int, XSSFMap> maps;

	private XmlDocument xml;

	public XSSFWorkbook Workbook => (XSSFWorkbook)GetParent();

	public MapInfo()
	{
		mapInfo = new CT_MapInfo();
	}

	internal MapInfo(PackagePart part)
		: base(part)
	{
		ReadFrom(part.GetInputStream());
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	public MapInfo(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	public void ReadFrom(Stream is1)
	{
		try
		{
			MapInfoDocument mapInfoDocument = MapInfoDocument.Parse(POIXMLDocumentPart.ConvertStreamToXml(is1), POIXMLDocumentPart.NamespaceManager);
			mapInfo = mapInfoDocument.GetMapInfo();
			maps = new Dictionary<int, XSSFMap>();
			foreach (CT_Map item in mapInfo.Map)
			{
				maps[(int)item.ID] = new XSSFMap(item, this);
			}
		}
		catch (XmlException ex)
		{
			throw new IOException(ex.Message);
		}
	}

	public CT_MapInfo GetCTMapInfo()
	{
		return mapInfo;
	}

	public CT_Schema GetCTSchemaById(string schemaId)
	{
		CT_Schema result = null;
		foreach (CT_Schema item in mapInfo.Schema)
		{
			if (item.ID.Equals(schemaId))
			{
				result = item;
				break;
			}
		}
		return result;
	}

	public XSSFMap GetXSSFMapById(int id)
	{
		return maps[id];
	}

	public XSSFMap GetXSSFMapByName(string name)
	{
		XSSFMap result = null;
		foreach (XSSFMap value in maps.Values)
		{
			if (value.GetCTMap().Name != null && value.GetCTMap().Name.Equals(name))
			{
				result = value;
			}
		}
		return result;
	}

	public List<XSSFMap> GetAllXSSFMaps()
	{
		List<XSSFMap> list = new List<XSSFMap>();
		foreach (XSSFMap value in maps.Values)
		{
			list.Add(value);
		}
		return list;
	}

	protected void WriteTo(Stream out1)
	{
		xml.Save(out1);
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		WriteTo(outputStream);
		outputStream.Close();
	}
}
