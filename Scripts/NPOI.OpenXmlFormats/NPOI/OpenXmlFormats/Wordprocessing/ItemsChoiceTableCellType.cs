using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IncludeInSchema = false)]
public enum ItemsChoiceTableCellType
{
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:oMath")]
	oMath = 0,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/math:oMathPara")]
	oMathPara = 1,
	altChunk = 2,
	bookmarkEnd = 3,
	bookmarkStart = 4,
	commentRangeEnd = 5,
	commentRangeStart = 6,
	customXml = 7,
	customXmlDelRangeEnd = 8,
	customXmlDelRangeStart = 9,
	customXmlInsRangeEnd = 10,
	customXmlInsRangeStart = 11,
	customXmlMoveFromRangeEnd = 12,
	customXmlMoveFromRangeStart = 13,
	customXmlMoveToRangeEnd = 14,
	customXmlMoveToRangeStart = 15,
	del = 16,
	ins = 17,
	moveFrom = 18,
	moveFromRangeEnd = 19,
	moveFromRangeStart = 20,
	moveTo = 21,
	moveToRangeEnd = 22,
	moveToRangeStart = 23,
	p = 24,
	permEnd = 25,
	permStart = 26,
	proofErr = 27,
	sdt = 28,
	tbl = 29
}
