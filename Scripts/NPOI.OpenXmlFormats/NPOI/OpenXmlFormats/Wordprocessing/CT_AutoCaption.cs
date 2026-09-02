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
public class CT_AutoCaption
{
	private string nameField;

	private string captionField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string name
	{
		get
		{
			return nameField;
		}
		set
		{
			nameField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string caption
	{
		get
		{
			return captionField;
		}
		set
		{
			captionField = value;
		}
	}

	public static CT_AutoCaption Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_AutoCaption
		{
			name = XmlHelper.ReadString(node.Attributes["w:name"]),
			caption = XmlHelper.ReadString(node.Attributes["w:caption"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:name", name);
		XmlHelper.WriteAttribute(sw, "w:caption", caption);
		sw.Write(">");
		sw.Write($"</w:{nodeName}>");
	}
}
