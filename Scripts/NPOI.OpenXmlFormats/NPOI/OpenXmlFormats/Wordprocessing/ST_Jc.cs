using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_Jc
{
	left = 0,
	center = 1,
	right = 2,
	both = 3,
	mediumKashida = 4,
	distribute = 5,
	numTab = 6,
	highKashida = 7,
	lowKashida = 8,
	thaiDistribute = 9
}
