using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_CompoundLine
{
	NONE = 0,
	sng = 1,
	dbl = 2,
	thickThin = 3,
	thinThick = 4,
	tri = 5
}
