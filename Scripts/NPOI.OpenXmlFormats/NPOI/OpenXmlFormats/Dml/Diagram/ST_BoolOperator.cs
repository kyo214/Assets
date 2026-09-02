using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Diagram;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram")]
public enum ST_BoolOperator
{
	none = 0,
	equ = 1,
	gte = 2,
	lte = 3
}
