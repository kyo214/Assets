using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart", IsNullable = true)]
public class CT_MarkerSize
{
	private byte valField;

	[XmlAttribute]
	[DefaultValue(typeof(byte), "5")]
	public byte val
	{
		get
		{
			return valField;
		}
		set
		{
			valField = value;
		}
	}

	public CT_MarkerSize()
	{
		valField = 5;
	}

	public static CT_MarkerSize Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_MarkerSize
		{
			val = XmlHelper.ReadByte(node.Attributes["val"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<c:{nodeName}");
		XmlHelper.WriteAttribute(sw, "val", val);
		sw.Write(">");
		sw.Write($"</c:{nodeName}>");
	}
}
