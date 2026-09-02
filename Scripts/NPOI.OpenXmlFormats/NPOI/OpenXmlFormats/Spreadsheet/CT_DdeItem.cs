using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_DdeItem
{
	private CT_DdeValues valuesField;

	private string nameField;

	private bool oleField;

	private bool adviseField;

	private bool preferPicField;

	public CT_DdeValues values
	{
		get
		{
			return valuesField;
		}
		set
		{
			valuesField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue("0")]
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

	[XmlAttribute]
	[DefaultValue(false)]
	public bool ole
	{
		get
		{
			return oleField;
		}
		set
		{
			oleField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool advise
	{
		get
		{
			return adviseField;
		}
		set
		{
			adviseField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool preferPic
	{
		get
		{
			return preferPicField;
		}
		set
		{
			preferPicField = value;
		}
	}

	public CT_DdeItem()
	{
		nameField = "0";
		oleField = false;
		adviseField = false;
		preferPicField = false;
	}
}
