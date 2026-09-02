using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_MeasureDimensionMaps
{
	private List<CT_MeasureDimensionMap> mapField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("map", Order = 0)]
	public List<CT_MeasureDimensionMap> map
	{
		get
		{
			return mapField;
		}
		set
		{
			mapField = value;
		}
	}

	[XmlAttribute]
	public uint count
	{
		get
		{
			return countField;
		}
		set
		{
			countField = value;
		}
	}

	[XmlIgnore]
	public bool countSpecified
	{
		get
		{
			return countFieldSpecified;
		}
		set
		{
			countFieldSpecified = value;
		}
	}

	public static CT_MeasureDimensionMaps Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_MeasureDimensionMaps cT_MeasureDimensionMaps = new CT_MeasureDimensionMaps();
		if (node.Attributes["count"] != null)
		{
			cT_MeasureDimensionMaps.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_MeasureDimensionMaps.map = new List<CT_MeasureDimensionMap>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "map")
			{
				cT_MeasureDimensionMaps.map.Add(CT_MeasureDimensionMap.Parse(childNode, namespaceManager));
			}
		}
		return cT_MeasureDimensionMaps;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (map != null)
		{
			foreach (CT_MeasureDimensionMap item in map)
			{
				item.Write(sw, "map");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_MeasureDimensionMaps()
	{
		mapField = new List<CT_MeasureDimensionMap>();
	}
}
