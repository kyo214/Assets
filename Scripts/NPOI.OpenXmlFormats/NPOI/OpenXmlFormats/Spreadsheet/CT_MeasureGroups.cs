using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_MeasureGroups
{
	private List<CT_MeasureGroup> measureGroupField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("measureGroup", Order = 0)]
	public List<CT_MeasureGroup> measureGroup
	{
		get
		{
			return measureGroupField;
		}
		set
		{
			measureGroupField = value;
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

	public static CT_MeasureGroups Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_MeasureGroups cT_MeasureGroups = new CT_MeasureGroups();
		if (node.Attributes["count"] != null)
		{
			cT_MeasureGroups.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_MeasureGroups.measureGroup = new List<CT_MeasureGroup>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "measureGroup")
			{
				cT_MeasureGroups.measureGroup.Add(CT_MeasureGroup.Parse(childNode, namespaceManager));
			}
		}
		return cT_MeasureGroups;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (measureGroup != null)
		{
			foreach (CT_MeasureGroup item in measureGroup)
			{
				item.Write(sw, "measureGroup");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_MeasureGroups()
	{
		measureGroupField = new List<CT_MeasureGroup>();
	}
}
