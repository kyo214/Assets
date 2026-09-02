using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Office;

[Serializable]
[XmlType(TypeName = "ST_TrueFalse", Namespace = "urn:schemas-microsoft-com:office:office")]
[XmlRoot("ST_TrueFalse", Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = false)]
public enum ST_TrueFalse
{
	f = 0,
	t = 1,
	@true = 2,
	@false = 3
}
