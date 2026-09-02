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
public class CT_ExternalSheetNames
{
	private List<CT_ExternalSheetName> sheetNameField;

	[XmlElement("sheetName")]
	public CT_ExternalSheetName[] sheetName
	{
		get
		{
			return sheetNameField.ToArray();
		}
		set
		{
			sheetNameField.Clear();
			sheetNameField.AddRange(value);
		}
	}

	public CT_ExternalSheetNames()
	{
		sheetNameField = new List<CT_ExternalSheetName>();
	}

	internal static CT_ExternalSheetNames Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		CT_ExternalSheetNames cT_ExternalSheetNames = new CT_ExternalSheetNames();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			cT_ExternalSheetNames.sheetNameField.Add(CT_ExternalSheetName.Parse(childNode, namespaceManager));
		}
		return cT_ExternalSheetNames;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}>");
		foreach (CT_ExternalSheetName item in sheetNameField)
		{
			item.Write(sw, "sheetName");
		}
		sw.Write($"</{nodeName}>");
	}
}
