using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_OnOff
{
	off = 0,
	on = 1,
	[XmlEnum("true")]
	True = 2,
	[XmlEnum("false")]
	False = 3
}
