using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.WordProcessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing")]
public enum ST_RelFromH
{
	margin = 0,
	page = 1,
	column = 2,
	character = 3,
	leftMargin = 4,
	rightMargin = 5,
	insideMargin = 6,
	outsideMargin = 7
}
