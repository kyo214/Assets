using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Office;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:office")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = false)]
public enum ST_ScreenSize
{
	[XmlEnum("544,376")]
	Item544376 = 0,
	[XmlEnum("640,480")]
	Item640480 = 1,
	[XmlEnum("720,512")]
	Item720512 = 2,
	[XmlEnum("800,600")]
	Item800600 = 3,
	[XmlEnum("1024,768")]
	Item1024768 = 4,
	[XmlEnum("1152,862")]
	Item1152862 = 5
}
