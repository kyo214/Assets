using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Spreadsheet;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:excel")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:excel", IsNullable = false)]
public enum ST_ObjectType
{
	Button = 0,
	Checkbox = 1,
	Dialog = 2,
	Drop = 3,
	Edit = 4,
	GBox = 5,
	Label = 6,
	LineA = 7,
	List = 8,
	Movie = 9,
	Note = 10,
	Pict = 11,
	Radio = 12,
	RectA = 13,
	Scroll = 14,
	Spin = 15,
	Shape = 16,
	Group = 17,
	Rect = 18
}
