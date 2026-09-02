using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_DdeItems
{
	private CT_DdeItem[] ddeItemField;

	[XmlElement("ddeItem")]
	public CT_DdeItem[] ddeItem
	{
		get
		{
			return ddeItemField;
		}
		set
		{
			ddeItemField = value;
		}
	}
}
