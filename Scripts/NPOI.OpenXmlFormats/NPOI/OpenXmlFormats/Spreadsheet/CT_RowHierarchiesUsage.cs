using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_RowHierarchiesUsage
{
	private List<CT_HierarchyUsage> rowHierarchyUsageField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("rowHierarchyUsage", Order = 0)]
	public List<CT_HierarchyUsage> rowHierarchyUsage
	{
		get
		{
			return rowHierarchyUsageField;
		}
		set
		{
			rowHierarchyUsageField = value;
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

	public static CT_RowHierarchiesUsage Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_RowHierarchiesUsage cT_RowHierarchiesUsage = new CT_RowHierarchiesUsage();
		if (node.Attributes["count"] != null)
		{
			cT_RowHierarchiesUsage.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_RowHierarchiesUsage.rowHierarchyUsage = new List<CT_HierarchyUsage>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "rowHierarchyUsage")
			{
				cT_RowHierarchiesUsage.rowHierarchyUsage.Add(CT_HierarchyUsage.Parse(childNode, namespaceManager));
			}
		}
		return cT_RowHierarchiesUsage;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (rowHierarchyUsage != null)
		{
			foreach (CT_HierarchyUsage item in rowHierarchyUsage)
			{
				item.Write(sw, "rowHierarchyUsage");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_RowHierarchiesUsage()
	{
		rowHierarchyUsageField = new List<CT_HierarchyUsage>();
	}
}
