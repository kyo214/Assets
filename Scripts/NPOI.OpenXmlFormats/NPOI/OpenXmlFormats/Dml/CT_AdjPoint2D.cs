using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_AdjPoint2D
{
	private string xField;

	private string yField;

	[XmlAttribute]
	public string x
	{
		get
		{
			return xField;
		}
		set
		{
			xField = value;
		}
	}

	[XmlAttribute]
	public string y
	{
		get
		{
			return yField;
		}
		set
		{
			yField = value;
		}
	}

	public static CT_AdjPoint2D Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_AdjPoint2D
		{
			x = XmlHelper.ReadString(node.Attributes["x"]),
			y = XmlHelper.ReadString(node.Attributes["y"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "x", x);
		XmlHelper.WriteAttribute(sw, "y", y);
		sw.Write(">");
		sw.Write($"</a:{nodeName}>");
	}
}
