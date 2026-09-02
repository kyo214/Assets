using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Diagram;

[Serializable]
[GeneratedCode("System.Xml", "4.0.30319.17379")]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram", IsNullable = true)]
public class CT_CTDescription
{
	private string langField;

	private string valField;

	[XmlAttribute]
	[DefaultValue("")]
	public string lang
	{
		get
		{
			return langField;
		}
		set
		{
			langField = value;
		}
	}

	[XmlAttribute]
	public string val
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

	public CT_CTDescription()
	{
		langField = "";
	}
}
