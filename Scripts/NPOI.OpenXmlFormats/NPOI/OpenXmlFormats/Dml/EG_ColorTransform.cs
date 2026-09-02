using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IncludeInSchema = false)]
public enum EG_ColorTransform
{
	alpha = 0,
	alphaMod = 1,
	alphaOff = 2,
	blue = 3,
	blueMod = 4,
	blueOff = 5,
	comp = 6,
	gamma = 7,
	gray = 8,
	green = 9,
	greenMod = 10,
	greenOff = 11,
	hue = 12,
	hueMod = 13,
	hueOff = 14,
	inv = 15,
	invGamma = 16,
	lum = 17,
	lumMod = 18,
	lumOff = 19,
	red = 20,
	redMod = 21,
	redOff = 22,
	sat = 23,
	satMod = 24,
	satOff = 25,
	shade = 26,
	tint = 27
}
