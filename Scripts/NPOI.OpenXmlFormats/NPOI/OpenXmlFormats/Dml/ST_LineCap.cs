using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_LineCap
{
	NONE = 0,
	rnd = 1,
	sq = 2,
	flat = 3
}
