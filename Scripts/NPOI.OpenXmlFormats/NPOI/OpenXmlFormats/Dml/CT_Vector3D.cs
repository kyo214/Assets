using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_Vector3D
{
	private long dxField;

	private long dyField;

	private long dzField;

	[XmlAttribute]
	public long dx
	{
		get
		{
			return dxField;
		}
		set
		{
			dxField = value;
		}
	}

	[XmlAttribute]
	public long dy
	{
		get
		{
			return dyField;
		}
		set
		{
			dyField = value;
		}
	}

	[XmlAttribute]
	public long dz
	{
		get
		{
			return dzField;
		}
		set
		{
			dzField = value;
		}
	}

	public static CT_Vector3D Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Vector3D
		{
			dx = XmlHelper.ReadLong(node.Attributes["dx"]),
			dy = XmlHelper.ReadLong(node.Attributes["dy"]),
			dz = XmlHelper.ReadLong(node.Attributes["dz"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "dx", dx);
		XmlHelper.WriteAttribute(sw, "dy", dy);
		XmlHelper.WriteAttribute(sw, "dz", dz);
		sw.Write(">");
		sw.Write($"</a:{nodeName}>");
	}
}
