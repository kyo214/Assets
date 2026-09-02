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
public class CT_AnimLvl
{
	private ST_AnimLvlStr valField;

	[XmlAttribute]
	[DefaultValue(ST_AnimLvlStr.none)]
	public ST_AnimLvlStr val
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

	public CT_AnimLvl()
	{
		valField = ST_AnimLvlStr.none;
	}
}
