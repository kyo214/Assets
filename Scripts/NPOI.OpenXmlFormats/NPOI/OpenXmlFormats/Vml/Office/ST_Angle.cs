using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Office;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:office")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = false)]
public enum ST_Angle
{
	any = 0,
	[XmlEnum("30")]
	Item30 = 1,
	[XmlEnum("45")]
	Item45 = 2,
	[XmlEnum("60")]
	Item60 = 3,
	[XmlEnum("90")]
	Item90 = 4,
	auto = 5
}
