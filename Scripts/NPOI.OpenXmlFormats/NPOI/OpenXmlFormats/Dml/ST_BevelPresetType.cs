using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_BevelPresetType
{
	relaxedInset = 0,
	circle = 1,
	slope = 2,
	cross = 3,
	angle = 4,
	softRound = 5,
	convex = 6,
	coolSlant = 7,
	divot = 8,
	riblet = 9,
	hardEdge = 10,
	artDeco = 11
}
