using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Shared;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math", IncludeInSchema = false)]
public enum ItemsChoiceType7
{
	acc = 0,
	bar = 1,
	borderBox = 2,
	box = 3,
	d = 4,
	eqArr = 5,
	f = 6,
	func = 7,
	groupChr = 8,
	limLow = 9,
	limUpp = 10,
	m = 11,
	nary = 12,
	oMath = 13,
	oMathPara = 14,
	phant = 15,
	r = 16,
	rad = 17,
	sPre = 18,
	sSub = 19,
	sSubSup = 20,
	sSup = 21,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:bookmarkEnd")]
	bookmarkEnd = 22,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:bookmarkStart")]
	bookmarkStart = 23,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:commentRangeEnd")]
	commentRangeEnd = 24,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:commentRangeStart")]
	commentRangeStart = 25,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:customXmlDelRangeEnd")]
	customXmlDelRangeEnd = 26,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:customXmlDelRangeStart")]
	customXmlDelRangeStart = 27,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:customXmlInsRangeEnd")]
	customXmlInsRangeEnd = 28,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:customXmlInsRangeStart")]
	customXmlInsRangeStart = 29,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:customXmlMoveFromRangeEnd")]
	customXmlMoveFromRangeEnd = 30,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:customXmlMoveFromRangeStart")]
	customXmlMoveFromRangeStart = 31,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:customXmlMoveToRangeEnd")]
	customXmlMoveToRangeEnd = 32,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:customXmlMoveToRangeStart")]
	customXmlMoveToRangeStart = 33,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:del")]
	del = 34,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:ins")]
	ins = 35,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:moveFrom")]
	moveFrom = 36,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:moveFromRangeEnd")]
	moveFromRangeEnd = 37,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:moveFromRangeStart")]
	moveFromRangeStart = 38,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:moveTo")]
	moveTo = 39,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:moveToRangeEnd")]
	moveToRangeEnd = 40,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:moveToRangeStart")]
	moveToRangeStart = 41,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:permEnd")]
	permEnd = 42,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:permStart")]
	permStart = 43,
	[XmlEnum("http://schemas.openxmlformats.org/wordprocessingml/2006/main:proofErr")]
	proofErr = 44
}
