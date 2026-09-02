using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_PivotHierarchies
{
	private List<CT_PivotHierarchy> pivotHierarchyField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("pivotHierarchy", Order = 0)]
	public List<CT_PivotHierarchy> pivotHierarchy
	{
		get
		{
			return pivotHierarchyField;
		}
		set
		{
			pivotHierarchyField = value;
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

	public static CT_PivotHierarchies Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PivotHierarchies cT_PivotHierarchies = new CT_PivotHierarchies();
		if (node.Attributes["count"] != null)
		{
			cT_PivotHierarchies.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_PivotHierarchies.pivotHierarchy = new List<CT_PivotHierarchy>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "pivotHierarchy")
			{
				cT_PivotHierarchies.pivotHierarchy.Add(CT_PivotHierarchy.Parse(childNode, namespaceManager));
			}
		}
		return cT_PivotHierarchies;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (pivotHierarchy != null)
		{
			foreach (CT_PivotHierarchy item in pivotHierarchy)
			{
				item.Write(sw, "pivotHierarchy");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_PivotHierarchies()
	{
		pivotHierarchyField = new List<CT_PivotHierarchy>();
	}
}
