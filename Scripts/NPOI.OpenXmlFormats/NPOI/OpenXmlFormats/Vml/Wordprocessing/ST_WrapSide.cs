using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Wordprocessing;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:word")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:word", IsNullable = false)]
public enum ST_WrapSide
{
	both = 0,
	left = 1,
	right = 2,
	largest = 3
}
