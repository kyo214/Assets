using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_Lock
{
	sdtLocked = 0,
	contentLocked = 1,
	unlocked = 2,
	sdtContentLocked = 3
}
