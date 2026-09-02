using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_Point2D
{
	private long xField;

	private long yField;

	private string name;

	[XmlAttribute]
	public long x
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
	public long y
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

	public static CT_Point2D Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Point2D
		{
			name = node.Name,
			x = XmlHelper.ReadLong(node.Attributes["x"]),
			y = XmlHelper.ReadLong(node.Attributes["y"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		if (name == null)
		{
			sw.Write($"<a:{nodeName}");
		}
		else
		{
			sw.Write($"<{name}");
		}
		XmlHelper.WriteAttribute(sw, "x", x, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "y", y, writeIfBlank: true);
		sw.Write("/>");
	}
}
