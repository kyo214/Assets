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
[XmlRoot("colorsDefHdr", Namespace = "http://schemas.openxmlformats.org/drawingml/2006/diagram", IsNullable = false)]
public class CT_ColorTransformHeader
{
	private List<CT_CTName> titleField;

	private List<CT_CTDescription> descField;

	private List<CT_CTCategory> catLstField;

	private CT_OfficeArtExtensionList extLstField;

	private string uniqueIdField;

	private string minVerField;

	private int resIdField;

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

	[XmlElement(Order = 3)]
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

	[XmlAttribute]
	[DefaultValue(0)]
	public int resId
	{
		get
		{
			return resIdField;
		}
		set
		{
			resIdField = value;
		}
	}

	public CT_ColorTransformHeader()
	{
		catLstField = new List<CT_CTCategory>();
		descField = new List<CT_CTDescription>();
		titleField = new List<CT_CTName>();
		minVerField = "http://schemas.openxmlformats.org/drawingml/2006/diagram";
		resIdField = 0;
	}
}
