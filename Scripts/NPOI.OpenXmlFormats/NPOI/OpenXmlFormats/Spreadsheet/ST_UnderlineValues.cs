using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public enum ST_UnderlineValues
{
	none = 0,
	single = 1,
	[XmlEnum("double")]
	@double = 2,
	singleAccounting = 3,
	doubleAccounting = 4
}
