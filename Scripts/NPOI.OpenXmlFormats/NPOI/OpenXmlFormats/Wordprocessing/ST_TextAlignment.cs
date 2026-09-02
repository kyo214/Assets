using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_TextAlignment
{
	top = 0,
	center = 1,
	baseline = 2,
	bottom = 3,
	auto = 4
}
