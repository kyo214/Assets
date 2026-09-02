using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_ColorSchemeIndex
{
	dk1 = 0,
	lt1 = 1,
	dk2 = 2,
	lt2 = 3,
	accent1 = 4,
	accent2 = 5,
	accent3 = 6,
	accent4 = 7,
	accent5 = 8,
	accent6 = 9,
	hlink = 10,
	folHlink = 11
}
