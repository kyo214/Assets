using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Office;

[Serializable]
[XmlType(TypeName = "ST_FillType", Namespace = "urn:schemas-microsoft-com:office:office")]
[XmlRoot("ST_FillType", Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = false)]
public enum ST_FillType1
{
	gradientCenter = 0,
	solid = 1,
	pattern = 2,
	tile = 3,
	frame = 4,
	gradientUnscaled = 5,
	gradientRadial = 6,
	gradient = 7,
	background = 8
}
