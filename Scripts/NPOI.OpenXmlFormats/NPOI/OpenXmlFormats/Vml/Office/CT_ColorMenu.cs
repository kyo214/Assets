using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Office;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:office")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = true)]
public class CT_ColorMenu
{
	private ST_Ext extField;

	private string strokecolorField;

	private string fillcolorField;

	private string shadowcolorField;

	private string extrusioncolorField;

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
	public string strokecolor
	{
		get
		{
			return strokecolorField;
		}
		set
		{
			strokecolorField = value;
		}
	}

	[XmlAttribute]
	public string fillcolor
	{
		get
		{
			return fillcolorField;
		}
		set
		{
			fillcolorField = value;
		}
	}

	[XmlAttribute]
	public string shadowcolor
	{
		get
		{
			return shadowcolorField;
		}
		set
		{
			shadowcolorField = value;
		}
	}

	[XmlAttribute]
	public string extrusioncolor
	{
		get
		{
			return extrusioncolorField;
		}
		set
		{
			extrusioncolorField = value;
		}
	}
}
