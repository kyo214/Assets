using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_TextFields
{
	private CT_TextField[] textFieldField;

	private uint countField;

	[XmlElement("textField")]
	public CT_TextField[] textField
	{
		get
		{
			return textFieldField;
		}
		set
		{
			textFieldField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(uint), "1")]
	public uint count
	{
		get
		{
			return countField;
		}
		set
		{
			countField = value;
		}
	}

	public CT_TextFields()
	{
		countField = 1u;
	}
}
