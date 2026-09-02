using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_TextEffect
{
	blinkBackground = 0,
	lights = 1,
	antsBlack = 2,
	antsRed = 3,
	shimmer = 4,
	sparkle = 5,
	none = 6
}
