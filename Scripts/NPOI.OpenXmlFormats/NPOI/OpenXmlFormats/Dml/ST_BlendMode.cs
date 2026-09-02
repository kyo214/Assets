using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_BlendMode
{
	over = 0,
	mult = 1,
	screen = 2,
	darken = 3,
	lighten = 4
}
