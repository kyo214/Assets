using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:vml")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:vml", IsNullable = false)]
public enum ST_ShadowType
{
	single = 0,
	@double = 1,
	emboss = 2,
	perspective = 3
}
