using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_LightRigDirection
{
	tl = 0,
	t = 1,
	tr = 2,
	l = 3,
	r = 4,
	bl = 5,
	b = 6,
	br = 7
}
