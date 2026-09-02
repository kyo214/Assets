using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_MailMergeDocType
{
	catalog = 0,
	envelopes = 1,
	mailingLabels = 2,
	formLetters = 3,
	email = 4,
	fax = 5
}
