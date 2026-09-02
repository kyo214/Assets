using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:vml")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:vml", IsNullable = false)]
public enum ST_StrokeLineStyle
{
	single = 0,
	thinThin = 1,
	thinThick = 2,
	thickThin = 3,
	thickBetweenThin = 4
}
