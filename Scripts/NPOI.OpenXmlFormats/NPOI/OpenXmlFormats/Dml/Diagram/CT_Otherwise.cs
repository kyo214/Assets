using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Diagram;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram", IsNullable = true)]
public class CT_Otherwise
{
	private List<object> itemsField;

	private string nameField;

	[XmlElement("alg", typeof(CT_Algorithm), Order = 0)]
	[XmlElement("choose", typeof(CT_Choose), Order = 0)]
	[XmlElement("constrLst", typeof(CT_Constraints), Order = 0)]
	[XmlElement("extLst", typeof(CT_OfficeArtExtensionList), Order = 0)]
	[XmlElement("forEach", typeof(CT_ForEach), Order = 0)]
	[XmlElement("layoutNode", typeof(CT_LayoutNode), Order = 0)]
	[XmlElement("presOf", typeof(CT_PresentationOf), Order = 0)]
	[XmlElement("ruleLst", typeof(CT_Rules), Order = 0)]
	[XmlElement("shape", typeof(CT_Shape), Order = 0)]
	public List<object> Items
	{
		get
		{
			return itemsField;
		}
		set
		{
			itemsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue("")]
	public string name
	{
		get
		{
			return nameField;
		}
		set
		{
			nameField = value;
		}
	}

	public CT_Otherwise()
	{
		itemsField = new List<object>();
		nameField = "";
	}
}
