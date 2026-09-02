using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart", IsNullable = true)]
public class CT_ChartLines
{
	private CT_ShapeProperties spPrField;

	[XmlElement(Order = 0)]
	public CT_ShapeProperties spPr
	{
		get
		{
			return spPrField;
		}
		set
		{
			spPrField = value;
		}
	}

	public static CT_ChartLines Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_ChartLines cT_ChartLines = new CT_ChartLines();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "spPr")
			{
				cT_ChartLines.spPr = CT_ShapeProperties.Parse(childNode, namespaceManager);
			}
		}
		return cT_ChartLines;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		sw.Write(">");
		if (spPr != null)
		{
			spPr.Write(sw, "spPr");
		}
		sw.Write($"</c:{nodeName}>");
	}
}
