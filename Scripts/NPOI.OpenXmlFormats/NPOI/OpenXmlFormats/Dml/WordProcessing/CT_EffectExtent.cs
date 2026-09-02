using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml.WordProcessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing", IsNullable = true)]
public class CT_EffectExtent
{
	private long lField;

	private long tField;

	private long rField;

	private long bField;

	[XmlAttribute]
	public long l
	{
		get
		{
			return lField;
		}
		set
		{
			lField = value;
		}
	}

	[XmlAttribute]
	public long t
	{
		get
		{
			return tField;
		}
		set
		{
			tField = value;
		}
	}

	[XmlAttribute]
	public long r
	{
		get
		{
			return rField;
		}
		set
		{
			rField = value;
		}
	}

	[XmlAttribute]
	public long b
	{
		get
		{
			return bField;
		}
		set
		{
			bField = value;
		}
	}

	public static CT_EffectExtent Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_EffectExtent
		{
			l = XmlHelper.ReadLong(node.Attributes["l"]),
			t = XmlHelper.ReadLong(node.Attributes["t"]),
			r = XmlHelper.ReadLong(node.Attributes["r"]),
			b = XmlHelper.ReadLong(node.Attributes["b"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<wp:{nodeName}");
		XmlHelper.WriteAttribute(sw, "l", l, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "t", t, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "r", r, writeIfBlank: true);
		XmlHelper.WriteAttribute(sw, "b", b, writeIfBlank: true);
		sw.Write("/>");
	}
}
