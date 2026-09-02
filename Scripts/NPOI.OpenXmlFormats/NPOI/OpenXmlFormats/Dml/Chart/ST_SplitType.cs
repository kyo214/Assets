using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
public enum ST_SplitType
{
	auto = 0,
	cust = 1,
	percent = 2,
	pos = 3,
	val = 4
}
