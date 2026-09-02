using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IncludeInSchema = false)]
public enum ItemsChoiceType11
{
	behaviors = 0,
	category = 1,
	description = 2,
	guid = 3,
	name = 4,
	style = 5,
	types = 6
}
