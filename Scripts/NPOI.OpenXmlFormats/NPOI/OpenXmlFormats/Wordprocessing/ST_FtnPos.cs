using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_FtnPos
{
	pageBottom = 0,
	beneathText = 1,
	sectEnd = 2,
	docEnd = 3
}
