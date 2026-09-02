using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Dimensions
{
	private List<CT_PivotDimension> dimensionField;

	private uint countField;

	private bool countFieldSpecified;

	[XmlElement("dimension", Order = 0)]
	public List<CT_PivotDimension> dimension
	{
		get
		{
			return dimensionField;
		}
		set
		{
			dimensionField = value;
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

	public static CT_Dimensions Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_Dimensions cT_Dimensions = new CT_Dimensions();
		if (node.Attributes["count"] != null)
		{
			cT_Dimensions.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_Dimensions.dimension = new List<CT_PivotDimension>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "dimension")
			{
				cT_Dimensions.dimension.Add(CT_PivotDimension.Parse(childNode, namespaceManager));
			}
		}
		return cT_Dimensions;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (dimension != null)
		{
			foreach (CT_PivotDimension item in dimension)
			{
				item.Write(sw, "dimension");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_Dimensions()
	{
		dimensionField = new List<CT_PivotDimension>();
	}
}
