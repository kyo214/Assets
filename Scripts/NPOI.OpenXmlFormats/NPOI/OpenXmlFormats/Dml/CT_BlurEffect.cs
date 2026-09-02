using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_BlurEffect
{
	private long radField;

	private bool growField;

	[XmlAttribute]
	[DefaultValue(typeof(long), "0")]
	public long rad
	{
		get
		{
			return radField;
		}
		set
		{
			radField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool grow
	{
		get
		{
			return growField;
		}
		set
		{
			growField = value;
		}
	}

	public static CT_BlurEffect Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_BlurEffect
		{
			rad = XmlHelper.ReadLong(node.Attributes["rad"]),
			grow = XmlHelper.ReadBool(node.Attributes["grow"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "rad", rad);
		XmlHelper.WriteAttribute(sw, "grow", grow);
		sw.Write(">");
		sw.Write($"</a:{nodeName}>");
	}

	public CT_BlurEffect()
	{
		radField = 0L;
		growField = true;
	}
}
