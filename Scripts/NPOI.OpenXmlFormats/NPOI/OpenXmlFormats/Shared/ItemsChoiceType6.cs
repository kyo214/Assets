using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Shared;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math", IncludeInSchema = false)]
public enum ItemsChoiceType6
{
	t = 0,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:annotationRef")]
	annotationRef = 1,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:br")]
	br = 2,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:commentReference")]
	commentReference = 3,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:continuationSeparator")]
	continuationSeparator = 4,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:cr")]
	cr = 5,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:dayLong")]
	dayLong = 6,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:dayShort")]
	dayShort = 7,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:delInstrText")]
	delInstrText = 8,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:delText")]
	delText = 9,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:drawing")]
	drawing = 10,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:endnoteRef")]
	endnoteRef = 11,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:endnoteReference")]
	endnoteReference = 12,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:fldChar")]
	fldChar = 13,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:footnoteRef")]
	footnoteRef = 14,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:footnoteReference")]
	footnoteReference = 15,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:instrText")]
	instrText = 16,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:lastRenderedPageBreak")]
	lastRenderedPageBreak = 17,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:monthLong")]
	monthLong = 18,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:monthShort")]
	monthShort = 19,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:noBreakHyphen")]
	noBreakHyphen = 20,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:object")]
	@object = 21,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:pgNum")]
	pgNum = 22,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:pict")]
	pict = 23,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:ptab")]
	ptab = 24,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:ruby")]
	ruby = 25,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:separator")]
	separator = 26,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:softHyphen")]
	softHyphen = 27,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:sym")]
	sym = 28,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:t")]
	t1 = 29,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:tab")]
	tab = 30,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:yearLong")]
	yearLong = 31,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:yearShort")]
	yearShort = 32
}
