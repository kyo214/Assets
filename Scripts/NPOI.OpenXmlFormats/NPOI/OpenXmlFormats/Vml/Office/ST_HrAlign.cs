using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Office;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:office")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = false)]
public enum ST_HrAlign
{
	left = 0,
	right = 1,
	center = 2
}
