using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = false)]
public enum ST_ExternalConnectionType
{
	general = 0,
	text = 1,
	MDY = 2,
	DMY = 3,
	YMD = 4,
	MYD = 5,
	DYM = 6,
	YDM = 7,
	skip = 8,
	EMD = 9
}
