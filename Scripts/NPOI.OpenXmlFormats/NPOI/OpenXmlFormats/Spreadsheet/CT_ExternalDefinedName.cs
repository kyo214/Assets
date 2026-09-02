using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_ExternalDefinedName
{
	private string nameField;

	private string refersToField;

	private uint sheetIdField;

	private bool sheetIdFieldSpecified;

	[XmlAttribute]
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

	[XmlAttribute]
	public string refersTo
	{
		get
		{
			return refersToField;
		}
		set
		{
			refersToField = value;
		}
	}

	[XmlAttribute]
	public uint sheetId
	{
		get
		{
			return sheetIdField;
		}
		set
		{
			sheetIdFieldSpecified = true;
			sheetIdField = value;
		}
	}

	[XmlIgnore]
	public bool sheetIdSpecified
	{
		get
		{
			return sheetIdFieldSpecified;
		}
		set
		{
			sheetIdFieldSpecified = value;
		}
	}

	public bool IsSetSheetId()
	{
		if (sheetIdFieldSpecified)
		{
			return sheetIdField != 0;
		}
		return false;
	}

	internal static CT_ExternalDefinedName Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		CT_ExternalDefinedName cT_ExternalDefinedName = new CT_ExternalDefinedName();
		cT_ExternalDefinedName.nameField = XmlHelper.ReadString(node.Attributes["name"]);
		cT_ExternalDefinedName.refersToField = XmlHelper.ReadString(node.Attributes["refersTo"]);
		cT_ExternalDefinedName.sheetIdFieldSpecified = node.Attributes["sheetId"] != null;
		if (cT_ExternalDefinedName.sheetIdFieldSpecified)
		{
			cT_ExternalDefinedName.sheetIdField = XmlHelper.ReadUInt(node.Attributes["sheetId"]);
		}
		return cT_ExternalDefinedName;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "name", nameField);
		XmlHelper.WriteAttribute(sw, "refersTo", refersToField);
		if (sheetIdFieldSpecified)
		{
			XmlHelper.WriteAttribute(sw, "sheetId", sheetIdField);
		}
		sw.Write("/>");
	}
}
