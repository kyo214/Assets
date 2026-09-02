using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_DocPartGallery
{
	placeholder = 0,
	any = 1,
	@default = 2,
	docParts = 3,
	coverPg = 4,
	eq = 5,
	ftrs = 6,
	hdrs = 7,
	pgNum = 8,
	tbls = 9,
	watermarks = 10,
	autoTxt = 11,
	txtBox = 12,
	pgNumT = 13,
	pgNumB = 14,
	pgNumMargins = 15,
	tblOfContents = 16,
	bib = 17,
	custQuickParts = 18,
	custCoverPg = 19,
	custEq = 20,
	custFtrs = 21,
	custHdrs = 22,
	custPgNum = 23,
	custTbls = 24,
	custWatermarks = 25,
	custAutoTxt = 26,
	custTxtBox = 27,
	custPgNumT = 28,
	custPgNumB = 29,
	custPgNumMargins = 30,
	custTblOfContents = 31,
	custBib = 32,
	custom1 = 33,
	custom2 = 34,
	custom3 = 35,
	custom4 = 36,
	custom5 = 37
}
