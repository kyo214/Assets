using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_XAlign
{
	left = 0,
	center = 1,
	right = 2,
	inside = 3,
	outside = 4
}
