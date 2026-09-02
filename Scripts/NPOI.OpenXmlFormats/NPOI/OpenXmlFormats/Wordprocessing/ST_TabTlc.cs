using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_TabTlc
{
	none = 0,
	dot = 1,
	hyphen = 2,
	underscore = 3,
	heavy = 4,
	middleDot = 5
}
