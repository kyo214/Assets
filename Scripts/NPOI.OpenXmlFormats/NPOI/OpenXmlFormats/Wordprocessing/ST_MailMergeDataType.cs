using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_MailMergeDataType
{
	textFile = 0,
	database = 1,
	spreadsheet = 2,
	query = 3,
	odbc = 4,
	native = 5
}
