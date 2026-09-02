using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_PivotFilters
{
	private List<CT_PivotFilter> filterField;

	private uint countField;

	[XmlElement("filter", Order = 0)]
	public List<CT_PivotFilter> filter
	{
		get
		{
			return filterField;
		}
		set
		{
			filterField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(uint), "0")]
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

	public static CT_PivotFilters Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PivotFilters cT_PivotFilters = new CT_PivotFilters();
		if (node.Attributes["count"] != null)
		{
			cT_PivotFilters.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_PivotFilters.filter = new List<CT_PivotFilter>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "filter")
			{
				cT_PivotFilters.filter.Add(CT_PivotFilter.Parse(childNode, namespaceManager));
			}
		}
		return cT_PivotFilters;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (filter != null)
		{
			foreach (CT_PivotFilter item in filter)
			{
				item.Write(sw, "filter");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_PivotFilters()
	{
		filterField = new List<CT_PivotFilter>();
		countField = 0u;
	}
}
