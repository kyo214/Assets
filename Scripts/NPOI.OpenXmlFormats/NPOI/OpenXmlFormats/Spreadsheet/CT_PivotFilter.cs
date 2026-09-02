using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_PivotFilter
{
	private CT_AutoFilter autoFilterField;

	private CT_ExtensionList extLstField;

	private uint fldField;

	private uint mpFldField;

	private bool mpFldFieldSpecified;

	private ST_PivotFilterType typeField;

	private int evalOrderField;

	private uint idField;

	private uint iMeasureHierField;

	private bool iMeasureHierFieldSpecified;

	private uint iMeasureFldField;

	private bool iMeasureFldFieldSpecified;

	private string nameField;

	private string descriptionField;

	private string stringValue1Field;

	private string stringValue2Field;

	[XmlElement(Order = 0)]
	public CT_AutoFilter autoFilter
	{
		get
		{
			return autoFilterField;
		}
		set
		{
			autoFilterField = value;
		}
	}

	[XmlElement(Order = 1)]
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
	public uint fld
	{
		get
		{
			return fldField;
		}
		set
		{
			fldField = value;
		}
	}

	[XmlAttribute]
	public uint mpFld
	{
		get
		{
			return mpFldField;
		}
		set
		{
			mpFldField = value;
		}
	}

	[XmlIgnore]
	public bool mpFldSpecified
	{
		get
		{
			return mpFldFieldSpecified;
		}
		set
		{
			mpFldFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public ST_PivotFilterType type
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
	[DefaultValue(0)]
	public int evalOrder
	{
		get
		{
			return evalOrderField;
		}
		set
		{
			evalOrderField = value;
		}
	}

	[XmlAttribute]
	public uint id
	{
		get
		{
			return idField;
		}
		set
		{
			idField = value;
		}
	}

	[XmlAttribute]
	public uint iMeasureHier
	{
		get
		{
			return iMeasureHierField;
		}
		set
		{
			iMeasureHierField = value;
		}
	}

	[XmlIgnore]
	public bool iMeasureHierSpecified
	{
		get
		{
			return iMeasureHierFieldSpecified;
		}
		set
		{
			iMeasureHierFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint iMeasureFld
	{
		get
		{
			return iMeasureFldField;
		}
		set
		{
			iMeasureFldField = value;
		}
	}

	[XmlIgnore]
	public bool iMeasureFldSpecified
	{
		get
		{
			return iMeasureFldFieldSpecified;
		}
		set
		{
			iMeasureFldFieldSpecified = value;
		}
	}

	[XmlAttribute]
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
	public string description
	{
		get
		{
			return descriptionField;
		}
		set
		{
			descriptionField = value;
		}
	}

	[XmlAttribute]
	public string stringValue1
	{
		get
		{
			return stringValue1Field;
		}
		set
		{
			stringValue1Field = value;
		}
	}

	[XmlAttribute]
	public string stringValue2
	{
		get
		{
			return stringValue2Field;
		}
		set
		{
			stringValue2Field = value;
		}
	}

	public static CT_PivotFilter Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PivotFilter cT_PivotFilter = new CT_PivotFilter();
		if (node.Attributes["fld"] != null)
		{
			cT_PivotFilter.fld = XmlHelper.ReadUInt(node.Attributes["fld"]);
		}
		if (node.Attributes["mpFld"] != null)
		{
			cT_PivotFilter.mpFld = XmlHelper.ReadUInt(node.Attributes["mpFld"]);
		}
		if (node.Attributes["type"] != null)
		{
			cT_PivotFilter.type = (ST_PivotFilterType)Enum.Parse(typeof(ST_PivotFilterType), node.Attributes["type"].Value);
		}
		if (node.Attributes["evalOrder"] != null)
		{
			cT_PivotFilter.evalOrder = XmlHelper.ReadInt(node.Attributes["evalOrder"]);
		}
		if (node.Attributes["id"] != null)
		{
			cT_PivotFilter.id = XmlHelper.ReadUInt(node.Attributes["id"]);
		}
		if (node.Attributes["iMeasureHier"] != null)
		{
			cT_PivotFilter.iMeasureHier = XmlHelper.ReadUInt(node.Attributes["iMeasureHier"]);
		}
		if (node.Attributes["iMeasureFld"] != null)
		{
			cT_PivotFilter.iMeasureFld = XmlHelper.ReadUInt(node.Attributes["iMeasureFld"]);
		}
		cT_PivotFilter.name = XmlHelper.ReadString(node.Attributes["name"]);
		cT_PivotFilter.description = XmlHelper.ReadString(node.Attributes["description"]);
		cT_PivotFilter.stringValue1 = XmlHelper.ReadString(node.Attributes["stringValue1"]);
		cT_PivotFilter.stringValue2 = XmlHelper.ReadString(node.Attributes["stringValue2"]);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "autoFilter")
			{
				cT_PivotFilter.autoFilter = CT_AutoFilter.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_PivotFilter.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_PivotFilter;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "fld", fld);
		XmlHelper.WriteAttribute(sw, "mpFld", mpFld);
		XmlHelper.WriteAttribute(sw, "type", type.ToString());
		XmlHelper.WriteAttribute(sw, "evalOrder", evalOrder);
		XmlHelper.WriteAttribute(sw, "id", id);
		XmlHelper.WriteAttribute(sw, "iMeasureHier", iMeasureHier);
		XmlHelper.WriteAttribute(sw, "iMeasureFld", iMeasureFld);
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "description", description);
		XmlHelper.WriteAttribute(sw, "stringValue1", stringValue1);
		XmlHelper.WriteAttribute(sw, "stringValue2", stringValue2);
		sw.Write(">");
		if (autoFilter != null)
		{
			autoFilter.Write(sw, "autoFilter");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_PivotFilter()
	{
		extLstField = new CT_ExtensionList();
		autoFilterField = new CT_AutoFilter();
		evalOrderField = 0;
	}
}
