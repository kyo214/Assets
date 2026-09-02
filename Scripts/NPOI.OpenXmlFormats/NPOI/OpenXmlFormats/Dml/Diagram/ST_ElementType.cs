using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Diagram;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram")]
public enum ST_ElementType
{
	all = 0,
	doc = 1,
	node = 2,
	norm = 3,
	nonNorm = 4,
	asst = 5,
	nonAsst = 6,
	parTrans = 7,
	pres = 8,
	sibTrans = 9
}
