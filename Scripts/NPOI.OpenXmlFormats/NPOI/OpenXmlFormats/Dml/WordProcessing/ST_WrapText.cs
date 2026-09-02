using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.WordProcessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing")]
public enum ST_WrapText
{
	bothSides = 0,
	left = 1,
	right = 2,
	largest = 3
}
