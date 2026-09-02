using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IncludeInSchema = false)]
public enum ItemsChoiceType2
{
	cantSplit = 0,
	cnfStyle = 1,
	divId = 2,
	gridAfter = 3,
	gridBefore = 4,
	hidden = 5,
	jc = 6,
	tblCellSpacing = 7,
	tblHeader = 8,
	trHeight = 9,
	wAfter = 10,
	wBefore = 11
}
