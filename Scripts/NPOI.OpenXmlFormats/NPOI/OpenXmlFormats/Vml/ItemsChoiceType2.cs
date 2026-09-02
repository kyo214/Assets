using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:vml", IncludeInSchema = false)]
public enum ItemsChoiceType2
{
	[XmlEnum("urn:schemas-microsoft-com:office:excel:ClientData")]
	ClientData = 0,
	[XmlEnum("urn:schemas-microsoft-com:office:powerpoint:textdata")]
	textdata = 1,
	[XmlEnum("urn:schemas-microsoft-com:office:word:anchorlock")]
	anchorlock = 2,
	[XmlEnum("urn:schemas-microsoft-com:office:word:borderbottom")]
	borderbottom = 3,
	[XmlEnum("urn:schemas-microsoft-com:office:word:borderleft")]
	borderleft = 4,
	[XmlEnum("urn:schemas-microsoft-com:office:word:borderright")]
	borderright = 5,
	[XmlEnum("urn:schemas-microsoft-com:office:word:bordertop")]
	bordertop = 6,
	[XmlEnum("urn:schemas-microsoft-com:office:word:wrap")]
	wrap = 7,
	fill = 8,
	formulas = 9,
	handles = 10,
	imagedata = 11,
	path = 12,
	shadow = 13,
	stroke = 14,
	textbox = 15,
	textpath = 16
}
