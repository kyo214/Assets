using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_CombineBrackets
{
	none = 0,
	round = 1,
	square = 2,
	angle = 3,
	curly = 4
}
