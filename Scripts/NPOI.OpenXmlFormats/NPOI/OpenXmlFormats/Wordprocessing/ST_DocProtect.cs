using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_DocProtect
{
	none = 0,
	readOnly = 1,
	comments = 2,
	trackedChanges = 3,
	forms = 4
}
