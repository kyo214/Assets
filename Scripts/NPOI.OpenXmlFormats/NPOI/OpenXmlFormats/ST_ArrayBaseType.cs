using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
public enum ST_ArrayBaseType
{
	variant = 0,
	i1 = 1,
	i2 = 2,
	i4 = 3,
	@int = 4,
	ui1 = 5,
	ui2 = 6,
	ui4 = 7,
	@uint = 8,
	r4 = 9,
	r8 = 10,
	@decimal = 11,
	bstr = 12,
	date = 13,
	@bool = 14,
	cy = 15,
	error = 16
}
