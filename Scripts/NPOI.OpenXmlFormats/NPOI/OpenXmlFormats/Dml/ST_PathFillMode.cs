using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_PathFillMode
{
	none = 0,
	norm = 1,
	lighten = 2,
	lightenLess = 3,
	darken = 4,
	darkenLess = 5
}
