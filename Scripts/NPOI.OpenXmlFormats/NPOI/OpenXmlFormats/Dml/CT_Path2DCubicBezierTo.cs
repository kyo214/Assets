using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/main", IsNullable = true)]
public class CT_Path2DCubicBezierTo
{
	private CT_AdjPoint2D[] ptField;

	[XmlElement("pt", Order = 0)]
	public CT_AdjPoint2D[] pt
	{
		get
		{
			return ptField;
		}
		set
		{
			ptField = value;
		}
	}
}
