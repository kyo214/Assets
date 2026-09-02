using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_CalendarType
{
	gregorian = 0,
	hijri = 1,
	hebrew = 2,
	taiwan = 3,
	japan = 4,
	thai = 5,
	korea = 6,
	saka = 7,
	gregorianXlitEnglish = 8,
	gregorianXlitFrench = 9
}
