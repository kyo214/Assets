using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Diagram;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram", IsNullable = true)]
public class CT_Constraints
{
	private List<CT_Constraint> constrField;

	[XmlElement("constr", Order = 0)]
	public List<CT_Constraint> constr
	{
		get
		{
			return constrField;
		}
		set
		{
			constrField = value;
		}
	}

	public CT_Constraints()
	{
		constrField = new List<CT_Constraint>();
	}
}
