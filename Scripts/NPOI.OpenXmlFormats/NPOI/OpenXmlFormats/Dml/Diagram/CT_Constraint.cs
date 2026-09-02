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
public class CT_Constraint
{
	private CT_OfficeArtExtensionList extLstField;

	private ST_ConstraintType typeField;

	private ST_ConstraintRelationship forField;

	private string forNameField;

	private List<ST_ElementType> ptTypeField;

	private ST_ConstraintType refTypeField;

	private ST_ConstraintRelationship refForField;

	private string refForNameField;

	private List<ST_ElementType> refPtTypeField;

	private ST_BoolOperator opField;

	private double valField;

	private double factField;

	[XmlElement(Order = 0)]
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
	public ST_ConstraintType type
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

	[XmlAttribute]
	[DefaultValue(ST_ConstraintRelationship.self)]
	public ST_ConstraintRelationship @for
	{
		get
		{
			return forField;
		}
		set
		{
			forField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue("")]
	public string forName
	{
		get
		{
			return forNameField;
		}
		set
		{
			forNameField = value;
		}
	}

	[XmlAttribute]
	public List<ST_ElementType> ptType
	{
		get
		{
			return ptTypeField;
		}
		set
		{
			ptTypeField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(ST_ConstraintType.none)]
	public ST_ConstraintType refType
	{
		get
		{
			return refTypeField;
		}
		set
		{
			refTypeField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(ST_ConstraintRelationship.self)]
	public ST_ConstraintRelationship refFor
	{
		get
		{
			return refForField;
		}
		set
		{
			refForField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue("")]
	public string refForName
	{
		get
		{
			return refForNameField;
		}
		set
		{
			refForNameField = value;
		}
	}

	[XmlAttribute]
	public List<ST_ElementType> refPtType
	{
		get
		{
			return refPtTypeField;
		}
		set
		{
			refPtTypeField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(ST_BoolOperator.none)]
	public ST_BoolOperator op
	{
		get
		{
			return opField;
		}
		set
		{
			opField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(0.0)]
	public double val
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

	[XmlAttribute]
	[DefaultValue(1.0)]
	public double fact
	{
		get
		{
			return factField;
		}
		set
		{
			factField = value;
		}
	}

	public CT_Constraint()
	{
		refPtTypeField = new List<ST_ElementType>();
		ptTypeField = new List<ST_ElementType>();
		forField = ST_ConstraintRelationship.self;
		forNameField = "";
		ptTypeField = new List<ST_ElementType>(new ST_ElementType[1]);
		refTypeField = ST_ConstraintType.none;
		refForField = ST_ConstraintRelationship.self;
		refForNameField = "";
		refPtTypeField = new List<ST_ElementType>(new ST_ElementType[1]);
		opField = ST_BoolOperator.none;
		valField = 0.0;
		factField = 1.0;
	}
}
