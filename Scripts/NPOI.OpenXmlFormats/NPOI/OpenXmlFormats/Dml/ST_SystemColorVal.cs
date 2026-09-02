using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_SystemColorVal
{
	scrollBar = 0,
	background = 1,
	activeCaption = 2,
	inactiveCaption = 3,
	menu = 4,
	window = 5,
	windowFrame = 6,
	menuText = 7,
	windowText = 8,
	captionText = 9,
	activeBorder = 10,
	inactiveBorder = 11,
	appWorkspace = 12,
	highlight = 13,
	highlightText = 14,
	btnFace = 15,
	btnShadow = 16,
	grayText = 17,
	btnText = 18,
	inactiveCaptionText = 19,
	btnHighlight = 20,
	[XmlEnum("3dDkShadow")]
	Item3dDkShadow = 21,
	[XmlEnum("3dLight")]
	Item3dLight = 22,
	infoText = 23,
	infoBk = 24,
	hotLight = 25,
	gradientActiveCaption = 26,
	gradientInactiveCaption = 27,
	menuHighlight = 28,
	menuBar = 29
}
