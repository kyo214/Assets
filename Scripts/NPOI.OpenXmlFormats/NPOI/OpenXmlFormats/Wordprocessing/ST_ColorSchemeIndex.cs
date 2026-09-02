using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_ColorSchemeIndex
{
	dark1 = 0,
	light1 = 1,
	dark2 = 2,
	light2 = 3,
	accent1 = 4,
	accent2 = 5,
	accent3 = 6,
	accent4 = 7,
	accent5 = 8,
	accent6 = 9,
	hyperlink = 10,
	followedHyperlink = 11
}
