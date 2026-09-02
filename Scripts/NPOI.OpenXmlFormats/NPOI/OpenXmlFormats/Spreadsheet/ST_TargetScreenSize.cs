using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

public enum ST_TargetScreenSize
{
	[XmlEnum("544x376")]
	Item544x376 = 0,
	[XmlEnum("640x480")]
	Item640x480 = 1,
	[XmlEnum("720x512")]
	Item720x512 = 2,
	[XmlEnum("800x600")]
	Item800x600 = 3,
	[XmlEnum("1024x768")]
	Item1024x768 = 4,
	[XmlEnum("1152x882")]
	Item1152x882 = 5,
	[XmlEnum("1152x900")]
	Item1152x900 = 6,
	[XmlEnum("1280x1024")]
	Item1280x1024 = 7,
	[XmlEnum("1600x1200")]
	Item1600x1200 = 8,
	[XmlEnum("1800x1440")]
	Item1800x1440 = 9,
	[XmlEnum("1920x1200")]
	Item1920x1200 = 10
}
