using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Office;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:office")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = false)]
public enum ST_BWMode
{
	color = 0,
	auto = 1,
	grayScale = 2,
	lightGrayscale = 3,
	inverseGray = 4,
	grayOutline = 5,
	highContrast = 6,
	black = 7,
	white = 8,
	hide = 9,
	undrawn = 10,
	blackTextAndLines = 11
}
