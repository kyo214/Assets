using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IncludeInSchema = false)]
public enum ItemsChoiceType18
{
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:oMath")]
	oMath = 0,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:oMathPara")]
	oMathPara = 1,
	bookmarkEnd = 2,
	bookmarkStart = 3,
	commentRangeEnd = 4,
	commentRangeStart = 5,
	customXml = 6,
	customXmlDelRangeEnd = 7,
	customXmlDelRangeStart = 8,
	customXmlInsRangeEnd = 9,
	customXmlInsRangeStart = 10,
	customXmlMoveFromRangeEnd = 11,
	customXmlMoveFromRangeStart = 12,
	customXmlMoveToRangeEnd = 13,
	customXmlMoveToRangeStart = 14,
	del = 15,
	fldSimple = 16,
	hyperlink = 17,
	ins = 18,
	moveFrom = 19,
	moveFromRangeEnd = 20,
	moveFromRangeStart = 21,
	moveTo = 22,
	moveToRangeEnd = 23,
	moveToRangeStart = 24,
	permEnd = 25,
	permStart = 26,
	proofErr = 27,
	r = 28,
	sdt = 29,
	smartTag = 30,
	subDoc = 31
}
