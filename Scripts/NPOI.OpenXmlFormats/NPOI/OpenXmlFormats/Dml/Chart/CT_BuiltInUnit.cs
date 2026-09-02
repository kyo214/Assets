using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Chart;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/chart", IsNullable = true)]
public class CT_BuiltInUnit
{
	private ST_BuiltInUnit valField;

	[XmlAttribute]
	[DefaultValue(ST_BuiltInUnit.thousands)]
	public ST_BuiltInUnit val
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

	public CT_BuiltInUnit()
	{
		valField = ST_BuiltInUnit.thousands;
	}
}
