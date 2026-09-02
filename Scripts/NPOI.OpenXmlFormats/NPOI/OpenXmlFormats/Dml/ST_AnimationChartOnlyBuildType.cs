using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_AnimationChartOnlyBuildType
{
	series = 0,
	category = 1,
	seriesEl = 2,
	categoryEl = 3
}
