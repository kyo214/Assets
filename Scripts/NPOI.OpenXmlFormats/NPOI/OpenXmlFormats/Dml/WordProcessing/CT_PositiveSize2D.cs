using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml.WordProcessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing")]
[XmlRoot("inline", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing", IsNullable = false)]
public class CT_PositiveSize2D
{
	private long cxField;

	private long cyField;

	[XmlAttribute]
	public long cx
	{
		get
		{
			return cxField;
		}
		set
		{
			cxField = value;
		}
	}

	[XmlAttribute]
	public long cy
	{
		get
		{
			return cyField;
		}
		set
		{
			cyField = value;
		}
	}

	public static CT_PositiveSize2D Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_PositiveSize2D
		{
			cx = XmlHelper.ReadLong(node.Attributes["cx"]),
			cy = XmlHelper.ReadLong(node.Attributes["cy"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<wp:{nodeName}");
		XmlHelper.WriteAttribute(sw, "cx", cx, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "cy", cy, writeIfBlank: true);
		sw.Write("/>");
	}
}
