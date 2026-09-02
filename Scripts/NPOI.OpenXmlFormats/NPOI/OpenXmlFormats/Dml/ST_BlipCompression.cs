using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_BlipCompression
{
	email = 0,
	screen = 1,
	print = 2,
	hqprint = 3,
	none = 4
}
