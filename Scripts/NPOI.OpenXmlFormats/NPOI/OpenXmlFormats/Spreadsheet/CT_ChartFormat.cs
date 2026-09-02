using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_ChartFormat
{
	private CT_PivotArea pivotAreaField;

	private uint chartField;

	private uint formatField;

	private bool seriesField;

	[XmlElement(Order = 0)]
	public CT_PivotArea pivotArea
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
	public uint chart
	{
		get
		{
			return chartField;
		}
		set
		{
			chartField = value;
		}
	}

	[XmlAttribute]
	public uint format
	{
		get
		{
			return formatField;
		}
		set
		{
			formatField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool series
	{
		get
		{
			return seriesField;
		}
		set
		{
			seriesField = value;
		}
	}

	public static CT_ChartFormat Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_ChartFormat cT_ChartFormat = new CT_ChartFormat();
		if (node.Attributes["chart"] != null)
		{
			cT_ChartFormat.chart = XmlHelper.ReadUInt(node.Attributes["chart"]);
		}
		if (node.Attributes["format"] != null)
		{
			cT_ChartFormat.format = XmlHelper.ReadUInt(node.Attributes["format"]);
		}
		if (node.Attributes["series"] != null)
		{
			cT_ChartFormat.series = XmlHelper.ReadBool(node.Attributes["series"]);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "pivotArea")
			{
				cT_ChartFormat.pivotArea = CT_PivotArea.Parse(childNode, namespaceManager);
			}
		}
		return cT_ChartFormat;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "chart", chart);
		XmlHelper.WriteAttribute(sw, "format", format);
		XmlHelper.WriteAttribute(sw, "series", series);
		sw.Write(">");
		if (pivotArea != null)
		{
			pivotArea.Write(sw, "pivotArea");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_ChartFormat()
	{
		pivotAreaField = new CT_PivotArea();
		seriesField = false;
	}
}
