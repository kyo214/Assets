using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_ProofErr
{
	spellStart = 0,
	spellEnd = 1,
	gramStart = 2,
	gramEnd = 3
}
