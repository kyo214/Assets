using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:vml", IncludeInSchema = false)]
public enum ItemsChoiceType1
{
	[XmlEnum("urn:schemas-microsoft-com:office:excel:ClientData")]
	ClientData = 0,
	[XmlEnum("urn:schemas-microsoft-com:office:powerpoint:iscomment")]
	iscomment = 1,
	[XmlEnum("urn:schemas-microsoft-com:office:powerpoint:textdata")]
	textdata = 2,
	[XmlEnum("urn:schemas-microsoft-com:office:word:anchorlock")]
	anchorlock = 3,
	[XmlEnum("urn:schemas-microsoft-com:office:word:borderbottom")]
	borderbottom = 4,
	[XmlEnum("urn:schemas-microsoft-com:office:word:borderleft")]
	borderleft = 5,
	[XmlEnum("urn:schemas-microsoft-com:office:word:borderright")]
	borderright = 6,
	[XmlEnum("urn:schemas-microsoft-com:office:word:bordertop")]
	bordertop = 7,
	[XmlEnum("urn:schemas-microsoft-com:office:word:wrap")]
	wrap = 8,
	fill = 9,
	formulas = 10,
	handles = 11,
	imagedata = 12,
	path = 13,
	shadow = 14,
	stroke = 15,
	textbox = 16,
	textpath = 17
}
