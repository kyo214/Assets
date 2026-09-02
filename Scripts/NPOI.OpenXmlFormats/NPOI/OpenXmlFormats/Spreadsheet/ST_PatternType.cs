using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public enum ST_PatternType
{
	none = 0,
	solid = 1,
	mediumGray = 2,
	darkGray = 3,
	lightGray = 4,
	darkHorizontal = 5,
	darkVertical = 6,
	darkDown = 7,
	darkUp = 8,
	darkGrid = 9,
	darkTrellis = 10,
	lightHorizontal = 11,
	lightVertical = 12,
	lightDown = 13,
	lightUp = 14,
	lightGrid = 15,
	lightTrellis = 16,
	gray125 = 17,
	gray0625 = 18
}
