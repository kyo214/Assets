using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Dml.Diagram;

[Serializable]
[GeneratedCode("System.Xml", "4.0.30319.17379")]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram")]
[XmlRoot("colorsDef", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram", IsNullable = false)]
public class CT_ColorTransform
{
	private List<CT_CTName> titleField;

	private List<CT_CTDescription> descField;

	private List<CT_CTCategory> catLstField;

	private List<CT_CTStyleLabel> styleLblField;

	private CT_OfficeArtExtensionList extLstField;

	private string uniqueIdField;

	private string minVerField;

	[XmlElement("title", Order = 0)]
	public List<CT_CTName> title
	{
		get
		{
			return titleField;
		}
		set
		{
			titleField = value;
		}
	}

	[XmlElement("desc", Order = 1)]
	public List<CT_CTDescription> desc
	{
		get
		{
			return descField;
		}
		set
		{
			descField = value;
		}
	}

	[XmlArray(Order = 2)]
	[XmlArrayItem("cat", IsNullable = false)]
	public List<CT_CTCategory> catLst
	{
		get
		{
			return catLstField;
		}
		set
		{
			catLstField = value;
		}
	}

	[XmlElement("styleLbl", Order = 3)]
	public List<CT_CTStyleLabel> styleLbl
	{
		get
		{
			return styleLblField;
		}
		set
		{
			styleLblField = value;
		}
	}

	[XmlElement(Order = 4)]
	public CT_OfficeArtExtensionList extLst
	{
		get
		{
			return extLstField;
		}
		set
		{
			extLstField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue("")]
	public string uniqueId
	{
		get
		{
			return uniqueIdField;
		}
		set
		{
			uniqueIdField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue("http://schemas.openxmlformats.org/drawingml/2006/diagram")]
	public string minVer
	{
		get
		{
			return minVerField;
		}
		set
		{
			minVerField = value;
		}
	}

	public CT_ColorTransform()
	{
		extLstField = new CT_OfficeArtExtensionList();
		styleLblField = new List<CT_CTStyleLabel>();
		catLstField = new List<CT_CTCategory>();
		descField = new List<CT_CTDescription>();
		titleField = new List<CT_CTName>();
		uniqueIdField = "";
		minVerField = "http://schemas.openxmlformats.org/drawingml/2006/diagram";
	}
}
