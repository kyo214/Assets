using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_CacheField
{
	private CT_SharedItems sharedItemsField;

	private CT_FieldGroup fieldGroupField;

	private List<CT_X> mpMapField;

	private CT_ExtensionList extLstField;

	private string nameField;

	private string captionField;

	private string propertyNameField;

	private bool serverFieldField;

	private bool uniqueListField;

	private uint numFmtIdField;

	private bool numFmtIdFieldSpecified;

	private string formulaField;

	private int sqlTypeField;

	private int hierarchyField;

	private uint levelField;

	private bool databaseFieldField;

	private uint mappingCountField;

	private bool mappingCountFieldSpecified;

	private bool memberPropertyFieldField;

	[XmlElement(Order = 0)]
	public CT_SharedItems sharedItems
	{
		get
		{
			return sharedItemsField;
		}
		set
		{
			sharedItemsField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_FieldGroup fieldGroup
	{
		get
		{
			return fieldGroupField;
		}
		set
		{
			fieldGroupField = value;
		}
	}

	[XmlElement("mpMap", Order = 2)]
	public List<CT_X> mpMap
	{
		get
		{
			return mpMapField;
		}
		set
		{
			mpMapField = value;
		}
	}

	[XmlElement(Order = 3)]
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
	public string caption
	{
		get
		{
			return captionField;
		}
		set
		{
			captionField = value;
		}
	}

	[XmlAttribute]
	public string propertyName
	{
		get
		{
			return propertyNameField;
		}
		set
		{
			propertyNameField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool serverField
	{
		get
		{
			return serverFieldField;
		}
		set
		{
			serverFieldField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool uniqueList
	{
		get
		{
			return uniqueListField;
		}
		set
		{
			uniqueListField = value;
		}
	}

	[XmlAttribute]
	public uint numFmtId
	{
		get
		{
			return numFmtIdField;
		}
		set
		{
			numFmtIdField = value;
		}
	}

	[XmlIgnore]
	public bool numFmtIdSpecified
	{
		get
		{
			return numFmtIdFieldSpecified;
		}
		set
		{
			numFmtIdFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public string formula
	{
		get
		{
			return formulaField;
		}
		set
		{
			formulaField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(0)]
	public int sqlType
	{
		get
		{
			return sqlTypeField;
		}
		set
		{
			sqlTypeField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(0)]
	public int hierarchy
	{
		get
		{
			return hierarchyField;
		}
		set
		{
			hierarchyField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(uint), "0")]
	public uint level
	{
		get
		{
			return levelField;
		}
		set
		{
			levelField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool databaseField
	{
		get
		{
			return databaseFieldField;
		}
		set
		{
			databaseFieldField = value;
		}
	}

	[XmlAttribute]
	public uint mappingCount
	{
		get
		{
			return mappingCountField;
		}
		set
		{
			mappingCountField = value;
		}
	}

	[XmlIgnore]
	public bool mappingCountSpecified
	{
		get
		{
			return mappingCountFieldSpecified;
		}
		set
		{
			mappingCountFieldSpecified = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool memberPropertyField
	{
		get
		{
			return memberPropertyFieldField;
		}
		set
		{
			memberPropertyFieldField = value;
		}
	}

	public static CT_CacheField Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_CacheField cT_CacheField = new CT_CacheField();
		cT_CacheField.name = XmlHelper.ReadString(node.Attributes["name"]);
		cT_CacheField.caption = XmlHelper.ReadString(node.Attributes["caption"]);
		cT_CacheField.propertyName = XmlHelper.ReadString(node.Attributes["propertyName"]);
		if (node.Attributes["serverField"] != null)
		{
			cT_CacheField.serverField = XmlHelper.ReadBool(node.Attributes["serverField"]);
		}
		if (node.Attributes["uniqueList"] != null)
		{
			cT_CacheField.uniqueList = XmlHelper.ReadBool(node.Attributes["uniqueList"]);
		}
		if (node.Attributes["numFmtId"] != null)
		{
			cT_CacheField.numFmtId = XmlHelper.ReadUInt(node.Attributes["numFmtId"]);
		}
		cT_CacheField.formula = XmlHelper.ReadString(node.Attributes["formula"]);
		if (node.Attributes["sqlType"] != null)
		{
			cT_CacheField.sqlType = XmlHelper.ReadInt(node.Attributes["sqlType"]);
		}
		if (node.Attributes["hierarchy"] != null)
		{
			cT_CacheField.hierarchy = XmlHelper.ReadInt(node.Attributes["hierarchy"]);
		}
		if (node.Attributes["level"] != null)
		{
			cT_CacheField.level = XmlHelper.ReadUInt(node.Attributes["level"]);
		}
		if (node.Attributes["databaseField"] != null)
		{
			cT_CacheField.databaseField = XmlHelper.ReadBool(node.Attributes["databaseField"]);
		}
		if (node.Attributes["mappingCount"] != null)
		{
			cT_CacheField.mappingCount = XmlHelper.ReadUInt(node.Attributes["mappingCount"]);
		}
		if (node.Attributes["memberPropertyField"] != null)
		{
			cT_CacheField.memberPropertyField = XmlHelper.ReadBool(node.Attributes["memberPropertyField"]);
		}
		cT_CacheField.mpMap = new List<CT_X>();
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "sharedItems")
			{
				cT_CacheField.sharedItems = CT_SharedItems.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "fieldGroup")
			{
				cT_CacheField.fieldGroup = CT_FieldGroup.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_CacheField.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "mpMap")
			{
				cT_CacheField.mpMap.Add(CT_X.Parse(childNode, namespaceManager));
			}
		}
		return cT_CacheField;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "caption", caption);
		XmlHelper.WriteAttribute(sw, "propertyName", propertyName);
		XmlHelper.WriteAttribute(sw, "serverField", serverField);
		XmlHelper.WriteAttribute(sw, "uniqueList", uniqueList);
		XmlHelper.WriteAttribute(sw, "numFmtId", numFmtId);
		XmlHelper.WriteAttribute(sw, "formula", formula);
		XmlHelper.WriteAttribute(sw, "sqlType", sqlType);
		XmlHelper.WriteAttribute(sw, "hierarchy", hierarchy);
		XmlHelper.WriteAttribute(sw, "level", level);
		XmlHelper.WriteAttribute(sw, "databaseField", databaseField);
		XmlHelper.WriteAttribute(sw, "mappingCount", mappingCount);
		XmlHelper.WriteAttribute(sw, "memberPropertyField", memberPropertyField);
		sw.Write(">");
		if (sharedItems != null)
		{
			sharedItems.Write(sw, "sharedItems");
		}
		if (fieldGroup != null)
		{
			fieldGroup.Write(sw, "fieldGroup");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		if (mpMap != null)
		{
			foreach (CT_X item in mpMap)
			{
				item.Write(sw, "mpMap");
			}
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_CacheField()
	{
		extLstField = new CT_ExtensionList();
		mpMapField = new List<CT_X>();
		fieldGroupField = new CT_FieldGroup();
		sharedItemsField = new CT_SharedItems();
		serverFieldField = false;
		uniqueListField = true;
		sqlTypeField = 0;
		hierarchyField = 0;
		levelField = 0u;
		databaseFieldField = true;
		memberPropertyFieldField = false;
	}

	public CT_SharedItems AddNewSharedItems()
	{
		sharedItemsField = new CT_SharedItems();
		return sharedItemsField;
	}
}
