using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Spreadsheet;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:excel")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:excel", IsNullable = false)]
public enum ST_TrueFalseBlank
{
	NONE = 0,
	[XmlEnum("True")]
	@true = 1,
	t = 2,
	[XmlEnum("False")]
	@false = 3,
	f = 4
}
