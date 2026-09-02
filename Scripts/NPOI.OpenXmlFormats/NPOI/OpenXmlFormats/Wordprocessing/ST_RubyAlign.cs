using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_RubyAlign
{
	center = 0,
	distributeLetter = 1,
	distributeSpace = 2,
	left = 3,
	right = 4,
	rightVertical = 5
}
