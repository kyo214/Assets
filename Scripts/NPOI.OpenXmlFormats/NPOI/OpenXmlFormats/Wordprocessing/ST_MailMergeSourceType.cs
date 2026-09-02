using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_MailMergeSourceType
{
	database = 0,
	addressBook = 1,
	document1 = 2,
	document2 = 3,
	text = 4,
	email = 5,
	native = 6,
	legacy = 7,
	master = 8
}
