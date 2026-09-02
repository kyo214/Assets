using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/customXml")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/customXml", IsNullable = true)]
public class CT_DatastoreSchemaRef
{
	private string uriField;

	[XmlAttribute(Form = XmlSchemaForm.Qualified)]
	public string uri
	{
		get
		{
			return uriField;
		}
		set
		{
			uriField = value;
		}
	}
}
