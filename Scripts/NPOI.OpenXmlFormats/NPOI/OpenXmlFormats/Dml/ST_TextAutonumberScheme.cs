using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_TextAutonumberScheme
{
	alphaLcParenBoth = 0,
	alphaUcParenBoth = 1,
	alphaLcParenR = 2,
	alphaUcParenR = 3,
	alphaLcPeriod = 4,
	alphaUcPeriod = 5,
	arabicParenBoth = 6,
	arabicParenR = 7,
	arabicPeriod = 8,
	arabicPlain = 9,
	romanLcParenBoth = 10,
	romanUcParenBoth = 11,
	romanLcParenR = 12,
	romanUcParenR = 13,
	romanLcPeriod = 14,
	romanUcPeriod = 15,
	circleNumDbPlain = 16,
	circleNumWdBlackPlain = 17,
	circleNumWdWhitePlain = 18,
	arabicDbPeriod = 19,
	arabicDbPlain = 20,
	ea1ChsPeriod = 21,
	ea1ChsPlain = 22,
	ea1ChtPeriod = 23,
	ea1ChtPlain = 24,
	ea1JpnChsDbPeriod = 25,
	ea1JpnKorPlain = 26,
	ea1JpnKorPeriod = 27,
	arabic1Minus = 28,
	arabic2Minus = 29,
	hebrew2Minus = 30,
	thaiAlphaPeriod = 31,
	thaiAlphaParenR = 32,
	thaiAlphaParenBoth = 33,
	thaiNumPeriod = 34,
	thaiNumParenR = 35,
	thaiNumParenBoth = 36,
	hindiAlphaPeriod = 37,
	hindiNumPeriod = 38,
	hindiNumParenR = 39,
	hindiAlpha1Period = 40
}
