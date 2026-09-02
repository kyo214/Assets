using System;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = true)]
public class CT_PivotField
{
	private CT_Items itemsField;

	private CT_AutoSortScope autoSortScopeField;

	private CT_ExtensionList extLstField;

	private string nameField;

	private ST_Axis axisField;

	private bool axisFieldSpecified;

	private bool dataFieldField;

	private string subtotalCaptionField;

	private bool showDropDownsField;

	private bool hiddenLevelField;

	private string uniqueMemberPropertyField;

	private bool compactField;

	private bool allDrilledField;

	private uint numFmtIdField;

	private bool numFmtIdFieldSpecified;

	private bool outlineField;

	private bool subtotalTopField;

	private bool dragToRowField;

	private bool dragToColField;

	private bool multipleItemSelectionAllowedField;

	private bool dragToPageField;

	private bool dragToDataField;

	private bool dragOffField;

	private bool showAllField;

	private bool insertBlankRowField;

	private bool serverFieldField;

	private bool insertPageBreakField;

	private bool autoShowField;

	private bool topAutoShowField;

	private bool hideNewItemsField;

	private bool measureFilterField;

	private bool includeNewItemsInFilterField;

	private uint itemPageCountField;

	private ST_FieldSortType sortTypeField;

	private bool dataSourceSortField;

	private bool dataSourceSortFieldSpecified;

	private bool nonAutoSortDefaultField;

	private uint rankByField;

	private bool rankByFieldSpecified;

	private bool defaultSubtotalField;

	private bool sumSubtotalField;

	private bool countASubtotalField;

	private bool avgSubtotalField;

	private bool maxSubtotalField;

	private bool minSubtotalField;

	private bool productSubtotalField;

	private bool countSubtotalField;

	private bool stdDevSubtotalField;

	private bool stdDevPSubtotalField;

	private bool varSubtotalField;

	private bool varPSubtotalField;

	private bool showPropCellField;

	private bool showPropTipField;

	private bool showPropAsCaptionField;

	private bool defaultAttributeDrillStateField;

	[XmlElement(Order = 0)]
	public CT_Items items
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

	[XmlElement(Order = 1)]
	public CT_AutoSortScope autoSortScope
	{
		get
		{
			return autoSortScopeField;
		}
		set
		{
			autoSortScopeField = value;
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
	public ST_Axis axis
	{
		get
		{
			return axisField;
		}
		set
		{
			axisField = value;
		}
	}

	[XmlIgnore]
	public bool axisSpecified
	{
		get
		{
			return axisFieldSpecified;
		}
		set
		{
			axisFieldSpecified = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool dataField
	{
		get
		{
			return dataFieldField;
		}
		set
		{
			dataFieldField = value;
		}
	}

	[XmlAttribute]
	public string subtotalCaption
	{
		get
		{
			return subtotalCaptionField;
		}
		set
		{
			subtotalCaptionField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool showDropDowns
	{
		get
		{
			return showDropDownsField;
		}
		set
		{
			showDropDownsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool hiddenLevel
	{
		get
		{
			return hiddenLevelField;
		}
		set
		{
			hiddenLevelField = value;
		}
	}

	[XmlAttribute]
	public string uniqueMemberProperty
	{
		get
		{
			return uniqueMemberPropertyField;
		}
		set
		{
			uniqueMemberPropertyField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool compact
	{
		get
		{
			return compactField;
		}
		set
		{
			compactField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool allDrilled
	{
		get
		{
			return allDrilledField;
		}
		set
		{
			allDrilledField = value;
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
	[DefaultValue(true)]
	public bool outline
	{
		get
		{
			return outlineField;
		}
		set
		{
			outlineField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool subtotalTop
	{
		get
		{
			return subtotalTopField;
		}
		set
		{
			subtotalTopField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool dragToRow
	{
		get
		{
			return dragToRowField;
		}
		set
		{
			dragToRowField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool dragToCol
	{
		get
		{
			return dragToColField;
		}
		set
		{
			dragToColField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool multipleItemSelectionAllowed
	{
		get
		{
			return multipleItemSelectionAllowedField;
		}
		set
		{
			multipleItemSelectionAllowedField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool dragToPage
	{
		get
		{
			return dragToPageField;
		}
		set
		{
			dragToPageField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool dragToData
	{
		get
		{
			return dragToDataField;
		}
		set
		{
			dragToDataField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool dragOff
	{
		get
		{
			return dragOffField;
		}
		set
		{
			dragOffField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool showAll
	{
		get
		{
			return showAllField;
		}
		set
		{
			showAllField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool insertBlankRow
	{
		get
		{
			return insertBlankRowField;
		}
		set
		{
			insertBlankRowField = value;
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
	[DefaultValue(false)]
	public bool insertPageBreak
	{
		get
		{
			return insertPageBreakField;
		}
		set
		{
			insertPageBreakField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool autoShow
	{
		get
		{
			return autoShowField;
		}
		set
		{
			autoShowField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool topAutoShow
	{
		get
		{
			return topAutoShowField;
		}
		set
		{
			topAutoShowField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool hideNewItems
	{
		get
		{
			return hideNewItemsField;
		}
		set
		{
			hideNewItemsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool measureFilter
	{
		get
		{
			return measureFilterField;
		}
		set
		{
			measureFilterField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool includeNewItemsInFilter
	{
		get
		{
			return includeNewItemsInFilterField;
		}
		set
		{
			includeNewItemsInFilterField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(uint), "10")]
	public uint itemPageCount
	{
		get
		{
			return itemPageCountField;
		}
		set
		{
			itemPageCountField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(ST_FieldSortType.manual)]
	public ST_FieldSortType sortType
	{
		get
		{
			return sortTypeField;
		}
		set
		{
			sortTypeField = value;
		}
	}

	[XmlAttribute]
	public bool dataSourceSort
	{
		get
		{
			return dataSourceSortField;
		}
		set
		{
			dataSourceSortField = value;
		}
	}

	[XmlIgnore]
	public bool dataSourceSortSpecified
	{
		get
		{
			return dataSourceSortFieldSpecified;
		}
		set
		{
			dataSourceSortFieldSpecified = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool nonAutoSortDefault
	{
		get
		{
			return nonAutoSortDefaultField;
		}
		set
		{
			nonAutoSortDefaultField = value;
		}
	}

	[XmlAttribute]
	public uint rankBy
	{
		get
		{
			return rankByField;
		}
		set
		{
			rankByField = value;
		}
	}

	[XmlIgnore]
	public bool rankBySpecified
	{
		get
		{
			return rankByFieldSpecified;
		}
		set
		{
			rankByFieldSpecified = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool defaultSubtotal
	{
		get
		{
			return defaultSubtotalField;
		}
		set
		{
			defaultSubtotalField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool sumSubtotal
	{
		get
		{
			return sumSubtotalField;
		}
		set
		{
			sumSubtotalField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool countASubtotal
	{
		get
		{
			return countASubtotalField;
		}
		set
		{
			countASubtotalField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool avgSubtotal
	{
		get
		{
			return avgSubtotalField;
		}
		set
		{
			avgSubtotalField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool maxSubtotal
	{
		get
		{
			return maxSubtotalField;
		}
		set
		{
			maxSubtotalField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool minSubtotal
	{
		get
		{
			return minSubtotalField;
		}
		set
		{
			minSubtotalField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool productSubtotal
	{
		get
		{
			return productSubtotalField;
		}
		set
		{
			productSubtotalField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool countSubtotal
	{
		get
		{
			return countSubtotalField;
		}
		set
		{
			countSubtotalField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool stdDevSubtotal
	{
		get
		{
			return stdDevSubtotalField;
		}
		set
		{
			stdDevSubtotalField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool stdDevPSubtotal
	{
		get
		{
			return stdDevPSubtotalField;
		}
		set
		{
			stdDevPSubtotalField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool varSubtotal
	{
		get
		{
			return varSubtotalField;
		}
		set
		{
			varSubtotalField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool varPSubtotal
	{
		get
		{
			return varPSubtotalField;
		}
		set
		{
			varPSubtotalField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool showPropCell
	{
		get
		{
			return showPropCellField;
		}
		set
		{
			showPropCellField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool showPropTip
	{
		get
		{
			return showPropTipField;
		}
		set
		{
			showPropTipField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool showPropAsCaption
	{
		get
		{
			return showPropAsCaptionField;
		}
		set
		{
			showPropAsCaptionField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool defaultAttributeDrillState
	{
		get
		{
			return defaultAttributeDrillStateField;
		}
		set
		{
			defaultAttributeDrillStateField = value;
		}
	}

	public static CT_PivotField Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PivotField cT_PivotField = new CT_PivotField();
		cT_PivotField.name = XmlHelper.ReadString(node.Attributes["name"]);
		if (node.Attributes["axis"] != null)
		{
			cT_PivotField.axis = (ST_Axis)Enum.Parse(typeof(ST_Axis), node.Attributes["axis"].Value);
		}
		if (node.Attributes["dataField"] != null)
		{
			cT_PivotField.dataField = XmlHelper.ReadBool(node.Attributes["dataField"]);
		}
		cT_PivotField.subtotalCaption = XmlHelper.ReadString(node.Attributes["subtotalCaption"]);
		if (node.Attributes["showDropDowns"] != null)
		{
			cT_PivotField.showDropDowns = XmlHelper.ReadBool(node.Attributes["showDropDowns"]);
		}
		if (node.Attributes["hiddenLevel"] != null)
		{
			cT_PivotField.hiddenLevel = XmlHelper.ReadBool(node.Attributes["hiddenLevel"]);
		}
		cT_PivotField.uniqueMemberProperty = XmlHelper.ReadString(node.Attributes["uniqueMemberProperty"]);
		if (node.Attributes["compact"] != null)
		{
			cT_PivotField.compact = XmlHelper.ReadBool(node.Attributes["compact"]);
		}
		if (node.Attributes["allDrilled"] != null)
		{
			cT_PivotField.allDrilled = XmlHelper.ReadBool(node.Attributes["allDrilled"]);
		}
		if (node.Attributes["numFmtId"] != null)
		{
			cT_PivotField.numFmtId = XmlHelper.ReadUInt(node.Attributes["numFmtId"]);
		}
		if (node.Attributes["outline"] != null)
		{
			cT_PivotField.outline = XmlHelper.ReadBool(node.Attributes["outline"]);
		}
		if (node.Attributes["subtotalTop"] != null)
		{
			cT_PivotField.subtotalTop = XmlHelper.ReadBool(node.Attributes["subtotalTop"]);
		}
		if (node.Attributes["dragToRow"] != null)
		{
			cT_PivotField.dragToRow = XmlHelper.ReadBool(node.Attributes["dragToRow"]);
		}
		if (node.Attributes["dragToCol"] != null)
		{
			cT_PivotField.dragToCol = XmlHelper.ReadBool(node.Attributes["dragToCol"]);
		}
		if (node.Attributes["multipleItemSelectionAllowed"] != null)
		{
			cT_PivotField.multipleItemSelectionAllowed = XmlHelper.ReadBool(node.Attributes["multipleItemSelectionAllowed"]);
		}
		if (node.Attributes["dragToPage"] != null)
		{
			cT_PivotField.dragToPage = XmlHelper.ReadBool(node.Attributes["dragToPage"]);
		}
		if (node.Attributes["dragToData"] != null)
		{
			cT_PivotField.dragToData = XmlHelper.ReadBool(node.Attributes["dragToData"]);
		}
		if (node.Attributes["dragOff"] != null)
		{
			cT_PivotField.dragOff = XmlHelper.ReadBool(node.Attributes["dragOff"]);
		}
		if (node.Attributes["showAll"] != null)
		{
			cT_PivotField.showAll = XmlHelper.ReadBool(node.Attributes["showAll"]);
		}
		if (node.Attributes["insertBlankRow"] != null)
		{
			cT_PivotField.insertBlankRow = XmlHelper.ReadBool(node.Attributes["insertBlankRow"]);
		}
		if (node.Attributes["serverField"] != null)
		{
			cT_PivotField.serverField = XmlHelper.ReadBool(node.Attributes["serverField"]);
		}
		if (node.Attributes["insertPageBreak"] != null)
		{
			cT_PivotField.insertPageBreak = XmlHelper.ReadBool(node.Attributes["insertPageBreak"]);
		}
		if (node.Attributes["autoShow"] != null)
		{
			cT_PivotField.autoShow = XmlHelper.ReadBool(node.Attributes["autoShow"]);
		}
		if (node.Attributes["topAutoShow"] != null)
		{
			cT_PivotField.topAutoShow = XmlHelper.ReadBool(node.Attributes["topAutoShow"]);
		}
		if (node.Attributes["hideNewItems"] != null)
		{
			cT_PivotField.hideNewItems = XmlHelper.ReadBool(node.Attributes["hideNewItems"]);
		}
		if (node.Attributes["measureFilter"] != null)
		{
			cT_PivotField.measureFilter = XmlHelper.ReadBool(node.Attributes["measureFilter"]);
		}
		if (node.Attributes["includeNewItemsInFilter"] != null)
		{
			cT_PivotField.includeNewItemsInFilter = XmlHelper.ReadBool(node.Attributes["includeNewItemsInFilter"]);
		}
		if (node.Attributes["itemPageCount"] != null)
		{
			cT_PivotField.itemPageCount = XmlHelper.ReadUInt(node.Attributes["itemPageCount"]);
		}
		if (node.Attributes["sortType"] != null)
		{
			cT_PivotField.sortType = (ST_FieldSortType)Enum.Parse(typeof(ST_FieldSortType), node.Attributes["sortType"].Value);
		}
		if (node.Attributes["dataSourceSort"] != null)
		{
			cT_PivotField.dataSourceSort = XmlHelper.ReadBool(node.Attributes["dataSourceSort"]);
		}
		if (node.Attributes["nonAutoSortDefault"] != null)
		{
			cT_PivotField.nonAutoSortDefault = XmlHelper.ReadBool(node.Attributes["nonAutoSortDefault"]);
		}
		if (node.Attributes["rankBy"] != null)
		{
			cT_PivotField.rankBy = XmlHelper.ReadUInt(node.Attributes["rankBy"]);
		}
		if (node.Attributes["defaultSubtotal"] != null)
		{
			cT_PivotField.defaultSubtotal = XmlHelper.ReadBool(node.Attributes["defaultSubtotal"]);
		}
		if (node.Attributes["sumSubtotal"] != null)
		{
			cT_PivotField.sumSubtotal = XmlHelper.ReadBool(node.Attributes["sumSubtotal"]);
		}
		if (node.Attributes["countASubtotal"] != null)
		{
			cT_PivotField.countASubtotal = XmlHelper.ReadBool(node.Attributes["countASubtotal"]);
		}
		if (node.Attributes["avgSubtotal"] != null)
		{
			cT_PivotField.avgSubtotal = XmlHelper.ReadBool(node.Attributes["avgSubtotal"]);
		}
		if (node.Attributes["maxSubtotal"] != null)
		{
			cT_PivotField.maxSubtotal = XmlHelper.ReadBool(node.Attributes["maxSubtotal"]);
		}
		if (node.Attributes["minSubtotal"] != null)
		{
			cT_PivotField.minSubtotal = XmlHelper.ReadBool(node.Attributes["minSubtotal"]);
		}
		if (node.Attributes["productSubtotal"] != null)
		{
			cT_PivotField.productSubtotal = XmlHelper.ReadBool(node.Attributes["productSubtotal"]);
		}
		if (node.Attributes["countSubtotal"] != null)
		{
			cT_PivotField.countSubtotal = XmlHelper.ReadBool(node.Attributes["countSubtotal"]);
		}
		if (node.Attributes["stdDevSubtotal"] != null)
		{
			cT_PivotField.stdDevSubtotal = XmlHelper.ReadBool(node.Attributes["stdDevSubtotal"]);
		}
		if (node.Attributes["stdDevPSubtotal"] != null)
		{
			cT_PivotField.stdDevPSubtotal = XmlHelper.ReadBool(node.Attributes["stdDevPSubtotal"]);
		}
		if (node.Attributes["varSubtotal"] != null)
		{
			cT_PivotField.varSubtotal = XmlHelper.ReadBool(node.Attributes["varSubtotal"]);
		}
		if (node.Attributes["varPSubtotal"] != null)
		{
			cT_PivotField.varPSubtotal = XmlHelper.ReadBool(node.Attributes["varPSubtotal"]);
		}
		if (node.Attributes["showPropCell"] != null)
		{
			cT_PivotField.showPropCell = XmlHelper.ReadBool(node.Attributes["showPropCell"]);
		}
		if (node.Attributes["showPropTip"] != null)
		{
			cT_PivotField.showPropTip = XmlHelper.ReadBool(node.Attributes["showPropTip"]);
		}
		if (node.Attributes["showPropAsCaption"] != null)
		{
			cT_PivotField.showPropAsCaption = XmlHelper.ReadBool(node.Attributes["showPropAsCaption"]);
		}
		if (node.Attributes["defaultAttributeDrillState"] != null)
		{
			cT_PivotField.defaultAttributeDrillState = XmlHelper.ReadBool(node.Attributes["defaultAttributeDrillState"]);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "items")
			{
				cT_PivotField.items = CT_Items.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "autoSortScope")
			{
				cT_PivotField.autoSortScope = CT_AutoSortScope.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_PivotField.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_PivotField;
	}

	internal void Write(StreamWriter sw, string nodeName)
	{
		sw.Write($"<{nodeName}");
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "axis", axis.ToString());
		XmlHelper.WriteAttribute(sw, "dataField", dataField);
		XmlHelper.WriteAttribute(sw, "subtotalCaption", subtotalCaption);
		XmlHelper.WriteAttribute(sw, "showDropDowns", showDropDowns);
		XmlHelper.WriteAttribute(sw, "hiddenLevel", hiddenLevel);
		XmlHelper.WriteAttribute(sw, "uniqueMemberProperty", uniqueMemberProperty);
		XmlHelper.WriteAttribute(sw, "compact", compact);
		XmlHelper.WriteAttribute(sw, "allDrilled", allDrilled);
		XmlHelper.WriteAttribute(sw, "numFmtId", numFmtId);
		XmlHelper.WriteAttribute(sw, "outline", outline);
		XmlHelper.WriteAttribute(sw, "subtotalTop", subtotalTop);
		XmlHelper.WriteAttribute(sw, "dragToRow", dragToRow);
		XmlHelper.WriteAttribute(sw, "dragToCol", dragToCol);
		XmlHelper.WriteAttribute(sw, "multipleItemSelectionAllowed", multipleItemSelectionAllowed);
		XmlHelper.WriteAttribute(sw, "dragToPage", dragToPage);
		XmlHelper.WriteAttribute(sw, "dragToData", dragToData);
		XmlHelper.WriteAttribute(sw, "dragOff", dragOff);
		XmlHelper.WriteAttribute(sw, "showAll", showAll);
		XmlHelper.WriteAttribute(sw, "insertBlankRow", insertBlankRow);
		XmlHelper.WriteAttribute(sw, "serverField", serverField);
		XmlHelper.WriteAttribute(sw, "insertPageBreak", insertPageBreak);
		XmlHelper.WriteAttribute(sw, "autoShow", autoShow);
		XmlHelper.WriteAttribute(sw, "topAutoShow", topAutoShow);
		XmlHelper.WriteAttribute(sw, "hideNewItems", hideNewItems);
		XmlHelper.WriteAttribute(sw, "measureFilter", measureFilter);
		XmlHelper.WriteAttribute(sw, "includeNewItemsInFilter", includeNewItemsInFilter);
		XmlHelper.WriteAttribute(sw, "itemPageCount", itemPageCount);
		XmlHelper.WriteAttribute(sw, "sortType", sortType.ToString());
		XmlHelper.WriteAttribute(sw, "dataSourceSort", dataSourceSort);
		XmlHelper.WriteAttribute(sw, "nonAutoSortDefault", nonAutoSortDefault);
		XmlHelper.WriteAttribute(sw, "rankBy", rankBy);
		XmlHelper.WriteAttribute(sw, "defaultSubtotal", defaultSubtotal);
		XmlHelper.WriteAttribute(sw, "sumSubtotal", sumSubtotal);
		XmlHelper.WriteAttribute(sw, "countASubtotal", countASubtotal);
		XmlHelper.WriteAttribute(sw, "avgSubtotal", avgSubtotal);
		XmlHelper.WriteAttribute(sw, "maxSubtotal", maxSubtotal);
		XmlHelper.WriteAttribute(sw, "minSubtotal", minSubtotal);
		XmlHelper.WriteAttribute(sw, "productSubtotal", productSubtotal);
		XmlHelper.WriteAttribute(sw, "countSubtotal", countSubtotal);
		XmlHelper.WriteAttribute(sw, "stdDevSubtotal", stdDevSubtotal);
		XmlHelper.WriteAttribute(sw, "stdDevPSubtotal", stdDevPSubtotal);
		XmlHelper.WriteAttribute(sw, "varSubtotal", varSubtotal);
		XmlHelper.WriteAttribute(sw, "varPSubtotal", varPSubtotal);
		XmlHelper.WriteAttribute(sw, "showPropCell", showPropCell);
		XmlHelper.WriteAttribute(sw, "showPropTip", showPropTip);
		XmlHelper.WriteAttribute(sw, "showPropAsCaption", showPropAsCaption);
		XmlHelper.WriteAttribute(sw, "defaultAttributeDrillState", defaultAttributeDrillState);
		sw.Write(">");
		if (items != null)
		{
			items.Write(sw, "items");
		}
		if (autoSortScope != null)
		{
			autoSortScope.Write(sw, "autoSortScope");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</{nodeName}>");
	}

	public CT_PivotField()
	{
		extLstField = new CT_ExtensionList();
		autoSortScopeField = new CT_AutoSortScope();
		itemsField = new CT_Items();
		dataFieldField = false;
		showDropDownsField = true;
		hiddenLevelField = false;
		compactField = true;
		allDrilledField = false;
		outlineField = true;
		subtotalTopField = true;
		dragToRowField = true;
		dragToColField = true;
		multipleItemSelectionAllowedField = false;
		dragToPageField = true;
		dragToDataField = true;
		dragOffField = true;
		showAllField = true;
		insertBlankRowField = false;
		serverFieldField = false;
		insertPageBreakField = false;
		autoShowField = false;
		topAutoShowField = true;
		hideNewItemsField = false;
		measureFilterField = false;
		includeNewItemsInFilterField = false;
		itemPageCountField = 10u;
		sortTypeField = ST_FieldSortType.manual;
		nonAutoSortDefaultField = false;
		defaultSubtotalField = true;
		sumSubtotalField = false;
		countASubtotalField = false;
		avgSubtotalField = false;
		maxSubtotalField = false;
		minSubtotalField = false;
		productSubtotalField = false;
		countSubtotalField = false;
		stdDevSubtotalField = false;
		stdDevPSubtotalField = false;
		varSubtotalField = false;
		varPSubtotalField = false;
		showPropCellField = false;
		showPropTipField = false;
		showPropAsCaptionField = false;
		defaultAttributeDrillStateField = false;
	}

	public CT_Items AddNewItems()
	{
		itemsField = new CT_Items();
		return itemsField;
	}
}
