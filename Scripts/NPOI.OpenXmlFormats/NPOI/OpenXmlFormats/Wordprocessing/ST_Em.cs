using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_Em
{
	none = 0,
	dot = 1,
	comma = 2,
	circle = 3,
	underDot = 4
}
