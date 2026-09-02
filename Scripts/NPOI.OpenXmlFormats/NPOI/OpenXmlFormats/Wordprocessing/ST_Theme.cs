using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_Theme
{
	majorEastAsia = 0,
	majorBidi = 1,
	majorAscii = 2,
	majorHAnsi = 3,
	minorEastAsia = 4,
	minorBidi = 5,
	minorAscii = 6,
	minorHAnsi = 7
}
