using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_ExternalSheetDataSet
{
	private List<CT_ExternalSheetData> sheetDataField;

	[XmlElement("sheetData")]
	public CT_ExternalSheetData[] sheetData
	{
		get
		{
			return sheetDataField.ToArray();
		}
		set
		{
			sheetDataField.Clear();
			sheetDataField.AddRange(value);
		}
	}

	public CT_ExternalSheetDataSet()
	{
		sheetDataField = new List<CT_ExternalSheetData>();
	}

	internal static CT_ExternalSheetDataSet Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		CT_ExternalSheetDataSet cT_ExternalSheetDataSet = new CT_ExternalSheetDataSet();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			cT_ExternalSheetDataSet.sheetDataField.Add(CT_ExternalSheetData.Parse(childNode, namespaceManager));
		}
		return cT_ExternalSheetDataSet;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		sw.Write(">");
		foreach (CT_ExternalSheetData item in sheetDataField)
		{
			item.Write(sw, "sheetData");
		}
		sw.Write($"</{nodeName}>");
	}
}
