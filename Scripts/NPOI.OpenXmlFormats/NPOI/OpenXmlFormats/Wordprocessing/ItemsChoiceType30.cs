using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IncludeInSchema = false)]
public enum ItemsChoiceType30
{
	bookmarkEnd = 0,
	bookmarkStart = 1,
	commentRangeEnd = 2,
	commentRangeStart = 3,
	customXmlDelRangeEnd = 4,
	customXmlDelRangeStart = 5,
	customXmlInsRangeEnd = 6,
	customXmlInsRangeStart = 7,
	customXmlMoveFromRangeEnd = 8,
	customXmlMoveFromRangeStart = 9,
	customXmlMoveToRangeEnd = 10,
	customXmlMoveToRangeStart = 11,
	moveFromRangeEnd = 12,
	moveFromRangeStart = 13,
	moveToRangeEnd = 14,
	moveToRangeStart = 15
}
