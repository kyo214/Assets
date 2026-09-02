using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_MultiLevelType
{
	singleLevel = 0,
	multilevel = 1,
	hybridMultilevel = 2
}
