using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_Underline
{
	single = 0,
	words = 1,
	@double = 2,
	thick = 3,
	dotted = 4,
	dottedHeavy = 5,
	dash = 6,
	dashedHeavy = 7,
	dashLong = 8,
	dashLongHeavy = 9,
	dotDash = 10,
	dashDotHeavy = 11,
	dotDotDash = 12,
	dashDotDotHeavy = 13,
	wave = 14,
	wavyHeavy = 15,
	wavyDouble = 16,
	none = 17
}
