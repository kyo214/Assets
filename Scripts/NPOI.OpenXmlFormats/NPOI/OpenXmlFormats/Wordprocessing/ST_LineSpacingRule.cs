using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_LineSpacingRule
{
	nil = 0,
	auto = 1,
	exact = 2,
	atLeast = 3
}
