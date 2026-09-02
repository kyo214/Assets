using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Office;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:office")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = false)]
public enum ST_How
{
	NONE = 0,
	top = 1,
	middle = 2,
	bottom = 3,
	left = 4,
	center = 5,
	right = 6
}
