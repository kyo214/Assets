using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Diagram;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram")]
public enum ST_HierBranchStyle
{
	l = 0,
	r = 1,
	hang = 2,
	std = 3,
	init = 4
}
