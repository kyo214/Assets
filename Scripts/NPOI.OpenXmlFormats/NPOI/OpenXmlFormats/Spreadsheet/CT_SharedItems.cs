using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_SharedItems
{
	private List<object> itemsField;

	private bool containsSemiMixedTypesField;

	private bool containsNonDateField;

	private bool containsDateField;

	private bool containsStringField;

	private bool containsBlankField;

	private bool containsMixedTypesField;

	private bool containsNumberField;

	private bool containsIntegerField;

	private double minValueField;

	private bool minValueFieldSpecified;

	private double maxValueField;

	private bool maxValueFieldSpecified;

	private DateTime? minDateField;

	private bool minDateFieldSpecified;

	private DateTime? maxDateField;

	private bool maxDateFieldSpecified;

	private uint countField;

	private bool countFieldSpecified;

	private bool longTextField;

	[XmlElement("b", typeof(CT_Boolean), Order = 0)]
	[XmlElement("d", typeof(CT_DateTime), Order = 0)]
	[XmlElement("e", typeof(CT_Error), Order = 0)]
	[XmlElement("m", typeof(CT_Missing), Order = 0)]
	[XmlElement("n", typeof(CT_Number), Order = 0)]
	[XmlElement("s", typeof(CT_String), Order = 0)]
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
	[DefaultValue(true)]
	public bool containsSemiMixedTypes
	{
		get
		{
			return containsSemiMixedTypesField;
		}
		set
		{
			containsSemiMixedTypesField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool containsNonDate
	{
		get
		{
			return containsNonDateField;
		}
		set
		{
			containsNonDateField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool containsDate
	{
		get
		{
			return containsDateField;
		}
		set
		{
			containsDateField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool containsString
	{
		get
		{
			return containsStringField;
		}
		set
		{
			containsStringField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool containsBlank
	{
		get
		{
			return containsBlankField;
		}
		set
		{
			containsBlankField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool containsMixedTypes
	{
		get
		{
			return containsMixedTypesField;
		}
		set
		{
			containsMixedTypesField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool containsNumber
	{
		get
		{
			return containsNumberField;
		}
		set
		{
			containsNumberField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool containsInteger
	{
		get
		{
			return containsIntegerField;
		}
		set
		{
			containsIntegerField = value;
		}
	}

	[XmlAttribute]
	public double minValue
	{
		get
		{
			return minValueField;
		}
		set
		{
			minValueField = value;
		}
	}

	[XmlIgnore]
	public bool minValueSpecified
	{
		get
		{
			return minValueFieldSpecified;
		}
		set
		{
			minValueFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public double maxValue
	{
		get
		{
			return maxValueField;
		}
		set
		{
			maxValueField = value;
		}
	}

	[XmlIgnore]
	public bool maxValueSpecified
	{
		get
		{
			return maxValueFieldSpecified;
		}
		set
		{
			maxValueFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public DateTime? minDate
	{
		get
		{
			return minDateField;
		}
		set
		{
			minDateField = value;
		}
	}

	[XmlIgnore]
	public bool minDateSpecified
	{
		get
		{
			return minDateFieldSpecified;
		}
		set
		{
			minDateFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public DateTime? maxDate
	{
		get
		{
			return maxDateField;
		}
		set
		{
			maxDateField = value;
		}
	}

	[XmlIgnore]
	public bool maxDateSpecified
	{
		get
		{
			return maxDateFieldSpecified;
		}
		set
		{
			maxDateFieldSpecified = value;
		}
	}

	[XmlAttribute]
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

	[XmlIgnore]
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
	[DefaultValue(false)]
	public bool longText
	{
		get
		{
			return longTextField;
		}
		set
		{
			longTextField = value;
		}
	}

	public static CT_SharedItems Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_SharedItems cT_SharedItems = new CT_SharedItems();
		if (node.Attributes["containsSemiMixedTypes"] != null)
		{
			cT_SharedItems.containsSemiMixedTypes = XmlHelper.ReadBool(node.Attributes["containsSemiMixedTypes"]);
		}
		if (node.Attributes["containsNonDate"] != null)
		{
			cT_SharedItems.containsNonDate = XmlHelper.ReadBool(node.Attributes["containsNonDate"]);
		}
		if (node.Attributes["containsDate"] != null)
		{
			cT_SharedItems.containsDate = XmlHelper.ReadBool(node.Attributes["containsDate"]);
		}
		if (node.Attributes["containsString"] != null)
		{
			cT_SharedItems.containsString = XmlHelper.ReadBool(node.Attributes["containsString"]);
		}
		if (node.Attributes["containsBlank"] != null)
		{
			cT_SharedItems.containsBlank = XmlHelper.ReadBool(node.Attributes["containsBlank"]);
		}
		if (node.Attributes["containsMixedTypes"] != null)
		{
			cT_SharedItems.containsMixedTypes = XmlHelper.ReadBool(node.Attributes["containsMixedTypes"]);
		}
		if (node.Attributes["containsNumber"] != null)
		{
			cT_SharedItems.containsNumber = XmlHelper.ReadBool(node.Attributes["containsNumber"]);
		}
		if (node.Attributes["containsInteger"] != null)
		{
			cT_SharedItems.containsInteger = XmlHelper.ReadBool(node.Attributes["containsInteger"]);
		}
		if (node.Attributes["minValue"] != null)
		{
			cT_SharedItems.minValue = XmlHelper.ReadDouble(node.Attributes["minValue"]);
		}
		if (node.Attributes["maxValue"] != null)
		{
			cT_SharedItems.maxValue = XmlHelper.ReadDouble(node.Attributes["maxValue"]);
		}
		if (node.Attributes["minDate"] != null)
		{
			cT_SharedItems.minDate = XmlHelper.ReadDateTime(node.Attributes["minDate"]);
		}
		if (node.Attributes["maxDate"] != null)
		{
			cT_SharedItems.maxDate = XmlHelper.ReadDateTime(node.Attributes["maxDate"]);
		}
		if (node.Attributes["count"] != null)
		{
			cT_SharedItems.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		if (node.Attributes["longText"] != null)
		{
			cT_SharedItems.longText = XmlHelper.ReadBool(node.Attributes["longText"]);
		}
		cT_SharedItems.Items = new List<object>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "n")
			{
				cT_SharedItems.Items.Add(CT_Number.Parse(childNode, namespaceManager));
			}
			else if (childNode.LocalName == "b")
			{
				cT_SharedItems.Items.Add(CT_Boolean.Parse(childNode, namespaceManager));
			}
			else if (childNode.LocalName == "d")
			{
				cT_SharedItems.Items.Add(CT_DateTime.Parse(childNode, namespaceManager));
			}
			else if (childNode.LocalName == "e")
			{
				cT_SharedItems.Items.Add(CT_Error.Parse(childNode, namespaceManager));
			}
			else if (childNode.LocalName == "m")
			{
				cT_SharedItems.Items.Add(CT_Missing.Parse(childNode, namespaceManager));
			}
			else if (childNode.LocalName == "s")
			{
				cT_SharedItems.Items.Add(CT_String.Parse(childNode, namespaceManager));
			}
		}
		return cT_SharedItems;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "containsSemiMixedTypes", containsSemiMixedTypes);
		XmlHelper.WriteAttribute(sw, "containsNonDate", containsNonDate);
		XmlHelper.WriteAttribute(sw, "containsDate", containsDate);
		XmlHelper.WriteAttribute(sw, "containsString", containsString);
		XmlHelper.WriteAttribute(sw, "containsBlank", containsBlank);
		XmlHelper.WriteAttribute(sw, "containsMixedTypes", containsMixedTypes);
		XmlHelper.WriteAttribute(sw, "containsNumber", containsNumber);
		XmlHelper.WriteAttribute(sw, "containsInteger", containsInteger);
		XmlHelper.WriteAttribute(sw, "minValue", minValue);
		XmlHelper.WriteAttribute(sw, "maxValue", maxValue);
		XmlHelper.WriteAttribute(sw, "minDate", minDate);
		XmlHelper.WriteAttribute(sw, "maxDate", maxDate);
		XmlHelper.WriteAttribute(sw, "count", count);
		XmlHelper.WriteAttribute(sw, "longText", longText);
		sw.Write(">");
		foreach (object item in Items)
		{
			if (item is CT_Number)
			{
				((CT_Number)item).Write(sw, "n");
			}
			else if (item is CT_Boolean)
			{
				((CT_Boolean)item).Write(sw, "b");
			}
			else if (item is CT_DateTime)
			{
				((CT_DateTime)item).Write(sw, "d");
			}
			else if (item is CT_Error)
			{
				((CT_Error)item).Write(sw, "e");
			}
			else if (item is CT_Missing)
			{
				((CT_Missing)item).Write(sw, "m");
			}
			else if (item is CT_String)
			{
				((CT_String)item).Write(sw, "s");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_SharedItems()
	{
		itemsField = new List<object>();
		containsSemiMixedTypesField = true;
		containsNonDateField = true;
		containsDateField = false;
		containsStringField = true;
		containsBlankField = false;
		containsMixedTypesField = false;
		containsNumberField = false;
		containsIntegerField = false;
		longTextField = false;
	}
}
