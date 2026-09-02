using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_Sym
{
	private string fontField;

	private byte[] charField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string font
	{
		get
		{
			return fontField;
		}
		set
		{
			fontField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified, DataType = "hexBinary")]
	public byte[] @char
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

	public static CT_Sym Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Sym
		{
			font = XmlHelper.ReadString(node.Attributes["w:font"]),
			@char = XmlHelper.ReadBytes(node.Attributes["w:char"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:font", font);
		XmlHelper.WriteAttribute(sw, "w:char", @char);
		sw.Write(">");
		sw.Write($"</w:{nodeName}>");
	}
}
