using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
public enum ST_TextFontAlignType
{
	auto = 0,
	t = 1,
	ctr = 2,
	@base = 3,
	b = 4
}
