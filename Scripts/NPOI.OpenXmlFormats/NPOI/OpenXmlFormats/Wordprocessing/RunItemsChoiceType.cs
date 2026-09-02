using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IncludeInSchema = false)]
public enum RunItemsChoiceType
{
	annotationRef = 0,
	br = 1,
	commentReference = 2,
	continuationSeparator = 3,
	cr = 4,
	dayLong = 5,
	dayShort = 6,
	delInstrText = 7,
	delText = 8,
	drawing = 9,
	endnoteRef = 10,
	endnoteReference = 11,
	fldChar = 12,
	footnoteRef = 13,
	footnoteReference = 14,
	instrText = 15,
	lastRenderedPageBreak = 16,
	monthLong = 17,
	monthShort = 18,
	noBreakHyphen = 19,
	@object = 20,
	pgNum = 21,
	pict = 22,
	ptab = 23,
	ruby = 24,
	separator = 25,
	softHyphen = 26,
	sym = 27,
	t = 28,
	tab = 29,
	yearLong = 30,
	yearShort = 31
}
