using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Diagram;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram")]
public enum ST_FunctionType
{
	cnt = 0,
	pos = 1,
	revPos = 2,
	posEven = 3,
	posOdd = 4,
	var = 5,
	depth = 6,
	maxDepth = 7
}
