using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", IncludeInSchema = false)]
public enum ItemChoiceType
{
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:array")]
	array = 0,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:blob")]
	blob = 1,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:bool")]
	@bool = 2,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:bstr")]
	bstr = 3,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:cf")]
	cf = 4,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:clsid")]
	clsid = 5,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:cy")]
	cy = 6,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:date")]
	date = 7,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:decimal")]
	@decimal = 8,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:empty")]
	empty = 9,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:error")]
	error = 10,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:filetime")]
	filetime = 11,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:i1")]
	i1 = 12,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:i2")]
	i2 = 13,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:i4")]
	i4 = 14,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:i8")]
	i8 = 15,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:int")]
	@int = 16,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:lpstr")]
	lpstr = 17,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:lpwstr")]
	lpwstr = 18,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:null")]
	@null = 19,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:oblob")]
	oblob = 20,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:ostorage")]
	ostorage = 21,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:ostream")]
	ostream = 22,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:r4")]
	r4 = 23,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:r8")]
	r8 = 24,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:storage")]
	storage = 25,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:stream")]
	stream = 26,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:ui1")]
	ui1 = 27,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:ui2")]
	ui2 = 28,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:ui4")]
	ui4 = 29,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:ui8")]
	ui8 = 30,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:uint")]
	@uint = 31,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:vector")]
	vector = 32,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:vstream")]
	vstream = 33
}
