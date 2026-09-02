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
public class CT_DdeLink
{
	private List<CT_DdeItem> ddeItemsField;

	private string ddeServiceField;

	private string ddeTopicField;

	[XmlArray("ddeItems")]
	[XmlArrayItem("ddeItem")]
	public List<CT_DdeItem> ddeItems
	{
		get
		{
			return ddeItemsField;
		}
		set
		{
			ddeItemsField = value;
		}
	}

	[XmlAttribute]
	public string ddeService
	{
		get
		{
			return ddeServiceField;
		}
		set
		{
			ddeServiceField = value;
		}
	}

	[XmlAttribute]
	public string ddeTopic
	{
		get
		{
			return ddeTopicField;
		}
		set
		{
			ddeTopicField = value;
		}
	}

	internal static CT_DdeLink Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		throw new NotImplementedException();
	}

	internal void Write(StreamWriter sw, string p)
	{
		throw new NotImplementedException();
	}
}
