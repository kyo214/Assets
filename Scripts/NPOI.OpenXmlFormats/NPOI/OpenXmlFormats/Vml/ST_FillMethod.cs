using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:vml")]
public enum ST_FillMethod
{
	none = 0,
	linear = 1,
	sigma = 2,
	any = 3,
	[XmlEnum("linear sigma")]
	linearsigma = 4
}
