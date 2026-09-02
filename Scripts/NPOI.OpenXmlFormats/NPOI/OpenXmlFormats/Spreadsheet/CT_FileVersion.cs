using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_FileVersion
{
	private string appNameField;

	private string lastEditedField;

	private string lowestEditedField;

	private string rupBuildField;

	private string codeNameField;

	[XmlAttribute]
	public string appName
	{
		get
		{
			return appNameField;
		}
		set
		{
			appNameField = value;
		}
	}

	[XmlAttribute]
	public string lastEdited
	{
		get
		{
			return lastEditedField;
		}
		set
		{
			lastEditedField = value;
		}
	}

	[XmlAttribute]
	public string lowestEdited
	{
		get
		{
			return lowestEditedField;
		}
		set
		{
			lowestEditedField = value;
		}
	}

	[XmlAttribute]
	public string rupBuild
	{
		get
		{
			return rupBuildField;
		}
		set
		{
			rupBuildField = value;
		}
	}

	[XmlAttribute]
	public string codeName
	{
		get
		{
			return codeNameField;
		}
		set
		{
			codeNameField = value;
		}
	}

	public static CT_FileVersion Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		return new CT_FileVersion
		{
			appName = XmlHelper.ReadString(node.Attributes["appName"]),
			lastEdited = XmlHelper.ReadString(node.Attributes["lastEdited"]),
			lowestEdited = XmlHelper.ReadString(node.Attributes["lowestEdited"]),
			rupBuild = XmlHelper.ReadString(node.Attributes["rupBuild"]),
			codeName = XmlHelper.ReadString(node.Attributes["codeName"])
		};
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "appName", appName);
		XmlHelper.WriteAttribute(sw, "lastEdited", lastEdited);
		XmlHelper.WriteAttribute(sw, "lowestEdited", lowestEdited);
		XmlHelper.WriteAttribute(sw, "rupBuild", rupBuild);
		XmlHelper.WriteAttribute(sw, "codeName", codeName);
		sw.Write("/>");
	}
}
