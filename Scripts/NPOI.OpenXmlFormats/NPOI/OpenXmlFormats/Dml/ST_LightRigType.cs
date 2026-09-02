using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_LightRigType
{
	legacyFlat1 = 0,
	legacyFlat2 = 1,
	legacyFlat3 = 2,
	legacyFlat4 = 3,
	legacyNormal1 = 4,
	legacyNormal2 = 5,
	legacyNormal3 = 6,
	legacyNormal4 = 7,
	legacyHarsh1 = 8,
	legacyHarsh2 = 9,
	legacyHarsh3 = 10,
	legacyHarsh4 = 11,
	threePt = 12,
	balanced = 13,
	soft = 14,
	harsh = 15,
	flood = 16,
	contrasting = 17,
	morning = 18,
	sunrise = 19,
	sunset = 20,
	chilly = 21,
	freezing = 22,
	flat = 23,
	twoPt = 24,
	glow = 25,
	brightRoom = 26
}
