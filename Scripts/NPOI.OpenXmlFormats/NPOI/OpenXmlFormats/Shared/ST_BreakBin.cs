using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Shared;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
public enum ST_BreakBin
{
	NONE = 0,
	before = 1,
	after = 2,
	repeat = 3
}
