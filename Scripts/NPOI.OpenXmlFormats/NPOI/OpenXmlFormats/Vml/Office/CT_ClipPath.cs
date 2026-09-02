using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Vml.Office;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "urn:schemas-microsoft-com:office:office")]
[XmlRoot(Namespace = "urn:schemas-microsoft-com:office:office", IsNullable = true)]
public class CT_ClipPath
{
	private string vField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string v
	{
		get
		{
			return vField;
		}
		set
		{
			vField = value;
		}
	}
}
