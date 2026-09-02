using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IncludeInSchema = false)]
public enum ItemsChoiceType16
{
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:oMath")]
	oMath = 0,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:oMathPara")]
	oMathPara = 1,
	bookmarkEnd = 2,
	bookmarkStart = 3,
	commentRangeEnd = 4,
	commentRangeStart = 5,
	customXmlDelRangeEnd = 6,
	customXmlDelRangeStart = 7,
	customXmlInsRangeEnd = 8,
	customXmlInsRangeStart = 9,
	customXmlMoveFromRangeEnd = 10,
	customXmlMoveFromRangeStart = 11,
	customXmlMoveToRangeEnd = 12,
	customXmlMoveToRangeStart = 13,
	del = 14,
	ins = 15,
	moveFrom = 16,
	moveFromRangeEnd = 17,
	moveFromRangeStart = 18,
	moveTo = 19,
	moveToRangeEnd = 20,
	moveToRangeStart = 21,
	permEnd = 22,
	permStart = 23,
	proofErr = 24,
	r = 25
}
