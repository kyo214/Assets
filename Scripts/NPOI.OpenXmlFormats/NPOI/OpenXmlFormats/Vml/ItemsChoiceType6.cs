using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:vml", IncludeInSchema = false)]
public enum ItemsChoiceType6
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
	arc = 8,
	curve = 9,
	fill = 10,
	formulas = 11,
	group = 12,
	handles = 13,
	image = 14,
	imagedata = 15,
	line = 16,
	oval = 17,
	path = 18,
	polyline = 19,
	rect = 20,
	roundrect = 21,
	shadow = 22,
	shape = 23,
	shapetype = 24,
	stroke = 25,
	textbox = 26,
	textpath = 27
}
