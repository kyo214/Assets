using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Office;

[Serializable]
[DesignerCategory("code")]
[XmlType(TypeName = "CT_Fill", Namespace = "urn:schemas-microsoft-com:office:office")]
[XmlRoot("CT_Fill", Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = true)]
public class CT_Fill
{
	private ST_Ext extField;

	private ST_FillType1 typeField;

	private bool typeFieldSpecified;

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
	public ST_FillType1 type
	{
		get
		{
			return typeField;
		}
		set
		{
			typeField = value;
		}
	}

	[XmlIgnore]
	public bool typeSpecified
	{
		get
		{
			return typeFieldSpecified;
		}
		set
		{
			typeFieldSpecified = value;
		}
	}
}
