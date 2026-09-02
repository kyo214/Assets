using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IncludeInSchema = false)]
public enum ItemsChoiceType
{
	arcTo = 0,
	close = 1,
	cubicBezTo = 2,
	lnTo = 3,
	moveTo = 4,
	quadBezTo = 5
}
