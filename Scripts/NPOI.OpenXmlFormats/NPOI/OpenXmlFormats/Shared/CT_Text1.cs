using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Shared;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math", IsNullable = true)]
public class CT_Text1
{
	private string spaceField;

	private string valueField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/XML/1998/namespace")]
	public string space
	{
		get
		{
			return spaceField;
		}
		set
		{
			spaceField = value;
		}
	}

	[XmlText]
	public string Value
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

	public static CT_Text1 Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Text1
		{
			space = XmlHelper.ReadString(node.Attributes["m:space"]),
			Value = node.InnerText
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<m:{nodeName}");
		XmlHelper.WriteAttribute(sw, "m:space", space);
		sw.Write(">");
		if (valueField != null)
		{
			sw.Write(XmlHelper.EncodeXml(valueField));
		}
		sw.Write($"</m:{nodeName}>");
	}
}
