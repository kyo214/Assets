using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NPOI.OpenXml4Net.Util;

namespace NPOI.OpenXmlFormats.Spreadsheet;

[XmlType(Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")]
[XmlRoot("pivotTableDefinition", Namespace = "http://schemas.openxmlformats.org/spreadsheetml/2006/main", IsNullable = false)]
public class CT_PivotTableDefinition
{
	private CT_Location locationField;

	private CT_PivotFields pivotFieldsField;

	private CT_RowFields rowFieldsField;

	private CT_rowItems rowItemsField;

	private CT_ColFields colFieldsField;

	private CT_colItems colItemsField;

	private CT_PageFields pageFieldsField;

	private CT_DataFields dataFieldsField;

	private CT_Formats formatsField;

	private CT_ConditionalFormats conditionalFormatsField;

	private CT_ChartFormats chartFormatsField;

	private CT_PivotHierarchies pivotHierarchiesField;

	private CT_PivotTableStyle pivotTableStyleInfoField;

	private CT_PivotFilters filtersField;

	private CT_RowHierarchiesUsage rowHierarchiesUsageField;

	private CT_ColHierarchiesUsage colHierarchiesUsageField;

	private CT_ExtensionList extLstField;

	private string nameField;

	private uint cacheIdField;

	private bool dataOnRowsField;

	private uint dataPositionField;

	private bool dataPositionFieldSpecified;

	private uint autoFormatIdField;

	private bool autoFormatIdFieldSpecified;

	private bool applyNumberFormatsField;

	private bool applyNumberFormatsFieldSpecified;

	private bool applyBorderFormatsField;

	private bool applyBorderFormatsFieldSpecified;

	private bool applyFontFormatsField;

	private bool applyFontFormatsFieldSpecified;

	private bool applyPatternFormatsField;

	private bool applyPatternFormatsFieldSpecified;

	private bool applyAlignmentFormatsField;

	private bool applyAlignmentFormatsFieldSpecified;

	private bool applyWidthHeightFormatsField;

	private bool applyWidthHeightFormatsFieldSpecified;

	private string dataCaptionField;

	private string grandTotalCaptionField;

	private string errorCaptionField;

	private bool showErrorField;

	private string missingCaptionField;

	private bool showMissingField;

	private string pageStyleField;

	private string pivotTableStyleField;

	private string vacatedStyleField;

	private string tagField;

	private byte updatedVersionField;

	private byte minRefreshableVersionField;

	private bool asteriskTotalsField;

	private bool showItemsField;

	private bool editDataField;

	private bool disableFieldListField;

	private bool showCalcMbrsField;

	private bool visualTotalsField;

	private bool showMultipleLabelField;

	private bool showDataDropDownField;

	private bool showDrillField;

	private bool printDrillField;

	private bool showMemberPropertyTipsField;

	private bool showDataTipsField;

	private bool enableWizardField;

	private bool enableDrillField;

	private bool enableFieldPropertiesField;

	private bool preserveFormattingField;

	private bool useAutoFormattingField;

	private uint pageWrapField;

	private bool pageOverThenDownField;

	private bool subtotalHiddenItemsField;

	private bool rowGrandTotalsField;

	private bool colGrandTotalsField;

	private bool fieldPrintTitlesField;

	private bool itemPrintTitlesField;

	private bool mergeItemField;

	private bool showDropZonesField;

	private byte createdVersionField;

	private uint indentField;

	private bool showEmptyRowField;

	private bool showEmptyColField;

	private bool showHeadersField;

	private bool compactField;

	private bool outlineField;

	private bool outlineDataField;

	private bool compactDataField;

	private bool publishedField;

	private bool gridDropZonesField;

	private bool immersiveField;

	private bool multipleFieldFiltersField;

	private uint chartFormatField;

	private string rowHeaderCaptionField;

	private string colHeaderCaptionField;

	private bool fieldListSortAscendingField;

	private bool mdxSubqueriesField;

	private bool customListSortField;

	[XmlElement(Order = 0)]
	public CT_Location location
	{
		get
		{
			return locationField;
		}
		set
		{
			locationField = value;
		}
	}

	[XmlElement(Order = 1)]
	public CT_PivotFields pivotFields
	{
		get
		{
			return pivotFieldsField;
		}
		set
		{
			pivotFieldsField = value;
		}
	}

	[XmlElement(Order = 2)]
	public CT_RowFields rowFields
	{
		get
		{
			return rowFieldsField;
		}
		set
		{
			rowFieldsField = value;
		}
	}

	[XmlElement(Order = 3)]
	public CT_rowItems rowItems
	{
		get
		{
			return rowItemsField;
		}
		set
		{
			rowItemsField = value;
		}
	}

	[XmlElement(Order = 4)]
	public CT_ColFields colFields
	{
		get
		{
			return colFieldsField;
		}
		set
		{
			colFieldsField = value;
		}
	}

	[XmlElement(Order = 5)]
	public CT_colItems colItems
	{
		get
		{
			return colItemsField;
		}
		set
		{
			colItemsField = value;
		}
	}

	[XmlElement(Order = 6)]
	public CT_PageFields pageFields
	{
		get
		{
			return pageFieldsField;
		}
		set
		{
			pageFieldsField = value;
		}
	}

	[XmlElement(Order = 7)]
	public CT_DataFields dataFields
	{
		get
		{
			return dataFieldsField;
		}
		set
		{
			dataFieldsField = value;
		}
	}

	[XmlElement(Order = 8)]
	public CT_Formats formats
	{
		get
		{
			return formatsField;
		}
		set
		{
			formatsField = value;
		}
	}

	[XmlElement(Order = 9)]
	public CT_ConditionalFormats conditionalFormats
	{
		get
		{
			return conditionalFormatsField;
		}
		set
		{
			conditionalFormatsField = value;
		}
	}

	[XmlElement(Order = 10)]
	public CT_ChartFormats chartFormats
	{
		get
		{
			return chartFormatsField;
		}
		set
		{
			chartFormatsField = value;
		}
	}

	[XmlElement(Order = 11)]
	public CT_PivotHierarchies pivotHierarchies
	{
		get
		{
			return pivotHierarchiesField;
		}
		set
		{
			pivotHierarchiesField = value;
		}
	}

	[XmlElement(Order = 12)]
	public CT_PivotTableStyle pivotTableStyleInfo
	{
		get
		{
			return pivotTableStyleInfoField;
		}
		set
		{
			pivotTableStyleInfoField = value;
		}
	}

	[XmlElement(Order = 13)]
	public CT_PivotFilters filters
	{
		get
		{
			return filtersField;
		}
		set
		{
			filtersField = value;
		}
	}

	[XmlElement(Order = 14)]
	public CT_RowHierarchiesUsage rowHierarchiesUsage
	{
		get
		{
			return rowHierarchiesUsageField;
		}
		set
		{
			rowHierarchiesUsageField = value;
		}
	}

	[XmlElement(Order = 15)]
	public CT_ColHierarchiesUsage colHierarchiesUsage
	{
		get
		{
			return colHierarchiesUsageField;
		}
		set
		{
			colHierarchiesUsageField = value;
		}
	}

	[XmlElement(Order = 16)]
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
	public uint cacheId
	{
		get
		{
			return cacheIdField;
		}
		set
		{
			cacheIdField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool dataOnRows
	{
		get
		{
			return dataOnRowsField;
		}
		set
		{
			dataOnRowsField = value;
		}
	}

	[XmlAttribute]
	public uint dataPosition
	{
		get
		{
			return dataPositionField;
		}
		set
		{
			dataPositionField = value;
		}
	}

	[XmlIgnore]
	public bool dataPositionSpecified
	{
		get
		{
			return dataPositionFieldSpecified;
		}
		set
		{
			dataPositionFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public uint autoFormatId
	{
		get
		{
			return autoFormatIdField;
		}
		set
		{
			autoFormatIdField = value;
		}
	}

	[XmlIgnore]
	public bool autoFormatIdSpecified
	{
		get
		{
			return autoFormatIdFieldSpecified;
		}
		set
		{
			autoFormatIdFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool applyNumberFormats
	{
		get
		{
			return applyNumberFormatsField;
		}
		set
		{
			applyNumberFormatsField = value;
		}
	}

	[XmlIgnore]
	public bool applyNumberFormatsSpecified
	{
		get
		{
			return applyNumberFormatsFieldSpecified;
		}
		set
		{
			applyNumberFormatsFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool applyBorderFormats
	{
		get
		{
			return applyBorderFormatsField;
		}
		set
		{
			applyBorderFormatsField = value;
		}
	}

	[XmlIgnore]
	public bool applyBorderFormatsSpecified
	{
		get
		{
			return applyBorderFormatsFieldSpecified;
		}
		set
		{
			applyBorderFormatsFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool applyFontFormats
	{
		get
		{
			return applyFontFormatsField;
		}
		set
		{
			applyFontFormatsField = value;
		}
	}

	[XmlIgnore]
	public bool applyFontFormatsSpecified
	{
		get
		{
			return applyFontFormatsFieldSpecified;
		}
		set
		{
			applyFontFormatsFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool applyPatternFormats
	{
		get
		{
			return applyPatternFormatsField;
		}
		set
		{
			applyPatternFormatsField = value;
		}
	}

	[XmlIgnore]
	public bool applyPatternFormatsSpecified
	{
		get
		{
			return applyPatternFormatsFieldSpecified;
		}
		set
		{
			applyPatternFormatsFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool applyAlignmentFormats
	{
		get
		{
			return applyAlignmentFormatsField;
		}
		set
		{
			applyAlignmentFormatsField = value;
		}
	}

	[XmlIgnore]
	public bool applyAlignmentFormatsSpecified
	{
		get
		{
			return applyAlignmentFormatsFieldSpecified;
		}
		set
		{
			applyAlignmentFormatsFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public bool applyWidthHeightFormats
	{
		get
		{
			return applyWidthHeightFormatsField;
		}
		set
		{
			applyWidthHeightFormatsField = value;
		}
	}

	[XmlIgnore]
	public bool applyWidthHeightFormatsSpecified
	{
		get
		{
			return applyWidthHeightFormatsFieldSpecified;
		}
		set
		{
			applyWidthHeightFormatsFieldSpecified = value;
		}
	}

	[XmlAttribute]
	public string dataCaption
	{
		get
		{
			return dataCaptionField;
		}
		set
		{
			dataCaptionField = value;
		}
	}

	[XmlAttribute]
	public string grandTotalCaption
	{
		get
		{
			return grandTotalCaptionField;
		}
		set
		{
			grandTotalCaptionField = value;
		}
	}

	[XmlAttribute]
	public string errorCaption
	{
		get
		{
			return errorCaptionField;
		}
		set
		{
			errorCaptionField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool showError
	{
		get
		{
			return showErrorField;
		}
		set
		{
			showErrorField = value;
		}
	}

	[XmlAttribute]
	public string missingCaption
	{
		get
		{
			return missingCaptionField;
		}
		set
		{
			missingCaptionField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool showMissing
	{
		get
		{
			return showMissingField;
		}
		set
		{
			showMissingField = value;
		}
	}

	[XmlAttribute]
	public string pageStyle
	{
		get
		{
			return pageStyleField;
		}
		set
		{
			pageStyleField = value;
		}
	}

	[XmlAttribute]
	public string pivotTableStyle
	{
		get
		{
			return pivotTableStyleField;
		}
		set
		{
			pivotTableStyleField = value;
		}
	}

	[XmlAttribute]
	public string vacatedStyle
	{
		get
		{
			return vacatedStyleField;
		}
		set
		{
			vacatedStyleField = value;
		}
	}

	[XmlAttribute]
	public string tag
	{
		get
		{
			return tagField;
		}
		set
		{
			tagField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(byte), "0")]
	public byte updatedVersion
	{
		get
		{
			return updatedVersionField;
		}
		set
		{
			updatedVersionField = value;
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
	[DefaultValue(false)]
	public bool asteriskTotals
	{
		get
		{
			return asteriskTotalsField;
		}
		set
		{
			asteriskTotalsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool showItems
	{
		get
		{
			return showItemsField;
		}
		set
		{
			showItemsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool editData
	{
		get
		{
			return editDataField;
		}
		set
		{
			editDataField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool disableFieldList
	{
		get
		{
			return disableFieldListField;
		}
		set
		{
			disableFieldListField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool showCalcMbrs
	{
		get
		{
			return showCalcMbrsField;
		}
		set
		{
			showCalcMbrsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool visualTotals
	{
		get
		{
			return visualTotalsField;
		}
		set
		{
			visualTotalsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool showMultipleLabel
	{
		get
		{
			return showMultipleLabelField;
		}
		set
		{
			showMultipleLabelField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool showDataDropDown
	{
		get
		{
			return showDataDropDownField;
		}
		set
		{
			showDataDropDownField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool showDrill
	{
		get
		{
			return showDrillField;
		}
		set
		{
			showDrillField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool printDrill
	{
		get
		{
			return printDrillField;
		}
		set
		{
			printDrillField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool showMemberPropertyTips
	{
		get
		{
			return showMemberPropertyTipsField;
		}
		set
		{
			showMemberPropertyTipsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool showDataTips
	{
		get
		{
			return showDataTipsField;
		}
		set
		{
			showDataTipsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool enableWizard
	{
		get
		{
			return enableWizardField;
		}
		set
		{
			enableWizardField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool enableDrill
	{
		get
		{
			return enableDrillField;
		}
		set
		{
			enableDrillField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool enableFieldProperties
	{
		get
		{
			return enableFieldPropertiesField;
		}
		set
		{
			enableFieldPropertiesField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool preserveFormatting
	{
		get
		{
			return preserveFormattingField;
		}
		set
		{
			preserveFormattingField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool useAutoFormatting
	{
		get
		{
			return useAutoFormattingField;
		}
		set
		{
			useAutoFormattingField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(uint), "0")]
	public uint pageWrap
	{
		get
		{
			return pageWrapField;
		}
		set
		{
			pageWrapField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool pageOverThenDown
	{
		get
		{
			return pageOverThenDownField;
		}
		set
		{
			pageOverThenDownField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool subtotalHiddenItems
	{
		get
		{
			return subtotalHiddenItemsField;
		}
		set
		{
			subtotalHiddenItemsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool rowGrandTotals
	{
		get
		{
			return rowGrandTotalsField;
		}
		set
		{
			rowGrandTotalsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool colGrandTotals
	{
		get
		{
			return colGrandTotalsField;
		}
		set
		{
			colGrandTotalsField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool fieldPrintTitles
	{
		get
		{
			return fieldPrintTitlesField;
		}
		set
		{
			fieldPrintTitlesField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool itemPrintTitles
	{
		get
		{
			return itemPrintTitlesField;
		}
		set
		{
			itemPrintTitlesField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool mergeItem
	{
		get
		{
			return mergeItemField;
		}
		set
		{
			mergeItemField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool showDropZones
	{
		get
		{
			return showDropZonesField;
		}
		set
		{
			showDropZonesField = value;
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
	[DefaultValue(typeof(uint), "1")]
	public uint indent
	{
		get
		{
			return indentField;
		}
		set
		{
			indentField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool showEmptyRow
	{
		get
		{
			return showEmptyRowField;
		}
		set
		{
			showEmptyRowField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool showEmptyCol
	{
		get
		{
			return showEmptyColField;
		}
		set
		{
			showEmptyColField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool showHeaders
	{
		get
		{
			return showHeadersField;
		}
		set
		{
			showHeadersField = value;
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
	[DefaultValue(false)]
	public bool outlineData
	{
		get
		{
			return outlineDataField;
		}
		set
		{
			outlineDataField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool compactData
	{
		get
		{
			return compactDataField;
		}
		set
		{
			compactDataField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool published
	{
		get
		{
			return publishedField;
		}
		set
		{
			publishedField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool gridDropZones
	{
		get
		{
			return gridDropZonesField;
		}
		set
		{
			gridDropZonesField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool immersive
	{
		get
		{
			return immersiveField;
		}
		set
		{
			immersiveField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool multipleFieldFilters
	{
		get
		{
			return multipleFieldFiltersField;
		}
		set
		{
			multipleFieldFiltersField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(typeof(uint), "0")]
	public uint chartFormat
	{
		get
		{
			return chartFormatField;
		}
		set
		{
			chartFormatField = value;
		}
	}

	[XmlAttribute]
	public string rowHeaderCaption
	{
		get
		{
			return rowHeaderCaptionField;
		}
		set
		{
			rowHeaderCaptionField = value;
		}
	}

	[XmlAttribute]
	public string colHeaderCaption
	{
		get
		{
			return colHeaderCaptionField;
		}
		set
		{
			colHeaderCaptionField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool fieldListSortAscending
	{
		get
		{
			return fieldListSortAscendingField;
		}
		set
		{
			fieldListSortAscendingField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(false)]
	public bool mdxSubqueries
	{
		get
		{
			return mdxSubqueriesField;
		}
		set
		{
			mdxSubqueriesField = value;
		}
	}

	[XmlAttribute]
	[DefaultValue(true)]
	public bool customListSort
	{
		get
		{
			return customListSortField;
		}
		set
		{
			customListSortField = value;
		}
	}

	public static CT_PivotTableDefinition Parse(XmlNode node, XmlNamespaceManager namespaceManager)
	{
		if (node == null)
		{
			return null;
		}
		CT_PivotTableDefinition cT_PivotTableDefinition = new CT_PivotTableDefinition();
		cT_PivotTableDefinition.name = XmlHelper.ReadString(node.Attributes["name"]);
		if (node.Attributes["cacheId"] != null)
		{
			cT_PivotTableDefinition.cacheId = XmlHelper.ReadUInt(node.Attributes["cacheId"]);
		}
		if (node.Attributes["dataOnRows"] != null)
		{
			cT_PivotTableDefinition.dataOnRows = XmlHelper.ReadBool(node.Attributes["dataOnRows"]);
		}
		if (node.Attributes["dataPosition"] != null)
		{
			cT_PivotTableDefinition.dataPosition = XmlHelper.ReadUInt(node.Attributes["dataPosition"]);
		}
		if (node.Attributes["autoFormatId"] != null)
		{
			cT_PivotTableDefinition.autoFormatId = XmlHelper.ReadUInt(node.Attributes["autoFormatId"]);
		}
		if (node.Attributes["applyNumberFormats"] != null)
		{
			cT_PivotTableDefinition.applyNumberFormats = XmlHelper.ReadBool(node.Attributes["applyNumberFormats"]);
		}
		if (node.Attributes["applyBorderFormats"] != null)
		{
			cT_PivotTableDefinition.applyBorderFormats = XmlHelper.ReadBool(node.Attributes["applyBorderFormats"]);
		}
		if (node.Attributes["applyFontFormats"] != null)
		{
			cT_PivotTableDefinition.applyFontFormats = XmlHelper.ReadBool(node.Attributes["applyFontFormats"]);
		}
		if (node.Attributes["applyPatternFormats"] != null)
		{
			cT_PivotTableDefinition.applyPatternFormats = XmlHelper.ReadBool(node.Attributes["applyPatternFormats"]);
		}
		if (node.Attributes["applyAlignmentFormats"] != null)
		{
			cT_PivotTableDefinition.applyAlignmentFormats = XmlHelper.ReadBool(node.Attributes["applyAlignmentFormats"]);
		}
		if (node.Attributes["applyWidthHeightFormats"] != null)
		{
			cT_PivotTableDefinition.applyWidthHeightFormats = XmlHelper.ReadBool(node.Attributes["applyWidthHeightFormats"]);
		}
		cT_PivotTableDefinition.dataCaption = XmlHelper.ReadString(node.Attributes["dataCaption"]);
		cT_PivotTableDefinition.grandTotalCaption = XmlHelper.ReadString(node.Attributes["grandTotalCaption"]);
		cT_PivotTableDefinition.errorCaption = XmlHelper.ReadString(node.Attributes["errorCaption"]);
		if (node.Attributes["showError"] != null)
		{
			cT_PivotTableDefinition.showError = XmlHelper.ReadBool(node.Attributes["showError"]);
		}
		cT_PivotTableDefinition.missingCaption = XmlHelper.ReadString(node.Attributes["missingCaption"]);
		if (node.Attributes["showMissing"] != null)
		{
			cT_PivotTableDefinition.showMissing = XmlHelper.ReadBool(node.Attributes["showMissing"]);
		}
		cT_PivotTableDefinition.pageStyle = XmlHelper.ReadString(node.Attributes["pageStyle"]);
		cT_PivotTableDefinition.pivotTableStyle = XmlHelper.ReadString(node.Attributes["pivotTableStyle"]);
		cT_PivotTableDefinition.vacatedStyle = XmlHelper.ReadString(node.Attributes["vacatedStyle"]);
		cT_PivotTableDefinition.tag = XmlHelper.ReadString(node.Attributes["tag"]);
		if (node.Attributes["updatedVersion"] != null)
		{
			cT_PivotTableDefinition.updatedVersion = XmlHelper.ReadByte(node.Attributes["updatedVersion"]);
		}
		if (node.Attributes["minRefreshableVersion"] != null)
		{
			cT_PivotTableDefinition.minRefreshableVersion = XmlHelper.ReadByte(node.Attributes["minRefreshableVersion"]);
		}
		if (node.Attributes["asteriskTotals"] != null)
		{
			cT_PivotTableDefinition.asteriskTotals = XmlHelper.ReadBool(node.Attributes["asteriskTotals"]);
		}
		if (node.Attributes["showItems"] != null)
		{
			cT_PivotTableDefinition.showItems = XmlHelper.ReadBool(node.Attributes["showItems"]);
		}
		if (node.Attributes["editData"] != null)
		{
			cT_PivotTableDefinition.editData = XmlHelper.ReadBool(node.Attributes["editData"]);
		}
		if (node.Attributes["disableFieldList"] != null)
		{
			cT_PivotTableDefinition.disableFieldList = XmlHelper.ReadBool(node.Attributes["disableFieldList"]);
		}
		if (node.Attributes["showCalcMbrs"] != null)
		{
			cT_PivotTableDefinition.showCalcMbrs = XmlHelper.ReadBool(node.Attributes["showCalcMbrs"]);
		}
		if (node.Attributes["visualTotals"] != null)
		{
			cT_PivotTableDefinition.visualTotals = XmlHelper.ReadBool(node.Attributes["visualTotals"]);
		}
		if (node.Attributes["showMultipleLabel"] != null)
		{
			cT_PivotTableDefinition.showMultipleLabel = XmlHelper.ReadBool(node.Attributes["showMultipleLabel"]);
		}
		if (node.Attributes["showDataDropDown"] != null)
		{
			cT_PivotTableDefinition.showDataDropDown = XmlHelper.ReadBool(node.Attributes["showDataDropDown"]);
		}
		if (node.Attributes["showDrill"] != null)
		{
			cT_PivotTableDefinition.showDrill = XmlHelper.ReadBool(node.Attributes["showDrill"]);
		}
		if (node.Attributes["printDrill"] != null)
		{
			cT_PivotTableDefinition.printDrill = XmlHelper.ReadBool(node.Attributes["printDrill"]);
		}
		if (node.Attributes["showMemberPropertyTips"] != null)
		{
			cT_PivotTableDefinition.showMemberPropertyTips = XmlHelper.ReadBool(node.Attributes["showMemberPropertyTips"]);
		}
		if (node.Attributes["showDataTips"] != null)
		{
			cT_PivotTableDefinition.showDataTips = XmlHelper.ReadBool(node.Attributes["showDataTips"]);
		}
		if (node.Attributes["enableWizard"] != null)
		{
			cT_PivotTableDefinition.enableWizard = XmlHelper.ReadBool(node.Attributes["enableWizard"]);
		}
		if (node.Attributes["enableDrill"] != null)
		{
			cT_PivotTableDefinition.enableDrill = XmlHelper.ReadBool(node.Attributes["enableDrill"]);
		}
		if (node.Attributes["enableFieldProperties"] != null)
		{
			cT_PivotTableDefinition.enableFieldProperties = XmlHelper.ReadBool(node.Attributes["enableFieldProperties"]);
		}
		if (node.Attributes["preserveFormatting"] != null)
		{
			cT_PivotTableDefinition.preserveFormatting = XmlHelper.ReadBool(node.Attributes["preserveFormatting"]);
		}
		if (node.Attributes["useAutoFormatting"] != null)
		{
			cT_PivotTableDefinition.useAutoFormatting = XmlHelper.ReadBool(node.Attributes["useAutoFormatting"]);
		}
		if (node.Attributes["pageWrap"] != null)
		{
			cT_PivotTableDefinition.pageWrap = XmlHelper.ReadUInt(node.Attributes["pageWrap"]);
		}
		if (node.Attributes["pageOverThenDown"] != null)
		{
			cT_PivotTableDefinition.pageOverThenDown = XmlHelper.ReadBool(node.Attributes["pageOverThenDown"]);
		}
		if (node.Attributes["subtotalHiddenItems"] != null)
		{
			cT_PivotTableDefinition.subtotalHiddenItems = XmlHelper.ReadBool(node.Attributes["subtotalHiddenItems"]);
		}
		if (node.Attributes["rowGrandTotals"] != null)
		{
			cT_PivotTableDefinition.rowGrandTotals = XmlHelper.ReadBool(node.Attributes["rowGrandTotals"]);
		}
		if (node.Attributes["colGrandTotals"] != null)
		{
			cT_PivotTableDefinition.colGrandTotals = XmlHelper.ReadBool(node.Attributes["colGrandTotals"]);
		}
		if (node.Attributes["fieldPrintTitles"] != null)
		{
			cT_PivotTableDefinition.fieldPrintTitles = XmlHelper.ReadBool(node.Attributes["fieldPrintTitles"]);
		}
		if (node.Attributes["itemPrintTitles"] != null)
		{
			cT_PivotTableDefinition.itemPrintTitles = XmlHelper.ReadBool(node.Attributes["itemPrintTitles"]);
		}
		if (node.Attributes["mergeItem"] != null)
		{
			cT_PivotTableDefinition.mergeItem = XmlHelper.ReadBool(node.Attributes["mergeItem"]);
		}
		if (node.Attributes["showDropZones"] != null)
		{
			cT_PivotTableDefinition.showDropZones = XmlHelper.ReadBool(node.Attributes["showDropZones"]);
		}
		if (node.Attributes["createdVersion"] != null)
		{
			cT_PivotTableDefinition.createdVersion = XmlHelper.ReadByte(node.Attributes["createdVersion"]);
		}
		if (node.Attributes["indent"] != null)
		{
			cT_PivotTableDefinition.indent = XmlHelper.ReadUInt(node.Attributes["indent"]);
		}
		if (node.Attributes["showEmptyRow"] != null)
		{
			cT_PivotTableDefinition.showEmptyRow = XmlHelper.ReadBool(node.Attributes["showEmptyRow"]);
		}
		if (node.Attributes["showEmptyCol"] != null)
		{
			cT_PivotTableDefinition.showEmptyCol = XmlHelper.ReadBool(node.Attributes["showEmptyCol"]);
		}
		if (node.Attributes["showHeaders"] != null)
		{
			cT_PivotTableDefinition.showHeaders = XmlHelper.ReadBool(node.Attributes["showHeaders"]);
		}
		if (node.Attributes["compact"] != null)
		{
			cT_PivotTableDefinition.compact = XmlHelper.ReadBool(node.Attributes["compact"]);
		}
		if (node.Attributes["outline"] != null)
		{
			cT_PivotTableDefinition.outline = XmlHelper.ReadBool(node.Attributes["outline"]);
		}
		if (node.Attributes["outlineData"] != null)
		{
			cT_PivotTableDefinition.outlineData = XmlHelper.ReadBool(node.Attributes["outlineData"]);
		}
		if (node.Attributes["compactData"] != null)
		{
			cT_PivotTableDefinition.compactData = XmlHelper.ReadBool(node.Attributes["compactData"]);
		}
		if (node.Attributes["published"] != null)
		{
			cT_PivotTableDefinition.published = XmlHelper.ReadBool(node.Attributes["published"]);
		}
		if (node.Attributes["gridDropZones"] != null)
		{
			cT_PivotTableDefinition.gridDropZones = XmlHelper.ReadBool(node.Attributes["gridDropZones"]);
		}
		if (node.Attributes["immersive"] != null)
		{
			cT_PivotTableDefinition.immersive = XmlHelper.ReadBool(node.Attributes["immersive"]);
		}
		if (node.Attributes["multipleFieldFilters"] != null)
		{
			cT_PivotTableDefinition.multipleFieldFilters = XmlHelper.ReadBool(node.Attributes["multipleFieldFilters"]);
		}
		if (node.Attributes["chartFormat"] != null)
		{
			cT_PivotTableDefinition.chartFormat = XmlHelper.ReadUInt(node.Attributes["chartFormat"]);
		}
		cT_PivotTableDefinition.rowHeaderCaption = XmlHelper.ReadString(node.Attributes["rowHeaderCaption"]);
		cT_PivotTableDefinition.colHeaderCaption = XmlHelper.ReadString(node.Attributes["colHeaderCaption"]);
		if (node.Attributes["fieldListSortAscending"] != null)
		{
			cT_PivotTableDefinition.fieldListSortAscending = XmlHelper.ReadBool(node.Attributes["fieldListSortAscending"]);
		}
		if (node.Attributes["mdxSubqueries"] != null)
		{
			cT_PivotTableDefinition.mdxSubqueries = XmlHelper.ReadBool(node.Attributes["mdxSubqueries"]);
		}
		if (node.Attributes["customListSort"] != null)
		{
			cT_PivotTableDefinition.customListSort = XmlHelper.ReadBool(node.Attributes["customListSort"]);
		}
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.LocalName == "location")
			{
				cT_PivotTableDefinition.location = CT_Location.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "pivotFields")
			{
				cT_PivotTableDefinition.pivotFields = CT_PivotFields.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "rowFields")
			{
				cT_PivotTableDefinition.rowFields = CT_RowFields.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "rowItems")
			{
				cT_PivotTableDefinition.rowItems = CT_rowItems.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "colFields")
			{
				cT_PivotTableDefinition.colFields = CT_ColFields.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "colItems")
			{
				cT_PivotTableDefinition.colItems = CT_colItems.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "pageFields")
			{
				cT_PivotTableDefinition.pageFields = CT_PageFields.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "dataFields")
			{
				cT_PivotTableDefinition.dataFields = CT_DataFields.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "formats")
			{
				cT_PivotTableDefinition.formats = CT_Formats.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "conditionalFormats")
			{
				cT_PivotTableDefinition.conditionalFormats = CT_ConditionalFormats.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "chartFormats")
			{
				cT_PivotTableDefinition.chartFormats = CT_ChartFormats.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "pivotHierarchies")
			{
				cT_PivotTableDefinition.pivotHierarchies = CT_PivotHierarchies.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "pivotTableStyleInfo")
			{
				cT_PivotTableDefinition.pivotTableStyleInfo = CT_PivotTableStyle.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "filters")
			{
				cT_PivotTableDefinition.filters = CT_PivotFilters.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "rowHierarchiesUsage")
			{
				cT_PivotTableDefinition.rowHierarchiesUsage = CT_RowHierarchiesUsage.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "colHierarchiesUsage")
			{
				cT_PivotTableDefinition.colHierarchiesUsage = CT_ColHierarchiesUsage.Parse(childNode, namespaceManager);
			}
			else if (childNode.LocalName == "extLst")
			{
				cT_PivotTableDefinition.extLst = CT_ExtensionList.Parse(childNode, namespaceManager);
			}
		}
		return cT_PivotTableDefinition;
	}

	internal void Write(StreamWriter sw)
	{
		sw.Write("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
		sw.Write("<pivotTableDefinition xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" ");
		sw.Write("xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" ");
		sw.Write("xmlns:s=\"http://schemas.openxmlformats.org/officeDocument/2006/sharedTypes\" ");
		XmlHelper.WriteAttribute(sw, "name", name);
		XmlHelper.WriteAttribute(sw, "cacheId", cacheId);
		XmlHelper.WriteAttribute(sw, "dataOnRows", dataOnRows);
		XmlHelper.WriteAttribute(sw, "dataPosition", dataPosition);
		XmlHelper.WriteAttribute(sw, "autoFormatId", autoFormatId);
		XmlHelper.WriteAttribute(sw, "applyNumberFormats", applyNumberFormats);
		XmlHelper.WriteAttribute(sw, "applyBorderFormats", applyBorderFormats);
		XmlHelper.WriteAttribute(sw, "applyFontFormats", applyFontFormats);
		XmlHelper.WriteAttribute(sw, "applyPatternFormats", applyPatternFormats);
		XmlHelper.WriteAttribute(sw, "applyAlignmentFormats", applyAlignmentFormats);
		XmlHelper.WriteAttribute(sw, "applyWidthHeightFormats", applyWidthHeightFormats);
		XmlHelper.WriteAttribute(sw, "dataCaption", dataCaption);
		XmlHelper.WriteAttribute(sw, "grandTotalCaption", grandTotalCaption);
		XmlHelper.WriteAttribute(sw, "errorCaption", errorCaption);
		XmlHelper.WriteAttribute(sw, "showError", showError);
		XmlHelper.WriteAttribute(sw, "missingCaption", missingCaption);
		XmlHelper.WriteAttribute(sw, "showMissing", showMissing);
		XmlHelper.WriteAttribute(sw, "pageStyle", pageStyle);
		XmlHelper.WriteAttribute(sw, "pivotTableStyle", pivotTableStyle);
		XmlHelper.WriteAttribute(sw, "vacatedStyle", vacatedStyle);
		XmlHelper.WriteAttribute(sw, "tag", tag);
		XmlHelper.WriteAttribute(sw, "updatedVersion", updatedVersion);
		XmlHelper.WriteAttribute(sw, "minRefreshableVersion", minRefreshableVersion);
		XmlHelper.WriteAttribute(sw, "asteriskTotals", asteriskTotals);
		XmlHelper.WriteAttribute(sw, "showItems", showItems);
		XmlHelper.WriteAttribute(sw, "editData", editData);
		XmlHelper.WriteAttribute(sw, "disableFieldList", disableFieldList);
		XmlHelper.WriteAttribute(sw, "showCalcMbrs", showCalcMbrs);
		XmlHelper.WriteAttribute(sw, "visualTotals", visualTotals);
		XmlHelper.WriteAttribute(sw, "showMultipleLabel", showMultipleLabel);
		XmlHelper.WriteAttribute(sw, "showDataDropDown", showDataDropDown);
		XmlHelper.WriteAttribute(sw, "showDrill", showDrill);
		XmlHelper.WriteAttribute(sw, "printDrill", printDrill);
		XmlHelper.WriteAttribute(sw, "showMemberPropertyTips", showMemberPropertyTips);
		XmlHelper.WriteAttribute(sw, "showDataTips", showDataTips);
		XmlHelper.WriteAttribute(sw, "enableWizard", enableWizard);
		XmlHelper.WriteAttribute(sw, "enableDrill", enableDrill);
		XmlHelper.WriteAttribute(sw, "enableFieldProperties", enableFieldProperties);
		XmlHelper.WriteAttribute(sw, "preserveFormatting", preserveFormatting);
		XmlHelper.WriteAttribute(sw, "useAutoFormatting", useAutoFormatting);
		XmlHelper.WriteAttribute(sw, "pageWrap", pageWrap);
		XmlHelper.WriteAttribute(sw, "pageOverThenDown", pageOverThenDown);
		XmlHelper.WriteAttribute(sw, "subtotalHiddenItems", subtotalHiddenItems);
		XmlHelper.WriteAttribute(sw, "rowGrandTotals", rowGrandTotals);
		XmlHelper.WriteAttribute(sw, "colGrandTotals", colGrandTotals);
		XmlHelper.WriteAttribute(sw, "fieldPrintTitles", fieldPrintTitles);
		XmlHelper.WriteAttribute(sw, "itemPrintTitles", itemPrintTitles);
		XmlHelper.WriteAttribute(sw, "mergeItem", mergeItem);
		XmlHelper.WriteAttribute(sw, "showDropZones", showDropZones);
		XmlHelper.WriteAttribute(sw, "createdVersion", createdVersion);
		XmlHelper.WriteAttribute(sw, "indent", indent);
		XmlHelper.WriteAttribute(sw, "showEmptyRow", showEmptyRow);
		XmlHelper.WriteAttribute(sw, "showEmptyCol", showEmptyCol);
		XmlHelper.WriteAttribute(sw, "showHeaders", showHeaders);
		XmlHelper.WriteAttribute(sw, "compact", compact);
		XmlHelper.WriteAttribute(sw, "outline", outline);
		XmlHelper.WriteAttribute(sw, "outlineData", outlineData);
		XmlHelper.WriteAttribute(sw, "compactData", compactData);
		XmlHelper.WriteAttribute(sw, "published", published);
		XmlHelper.WriteAttribute(sw, "gridDropZones", gridDropZones);
		XmlHelper.WriteAttribute(sw, "immersive", immersive);
		XmlHelper.WriteAttribute(sw, "multipleFieldFilters", multipleFieldFilters);
		XmlHelper.WriteAttribute(sw, "chartFormat", chartFormat);
		XmlHelper.WriteAttribute(sw, "rowHeaderCaption", rowHeaderCaption);
		XmlHelper.WriteAttribute(sw, "colHeaderCaption", colHeaderCaption);
		XmlHelper.WriteAttribute(sw, "fieldListSortAscending", fieldListSortAscending);
		XmlHelper.WriteAttribute(sw, "mdxSubqueries", mdxSubqueries);
		XmlHelper.WriteAttribute(sw, "customListSort", customListSort);
		sw.Write(">");
		if (location != null)
		{
			location.Write(sw, "location");
		}
		if (pivotFields != null)
		{
			pivotFields.Write(sw, "pivotFields");
		}
		if (rowFields != null)
		{
			rowFields.Write(sw, "rowFields");
		}
		if (rowItems != null)
		{
			rowItems.Write(sw, "rowItems");
		}
		if (colFields != null)
		{
			colFields.Write(sw, "colFields");
		}
		if (colItems != null)
		{
			colItems.Write(sw, "colItems");
		}
		if (pageFields != null)
		{
			pageFields.Write(sw, "pageFields");
		}
		if (dataFields != null)
		{
			dataFields.Write(sw, "dataFields");
		}
		if (formats != null)
		{
			formats.Write(sw, "formats");
		}
		if (conditionalFormats != null)
		{
			conditionalFormats.Write(sw, "conditionalFormats");
		}
		if (chartFormats != null)
		{
			chartFormats.Write(sw, "chartFormats");
		}
		if (pivotHierarchies != null)
		{
			pivotHierarchies.Write(sw, "pivotHierarchies");
		}
		if (pivotTableStyleInfo != null)
		{
			pivotTableStyleInfo.Write(sw, "pivotTableStyleInfo");
		}
		if (filters != null)
		{
			filters.Write(sw, "filters");
		}
		if (rowHierarchiesUsage != null)
		{
			rowHierarchiesUsage.Write(sw, "rowHierarchiesUsage");
		}
		if (colHierarchiesUsage != null)
		{
			colHierarchiesUsage.Write(sw, "colHierarchiesUsage");
		}
		if (extLst != null)
		{
			extLst.Write(sw, "extLst");
		}
		sw.Write($"</pivotTableDefinition>");
	}

	public void Save(Stream stream)
	{
		using StreamWriter sw = new StreamWriter(stream);
		Write(sw);
	}

	public CT_PivotTableDefinition()
	{
		dataOnRowsField = false;
		showErrorField = false;
		showMissingField = true;
		updatedVersionField = 0;
		minRefreshableVersionField = 0;
		asteriskTotalsField = false;
		showItemsField = true;
		editDataField = false;
		disableFieldListField = false;
		showCalcMbrsField = true;
		visualTotalsField = true;
		showMultipleLabelField = true;
		showDataDropDownField = true;
		showDrillField = true;
		printDrillField = false;
		showMemberPropertyTipsField = true;
		showDataTipsField = true;
		enableWizardField = true;
		enableDrillField = true;
		enableFieldPropertiesField = true;
		preserveFormattingField = true;
		useAutoFormattingField = false;
		pageWrapField = 0u;
		pageOverThenDownField = false;
		subtotalHiddenItemsField = false;
		rowGrandTotalsField = true;
		colGrandTotalsField = true;
		fieldPrintTitlesField = false;
		itemPrintTitlesField = false;
		mergeItemField = false;
		showDropZonesField = true;
		createdVersionField = 0;
		indentField = 1u;
		showEmptyRowField = false;
		showEmptyColField = false;
		showHeadersField = true;
		compactField = true;
		outlineField = false;
		outlineDataField = false;
		compactDataField = true;
		publishedField = false;
		gridDropZonesField = false;
		immersiveField = true;
		multipleFieldFiltersField = true;
		chartFormatField = 0u;
		fieldListSortAscendingField = false;
		mdxSubqueriesField = false;
		customListSortField = true;
	}

	public CT_PivotTableStyle AddNewPivotTableStyleInfo()
	{
		pivotTableStyleInfoField = new CT_PivotTableStyle();
		return pivotTableStyleInfoField;
	}

	public CT_RowFields AddNewRowFields()
	{
		rowFieldsField = new CT_RowFields();
		return rowFieldsField;
	}

	public CT_ColFields AddNewColFields()
	{
		colFieldsField = new CT_ColFields();
		return colFieldsField;
	}

	public CT_DataFields AddNewDataFields()
	{
		dataFieldsField = new CT_DataFields();
		return dataFieldsField;
	}

	public CT_PageFields AddNewPageFields()
	{
		pageFieldsField = new CT_PageFields();
		return pageFieldsField;
	}

	public CT_PivotFields AddNewPivotFields()
	{
		pivotFieldsField = new CT_PivotFields();
		return pivotFieldsField;
	}

	public CT_Location AddNewLocation()
	{
		locationField = new CT_Location();
		return locationField;
	}
}
