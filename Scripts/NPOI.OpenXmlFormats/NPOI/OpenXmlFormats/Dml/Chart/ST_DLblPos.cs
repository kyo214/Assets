using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
public enum ST_DLblPos
{
	bestFit = 0,
	b = 1,
	ctr = 2,
	inBase = 3,
	inEnd = 4,
	l = 5,
	outEnd = 6,
	r = 7,
	t = 8
}
