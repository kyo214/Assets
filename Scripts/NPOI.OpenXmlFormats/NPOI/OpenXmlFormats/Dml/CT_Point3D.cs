using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_Point3D
{
	private long xField;

	private long yField;

	private long zField;

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

	[XmlAttribute]
	public long z
	{
		get
		{
			return zField;
		}
		set
		{
			zField = value;
		}
	}

	public static CT_Point3D Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Point3D
		{
			x = XmlHelper.ReadLong(node.Attributes["x"]),
			y = XmlHelper.ReadLong(node.Attributes["y"]),
			z = XmlHelper.ReadLong(node.Attributes["z"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "x", x);
		XmlHelper.WriteAttribute(sw, "y", y);
		XmlHelper.WriteAttribute(sw, "z", z);
		sw.Write(">");
		sw.Write($"</a:{nodeName}>");
	}
}
