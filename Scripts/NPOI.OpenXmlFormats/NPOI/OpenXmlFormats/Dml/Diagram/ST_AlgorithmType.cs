using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Diagram;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram")]
public enum ST_AlgorithmType
{
	composite = 0,
	conn = 1,
	cycle = 2,
	hierChild = 3,
	hierRoot = 4,
	pyra = 5,
	lin = 6,
	sp = 7,
	tx = 8,
	snake = 9
}
