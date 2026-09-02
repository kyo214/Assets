using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
public enum ST_BuiltInUnit
{
	hundreds = 0,
	thousands = 1,
	tenThousands = 2,
	hundredThousands = 3,
	millions = 4,
	tenMillions = 5,
	hundredMillions = 6,
	billions = 7,
	trillions = 8
}
