using System;
using System.Collections.Generic;
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
public class CT_ExternalSheetData
{
	private List<CT_ExternalRow> rowField;

	private uint sheetIdField;

	private bool refreshErrorField;

	[XmlElement("row")]
	public CT_ExternalRow[] row
	{
		get
		{
			return rowField.ToArray();
		}
		set
		{
			rowField.Clear();
			rowField.AddRange(value);
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
			sheetIdField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool refreshError
	{
		get
		{
			return refreshErrorField;
		}
		set
		{
			refreshErrorField = value;
		}
	}

	public CT_ExternalSheetData()
	{
		rowField = new List<CT_ExternalRow>();
	}

	internal static CT_ExternalSheetData Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		CT_ExternalSheetData cT_ExternalSheetData = new CT_ExternalSheetData();
		cT_ExternalSheetData.refreshErrorField = XmlHelper.ReadBool(node.Attributes["refreshError"]);
		cT_ExternalSheetData.sheetIdField = XmlHelper.ReadUInt(node.Attributes["sheetId"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "row")
			{
				cT_ExternalSheetData.rowField.Add(CT_ExternalRow.Parse(childNode, namespaceManager));
			}
		}
		return cT_ExternalSheetData;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "sheetId", sheetIdField, writeIfBlank: true);
		if (refreshError)
		{
			XmlHelper.WriteAttribute(sw, "refreshError", refreshErrorField);
		}
		sw.Write(">");
		foreach (CT_ExternalRow item in rowField)
		{
			item.Write(sw, "row");
		}
		sw.Write($"</{nodeName}>");
	}
}
