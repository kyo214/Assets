using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_ThemeColor
{
	none = 0,
	dark1 = 1,
	light1 = 2,
	dark2 = 3,
	light2 = 4,
	accent1 = 5,
	accent2 = 6,
	accent3 = 7,
	accent4 = 8,
	accent5 = 9,
	accent6 = 10,
	hyperlink = 11,
	followedHyperlink = 12,
	background1 = 13,
	text1 = 14,
	background2 = 15,
	text2 = 16
}
