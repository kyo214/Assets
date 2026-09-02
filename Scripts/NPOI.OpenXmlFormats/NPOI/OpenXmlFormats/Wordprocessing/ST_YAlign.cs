using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_YAlign
{
	inline = 0,
	top = 1,
	center = 2,
	bottom = 3,
	inside = 4,
	outside = 5
}
