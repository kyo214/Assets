using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/bibliography")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/bibliography", IsNullable = true)]
public class CT_NameType
{
	private List<CT_PersonType> nameListField;

	[XmlArray(Order = 0)]
	[XmlArrayItem("Person", IsNullable = false)]
	public List<CT_PersonType> NameList
	{
		get
		{
			return nameListField;
		}
		set
		{
			nameListField = value;
		}
	}

	public CT_NameType()
	{
		nameListField = new List<CT_PersonType>();
	}
}
