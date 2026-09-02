using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = false)]
public enum ST_ShowDataAs
{
	normal = 0,
	difference = 1,
	percent = 2,
	percentDiff = 3,
	runTotal = 4,
	percentOfRow = 5,
	percentOfCol = 6,
	percentOfTotal = 7,
	index = 8
}
