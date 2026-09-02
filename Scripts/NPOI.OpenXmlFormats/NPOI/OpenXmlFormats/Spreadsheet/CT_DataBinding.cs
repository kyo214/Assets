using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[Serializable]
[DebuggerStepThrough]
[DesignerCategory("code")]
[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
public class CT_DataBinding
{
	private XmlElement anyField;

	private string dataBindingNameField;

	private bool? fileBindingField;

	private uint? connectionIDField;

	private string fileBindingNameField;

	private uint dataBindingLoadModeField;

	[XmlAnyElement]
	public XmlElement Any
	{
		get
		{
			return anyField;
		}
		set
		{
			anyField = value;
		}
	}

	[XmlAttribute]
	public string DataBindingName
	{
		get
		{
			return dataBindingNameField;
		}
		set
		{
			dataBindingNameField = value;
		}
	}

	[XmlIgnore]
	public bool outlineSpecified => dataBindingNameField != null;

	[XmlAttribute]
	public bool FileBinding
	{
		get
		{
			if (fileBindingField.HasValue)
			{
				return fileBindingField.Value;
			}
			return false;
		}
		set
		{
			fileBindingField = value;
		}
	}

	[XmlIgnore]
	public bool FileBindingSpecified => fileBindingField.HasValue;

	[XmlAttribute]
	public uint ConnectionID
	{
		get
		{
			if (connectionIDField.HasValue)
			{
				return connectionIDField.Value;
			}
			return 0u;
		}
		set
		{
			connectionIDField = value;
		}
	}

	[XmlIgnore]
	public bool ConnectionIDSpecified => connectionIDField.HasValue;

	[XmlAttribute]
	public string FileBindingName
	{
		get
		{
			return fileBindingNameField;
		}
		set
		{
			fileBindingNameField = value;
		}
	}

	[XmlIgnore]
	public bool FileBindingNameSpecified => fileBindingNameField != null;

	[XmlAttribute]
	public uint DataBindingLoadMode
	{
		get
		{
			return dataBindingLoadModeField;
		}
		set
		{
			dataBindingLoadModeField = value;
		}
	}
}
