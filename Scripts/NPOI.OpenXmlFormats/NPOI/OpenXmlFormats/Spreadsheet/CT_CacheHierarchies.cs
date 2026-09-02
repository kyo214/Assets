using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_CacheHierarchies
{
	private List<CT_CacheHierarchy> cacheHierarchyField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("cacheHierarchy", Order = 0)]
	public List<CT_CacheHierarchy> cacheHierarchy
	{
		get
		{
			return cacheHierarchyField;
		}
		set
		{
			cacheHierarchyField = value;
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

	public static CT_CacheHierarchies Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_CacheHierarchies cT_CacheHierarchies = new CT_CacheHierarchies();
		if (node.Attributes["count"] != null)
		{
			cT_CacheHierarchies.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_CacheHierarchies.cacheHierarchy = new List<CT_CacheHierarchy>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "cacheHierarchy")
			{
				cT_CacheHierarchies.cacheHierarchy.Add(CT_CacheHierarchy.Parse(childNode, namespaceManager));
			}
		}
		return cT_CacheHierarchies;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (cacheHierarchy != null)
		{
			foreach (CT_CacheHierarchy item in cacheHierarchy)
			{
				item.Write(sw, "cacheHierarchy");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_CacheHierarchies()
	{
		cacheHierarchyField = new List<CT_CacheHierarchy>();
	}
}
