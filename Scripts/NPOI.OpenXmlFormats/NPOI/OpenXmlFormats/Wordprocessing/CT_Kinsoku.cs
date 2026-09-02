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
public class CT_Kinsoku
{
	private string langField;

	private string valField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string lang
	{
		get
		{
			return langField;
		}
		set
		{
			langField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string val
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

	public static CT_Kinsoku Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Kinsoku
		{
			lang = XmlHelper.ReadString(node.Attributes["w:lang"]),
			val = XmlHelper.ReadString(node.Attributes["w:val"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:lang", lang);
		XmlHelper.WriteAttribute(sw, "w:val", val);
		sw.Write(">");
		sw.Write($"</w:{nodeName}>");
	}
}
