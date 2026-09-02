using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes", IncludeInSchema = false)]
public enum ItemsChoiceType
{
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:bool")]
	@bool = 0,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:bstr")]
	bstr = 1,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:cy")]
	cy = 2,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:date")]
	date = 3,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:decimal")]
	@decimal = 4,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:error")]
	error = 5,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:i1")]
	i1 = 6,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:i2")]
	i2 = 7,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:i4")]
	i4 = 8,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:int")]
	@int = 9,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:r4")]
	r4 = 10,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:r8")]
	r8 = 11,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:ui1")]
	ui1 = 12,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:ui2")]
	ui2 = 13,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:ui4")]
	ui4 = 14,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:uint")]
	@uint = 15,
	[XmlEnum("http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes:variant")]
	variant = 16
}
