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
public class CT_SdtListItem
{
	private string displayTextField;

	private string valueField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string displayText
	{
		get
		{
			return displayTextField;
		}
		set
		{
			displayTextField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string value
	{
		get
		{
			return valueField;
		}
		set
		{
			valueField = value;
		}
	}

	public static CT_SdtListItem Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_SdtListItem
		{
			displayText = XmlHelper.ReadString(node.Attributes["w:displayText"]),
			value = XmlHelper.ReadString(node.Attributes["w:value"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:displayText", displayText);
		XmlHelper.WriteAttribute(sw, "w:value", value);
		sw.Write(">");
		sw.Write($"</w:{nodeName}>");
	}
}
