using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_HdrFtr
{
	[XmlEnum("even")]
	even = 0,
	[XmlEnum("default")]
	@default = 1,
	[XmlEnum("first")]
	first = 2
}
