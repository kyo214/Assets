using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_HighlightColor
{
	black = 0,
	blue = 1,
	cyan = 2,
	green = 3,
	magenta = 4,
	red = 5,
	yellow = 6,
	white = 7,
	darkBlue = 8,
	darkCyan = 9,
	darkGreen = 10,
	darkMagenta = 11,
	darkRed = 12,
	darkYellow = 13,
	darkGray = 14,
	lightGray = 15,
	none = 16
}
