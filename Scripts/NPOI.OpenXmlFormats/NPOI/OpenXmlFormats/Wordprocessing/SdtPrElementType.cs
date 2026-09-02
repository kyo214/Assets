using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IncludeInSchema = false)]
public enum SdtPrElementType
{
	alias = 0,
	bibliography = 1,
	citation = 2,
	comboBox = 3,
	dataBinding = 4,
	date = 5,
	docPartList = 6,
	docPartObj = 7,
	dropDownList = 8,
	equation = 9,
	group = 10,
	id = 11,
	@lock = 12,
	picture = 13,
	placeholder = 14,
	rPr = 15,
	richText = 16,
	showingPlcHdr = 17,
	tag = 18,
	temporary = 19,
	text = 20
}
