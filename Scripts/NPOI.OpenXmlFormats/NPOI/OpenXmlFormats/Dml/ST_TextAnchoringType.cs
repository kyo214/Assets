using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = false)]
public enum ST_TextAnchoringType
{
	t = 0,
	ctr = 1,
	b = 2,
	just = 3,
	dist = 4
}
