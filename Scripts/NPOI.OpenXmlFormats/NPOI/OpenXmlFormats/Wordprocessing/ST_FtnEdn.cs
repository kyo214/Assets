using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_FtnEdn
{
	normal = 0,
	separator = 1,
	continuationSeparator = 2,
	continuationNotice = 3
}
