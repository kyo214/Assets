using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Wordprocessing;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:word")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:word", IsNullable = false)]
public enum ST_BorderShadow
{
	t = 0,
	@true = 1,
	f = 2,
	@false = 3
}
