using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_LineEndType
{
	none = 0,
	triangle = 1,
	stealth = 2,
	diamond = 3,
	oval = 4,
	arrow = 5
}
