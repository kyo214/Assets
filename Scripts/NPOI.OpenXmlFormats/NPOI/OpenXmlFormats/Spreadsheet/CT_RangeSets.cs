using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_RangeSets
{
	private List<CT_RangeSet> rangeSetField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("rangeSet", Order = 0)]
	public List<CT_RangeSet> rangeSet
	{
		get
		{
			return rangeSetField;
		}
		set
		{
			rangeSetField = value;
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

	public static CT_RangeSets Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_RangeSets cT_RangeSets = new CT_RangeSets();
		if (node.Attributes["count"] != null)
		{
			cT_RangeSets.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_RangeSets.rangeSet = new List<CT_RangeSet>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "rangeSet")
			{
				cT_RangeSets.rangeSet.Add(CT_RangeSet.Parse(childNode, namespaceManager));
			}
		}
		return cT_RangeSets;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (rangeSet != null)
		{
			foreach (CT_RangeSet item in rangeSet)
			{
				item.Write(sw, "rangeSet");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_RangeSets()
	{
		rangeSetField = new List<CT_RangeSet>();
	}
}
