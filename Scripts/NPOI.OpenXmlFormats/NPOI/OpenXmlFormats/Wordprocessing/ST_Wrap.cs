using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_Wrap
{
	auto = 0,
	notBeside = 1,
	around = 2,
	tight = 3,
	through = 4,
	none = 5
}
