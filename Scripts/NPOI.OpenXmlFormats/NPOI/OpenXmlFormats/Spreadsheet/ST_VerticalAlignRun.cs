using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public enum ST_VerticalAlignRun
{
	baseline = 0,
	superscript = 1,
	subscript = 2
}
