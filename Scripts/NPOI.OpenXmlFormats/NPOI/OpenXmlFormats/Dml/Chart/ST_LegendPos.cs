using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
public enum ST_LegendPos
{
	b = 0,
	tr = 1,
	l = 2,
	r = 3,
	t = 4
}
