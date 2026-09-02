using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_DocPartType
{
	none = 0,
	normal = 1,
	autoExp = 2,
	toolbar = 3,
	speller = 4,
	formFld = 5,
	bbPlcHdr = 6
}
