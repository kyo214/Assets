using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
public enum ST_TrendlineType
{
	exp = 0,
	linear = 1,
	log = 2,
	movingAvg = 3,
	poly = 4,
	power = 5
}
