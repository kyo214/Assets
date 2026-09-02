using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_RectAlignment
{
	tl = 0,
	t = 1,
	tr = 2,
	l = 3,
	ctr = 4,
	r = 5,
	bl = 6,
	b = 7,
	br = 8
}
