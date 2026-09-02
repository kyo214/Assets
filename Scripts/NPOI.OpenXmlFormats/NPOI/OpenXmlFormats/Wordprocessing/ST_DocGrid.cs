using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_DocGrid
{
	@default = 0,
	lines = 1,
	linesAndChars = 2,
	snapToChars = 3
}
