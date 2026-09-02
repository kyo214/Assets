using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_TabJc
{
	clear = 0,
	left = 1,
	center = 2,
	right = 3,
	@decimal = 4,
	bar = 5,
	num = 6
}
