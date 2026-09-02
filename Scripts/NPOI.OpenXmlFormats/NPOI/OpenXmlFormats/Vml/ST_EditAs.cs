using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:vml")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:vml", IsNullable = false)]
public enum ST_EditAs
{
	canvas = 0,
	orgchart = 1,
	radial = 2,
	cycle = 3,
	stacked = 4,
	venn = 5,
	bullseye = 6
}
