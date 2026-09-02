using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_SectionMark
{
	nextPage = 0,
	nextColumn = 1,
	continuous = 2,
	evenPage = 3,
	oddPage = 4
}
