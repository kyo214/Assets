using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_ExternalBook
{
	private CT_ExternalSheetNames sheetNamesField;

	private CT_ExternalDefinedNames definedNamesField;

	private CT_ExternalSheetDataSet sheetDataSetField;

	private string idField;

	[XmlArrayItem("sheetName", IsNullable = false)]
	public CT_ExternalSheetNames sheetNames
	{
		get
		{
			return sheetNamesField;
		}
		set
		{
			sheetNamesField = value;
		}
	}

	[XmlArrayItem("definedName", IsNullable = false)]
	public CT_ExternalDefinedNames definedNames
	{
		get
		{
			return definedNamesField;
		}
		set
		{
			definedNamesField = value;
		}
	}

	[XmlArrayItem("sheetData", IsNullable = false)]
	public CT_ExternalSheetDataSet sheetDataSet
	{
		get
		{
			return sheetDataSetField;
		}
		set
		{
			sheetDataSetField = value;
		}
	}

	[XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships")]
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

	internal static CT_ExternalBook Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_ExternalBook cT_ExternalBook = new CT_ExternalBook();
		cT_ExternalBook.idField = XmlHelper.ReadString(node.Attributes["id", namespaceManager.LookupNamespace("r")]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "sheetNames")
			{
				cT_ExternalBook.sheetNamesField = CT_ExternalSheetNames.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "definedNames")
			{
				cT_ExternalBook.definedNamesField = CT_ExternalDefinedNames.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "sheetDataSet")
			{
				cT_ExternalBook.sheetDataSetField = CT_ExternalSheetDataSet.Parse(childNode, namespaceManager);
			}
		}
		return cT_ExternalBook;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "r:id", idField);
		sw.Write(">");
		if (sheetNamesField != null)
		{
			sheetNamesField.Write(sw, "sheetNames");
		}
		if (definedNamesField != null)
		{
			definedNamesField.Write(sw, "definedNames");
		}
		if (sheetDataSetField != null)
		{
			sheetDataSetField.Write(sw, "sheetDataSet");
		}
		sw.Write($"</{nodeName}>");
	}
}
