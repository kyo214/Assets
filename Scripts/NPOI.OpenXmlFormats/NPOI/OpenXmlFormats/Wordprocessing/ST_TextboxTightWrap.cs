using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_TextboxTightWrap
{
	none = 0,
	allLines = 1,
	firstAndLastLine = 2,
	firstLineOnly = 3,
	lastLineOnly = 4
}
