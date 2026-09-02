using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.WordProcessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing")]
public enum ST_RelFromV
{
	margin = 0,
	page = 1,
	paragraph = 2,
	line = 3,
	topMargin = 4,
	bottomMargin = 5,
	insideMargin = 6,
	outsideMargin = 7
}
