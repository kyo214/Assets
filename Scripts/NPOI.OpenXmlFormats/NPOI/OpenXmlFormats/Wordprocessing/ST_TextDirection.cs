using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_TextDirection
{
	lrTb = 0,
	tbRl = 1,
	btLr = 2,
	lrTbV = 3,
	tbRlV = 4,
	tbLrV = 5
}
