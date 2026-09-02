using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = false)]
public enum ST_ItemType
{
	data = 0,
	@default = 1,
	sum = 2,
	countA = 3,
	avg = 4,
	max = 5,
	min = 6,
	product = 7,
	count = 8,
	stdDev = 9,
	stdDevP = 10,
	var = 11,
	varP = 12,
	grand = 13,
	blank = 14
}
