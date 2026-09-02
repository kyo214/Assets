using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_SchemeColorVal
{
	bg1 = 0,
	tx1 = 1,
	bg2 = 2,
	tx2 = 3,
	accent1 = 4,
	accent2 = 5,
	accent3 = 6,
	accent4 = 7,
	accent5 = 8,
	accent6 = 9,
	hlink = 10,
	folHlink = 11,
	phClr = 12,
	dk1 = 13,
	lt1 = 14,
	dk2 = 15,
	lt2 = 16
}
