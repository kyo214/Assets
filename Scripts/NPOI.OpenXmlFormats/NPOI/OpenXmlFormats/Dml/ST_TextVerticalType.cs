using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = false)]
public enum ST_TextVerticalType
{
	horz = 0,
	vert = 1,
	vert270 = 2,
	wordArtVert = 3,
	eaVert = 4,
	mongolianVert = 5,
	wordArtVertRtl = 6
}
