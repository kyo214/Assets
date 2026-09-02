using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Vml;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "urn:schemas-microsoft-com:vml")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:vml", IsNullable = true)]
public class CT_Textbox
{
	private string itemField;

	private string idField;

	private string styleField;

	private string insetField;

	public string ItemXml
	{
		get
		{
			return itemField;
		}
		set
		{
			itemField = value;
		}
	}

	[XmlAttribute]
	public string id
	{
		get
		{
			return idField;
		}
		set
		{
			idField = value;
		}
	}

	[XmlAttribute]
	public string style
	{
		get
		{
			return styleField;
		}
		set
		{
			styleField = value;
		}
	}

	[XmlAttribute]
	public string inset
	{
		get
		{
			return insetField;
		}
		set
		{
			insetField = value;
		}
	}

	public static CT_Textbox Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_Textbox
		{
			id = XmlHelper.ReadString(node.Attributes["id"]),
			style = XmlHelper.ReadString(node.Attributes["style"]),
			inset = XmlHelper.ReadString(node.Attributes["inset"]),
			ItemXml = node.InnerXml
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<v:{nodeName}");
		XmlHelper.WriteAttribute(sw, "id", id);
		XmlHelper.WriteAttribute(sw, "style", style);
		XmlHelper.WriteAttribute(sw, "inset", inset);
		sw.Write(">");
		if (ItemXml != null)
		{
			sw.Write(ItemXml);
		}
		sw.Write($"</v:{nodeName}>");
	}
}
