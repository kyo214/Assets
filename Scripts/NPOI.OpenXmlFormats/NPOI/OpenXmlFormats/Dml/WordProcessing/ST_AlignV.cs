using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.WordProcessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing")]
public enum ST_AlignV
{
	top = 0,
	bottom = 1,
	center = 2,
	inside = 3,
	outside = 4
}
