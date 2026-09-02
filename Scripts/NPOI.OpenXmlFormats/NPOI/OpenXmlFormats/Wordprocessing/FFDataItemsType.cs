using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IncludeInSchema = false)]
public enum FFDataItemsType
{
	calcOnExit = 0,
	checkBox = 1,
	ddList = 2,
	enabled = 3,
	entryMacro = 4,
	exitMacro = 5,
	helpText = 6,
	name = 7,
	statusText = 8,
	textInput = 9
}
