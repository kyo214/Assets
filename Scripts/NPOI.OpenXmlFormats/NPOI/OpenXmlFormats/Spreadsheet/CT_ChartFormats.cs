using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_ChartFormats
{
	private List<CT_ChartFormat> chartFormatField;

	private uint countField;

	[XmlElement("chartFormat", Order = 0)]
	public List<CT_ChartFormat> chartFormat
	{
		get
		{
			return chartFormatField;
		}
		set
		{
			chartFormatField = value;
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

	public static CT_ChartFormats Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_ChartFormats cT_ChartFormats = new CT_ChartFormats();
		if (node.Attributes["count"] != null)
		{
			cT_ChartFormats.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		cT_ChartFormats.chartFormat = new List<CT_ChartFormat>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "chartFormat")
			{
				cT_ChartFormats.chartFormat.Add(CT_ChartFormat.Parse(childNode, namespaceManager));
			}
		}
		return cT_ChartFormats;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "count", count);
		sw.Write(">");
		if (chartFormat != null)
		{
			foreach (CT_ChartFormat item in chartFormat)
			{
				item.Write(sw, "chartFormat");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_ChartFormats()
	{
		chartFormatField = new List<CT_ChartFormat>();
		countField = 0u;
	}
}
