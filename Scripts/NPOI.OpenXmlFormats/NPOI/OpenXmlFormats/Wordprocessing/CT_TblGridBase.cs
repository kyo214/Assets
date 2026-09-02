using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Wordprocessing;

[Serializable]
[XmlInclude(typeof(CT_TblGrid))]
[XmlType(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main", IsNullable = true)]
public class CT_TblGridBase
{
	private List<CT_TblGridCol> gridColField;

	[XmlElement("gridCol", Order = 0)]
	public List<CT_TblGridCol> gridCol
	{
		get
		{
			return gridColField;
		}
		set
		{
			gridColField = value;
		}
	}

	public CT_TblGridBase()
	{
		gridColField = new List<CT_TblGridCol>();
	}
}
