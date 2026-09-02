using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = false)]
public enum ST_GroupBy
{
	range = 0,
	seconds = 1,
	minutes = 2,
	hours = 3,
	days = 4,
	months = 5,
	quarters = 6,
	years = 7
}
