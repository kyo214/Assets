using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Office;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:office")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = false)]
public enum ST_ExtrusionRender
{
	solid = 0,
	wireFrame = 1,
	boundingCube = 2
}
