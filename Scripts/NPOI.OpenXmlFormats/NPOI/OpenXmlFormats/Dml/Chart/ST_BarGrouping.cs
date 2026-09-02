using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
public enum ST_BarGrouping
{
	percentStacked = 0,
	clustered = 1,
	standard = 2,
	stacked = 3
}
