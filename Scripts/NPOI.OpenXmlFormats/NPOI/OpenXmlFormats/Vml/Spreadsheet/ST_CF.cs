using System;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Spreadsheet;

[Serializable]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:excel")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:excel", IsNullable = false)]
public enum ST_CF
{
	PictOld = 0,
	Pict = 1,
	Bitmap = 2,
	PictPrint = 3,
	PictScreen = 4
}
