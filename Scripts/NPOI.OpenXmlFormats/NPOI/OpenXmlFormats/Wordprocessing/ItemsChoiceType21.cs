using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IncludeInSchema = false)]
public enum ItemsChoiceType21
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
	ins = 16,
	moveFrom = 17,
	moveFromRangeEnd = 18,
	moveFromRangeStart = 19,
	moveTo = 20,
	moveToRangeEnd = 21,
	moveToRangeStart = 22,
	permEnd = 23,
	permStart = 24,
	proofErr = 25,
	sdt = 26,
	tr = 27
}
