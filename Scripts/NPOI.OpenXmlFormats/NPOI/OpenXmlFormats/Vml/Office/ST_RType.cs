using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Office;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:office")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = false)]
public enum ST_RType
{
	NONE = 0,
	arc = 1,
	callout = 2,
	connector = 3,
	align = 4
}
