using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_TextShapeType
{
	textNoShape = 0,
	textPlain = 1,
	textStop = 2,
	textTriangle = 3,
	textTriangleInverted = 4,
	textChevron = 5,
	textChevronInverted = 6,
	textRingInside = 7,
	textRingOutside = 8,
	textArchUp = 9,
	textArchDown = 10,
	textCircle = 11,
	textButton = 12,
	textArchUpPour = 13,
	textArchDownPour = 14,
	textCirclePour = 15,
	textButtonPour = 16,
	textCurveUp = 17,
	textCurveDown = 18,
	textCanUp = 19,
	textCanDown = 20,
	textWave1 = 21,
	textWave2 = 22,
	textDoubleWave1 = 23,
	textWave4 = 24,
	textInflate = 25,
	textDeflate = 26,
	textInflateBottom = 27,
	textDeflateBottom = 28,
	textInflateTop = 29,
	textDeflateTop = 30,
	textDeflateInflate = 31,
	textDeflateInflateDeflate = 32,
	textFadeRight = 33,
	textFadeLeft = 34,
	textFadeUp = 35,
	textFadeDown = 36,
	textSlantUp = 37,
	textSlantDown = 38,
	textCascadeUp = 39,
	textCascadeDown = 40
}
