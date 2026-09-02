using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_View
{
	none = 0,
	print = 1,
	outline = 2,
	masterPages = 3,
	normal = 4,
	web = 5
}
