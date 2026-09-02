using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:vml")]
public enum ST_FillType
{
	solid = 0,
	gradient = 1,
	gradientRadial = 2,
	tile = 3,
	pattern = 4,
	frame = 5
}
