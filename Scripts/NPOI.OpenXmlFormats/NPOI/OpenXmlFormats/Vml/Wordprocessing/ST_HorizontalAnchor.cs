using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Wordprocessing;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:word")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:word", IsNullable = false)]
public enum ST_HorizontalAnchor
{
	margin = 0,
	page = 1,
	text = 2,
	@char = 3
}
