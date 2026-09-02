using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = false)]
public enum ST_FormatAction
{
	blank = 0,
	formatting = 1,
	drill = 2,
	formula = 3
}
