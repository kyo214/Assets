using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IncludeInSchema = false)]
public enum ItemsChoiceType6
{
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:acc")]
	acc = 0,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:bar")]
	bar = 1,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:borderBox")]
	borderBox = 2,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:box")]
	box = 3,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:d")]
	d = 4,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:eqArr")]
	eqArr = 5,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:f")]
	f = 6,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:func")]
	func = 7,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:groupChr")]
	groupChr = 8,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:limLow")]
	limLow = 9,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:limUpp")]
	limUpp = 10,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:m")]
	m = 11,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:nary")]
	nary = 12,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:oMath")]
	oMath = 13,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:oMathPara")]
	oMathPara = 14,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:phant")]
	phant = 15,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:r")]
	r = 16,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:rad")]
	rad = 17,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:sPre")]
	sPre = 18,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:sSub")]
	sSub = 19,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:sSubSup")]
	sSubSup = 20,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:sSup")]
	sSup = 21,
	bookmarkEnd = 22,
	bookmarkStart = 23,
	commentRangeEnd = 24,
	commentRangeStart = 25,
	customXml = 26,
	customXmlDelRangeEnd = 27,
	customXmlDelRangeStart = 28,
	customXmlInsRangeEnd = 29,
	customXmlInsRangeStart = 30,
	customXmlMoveFromRangeEnd = 31,
	customXmlMoveFromRangeStart = 32,
	customXmlMoveToRangeEnd = 33,
	customXmlMoveToRangeStart = 34,
	del = 35,
	ins = 36,
	moveFrom = 37,
	moveFromRangeEnd = 38,
	moveFromRangeStart = 39,
	moveTo = 40,
	moveToRangeEnd = 41,
	moveToRangeStart = 42,
	permEnd = 43,
	permStart = 44,
	proofErr = 45,
	[XmlEnum("r")]
	r1 = 46,
	sdt = 47,
	smartTag = 48
}
