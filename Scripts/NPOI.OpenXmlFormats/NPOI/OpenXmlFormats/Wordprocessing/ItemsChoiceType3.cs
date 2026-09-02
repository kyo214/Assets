using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IncludeInSchema = false)]
public enum ItemsChoiceType3
{
	b = 0,
	bCs = 1,
	bdr = 2,
	caps = 3,
	color = 4,
	cs = 5,
	dstrike = 6,
	eastAsianLayout = 7,
	effect = 8,
	em = 9,
	emboss = 10,
	fitText = 11,
	highlight = 12,
	i = 13,
	iCs = 14,
	imprint = 15,
	kern = 16,
	lang = 17,
	noProof = 18,
	oMath = 19,
	outline = 20,
	position = 21,
	rFonts = 22,
	rStyle = 23,
	rtl = 24,
	shadow = 25,
	shd = 26,
	smallCaps = 27,
	snapToGrid = 28,
	spacing = 29,
	specVanish = 30,
	strike = 31,
	sz = 32,
	szCs = 33,
	u = 34,
	vanish = 35,
	vertAlign = 36,
	w = 37,
	webHidden = 38
}
