using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_Shd
{
	nil = 0,
	clear = 1,
	solid = 2,
	horzStripe = 3,
	vertStripe = 4,
	reverseDiagStripe = 5,
	diagStripe = 6,
	horzCross = 7,
	diagCross = 8,
	thinHorzStripe = 9,
	thinVertStripe = 10,
	thinReverseDiagStripe = 11,
	thinDiagStripe = 12,
	thinHorzCross = 13,
	thinDiagCross = 14,
	pct5 = 15,
	pct10 = 16,
	pct12 = 17,
	pct15 = 18,
	pct20 = 19,
	pct25 = 20,
	pct30 = 21,
	pct35 = 22,
	pct37 = 23,
	pct40 = 24,
	pct45 = 25,
	pct50 = 26,
	pct55 = 27,
	pct60 = 28,
	pct62 = 29,
	pct65 = 30,
	pct70 = 31,
	pct75 = 32,
	pct80 = 33,
	pct85 = 34,
	pct87 = 35,
	pct90 = 36,
	pct95 = 37
}
