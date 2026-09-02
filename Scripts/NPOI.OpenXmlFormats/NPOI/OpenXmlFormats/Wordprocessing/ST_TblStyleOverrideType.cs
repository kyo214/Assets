using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
public enum ST_TblStyleOverrideType
{
	wholeTable = 0,
	firstRow = 1,
	lastRow = 2,
	firstCol = 3,
	lastCol = 4,
	band1Vert = 5,
	band2Vert = 6,
	band1Horz = 7,
	band2Horz = 8,
	neCell = 9,
	nwCell = 10,
	seCell = 11,
	swCell = 12
}
