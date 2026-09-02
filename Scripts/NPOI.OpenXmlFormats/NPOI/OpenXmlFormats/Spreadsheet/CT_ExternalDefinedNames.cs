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
public class CT_ExternalDefinedNames
{
	private List<CT_ExternalDefinedName> definedNameField;

	[XmlElement("definedName")]
	public CT_ExternalDefinedName[] definedName
	{
		get
		{
			return definedNameField.ToArray();
		}
		set
		{
			definedNameField.Clear();
			definedNameField.AddRange(value);
		}
	}

	public CT_ExternalDefinedNames()
	{
		definedNameField = new List<CT_ExternalDefinedName>();
	}

	internal static CT_ExternalDefinedNames Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		CT_ExternalDefinedNames cT_ExternalDefinedNames = new CT_ExternalDefinedNames();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			cT_ExternalDefinedNames.definedNameField.Add(CT_ExternalDefinedName.Parse(childNode, namespaceManager));
		}
		return cT_ExternalDefinedNames;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}>");
		foreach (CT_ExternalDefinedName item in definedNameField)
		{
			item.Write(sw, "definedName");
		}
		sw.Write($"</{nodeName}>");
	}
}
