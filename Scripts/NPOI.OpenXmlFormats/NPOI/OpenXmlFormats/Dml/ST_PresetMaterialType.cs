using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_PresetMaterialType
{
	legacyMatte = 0,
	legacyPlastic = 1,
	legacyMetal = 2,
	legacyWireframe = 3,
	matte = 4,
	plastic = 5,
	metal = 6,
	warmMatte = 7,
	translucentPowder = 8,
	powder = 9,
	dkEdge = 10,
	softEdge = 11,
	clear = 12,
	flat = 13,
	softmetal = 14
}
