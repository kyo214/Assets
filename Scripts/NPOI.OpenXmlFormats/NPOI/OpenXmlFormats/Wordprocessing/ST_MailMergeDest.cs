using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_MailMergeDest
{
	newDocument = 0,
	printer = 1,
	email = 2,
	fax = 3
}
