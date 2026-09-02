using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes")]
public enum ST_VectorBaseType
{
	variant = 0,
	i1 = 1,
	i2 = 2,
	i4 = 3,
	i8 = 4,
	ui1 = 5,
	ui2 = 6,
	ui4 = 7,
	ui8 = 8,
	r4 = 9,
	r8 = 10,
	lpstr = 11,
	lpwstr = 12,
	bstr = 13,
	date = 14,
	filetime = 15,
	@bool = 16,
	cy = 17,
	error = 18,
	clsid = 19,
	cf = 20
}
