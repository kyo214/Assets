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
public class CT_SmartTagType
{
	private string namespaceuriField;

	private string nameField;

	private string urlField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string namespaceuri
	{
		get
		{
			return namespaceuriField;
		}
		set
		{
			namespaceuriField = value;
		}
	}

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
	public string url
	{
		get
		{
			return urlField;
		}
		set
		{
			urlField = value;
		}
	}

	public static CT_SmartTagType Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_SmartTagType
		{
			namespaceuri = XmlHelper.ReadString(node.Attributes["w:namespaceuri"]),
			name = XmlHelper.ReadString(node.Attributes["w:name"]),
			url = XmlHelper.ReadString(node.Attributes["w:url"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<w:{nodeName}");
		XmlHelper.WriteAttribute(sw, "w:namespaceuri", namespaceuri);
		XmlHelper.WriteAttribute(sw, "w:name", name);
		XmlHelper.WriteAttribute(sw, "w:url", url);
		sw.Write(">");
		sw.Write($"</w:{nodeName}>");
	}
}
