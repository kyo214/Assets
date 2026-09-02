using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_EdGrp
{
	none = 0,
	everyone = 1,
	administrators = 2,
	contributors = 3,
	editors = 4,
	owners = 5,
	current = 6
}
