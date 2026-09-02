using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Diagram;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram")]
public enum ST_AxisType
{
	self = 0,
	ch = 1,
	des = 2,
	desOrSelf = 3,
	par = 4,
	ancst = 5,
	ancstOrSelf = 6,
	followSib = 7,
	precedSib = 8,
	follow = 9,
	preced = 10,
	root = 11,
	none = 12
}
