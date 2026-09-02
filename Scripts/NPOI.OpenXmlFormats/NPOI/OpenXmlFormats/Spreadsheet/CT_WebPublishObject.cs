using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_WebPublishObject
{
	private uint idField;

	private string divIdField;

	private string sourceObjectField;

	private string destinationFileField;

	private string titleField;

	private bool autoRepublishField;

	[XmlAnyAttribute]
	public uint id
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

	[XmlAnyAttribute]
	public string divId
	{
		get
		{
			return divIdField;
		}
		set
		{
			divIdField = value;
		}
	}

	[XmlAnyAttribute]
	public string sourceObject
	{
		get
		{
			return sourceObjectField;
		}
		set
		{
			sourceObjectField = value;
		}
	}

	[XmlAnyAttribute]
	public string destinationFile
	{
		get
		{
			return destinationFileField;
		}
		set
		{
			destinationFileField = value;
		}
	}

	[XmlAnyAttribute]
	public string title
	{
		get
		{
			return titleField;
		}
		set
		{
			titleField = value;
		}
	}

	[XmlAnyAttribute]
	[DefaultValue(false)]
	public bool autoRepublish
	{
		get
		{
			return autoRepublishField;
		}
		set
		{
			autoRepublishField = value;
		}
	}

	public CT_WebPublishObject()
	{
		autoRepublishField = false;
	}

	public static CT_WebPublishObject Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_WebPublishObject
		{
			id = XmlHelper.ReadUInt(node.Attributes["id"]),
			divId = XmlHelper.ReadString(node.Attributes["divId"]),
			sourceObject = XmlHelper.ReadString(node.Attributes["sourceObject"]),
			destinationFile = XmlHelper.ReadString(node.Attributes["destinationFile"]),
			title = XmlHelper.ReadString(node.Attributes["title"]),
			autoRepublish = XmlHelper.ReadBool(node.Attributes["autoRepublish"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "id", id);
		XmlHelper.WriteAttribute(sw, "divId", divId);
		XmlHelper.WriteAttribute(sw, "sourceObject", sourceObject);
		XmlHelper.WriteAttribute(sw, "destinationFile", destinationFile);
		XmlHelper.WriteAttribute(sw, "title", title);
		XmlHelper.WriteAttribute(sw, "autoRepublish", autoRepublish);
		sw.Write(">");
		sw.Write($"</{nodeName}>");
	}
}
