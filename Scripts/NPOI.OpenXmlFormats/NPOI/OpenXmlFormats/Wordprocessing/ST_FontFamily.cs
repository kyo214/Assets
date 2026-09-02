using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_FontFamily
{
	decorative = 0,
	modern = 1,
	roman = 2,
	script = 3,
	swiss = 4,
	auto = 5
}
