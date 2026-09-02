using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(ElementName = "sst", Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_Sst
{
	private List<CT_Rst> siField;

	private CT_ExtensionList extLstField;

	private int countField;

	private bool countFieldSpecified;

	private int uniqueCountField;

	private bool uniqueCountFieldSpecified;

	[XmlElement]
	public List<CT_Rst> si
	{
		get
		{
			return siField;
		}
		set
		{
			siField = value;
		}
	}

	[XmlElement]
	public CT_ExtensionList extLst
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
	public int count
	{
		get
		{
			return countField;
		}
		set
		{
			countField = value;
			countFieldSpecified = true;
		}
	}

	public bool countSpecified
	{
		get
		{
			return countFieldSpecified;
		}
		set
		{
			countFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public int uniqueCount
	{
		get
		{
			return uniqueCountField;
		}
		set
		{
			uniqueCountField = value;
			uniqueCountFieldSpecified = true;
		}
	}

	public bool uniqueCountSpecified
	{
		get
		{
			return uniqueCountFieldSpecified;
		}
		set
		{
			uniqueCountFieldSpecified = value;
		}
	}

	public CT_Sst()
	{
		siField = new List<CT_Rst>();
	}
}
