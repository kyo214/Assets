using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = false)]
public enum ST_SortType
{
	none = 0,
	ascending = 1,
	descending = 2,
	ascendingAlpha = 3,
	descendingAlpha = 4,
	ascendingNatural = 5,
	descendingNatural = 6
}
