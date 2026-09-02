using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/bibliography")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/bibliography", IsNullable = true)]
public class CT_NameOrCorporateType
{
	private object itemField;

	[XmlElement("Corporate", typeof(string))]
	[XmlElement("NameList", typeof(CT_NameListType))]
	public object Item
	{
		get
		{
			return itemField;
		}
		set
		{
			itemField = value;
		}
	}
}
