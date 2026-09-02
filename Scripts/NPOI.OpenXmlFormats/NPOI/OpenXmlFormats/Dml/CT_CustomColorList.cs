using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_CustomColorList
{
	private List<CT_CustomColor> custClrField;

	[XmlElement("custClr", Order = 0)]
	public List<CT_CustomColor> custClr
	{
		get
		{
			return custClrField;
		}
		set
		{
			custClrField = value;
		}
	}

	public CT_CustomColorList()
	{
		custClrField = new List<CT_CustomColor>();
	}
}
