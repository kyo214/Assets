using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Diagram;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram", IsNullable = true)]
public class CT_AnimOne
{
	private ST_AnimOneStr valField;

	[XmlAttribute]
	[DefaultValue(ST_AnimOneStr.one)]
	public ST_AnimOneStr val
	{
		get
		{
			return valField;
		}
		set
		{
			valField = value;
		}
	}

	public CT_AnimOne()
	{
		valField = ST_AnimOneStr.one;
	}
}
