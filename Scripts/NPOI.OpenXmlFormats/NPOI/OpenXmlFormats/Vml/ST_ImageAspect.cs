using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:vml")]
public enum ST_ImageAspect
{
	ignore = 0,
	atMost = 1,
	atLeast = 2
}
