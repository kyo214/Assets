using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
public enum ST_ScatterStyle
{
	none = 0,
	line = 1,
	lineMarker = 2,
	marker = 3,
	smooth = 4,
	smoothMarker = 5
}
