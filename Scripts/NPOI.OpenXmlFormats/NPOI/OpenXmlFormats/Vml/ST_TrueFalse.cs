using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:vml")]
public enum ST_TrueFalse
{
	f = 0,
	t = 1,
	@true = 2,
	@false = 3
}
