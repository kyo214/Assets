using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_SoftEdgesEffect
{
	private long radField;

	[XmlAttribute]
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

	public static CT_SoftEdgesEffect Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_SoftEdgesEffect
		{
			rad = XmlHelper.ReadLong(node.Attributes["rad"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "rad", rad);
		sw.Write(">");
		sw.Write($"</a:{nodeName}>");
	}
}
