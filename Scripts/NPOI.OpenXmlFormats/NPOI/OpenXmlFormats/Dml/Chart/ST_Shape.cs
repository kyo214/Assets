using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
public enum ST_Shape
{
	cone = 0,
	coneToMax = 1,
	box = 2,
	cylinder = 3,
	pyramid = 4,
	pyramidToMax = 5
}
