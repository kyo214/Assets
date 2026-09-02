using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_FFTextType
{
	regular = 0,
	number = 1,
	date = 2,
	currentTime = 3,
	currentDate = 4,
	calculated = 5
}
