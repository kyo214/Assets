using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Diagram;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram")]
public enum ST_FunctionOperator
{
	equ = 0,
	neq = 1,
	gt = 2,
	lt = 3,
	gte = 4,
	lte = 5
}
