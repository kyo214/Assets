using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_PageBorderDisplay
{
	allPages = 0,
	firstPage = 1,
	notFirstPage = 2
}
