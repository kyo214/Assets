using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot("pivotCacheDefinition", Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = false)]
public class CT_PivotCacheDefinition
{
	private CT_CacheSource cacheSourceField;

	private CT_CacheFields cacheFieldsField;

	private CT_CacheHierarchies cacheHierarchiesField;

	private CT_PCDKPIs kpisField;

	private CT_TupleCache tupleCacheField;

	private CT_CalculatedItems calculatedItemsField;

	private CT_CalculatedMembers calculatedMembersField;

	private CT_Dimensions dimensionsField;

	private CT_MeasureGroups measureGroupsField;

	private CT_MeasureDimensionMaps mapsField;

	private CT_ExtensionList extLstField;

	private string idField;

	private bool invalidField;

	private bool saveDataField;

	private bool refreshOnLoadField;

	private bool optimizeMemoryField;

	private bool enableRefreshField;

	private string refreshedByField;

	private double refreshedDateField;

	private bool refreshedDateFieldSpecified;

	private DateTime? refreshedDateIsoField;

	private bool refreshedDateIsoFieldSpecified;

	private bool backgroundQueryField;

	private uint missingItemsLimitField;

	private bool missingItemsLimitFieldSpecified;

	private byte createdVersionField;

	private byte refreshedVersionField;

	private byte minRefreshableVersionField;

	private uint recordCountField;

	private bool recordCountFieldSpecified;

	private bool upgradeOnRefreshField;

	private bool tupleCache1Field;

	private bool supportSubqueryField;

	private bool supportAdvancedDrillField;

	[XmlElement(Order = 0)]
	public CT_CacheSource cacheSource
	{
		get
		{
			return cacheSourceField;
		}
		set
		{
			cacheSourceField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_CacheFields cacheFields
	{
		get
		{
			return cacheFieldsField;
		}
		set
		{
			cacheFieldsField = value;
		}
	}

	[XmlElement(Order = 2)]
	public CT_CacheHierarchies cacheHierarchies
	{
		get
		{
			return cacheHierarchiesField;
		}
		set
		{
			cacheHierarchiesField = value;
		}
	}

	[XmlElement(Order = 3)]
	public CT_PCDKPIs kpis
	{
		get
		{
			return kpisField;
		}
		set
		{
			kpisField = value;
		}
	}

	[XmlElement(Order = 4)]
	public CT_TupleCache tupleCache
	{
		get
		{
			return tupleCacheField;
		}
		set
		{
			tupleCacheField = value;
		}
	}

	[XmlElement(Order = 5)]
	public CT_CalculatedItems calculatedItems
	{
		get
		{
			return calculatedItemsField;
		}
		set
		{
			calculatedItemsField = value;
		}
	}

	[XmlElement(Order = 6)]
	public CT_CalculatedMembers calculatedMembers
	{
		get
		{
			return calculatedMembersField;
		}
		set
		{
			calculatedMembersField = value;
		}
	}

	[XmlElement(Order = 7)]
	public CT_Dimensions dimensions
	{
		get
		{
			return dimensionsField;
		}
		set
		{
			dimensionsField = value;
		}
	}

	[XmlElement(Order = 8)]
	public CT_MeasureGroups measureGroups
	{
		get
		{
			return measureGroupsField;
		}
		set
		{
			measureGroupsField = value;
		}
	}

	[XmlElement(Order = 9)]
	public CT_MeasureDimensionMaps maps
	{
		get
		{
			return mapsField;
		}
		set
		{
			mapsField = value;
		}
	}

	[XmlElement(Order = 10)]
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

	[XmlAttribute(Form = XmlSchemaForm.Qualified, Namespace = "http://schemas.openxmlformats.org/officeDocument/2006/relationships")]
	public string id
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
	[DefaultValue(false)]
	public bool invalid
	{
		get
		{
			return invalidField;
		}
		set
		{
			invalidField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool saveData
	{
		get
		{
			return saveDataField;
		}
		set
		{
			saveDataField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool refreshOnLoad
	{
		get
		{
			return refreshOnLoadField;
		}
		set
		{
			refreshOnLoadField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool optimizeMemory
	{
		get
		{
			return optimizeMemoryField;
		}
		set
		{
			optimizeMemoryField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool enableRefresh
	{
		get
		{
			return enableRefreshField;
		}
		set
		{
			enableRefreshField = value;
		}
	}

	[XmlAttribute]
	public string refreshedBy
	{
		get
		{
			return refreshedByField;
		}
		set
		{
			refreshedByField = value;
		}
	}

	[XmlAttribute]
	public double refreshedDate
	{
		get
		{
			return refreshedDateField;
		}
		set
		{
			refreshedDateField = value;
		}
	}

	[XmlIgnore]
	public bool refreshedDateSpecified
	{
		get
		{
			return refreshedDateFieldSpecified;
		}
		set
		{
			refreshedDateFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public DateTime? refreshedDateIso
	{
		get
		{
			return refreshedDateIsoField;
		}
		set
		{
			refreshedDateIsoField = value;
		}
	}

	[XmlIgnore]
	public bool refreshedDateIsoSpecified
	{
		get
		{
			return refreshedDateIsoFieldSpecified;
		}
		set
		{
			refreshedDateIsoFieldSpecified = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool backgroundQuery
	{
		get
		{
			return backgroundQueryField;
		}
		set
		{
			backgroundQueryField = value;
		}
	}

	[XmlAttribute]
	public uint missingItemsLimit
	{
		get
		{
			return missingItemsLimitField;
		}
		set
		{
			missingItemsLimitField = value;
		}
	}

	[XmlIgnore]
	public bool missingItemsLimitSpecified
	{
		get
		{
			return missingItemsLimitFieldSpecified;
		}
		set
		{
			missingItemsLimitFieldSpecified = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(byte), "0")]
	public byte createdVersion
	{
		get
		{
			return createdVersionField;
		}
		set
		{
			createdVersionField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(byte), "0")]
	public byte refreshedVersion
	{
		get
		{
			return refreshedVersionField;
		}
		set
		{
			refreshedVersionField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(byte), "0")]
	public byte minRefreshableVersion
	{
		get
		{
			return minRefreshableVersionField;
		}
		set
		{
			minRefreshableVersionField = value;
		}
	}

	[XmlAttribute]
	public uint recordCount
	{
		get
		{
			return recordCountField;
		}
		set
		{
			recordCountField = value;
		}
	}

	[XmlIgnore]
	public bool recordCountSpecified
	{
		get
		{
			return recordCountFieldSpecified;
		}
		set
		{
			recordCountFieldSpecified = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool upgradeOnRefresh
	{
		get
		{
			return upgradeOnRefreshField;
		}
		set
		{
			upgradeOnRefreshField = value;
		}
	}

	[XmlAttribute("tupleCache")]
	[DefaultValue(false)]
	public bool tupleCache1
	{
		get
		{
			return tupleCache1Field;
		}
		set
		{
			tupleCache1Field = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool supportSubquery
	{
		get
		{
			return supportSubqueryField;
		}
		set
		{
			supportSubqueryField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool supportAdvancedDrill
	{
		get
		{
			return supportAdvancedDrillField;
		}
		set
		{
			supportAdvancedDrillField = value;
		}
	}

	public static CT_PivotCacheDefinition Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PivotCacheDefinition cT_PivotCacheDefinition = new CT_PivotCacheDefinition();
		cT_PivotCacheDefinition.id = XmlHelper.ReadString(node.Attributes["r:id"]);
		if (node.Attributes["invalid"] != null)
		{
			cT_PivotCacheDefinition.invalid = XmlHelper.ReadBool(node.Attributes["invalid"]);
		}
		if (node.Attributes["saveData"] != null)
		{
			cT_PivotCacheDefinition.saveData = XmlHelper.ReadBool(node.Attributes["saveData"]);
		}
		if (node.Attributes["refreshOnLoad"] != null)
		{
			cT_PivotCacheDefinition.refreshOnLoad = XmlHelper.ReadBool(node.Attributes["refreshOnLoad"]);
		}
		if (node.Attributes["optimizeMemory"] != null)
		{
			cT_PivotCacheDefinition.optimizeMemory = XmlHelper.ReadBool(node.Attributes["optimizeMemory"]);
		}
		if (node.Attributes["enableRefresh"] != null)
		{
			cT_PivotCacheDefinition.enableRefresh = XmlHelper.ReadBool(node.Attributes["enableRefresh"]);
		}
		cT_PivotCacheDefinition.refreshedBy = XmlHelper.ReadString(node.Attributes["refreshedBy"]);
		if (node.Attributes["refreshedDate"] != null)
		{
			cT_PivotCacheDefinition.refreshedDate = XmlHelper.ReadDouble(node.Attributes["refreshedDate"]);
		}
		if (node.Attributes["refreshedDateIso"] != null && node.Attributes["backgroundQuery"] != null)
		{
			cT_PivotCacheDefinition.backgroundQuery = XmlHelper.ReadBool(node.Attributes["backgroundQuery"]);
		}
		if (node.Attributes["missingItemsLimit"] != null)
		{
			cT_PivotCacheDefinition.missingItemsLimit = XmlHelper.ReadUInt(node.Attributes["missingItemsLimit"]);
		}
		if (node.Attributes["createdVersion"] != null)
		{
			cT_PivotCacheDefinition.createdVersion = XmlHelper.ReadByte(node.Attributes["createdVersion"]);
		}
		if (node.Attributes["refreshedVersion"] != null)
		{
			cT_PivotCacheDefinition.refreshedVersion = XmlHelper.ReadByte(node.Attributes["refreshedVersion"]);
		}
		if (node.Attributes["minRefreshableVersion"] != null)
		{
			cT_PivotCacheDefinition.minRefreshableVersion = XmlHelper.ReadByte(node.Attributes["minRefreshableVersion"]);
		}
		if (node.Attributes["recordCount"] != null)
		{
			cT_PivotCacheDefinition.recordCount = XmlHelper.ReadUInt(node.Attributes["recordCount"]);
		}
		if (node.Attributes["upgradeOnRefresh"] != null)
		{
			cT_PivotCacheDefinition.upgradeOnRefresh = XmlHelper.ReadBool(node.Attributes["upgradeOnRefresh"]);
		}
		if (node.Attributes["tupleCache1"] != null)
		{
			cT_PivotCacheDefinition.tupleCache1 = XmlHelper.ReadBool(node.Attributes["tupleCache1"]);
		}
		if (node.Attributes["supportSubquery"] != null)
		{
			cT_PivotCacheDefinition.supportSubquery = XmlHelper.ReadBool(node.Attributes["supportSubquery"]);
		}
		if (node.Attributes["supportAdvancedDrill"] != null)
		{
			cT_PivotCacheDefinition.supportAdvancedDrill = XmlHelper.ReadBool(node.Attributes["supportAdvancedDrill"]);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "cacheSource")
			{
				cT_PivotCacheDefinition.cacheSource = CT_CacheSource.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "cacheFields")
			{
				cT_PivotCacheDefinition.cacheFields = CT_CacheFields.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "cacheHierarchies")
			{
				cT_PivotCacheDefinition.cacheHierarchies = CT_CacheHierarchies.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "kpis")
			{
				cT_PivotCacheDefinition.kpis = CT_PCDKPIs.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "tupleCache")
			{
				cT_PivotCacheDefinition.tupleCache = CT_TupleCache.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "calculatedItems")
			{
				cT_PivotCacheDefinition.calculatedItems = CT_CalculatedItems.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "calculatedMembers")
			{
				cT_PivotCacheDefinition.calculatedMembers = CT_CalculatedMembers.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "dimensions")
			{
				cT_PivotCacheDefinition.dimensions = CT_Dimensions.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "measureGroups")
			{
				cT_PivotCacheDefinition.measureGroups = CT_MeasureGroups.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "maps")
			{
				cT_PivotCacheDefinition.maps = CT_MeasureDimensionMaps.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_PivotCacheDefinition.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_PivotCacheDefinition;
	}

	internal void Write(StreamWriter sw)
	{
		sw.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
		sw.Write("<pivotCacheDefinition xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" ");
		sw.Write("xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" ");
		sw.Write("xmlns:s=\"http://schemas.openxmlformats.org/officeDocument/2006/sharedTypes\" ");
		XmlHelper.WriteAttribute(sw, "r:id", id);
		XmlHelper.WriteAttribute(sw, "invalid", invalid);
		XmlHelper.WriteAttribute(sw, "saveData", saveData);
		XmlHelper.WriteAttribute(sw, "refreshOnLoad", refreshOnLoad);
		XmlHelper.WriteAttribute(sw, "optimizeMemory", optimizeMemory);
		XmlHelper.WriteAttribute(sw, "enableRefresh", enableRefresh);
		XmlHelper.WriteAttribute(sw, "refreshedBy", refreshedBy);
		XmlHelper.WriteAttribute(sw, "refreshedDate", refreshedDate);
		XmlHelper.WriteAttribute(sw, "refreshedDateIso", refreshedDateIso);
		XmlHelper.WriteAttribute(sw, "backgroundQuery", backgroundQuery);
		XmlHelper.WriteAttribute(sw, "missingItemsLimit", missingItemsLimit);
		XmlHelper.WriteAttribute(sw, "createdVersion", createdVersion);
		XmlHelper.WriteAttribute(sw, "refreshedVersion", refreshedVersion);
		XmlHelper.WriteAttribute(sw, "minRefreshableVersion", minRefreshableVersion);
		XmlHelper.WriteAttribute(sw, "recordCount", recordCount);
		XmlHelper.WriteAttribute(sw, "upgradeOnRefresh", upgradeOnRefresh);
		XmlHelper.WriteAttribute(sw, "tupleCache1", tupleCache1);
		XmlHelper.WriteAttribute(sw, "supportSubquery", supportSubquery);
		XmlHelper.WriteAttribute(sw, "supportAdvancedDrill", supportAdvancedDrill);
		sw.Write(">");
		if (cacheSource != null)
		{
			cacheSource.Write(sw, "cacheSource");
		}
		if (cacheFields != null)
		{
			cacheFields.Write(sw, "cacheFields");
		}
		if (cacheHierarchies != null)
		{
			cacheHierarchies.Write(sw, "cacheHierarchies");
		}
		if (kpis != null)
		{
			kpis.Write(sw, "kpis");
		}
		if (tupleCache != null)
		{
			tupleCache.Write(sw, "tupleCache");
		}
		if (calculatedItems != null)
		{
			calculatedItems.Write(sw, "calculatedItems");
		}
		if (calculatedMembers != null)
		{
			calculatedMembers.Write(sw, "calculatedMembers");
		}
		if (dimensions != null)
		{
			dimensions.Write(sw, "dimensions");
		}
		if (measureGroups != null)
		{
			measureGroups.Write(sw, "measureGroups");
		}
		if (maps != null)
		{
			maps.Write(sw, "maps");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</pivotCacheDefinition>");
	}

	public void Save(Stream stream)
	{
		using StreamWriter sw = new StreamWriter(stream);
		Write(sw);
	}

	public CT_PivotCacheDefinition()
	{
		extLstField = new CT_ExtensionList();
		mapsField = new CT_MeasureDimensionMaps();
		measureGroupsField = new CT_MeasureGroups();
		dimensionsField = new CT_Dimensions();
		calculatedMembersField = new CT_CalculatedMembers();
		calculatedItemsField = new CT_CalculatedItems();
		tupleCacheField = new CT_TupleCache();
		kpisField = new CT_PCDKPIs();
		cacheHierarchiesField = new CT_CacheHierarchies();
		cacheFieldsField = new CT_CacheFields();
		cacheSourceField = new CT_CacheSource();
		invalidField = false;
		saveDataField = true;
		refreshOnLoadField = false;
		optimizeMemoryField = false;
		enableRefreshField = true;
		backgroundQueryField = false;
		createdVersionField = 0;
		refreshedVersionField = 0;
		minRefreshableVersionField = 0;
		upgradeOnRefreshField = false;
		tupleCache1Field = false;
		supportSubqueryField = false;
		supportAdvancedDrillField = false;
	}

	public CT_CacheFields AddNewCacheFields()
	{
		cacheFieldsField = new CT_CacheFields();
		return cacheFieldsField;
	}

	public CT_CacheSource AddNewCacheSource()
	{
		cacheSourceField = new CT_CacheSource();
		return cacheSourceField;
	}
}
