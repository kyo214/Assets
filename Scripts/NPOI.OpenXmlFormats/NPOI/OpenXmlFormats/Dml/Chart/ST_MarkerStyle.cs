using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
public enum ST_MarkerStyle
{
	circle = 0,
	dash = 1,
	diamond = 2,
	dot = 3,
	none = 4,
	picture = 5,
	plus = 6,
	square = 7,
	star = 8,
	triangle = 9,
	x = 10
}
