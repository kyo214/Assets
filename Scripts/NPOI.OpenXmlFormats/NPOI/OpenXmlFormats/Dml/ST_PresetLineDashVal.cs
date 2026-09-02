using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_PresetLineDashVal
{
	solid = 0,
	dot = 1,
	dash = 2,
	lgDash = 3,
	dashDot = 4,
	lgDashDot = 5,
	lgDashDotDot = 6,
	sysDash = 7,
	sysDot = 8,
	sysDashDot = 9,
	sysDashDotDot = 10
}
