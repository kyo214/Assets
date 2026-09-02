using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using NPOI.OpenXml4Net.OPC;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.XSSF.UserModel;

public class XSSFPivotTable : POIXMLDocumentPart
{
	public interface IPivotTableReferenceConfigurator
	{
		void ConfigureReference(CT_WorksheetSource wsSource);
	}

	protected internal static short CREATED_VERSION = 3;

	protected internal static short MIN_REFRESHABLE_VERSION = 3;

	protected internal static short UPDATED_VERSION = 3;

	private CT_PivotTableDefinition pivotTableDefinition;

	private XSSFPivotCacheDefinition pivotCacheDefinition;

	private XSSFPivotCache pivotCache;

	private XSSFPivotCacheRecords pivotCacheRecords;

	private ISheet parentSheet;

	private ISheet dataSheet;

	public XSSFPivotTable()
	{
		pivotTableDefinition = new CT_PivotTableDefinition();
		pivotCache = new XSSFPivotCache();
		pivotCacheDefinition = new XSSFPivotCacheDefinition();
		pivotCacheRecords = new XSSFPivotCacheRecords();
	}

	protected XSSFPivotTable(PackagePart part)
		: base(part)
	{
		ReadFrom(part.GetInputStream());
	}

	[Obsolete("deprecated in POI 3.14, scheduled for removal in POI 3.16")]
	protected XSSFPivotTable(PackagePart part, PackageRelationship rel)
		: this(part)
	{
	}

	public void ReadFrom(Stream is1)
	{
		try
		{
			XmlDocument xmlDocument = POIXMLDocumentPart.ConvertStreamToXml(is1);
			pivotTableDefinition = CT_PivotTableDefinition.Parse(xmlDocument.DocumentElement, POIXMLDocumentPart.NamespaceManager);
		}
		catch (XmlException ex)
		{
			throw new IOException(ex.Message);
		}
	}

	public void SetPivotCache(XSSFPivotCache pivotCache)
	{
		this.pivotCache = pivotCache;
	}

	public XSSFPivotCache GetPivotCache()
	{
		return pivotCache;
	}

	public ISheet GetParentSheet()
	{
		return parentSheet;
	}

	public void SetParentSheet(XSSFSheet parentSheet)
	{
		this.parentSheet = parentSheet;
	}

	public CT_PivotTableDefinition GetCTPivotTableDefinition()
	{
		return pivotTableDefinition;
	}

	public void SetCTPivotTableDefinition(CT_PivotTableDefinition pivotTableDefinition)
	{
		this.pivotTableDefinition = pivotTableDefinition;
	}

	public XSSFPivotCacheDefinition GetPivotCacheDefinition()
	{
		return pivotCacheDefinition;
	}

	public void SetPivotCacheDefinition(XSSFPivotCacheDefinition pivotCacheDefinition)
	{
		this.pivotCacheDefinition = pivotCacheDefinition;
	}

	public XSSFPivotCacheRecords GetPivotCacheRecords()
	{
		return pivotCacheRecords;
	}

	public void SetPivotCacheRecords(XSSFPivotCacheRecords pivotCacheRecords)
	{
		this.pivotCacheRecords = pivotCacheRecords;
	}

	public ISheet GetDataSheet()
	{
		return dataSheet;
	}

	private void SetDataSheet(ISheet dataSheet)
	{
		this.dataSheet = dataSheet;
	}

	protected internal override void Commit()
	{
		Stream outputStream = GetPackagePart().GetOutputStream();
		pivotTableDefinition.Save(outputStream);
		outputStream.Close();
	}

	protected internal void SetDefaultPivotTableDefinition()
	{
		pivotTableDefinition.multipleFieldFilters = false;
		pivotTableDefinition.indent = 0u;
		pivotTableDefinition.createdVersion = (byte)CREATED_VERSION;
		pivotTableDefinition.minRefreshableVersion = (byte)MIN_REFRESHABLE_VERSION;
		pivotTableDefinition.updatedVersion = (byte)UPDATED_VERSION;
		pivotTableDefinition.itemPrintTitles = true;
		pivotTableDefinition.useAutoFormatting = true;
		pivotTableDefinition.applyNumberFormats = false;
		pivotTableDefinition.applyWidthHeightFormats = true;
		pivotTableDefinition.applyAlignmentFormats = false;
		pivotTableDefinition.applyPatternFormats = false;
		pivotTableDefinition.applyFontFormats = false;
		pivotTableDefinition.applyBorderFormats = false;
		pivotTableDefinition.cacheId = pivotCache.GetCTPivotCache().cacheId;
		pivotTableDefinition.name = "PivotTable" + pivotTableDefinition.cacheId;
		pivotTableDefinition.dataCaption = "Values";
		CT_PivotTableStyle cT_PivotTableStyle = pivotTableDefinition.AddNewPivotTableStyleInfo();
		cT_PivotTableStyle.name = "PivotStyleLight16";
		cT_PivotTableStyle.showLastColumn = true;
		cT_PivotTableStyle.showColStripes = false;
		cT_PivotTableStyle.showRowStripes = false;
		cT_PivotTableStyle.showColHeaders = true;
		cT_PivotTableStyle.showRowHeaders = true;
	}

	protected AreaReference GetPivotArea()
	{
		IWorkbook workbook = GetDataSheet().Workbook;
		return GetPivotCacheDefinition().GetPivotArea(workbook);
	}

	private void CheckColumnIndex(int columnIndex)
	{
		AreaReference pivotArea = GetPivotArea();
		int num = pivotArea.LastCell.Col - pivotArea.FirstCell.Col + 1;
		if (columnIndex < 0 || columnIndex >= num)
		{
			throw new IndexOutOfRangeException("Column Index: " + columnIndex + ", Size: " + num);
		}
	}

	public void AddRowLabel(int columnIndex)
	{
		CheckColumnIndex(columnIndex);
		AreaReference pivotArea = GetPivotArea();
		int num = pivotArea.LastCell.Row - pivotArea.FirstCell.Row;
		CT_PivotFields pivotFields = pivotTableDefinition.pivotFields;
		CT_PivotField cT_PivotField = new CT_PivotField();
		CT_Items cT_Items = cT_PivotField.AddNewItems();
		cT_PivotField.axis = ST_Axis.axisRow;
		cT_PivotField.showAll = false;
		for (int i = 0; i <= num; i++)
		{
			cT_Items.AddNewItem().t = ST_ItemType.@default;
		}
		cT_Items.count = cT_Items.SizeOfItemArray();
		pivotFields.SetPivotFieldArray(columnIndex, cT_PivotField);
		CT_RowFields cT_RowFields = ((pivotTableDefinition.rowFields == null) ? pivotTableDefinition.AddNewRowFields() : pivotTableDefinition.rowFields);
		cT_RowFields.AddNewField().x = columnIndex;
		cT_RowFields.count = cT_RowFields.SizeOfFieldArray();
	}

	public IList<int> GetRowLabelColumns()
	{
		if (pivotTableDefinition.rowFields != null)
		{
			List<int> list = new List<int>();
			{
				foreach (CT_Field item in pivotTableDefinition.rowFields.GetFieldArray())
				{
					list.Add(item.x);
				}
				return list;
			}
		}
		return new List<int>();
	}

	public void AddColumnLabel(DataConsolidateFunction function, int columnIndex, string valueFieldName)
	{
		CheckColumnIndex(columnIndex);
		AddDataColumn(columnIndex, isDataField: true);
		AddDataField(function, columnIndex, valueFieldName);
		if (pivotTableDefinition.dataFields.count == 2)
		{
			CT_ColFields cT_ColFields = ((pivotTableDefinition.colFields == null) ? pivotTableDefinition.AddNewColFields() : pivotTableDefinition.colFields);
			cT_ColFields.AddNewField().x = -2;
			cT_ColFields.count = cT_ColFields.SizeOfFieldArray();
		}
	}

	public void AddColumnLabel(DataConsolidateFunction function, int columnIndex)
	{
		AddColumnLabel(function, columnIndex, function.Name);
	}

	private void AddDataField(DataConsolidateFunction function, int columnIndex, string valueFieldName)
	{
		CheckColumnIndex(columnIndex);
		AreaReference pivotArea = GetPivotArea();
		CT_DataFields cT_DataFields = ((pivotTableDefinition.dataFields == null) ? pivotTableDefinition.AddNewDataFields() : pivotTableDefinition.dataFields);
		CT_DataField cT_DataField = cT_DataFields.AddNewDataField();
		cT_DataField.subtotal = (ST_DataConsolidateFunction)function.Value;
		GetDataSheet().GetRow(pivotArea.FirstCell.Row).GetCell(pivotArea.FirstCell.Col + columnIndex).SetCellType(CellType.String);
		cT_DataField.name = valueFieldName;
		cT_DataField.fld = (uint)columnIndex;
		cT_DataFields.count = cT_DataFields.SizeOfDataFieldArray();
	}

	public void AddDataColumn(int columnIndex, bool isDataField)
	{
		CheckColumnIndex(columnIndex);
		pivotTableDefinition.pivotFields.SetPivotFieldArray(columnIndex, new CT_PivotField
		{
			dataField = isDataField,
			showAll = false
		});
	}

	public void AddReportFilter(int columnIndex)
	{
		CheckColumnIndex(columnIndex);
		AreaReference pivotArea = GetPivotArea();
		int num = pivotArea.LastCell.Row - pivotArea.FirstCell.Row;
		CT_PivotFields pivotFields = pivotTableDefinition.pivotFields;
		CT_PivotField cT_PivotField = new CT_PivotField();
		CT_Items cT_Items = cT_PivotField.AddNewItems();
		cT_PivotField.axis = ST_Axis.axisPage;
		cT_PivotField.showAll = false;
		for (int i = 0; i <= num; i++)
		{
			cT_Items.AddNewItem().t = ST_ItemType.@default;
		}
		cT_Items.count = cT_Items.SizeOfItemArray();
		pivotFields.SetPivotFieldArray(columnIndex, cT_PivotField);
		CT_PageFields cT_PageFields;
		if (pivotTableDefinition.pageFields != null)
		{
			cT_PageFields = pivotTableDefinition.pageFields;
			pivotTableDefinition.multipleFieldFilters = true;
		}
		else
		{
			cT_PageFields = pivotTableDefinition.AddNewPageFields();
		}
		CT_PageField cT_PageField = cT_PageFields.AddNewPageField();
		cT_PageField.hier = -1;
		cT_PageField.fld = columnIndex;
		cT_PageFields.count = cT_PageFields.SizeOfPageFieldArray();
		pivotTableDefinition.location.colPageCount = cT_PageFields.count;
	}

	protected internal void CreateSourceReferences(CellReference position, ISheet sourceSheet, IPivotTableReferenceConfigurator refConfig)
	{
		AreaReference areaReference = new AreaReference(position, new CellReference(position.Row + 1, position.Col + 1));
		CT_Location cT_Location;
		if (pivotTableDefinition.location == null)
		{
			cT_Location = pivotTableDefinition.AddNewLocation();
			cT_Location.firstDataCol = 1u;
			cT_Location.firstDataRow = 1u;
			cT_Location.firstHeaderRow = 1u;
		}
		else
		{
			cT_Location = pivotTableDefinition.location;
		}
		cT_Location.@ref = areaReference.FormatAsString();
		pivotTableDefinition.location = cT_Location;
		CT_CacheSource cT_CacheSource = GetPivotCacheDefinition().GetCTPivotCacheDefinition().AddNewCacheSource();
		cT_CacheSource.type = ST_SourceType.worksheet;
		CT_WorksheetSource cT_WorksheetSource = cT_CacheSource.AddNewWorksheetSource();
		cT_WorksheetSource.sheet = sourceSheet.SheetName;
		SetDataSheet(sourceSheet);
		refConfig.ConfigureReference(cT_WorksheetSource);
		if (cT_WorksheetSource.name == null && cT_WorksheetSource.@ref == null)
		{
			throw new ArgumentException("Pivot table source area reference or name must be specified.");
		}
	}

	protected internal void CreateDefaultDataColumns()
	{
		CT_PivotFields cT_PivotFields = ((pivotTableDefinition.pivotFields == null) ? pivotTableDefinition.AddNewPivotFields() : pivotTableDefinition.pivotFields);
		AreaReference pivotArea = GetPivotArea();
		int col = pivotArea.FirstCell.Col;
		int col2 = pivotArea.LastCell.Col;
		for (int i = col; i <= col2; i++)
		{
			CT_PivotField cT_PivotField = cT_PivotFields.AddNewPivotField();
			cT_PivotField.dataField = false;
			cT_PivotField.showAll = false;
		}
		cT_PivotFields.count = cT_PivotFields.SizeOfPivotFieldArray();
	}
}
