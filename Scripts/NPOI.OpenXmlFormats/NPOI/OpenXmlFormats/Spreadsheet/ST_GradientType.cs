using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public enum ST_GradientType
{
	NONE = 0,
	linear = 1,
	path = 2
}
