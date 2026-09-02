using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Shared;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
public enum ST_Script
{
	roman = 0,
	script = 1,
	fraktur = 2,
	[XmlEnum("double-struck")]
	doublestruck = 3,
	[XmlEnum("sans-serif")]
	sansserif = 4,
	monospace = 5
}
