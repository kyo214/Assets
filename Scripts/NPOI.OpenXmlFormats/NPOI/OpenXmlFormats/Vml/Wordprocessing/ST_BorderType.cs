using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Wordprocessing;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:word")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:word", IsNullable = false)]
public enum ST_BorderType
{
	none = 0,
	single = 1,
	thick = 2,
	@double = 3,
	hairline = 4,
	dot = 5,
	dash = 6,
	dotDash = 7,
	dashDotDot = 8,
	triple = 9,
	thinThickSmall = 10,
	thickThinSmall = 11,
	thickBetweenThinSmall = 12,
	thinThick = 13,
	thickThin = 14,
	thickBetweenThin = 15,
	thinThickLarge = 16,
	thickThinLarge = 17,
	thickBetweenThinLarge = 18,
	wave = 19,
	doubleWave = 20,
	dashedSmall = 21,
	dashDotStroked = 22,
	threeDEmboss = 23,
	threeDEngrave = 24,
	HTMLOutset = 25,
	HTMLInset = 26
}
