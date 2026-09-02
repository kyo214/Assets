using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_PivotAreas
{
	private List<CT_PivotArea> pivotAreaField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("pivotArea", Order = 0)]
	public List<CT_PivotArea> pivotArea
	{
		get
		{
			return pivotAreaField;
		}
		set
		{
			pivotAreaField = value;
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

	public static CT_PivotAreas Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PivotAreas cT_PivotAreas = new CT_PivotAreas();
		if (node.Attributes["count"] != null)
		{
			cT_PivotAreas.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_PivotAreas.pivotArea = new List<CT_PivotArea>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "pivotArea")
			{
				cT_PivotAreas.pivotArea.Add(CT_PivotArea.Parse(childNode, namespaceManager));
			}
		}
		return cT_PivotAreas;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (pivotArea != null)
		{
			foreach (CT_PivotArea item in pivotArea)
			{
				item.Write(sw, "pivotArea");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_PivotAreas()
	{
		pivotAreaField = new List<CT_PivotArea>();
	}
}
