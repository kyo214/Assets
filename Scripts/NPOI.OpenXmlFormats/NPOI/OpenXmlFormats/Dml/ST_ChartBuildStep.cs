using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_ChartBuildStep
{
	category = 0,
	ptInCategory = 1,
	series = 2,
	ptInSeries = 3,
	allPts = 4,
	gridLegend = 5
}
