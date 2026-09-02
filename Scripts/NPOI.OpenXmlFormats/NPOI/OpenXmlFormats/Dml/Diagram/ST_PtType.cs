using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Diagram;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram")]
public enum ST_PtType
{
	node = 0,
	asst = 1,
	doc = 2,
	pres = 3,
	parTrans = 4,
	sibTrans = 5
}
