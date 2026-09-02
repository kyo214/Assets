using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_ChapterSep
{
	hyphen = 0,
	period = 1,
	colon = 2,
	emDash = 3,
	enDash = 4
}
