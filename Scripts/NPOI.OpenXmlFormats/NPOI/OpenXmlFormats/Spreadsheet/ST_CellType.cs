using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public enum ST_CellType
{
	b = 0,
	n = 1,
	e = 2,
	s = 3,
	str = 4,
	inlineStr = 5
}
