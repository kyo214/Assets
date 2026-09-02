using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Office;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:office")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = true)]
public class CT_ColorMru
{
	private ST_Ext extField;

	private string colorsField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "urn:schemas-microsoft-com:vml")]
	public ST_Ext ext
	{
		get
		{
			return extField;
		}
		set
		{
			extField = value;
		}
	}

	[XmlIgnore]
	public bool extSpecified => extField != ST_Ext.NONE;

	[XmlAttribute]
	public string colors
	{
		get
		{
			return colorsField;
		}
		set
		{
			colorsField = value;
		}
	}
}
