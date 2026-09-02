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
public class CT_TextCharBullet
{
	private string charField;

	[XmlAttribute]
	public string @char
	{
		get
		{
			return charField;
		}
		set
		{
			charField = value;
		}
	}

	public static CT_TextCharBullet Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_TextCharBullet
		{
			@char = XmlHelper.ReadString(node.Attributes["char"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<a:{nodeName}");
		XmlHelper.WriteAttribute(sw, "char", @char);
		sw.Write(">");
		sw.Write($"</a:{nodeName}>");
	}
}
