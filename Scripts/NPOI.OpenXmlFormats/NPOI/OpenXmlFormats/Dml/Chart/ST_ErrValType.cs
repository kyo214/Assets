using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
public enum ST_ErrValType
{
	cust = 0,
	fixedVal = 1,
	percentage = 2,
	stdDev = 3,
	stdErr = 4
}
