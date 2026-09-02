using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_TextUnderlineType
{
	none = 0,
	words = 1,
	sng = 2,
	dbl = 3,
	heavy = 4,
	dotted = 5,
	dottedHeavy = 6,
	dash = 7,
	dashHeavy = 8,
	dashLong = 9,
	dashLongHeavy = 10,
	dotDash = 11,
	dotDashHeavy = 12,
	dotDotDash = 13,
	dotDotDashHeavy = 14,
	wavy = 15,
	wavyHeavy = 16,
	wavyDbl = 17
}
