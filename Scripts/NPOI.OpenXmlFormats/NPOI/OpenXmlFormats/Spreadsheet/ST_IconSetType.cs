using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

public enum ST_IconSetType
{
	[XmlEnum("3Arrows")]
	Item3Arrows = 0,
	[XmlEnum("3ArrowsGray")]
	Item3ArrowsGray = 1,
	[XmlEnum("3Flags")]
	Item3Flags = 2,
	[XmlEnum("3TrafficLights1")]
	Item3TrafficLights1 = 3,
	[XmlEnum("3TrafficLights2")]
	Item3TrafficLights2 = 4,
	[XmlEnum("3Signs")]
	Item3Signs = 5,
	[XmlEnum("3Symbols")]
	Item3Symbols = 6,
	[XmlEnum("3Symbols2")]
	Item3Symbols2 = 7,
	[XmlEnum("4Arrows")]
	Item4Arrows = 8,
	[XmlEnum("4ArrowsGray")]
	Item4ArrowsGray = 9,
	[XmlEnum("4RedToBlack")]
	Item4RedToBlack = 10,
	[XmlEnum("4Rating")]
	Item4Rating = 11,
	[XmlEnum("4TrafficLights")]
	Item4TrafficLights = 12,
	[XmlEnum("5Arrows")]
	Item5Arrows = 13,
	[XmlEnum("5ArrowsGray")]
	Item5ArrowsGray = 14,
	[XmlEnum("5Rating")]
	Item5Rating = 15,
	[XmlEnum("5Quarters")]
	Item5Quarters = 16
}
