using System;
using System.IO;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.WordProcessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing", IsNullable = true)]
public class CT_WrapNone
{
	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<wp:{nodeName}");
		sw.Write("/>");
	}
}
