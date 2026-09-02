using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_ColHierarchiesUsage
{
	private List<CT_HierarchyUsage> colHierarchyUsageField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("colHierarchyUsage", Order = 0)]
	public List<CT_HierarchyUsage> colHierarchyUsage
	{
		get
		{
			return colHierarchyUsageField;
		}
		set
		{
			colHierarchyUsageField = value;
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

	public static CT_ColHierarchiesUsage Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_ColHierarchiesUsage cT_ColHierarchiesUsage = new CT_ColHierarchiesUsage();
		if (node.Attributes["count"] != null)
		{
			cT_ColHierarchiesUsage.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_ColHierarchiesUsage.colHierarchyUsage = new List<CT_HierarchyUsage>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "colHierarchyUsage")
			{
				cT_ColHierarchiesUsage.colHierarchyUsage.Add(CT_HierarchyUsage.Parse(childNode, namespaceManager));
			}
		}
		return cT_ColHierarchiesUsage;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (colHierarchyUsage != null)
		{
			foreach (CT_HierarchyUsage item in colHierarchyUsage)
			{
				item.Write(sw, "colHierarchyUsage");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_ColHierarchiesUsage()
	{
		colHierarchyUsageField = new List<CT_HierarchyUsage>();
	}
}
