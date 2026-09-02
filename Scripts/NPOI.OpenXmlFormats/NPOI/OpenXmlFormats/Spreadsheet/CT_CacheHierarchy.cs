using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_CacheHierarchy
{
	private CT_FieldsUsage fieldsUsageField;

	private CT_GroupLevels groupLevelsField;

	private CT_ExtensionList extLstField;

	private string uniqueNameField;

	private string captionField;

	private bool measureField;

	private bool setField;

	private uint parentSetField;

	private bool parentSetFieldSpecified;

	private int iconSetField;

	private bool attributeField;

	private bool timeField;

	private bool keyAttributeField;

	private string defaultMemberUniqueNameField;

	private string allUniqueNameField;

	private string allCaptionField;

	private string dimensionUniqueNameField;

	private string displayFolderField;

	private string measureGroupField;

	private bool measuresField;

	private uint countField;

	private bool oneFieldField;

	private ushort memberValueDatatypeField;

	private bool memberValueDatatypeFieldSpecified;

	private bool unbalancedField;

	private bool unbalancedFieldSpecified;

	private bool unbalancedGroupField;

	private bool unbalancedGroupFieldSpecified;

	private bool hiddenField;

	[XmlElement(Order = 0)]
	public CT_FieldsUsage fieldsUsage
	{
		get
		{
			return fieldsUsageField;
		}
		set
		{
			fieldsUsageField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_GroupLevels groupLevels
	{
		get
		{
			return groupLevelsField;
		}
		set
		{
			groupLevelsField = value;
		}
	}

	[XmlElement(Order = 2)]
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
	public string uniqueName
	{
		get
		{
			return uniqueNameField;
		}
		set
		{
			uniqueNameField = value;
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
	[DefaultValue(false)]
	public bool measure
	{
		get
		{
			return measureField;
		}
		set
		{
			measureField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool set
	{
		get
		{
			return setField;
		}
		set
		{
			setField = value;
		}
	}

	[XmlAttribute]
	public uint parentSet
	{
		get
		{
			return parentSetField;
		}
		set
		{
			parentSetField = value;
		}
	}

	[XmlIgnore]
	public bool parentSetSpecified
	{
		get
		{
			return parentSetFieldSpecified;
		}
		set
		{
			parentSetFieldSpecified = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(0)]
	public int iconSet
	{
		get
		{
			return iconSetField;
		}
		set
		{
			iconSetField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool attribute
	{
		get
		{
			return attributeField;
		}
		set
		{
			attributeField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool time
	{
		get
		{
			return timeField;
		}
		set
		{
			timeField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool keyAttribute
	{
		get
		{
			return keyAttributeField;
		}
		set
		{
			keyAttributeField = value;
		}
	}

	[XmlAttribute]
	public string defaultMemberUniqueName
	{
		get
		{
			return defaultMemberUniqueNameField;
		}
		set
		{
			defaultMemberUniqueNameField = value;
		}
	}

	[XmlAttribute]
	public string allUniqueName
	{
		get
		{
			return allUniqueNameField;
		}
		set
		{
			allUniqueNameField = value;
		}
	}

	[XmlAttribute]
	public string allCaption
	{
		get
		{
			return allCaptionField;
		}
		set
		{
			allCaptionField = value;
		}
	}

	[XmlAttribute]
	public string dimensionUniqueName
	{
		get
		{
			return dimensionUniqueNameField;
		}
		set
		{
			dimensionUniqueNameField = value;
		}
	}

	[XmlAttribute]
	public string displayFolder
	{
		get
		{
			return displayFolderField;
		}
		set
		{
			displayFolderField = value;
		}
	}

	[XmlAttribute]
	public string measureGroup
	{
		get
		{
			return measureGroupField;
		}
		set
		{
			measureGroupField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool measures
	{
		get
		{
			return measuresField;
		}
		set
		{
			measuresField = value;
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

	[XmlAttribute]
	[DefaultValue(false)]
	public bool oneField
	{
		get
		{
			return oneFieldField;
		}
		set
		{
			oneFieldField = value;
		}
	}

	[XmlAttribute]
	public ushort memberValueDatatype
	{
		get
		{
			return memberValueDatatypeField;
		}
		set
		{
			memberValueDatatypeField = value;
		}
	}

	[XmlIgnore]
	public bool memberValueDatatypeSpecified
	{
		get
		{
			return memberValueDatatypeFieldSpecified;
		}
		set
		{
			memberValueDatatypeFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool unbalanced
	{
		get
		{
			return unbalancedField;
		}
		set
		{
			unbalancedField = value;
		}
	}

	[XmlIgnore]
	public bool unbalancedSpecified
	{
		get
		{
			return unbalancedFieldSpecified;
		}
		set
		{
			unbalancedFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool unbalancedGroup
	{
		get
		{
			return unbalancedGroupField;
		}
		set
		{
			unbalancedGroupField = value;
		}
	}

	[XmlIgnore]
	public bool unbalancedGroupSpecified
	{
		get
		{
			return unbalancedGroupFieldSpecified;
		}
		set
		{
			unbalancedGroupFieldSpecified = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool hidden
	{
		get
		{
			return hiddenField;
		}
		set
		{
			hiddenField = value;
		}
	}

	public static CT_CacheHierarchy Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_CacheHierarchy cT_CacheHierarchy = new CT_CacheHierarchy();
		cT_CacheHierarchy.uniqueName = XmlHelper.ReadString(node.Attributes["uniqueName"]);
		cT_CacheHierarchy.caption = XmlHelper.ReadString(node.Attributes["caption"]);
		if (node.Attributes["measure"] != null)
		{
			cT_CacheHierarchy.measure = XmlHelper.ReadBool(node.Attributes["measure"]);
		}
		if (node.Attributes["set"] != null)
		{
			cT_CacheHierarchy.set = XmlHelper.ReadBool(node.Attributes["set"]);
		}
		if (node.Attributes["parentSet"] != null)
		{
			cT_CacheHierarchy.parentSet = XmlHelper.ReadUInt(node.Attributes["parentSet"]);
		}
		if (node.Attributes["iconSet"] != null)
		{
			cT_CacheHierarchy.iconSet = XmlHelper.ReadInt(node.Attributes["iconSet"]);
		}
		if (node.Attributes["attribute"] != null)
		{
			cT_CacheHierarchy.attribute = XmlHelper.ReadBool(node.Attributes["attribute"]);
		}
		if (node.Attributes["time"] != null)
		{
			cT_CacheHierarchy.time = XmlHelper.ReadBool(node.Attributes["time"]);
		}
		if (node.Attributes["keyAttribute"] != null)
		{
			cT_CacheHierarchy.keyAttribute = XmlHelper.ReadBool(node.Attributes["keyAttribute"]);
		}
		cT_CacheHierarchy.defaultMemberUniqueName = XmlHelper.ReadString(node.Attributes["defaultMemberUniqueName"]);
		cT_CacheHierarchy.allUniqueName = XmlHelper.ReadString(node.Attributes["allUniqueName"]);
		cT_CacheHierarchy.allCaption = XmlHelper.ReadString(node.Attributes["allCaption"]);
		cT_CacheHierarchy.dimensionUniqueName = XmlHelper.ReadString(node.Attributes["dimensionUniqueName"]);
		cT_CacheHierarchy.displayFolder = XmlHelper.ReadString(node.Attributes["displayFolder"]);
		cT_CacheHierarchy.measureGroup = XmlHelper.ReadString(node.Attributes["measureGroup"]);
		if (node.Attributes["measures"] != null)
		{
			cT_CacheHierarchy.measures = XmlHelper.ReadBool(node.Attributes["measures"]);
		}
		if (node.Attributes["count"] != null)
		{
			cT_CacheHierarchy.count = XmlHelper.ReadUInt(node.Attributes["count"]);
		}
		if (node.Attributes["oneField"] != null)
		{
			cT_CacheHierarchy.oneField = XmlHelper.ReadBool(node.Attributes["oneField"]);
		}
		if (node.Attributes["memberValueDatatype"] != null)
		{
			cT_CacheHierarchy.memberValueDatatype = XmlHelper.ReadUShort(node.Attributes["memberValueDatatype"]);
		}
		if (node.Attributes["unbalanced"] != null)
		{
			cT_CacheHierarchy.unbalanced = XmlHelper.ReadBool(node.Attributes["unbalanced"]);
		}
		if (node.Attributes["unbalancedGroup"] != null)
		{
			cT_CacheHierarchy.unbalancedGroup = XmlHelper.ReadBool(node.Attributes["unbalancedGroup"]);
		}
		if (node.Attributes["hidden"] != null)
		{
			cT_CacheHierarchy.hidden = XmlHelper.ReadBool(node.Attributes["hidden"]);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "fieldsUsage")
			{
				cT_CacheHierarchy.fieldsUsage = CT_FieldsUsage.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "groupLevels")
			{
				cT_CacheHierarchy.groupLevels = CT_GroupLevels.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_CacheHierarchy.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_CacheHierarchy;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "uniqueName", uniqueName);
		XmlHelper.WriteAttribute(sw, "caption", caption);
		XmlHelper.WriteAttribute(sw, "measure", measure);
		XmlHelper.WriteAttribute(sw, "set", set);
		XmlHelper.WriteAttribute(sw, "parentSet", parentSet);
		XmlHelper.WriteAttribute(sw, "iconSet", iconSet);
		XmlHelper.WriteAttribute(sw, "attribute", attribute);
		XmlHelper.WriteAttribute(sw, "time", time);
		XmlHelper.WriteAttribute(sw, "keyAttribute", keyAttribute);
		XmlHelper.WriteAttribute(sw, "defaultMemberUniqueName", defaultMemberUniqueName);
		XmlHelper.WriteAttribute(sw, "allUniqueName", allUniqueName);
		XmlHelper.WriteAttribute(sw, "allCaption", allCaption);
		XmlHelper.WriteAttribute(sw, "dimensionUniqueName", dimensionUniqueName);
		XmlHelper.WriteAttribute(sw, "displayFolder", displayFolder);
		XmlHelper.WriteAttribute(sw, "measureGroup", measureGroup);
		XmlHelper.WriteAttribute(sw, "measures", measures);
		XmlHelper.WriteAttribute(sw, "count", count);
		XmlHelper.WriteAttribute(sw, "oneField", oneField);
		XmlHelper.WriteAttribute(sw, "memberValueDatatype", memberValueDatatype);
		XmlHelper.WriteAttribute(sw, "unbalanced", unbalanced);
		XmlHelper.WriteAttribute(sw, "unbalancedGroup", unbalancedGroup);
		XmlHelper.WriteAttribute(sw, "hidden", hidden);
		sw.Write(">");
		if (fieldsUsage != null)
		{
			fieldsUsage.Write(sw, "fieldsUsage");
		}
		if (groupLevels != null)
		{
			groupLevels.Write(sw, "groupLevels");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_CacheHierarchy()
	{
		extLstField = new CT_ExtensionList();
		groupLevelsField = new CT_GroupLevels();
		fieldsUsageField = new CT_FieldsUsage();
		measureField = false;
		setField = false;
		iconSetField = 0;
		attributeField = false;
		timeField = false;
		keyAttributeField = false;
		measuresField = false;
		oneFieldField = false;
		hiddenField = false;
	}
}
