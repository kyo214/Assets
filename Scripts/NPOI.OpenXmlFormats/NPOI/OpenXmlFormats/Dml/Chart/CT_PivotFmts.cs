using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart", IsNullable = true)]
public class CT_PivotFmts
{
	private List<CT_PivotFmt> pivotFmtField;

	[XmlElement("pivotFmt", Order = 0)]
	public List<CT_PivotFmt> pivotFmt
	{
		get
		{
			return pivotFmtField;
		}
		set
		{
			pivotFmtField = value;
		}
	}
}
