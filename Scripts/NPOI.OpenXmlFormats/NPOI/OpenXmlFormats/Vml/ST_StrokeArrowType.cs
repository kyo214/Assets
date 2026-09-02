using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:vml")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:vml", IsNullable = false)]
public enum ST_StrokeArrowType
{
	none = 0,
	block = 1,
	classic = 2,
	oval = 3,
	diamond = 4,
	open = 5
}
