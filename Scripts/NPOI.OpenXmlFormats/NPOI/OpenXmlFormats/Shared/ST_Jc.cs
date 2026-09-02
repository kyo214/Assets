using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Shared;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/math")]
public enum ST_Jc
{
	left = 0,
	right = 1,
	center = 2,
	centerGroup = 3
}
