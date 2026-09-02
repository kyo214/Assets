using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using NPOI.DDF;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record;
using NPOI.HSSF.Record.Aggregates;
using NPOI.HSSF.Record.AutoFilter;
using NPOI.HSSF.UserModel.helpers;
using NPOI.HSSF.Util;
using NPOI.SS;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.HSSF.UserModel;

[Serializable]
public class HSSFSheet : ISheet
{
	private class RecordVisitor1 : RecordVisitor
	{
		private List<IDataValidation> hssfValidations;

		private IWorkbook workbook;

		private HSSFEvaluationWorkbook book;

		public RecordVisitor1(List<IDataValidation> hssfValidations, IWorkbook workbook)
		{
			this.hssfValidations = hssfValidations;
			this.workbook = workbook;
			book = HSSFEvaluationWorkbook.Create(workbook);
		}

		public void VisitRecord(NPOI.HSSF.Record.Record r)
		{
			if (r is DVRecord)
			{
				DVRecord dVRecord = (DVRecord)r;
				CellRangeAddressList regions = dVRecord.CellRangeAddress.Copy();
				DVConstraint constraint = DVConstraint.CreateDVConstraint(dVRecord, book);
				HSSFDataValidation hSSFDataValidation = new HSSFDataValidation(regions, constraint);
				hSSFDataValidation.ErrorStyle = dVRecord.ErrorStyle;
				hSSFDataValidation.EmptyCellAllowed = dVRecord.EmptyCellAllowed;
				hSSFDataValidation.SuppressDropDownArrow = dVRecord.SuppressDropdownArrow;
				hSSFDataValidation.CreatePromptBox(dVRecord.PromptTitle, dVRecord.PromptText);
				hSSFDataValidation.ShowPromptBox = dVRecord.ShowPromptOnCellSelected;
				hSSFDataValidation.CreateErrorBox(dVRecord.ErrorTitle, dVRecord.ErrorText);
				hSSFDataValidation.ShowErrorBox = dVRecord.ShowErrorOnInvalidValue;
				hssfValidations.Add(hSSFDataValidation);
			}
		}
	}

	private class Int32Comparer : IComparer<int>
	{
		public int Compare(int x, int y)
		{
			if (x < y)
			{
				return 1;
			}
			if (x > y)
			{
				return -1;
			}
			return 0;
		}
	}

	private static float PX_DEFAULT = 32f;

	private static float PX_MODIFIED = 36.56f;

	public const int INITIAL_CAPACITY = 20;

	private InternalSheet _sheet;

	private Dictionary<int, IRow> rows;

	public InternalWorkbook book;

	protected HSSFWorkbook _workbook;

	private int firstrow;

	private int lastrow;

	[NonSerialized]
	private HSSFPatriarch _patriarch;

	public bool DisplayZeros
	{
		get
		{
			return _sheet.WindowTwo.DisplayZeros;
		}
		set
		{
			_sheet.WindowTwo.DisplayZeros = value;
		}
	}

	public int PhysicalNumberOfRows => rows.Count;

	public int FirstRowNum => firstrow;

	public int LastRowNum => lastrow;

	public int DefaultColumnWidth
	{
		get
		{
			return _sheet.DefaultColumnWidth;
		}
		set
		{
			_sheet.DefaultColumnWidth = value;
		}
	}

	public short DefaultRowHeight
	{
		get
		{
			return _sheet.DefaultRowHeight;
		}
		set
		{
			_sheet.DefaultRowHeight = value;
		}
	}

	public float DefaultRowHeightInPoints
	{
		get
		{
			return (float)((double)_sheet.DefaultRowHeight / 20.0);
		}
		set
		{
			_sheet.DefaultRowHeight = (short)((double)value * 20.0);
		}
	}

	[Obsolete("Please use IsPrintGridlines instead")]
	public bool IsGridsPrinted
	{
		get
		{
			return _sheet.IsGridsPrinted;
		}
		set
		{
			_sheet.IsGridsPrinted = value;
		}
	}

	public bool ForceFormulaRecalculation
	{
		get
		{
			return _sheet.IsUncalced;
		}
		set
		{
			_sheet.IsUncalced = value;
		}
	}

	public bool VerticallyCenter
	{
		get
		{
			return _sheet.PageSettings.VCenter.VCenter;
		}
		set
		{
			_sheet.PageSettings.VCenter.VCenter = value;
		}
	}

	public bool HorizontallyCenter
	{
		get
		{
			return _sheet.PageSettings.HCenter.HCenter;
		}
		set
		{
			_sheet.PageSettings.HCenter.HCenter = value;
		}
	}

	public int NumMergedRegions => _sheet.NumMergedRegions;

	public InternalSheet Sheet => _sheet;

	public bool AlternativeExpression
	{
		get
		{
			return ((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).AlternateExpression;
		}
		set
		{
			((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).AlternateExpression = value;
		}
	}

	public bool AlternativeFormula
	{
		get
		{
			return ((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).AlternateFormula;
		}
		set
		{
			((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).AlternateFormula = value;
		}
	}

	public bool Autobreaks
	{
		get
		{
			return ((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).Autobreaks;
		}
		set
		{
			((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).Autobreaks = value;
		}
	}

	public bool Dialog
	{
		get
		{
			return ((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).Dialog;
		}
		set
		{
			((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).Dialog = value;
		}
	}

	public bool DisplayGuts
	{
		get
		{
			return ((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).DisplayGuts;
		}
		set
		{
			((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).DisplayGuts = value;
		}
	}

	public bool FitToPage
	{
		get
		{
			return ((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).FitToPage;
		}
		set
		{
			((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).FitToPage = value;
		}
	}

	public bool RowSumsBelow
	{
		get
		{
			return ((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).RowSumsBelow;
		}
		set
		{
			((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).RowSumsBelow = value;
		}
	}

	public bool RowSumsRight
	{
		get
		{
			return ((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).RowSumsRight;
		}
		set
		{
			((WSBoolRecord)_sheet.FindFirstRecordBySid(129)).RowSumsRight = value;
		}
	}

	public bool IsPrintGridlines
	{
		get
		{
			return Sheet.PrintGridlines.PrintGridlines;
		}
		set
		{
			Sheet.PrintGridlines.PrintGridlines = value;
		}
	}

	public bool IsPrintRowAndColumnHeadings
	{
		get
		{
			return Sheet.PrintHeaders.PrintHeaders;
		}
		set
		{
			Sheet.PrintHeaders.PrintHeaders = value;
		}
	}

	public IPrintSetup PrintSetup => new HSSFPrintSetup(_sheet.PageSettings.PrintSetup);

	public IHeader Header => new HSSFHeader(_sheet.PageSettings);

	public IFooter Footer => new HSSFFooter(_sheet.PageSettings);

	public bool IsRightToLeft
	{
		get
		{
			return _sheet.WindowTwo.Arabic;
		}
		set
		{
			_sheet.WindowTwo.Arabic = value;
		}
	}

	public bool IsSelected
	{
		get
		{
			return Sheet.GetWindowTwo().IsSelected;
		}
		set
		{
			Sheet.GetWindowTwo().IsSelected = value;
		}
	}

	public bool IsActive
	{
		get
		{
			return Sheet.GetWindowTwo().IsActive;
		}
		set
		{
			Sheet.GetWindowTwo().IsActive = value;
		}
	}

	private WorksheetProtectionBlock ProtectionBlock => _sheet.ProtectionBlock;

	public bool Protect => ProtectionBlock.IsSheetProtected;

	public short Password => (short)ProtectionBlock.PasswordHash;

	public bool ObjectProtect => ProtectionBlock.IsObjectProtected;

	public bool ScenarioProtect => ProtectionBlock.IsScenarioProtected;

	public short TopRow
	{
		get
		{
			return _sheet.TopRow;
		}
		set
		{
			_sheet.TopRow = value;
		}
	}

	public short LeftCol
	{
		get
		{
			return _sheet.LeftCol;
		}
		set
		{
			_sheet.LeftCol = value;
		}
	}

	public PaneInformation PaneInformation => Sheet.PaneInformation;

	public bool DisplayGridlines
	{
		get
		{
			return _sheet.DisplayGridlines;
		}
		set
		{
			_sheet.DisplayGridlines = value;
		}
	}

	public bool DisplayFormulas
	{
		get
		{
			return _sheet.DisplayFormulas;
		}
		set
		{
			_sheet.DisplayFormulas = value;
		}
	}

	public bool DisplayRowColHeadings
	{
		get
		{
			return _sheet.DisplayRowColHeadings;
		}
		set
		{
			_sheet.DisplayRowColHeadings = value;
		}
	}

	public int[] RowBreaks => _sheet.PageSettings.RowBreaks;

	public int[] ColumnBreaks => _sheet.PageSettings.ColumnBreaks;

	public EscherAggregate DrawingEscherAggregate
	{
		get
		{
			book.FindDrawingGroup();
			if (book.DrawingManager == null)
			{
				return null;
			}
			if (_sheet.AggregateDrawingRecords(book.DrawingManager, CreateIfMissing: false) == -1)
			{
				return null;
			}
			return (EscherAggregate)_sheet.FindFirstRecordBySid(9876);
		}
	}

	public IDrawing DrawingPatriarch
	{
		get
		{
			_patriarch = GetPatriarch(createIfMissing: false);
			return _patriarch;
		}
	}

	public short TabColorIndex
	{
		get
		{
			return _sheet.TabColorIndex;
		}
		set
		{
			_sheet.TabColorIndex = value;
		}
	}

	public bool IsAutoTabColor
	{
		get
		{
			return _sheet.IsAutoTabColor;
		}
		set
		{
			_sheet.IsAutoTabColor = value;
		}
	}

	public List<CellRangeAddress> MergedRegions
	{
		get
		{
			List<CellRangeAddress> list = new List<CellRangeAddress>();
			int numMergedRegions = _sheet.NumMergedRegions;
			for (int i = 0; i < numMergedRegions; i++)
			{
				list.Add(_sheet.GetMergedRegionAt(i));
			}
			return list;
		}
	}

	public ISheetConditionalFormatting SheetConditionalFormatting => new HSSFSheetConditionalFormatting(this);

	public IList DVRecords
	{
		get
		{
			IList list = new ArrayList();
			IList records = _sheet.Records;
			for (int i = 0; i < records.Count; i++)
			{
				if (records[i] is DVRecord)
				{
					list.Add(records[i]);
				}
			}
			return list;
		}
	}

	public IWorkbook Workbook => _workbook;

	public string SheetName
	{
		get
		{
			IWorkbook workbook = Workbook;
			int sheetIndex = workbook.GetSheetIndex(this);
			return workbook.GetSheetName(sheetIndex);
		}
	}

	public CellRangeAddress RepeatingRows
	{
		get
		{
			return GetRepeatingRowsOrColums(rows: true);
		}
		set
		{
			CellRangeAddress repeatingColumns = RepeatingColumns;
			SetRepeatingRowsAndColumns(value, repeatingColumns);
		}
	}

	public CellRangeAddress RepeatingColumns
	{
		get
		{
			return GetRepeatingRowsOrColums(rows: false);
		}
		set
		{
			CellRangeAddress repeatingRows = RepeatingRows;
			SetRepeatingRowsAndColumns(repeatingRows, value);
		}
	}

	public CellAddress ActiveCell
	{
		get
		{
			int activeCellRow = _sheet.ActiveCellRow;
			int activeCellCol = _sheet.ActiveCellCol;
			return new CellAddress(activeCellRow, activeCellCol);
		}
		set
		{
			int row = value.Row;
			short activeCellCol = (short)value.Column;
			_sheet.ActiveCellRow = row;
			_sheet.ActiveCellCol = activeCellCol;
		}
	}

	public HSSFSheet(HSSFWorkbook workbook)
	{
		_sheet = InternalSheet.CreateSheet();
		rows = new Dictionary<int, IRow>();
		_workbook = workbook;
		book = workbook.Workbook;
	}

	public HSSFSheet(HSSFWorkbook workbook, InternalSheet sheet)
	{
		_sheet = sheet;
		rows = new Dictionary<int, IRow>();
		_workbook = workbook;
		book = _workbook.Workbook;
		SetPropertiesFromSheet(_sheet);
	}

	public ISheet CloneSheet(HSSFWorkbook workbook)
	{
		_ = DrawingPatriarch;
		HSSFSheet hSSFSheet = new HSSFSheet(workbook, _sheet.CloneSheet());
		int index = hSSFSheet._sheet.FindFirstRecordLocBySid(236);
		DrawingRecord drawingRecord = (DrawingRecord)hSSFSheet._sheet.FindFirstRecordBySid(236);
		if (drawingRecord != null)
		{
			hSSFSheet._sheet.Records.Remove(drawingRecord);
		}
		if (DrawingPatriarch != null)
		{
			HSSFPatriarch hSSFPatriarch = HSSFPatriarch.CreatePatriarch(DrawingPatriarch as HSSFPatriarch, hSSFSheet);
			hSSFSheet._sheet.Records.Insert(index, hSSFPatriarch.GetBoundAggregate());
			hSSFSheet._patriarch = hSSFPatriarch;
		}
		return hSSFSheet;
	}

	internal void PreSerialize()
	{
		if (_patriarch != null)
		{
			_patriarch.PreSerialize();
		}
	}

	public IRow CopyRow(int sourceIndex, int targetIndex)
	{
		return SheetUtil.CopyRow(this, sourceIndex, targetIndex);
	}

	private void SetPropertiesFromSheet(InternalSheet sheet)
	{
		for (RowRecord nextRow = sheet.NextRow; nextRow != null; nextRow = sheet.NextRow)
		{
			CreateRowFromRecord(nextRow);
		}
		IEnumerator<CellValueRecordInterface> cellValueIterator = sheet.GetCellValueIterator();
		_ = DateTime.Now.Millisecond;
		HSSFRow hSSFRow = null;
		while (cellValueIterator.MoveNext())
		{
			CellValueRecordInterface current = cellValueIterator.Current;
			_ = DateTime.Now.Millisecond;
			HSSFRow hSSFRow2 = hSSFRow;
			if (hSSFRow == null || hSSFRow.RowNum != current.Row)
			{
				hSSFRow2 = (HSSFRow)GetRow(current.Row);
				if (hSSFRow2 == null)
				{
					RowRecord row = new RowRecord(current.Row);
					_sheet.AddRow(row);
					hSSFRow2 = CreateRowFromRecord(row);
				}
			}
			if (hSSFRow2 != null)
			{
				hSSFRow = hSSFRow2;
				hSSFRow2.CreateCellFromRecord(current);
			}
			else
			{
				current = null;
			}
		}
	}

	public IRow CreateRow(int rownum)
	{
		HSSFRow hSSFRow = new HSSFRow(_workbook, this, rownum);
		hSSFRow.Height = DefaultRowHeight;
		hSSFRow.RowRecord.BadFontHeight = false;
		AddRow(hSSFRow, addLow: true);
		return hSSFRow;
	}

	private HSSFRow CreateRowFromRecord(RowRecord row)
	{
		HSSFRow hSSFRow = new HSSFRow(_workbook, this, row);
		AddRow(hSSFRow, addLow: false);
		return hSSFRow;
	}

	public void RemoveRow(IRow row)
	{
		HSSFRow hSSFRow = (HSSFRow)row;
		if (row.Sheet != this)
		{
			throw new ArgumentException("Specified row does not belong to this sheet");
		}
		foreach (HSSFCell item in row)
		{
			if (item.IsPartOfArrayFormulaGroup)
			{
				string msg = "Row[rownum=" + row.RowNum + "] contains cell(s) included in a multi-cell array formula. You cannot change part of an array.";
				item.NotifyArrayFormulaChanging(msg);
			}
		}
		if (rows.Count <= 0)
		{
			return;
		}
		int rowNum = row.RowNum;
		HSSFRow hSSFRow2 = (HSSFRow)rows[rowNum];
		rows.Remove(rowNum);
		if (hSSFRow2 != row)
		{
			if (hSSFRow2 != null)
			{
				rows[rowNum] = hSSFRow2;
			}
			throw new InvalidOperationException("Specified row does not belong to this _sheet");
		}
		if (hSSFRow.RowNum == LastRowNum)
		{
			lastrow = FindLastRow(lastrow);
		}
		if (hSSFRow.RowNum == FirstRowNum)
		{
			firstrow = FindFirstRow(firstrow);
		}
		_sheet.RemoveRow(hSSFRow.RowRecord);
	}

	private int FindLastRow(int lastrow)
	{
		if (lastrow < 1)
		{
			return 0;
		}
		int num = lastrow - 1;
		IRow row = GetRow(num);
		while (row == null && num > 0)
		{
			row = GetRow(--num);
		}
		if (row == null)
		{
			return 0;
		}
		return num;
	}

	private int FindFirstRow(int firstrow)
	{
		int num = firstrow + 1;
		IRow row = GetRow(num);
		while (row == null && num <= LastRowNum)
		{
			row = GetRow(++num);
		}
		if (num > LastRowNum)
		{
			return 0;
		}
		return num;
	}

	private void AddRow(HSSFRow row, bool addLow)
	{
		rows[row.RowNum] = row;
		if (addLow)
		{
			_sheet.AddRow(row.RowRecord);
		}
		bool flag = rows.Count == 1;
		if ((row.RowNum > LastRowNum) | flag)
		{
			lastrow = row.RowNum;
		}
		if ((row.RowNum < FirstRowNum) | flag)
		{
			firstrow = row.RowNum;
		}
	}

	public ICellStyle GetColumnStyle(int column)
	{
		short xFIndexForColAt = _sheet.GetXFIndexForColAt((short)column);
		if (xFIndexForColAt == 15)
		{
			return null;
		}
		ExtendedFormatRecord exFormatAt = book.GetExFormatAt(xFIndexForColAt);
		return new HSSFCellStyle(xFIndexForColAt, exFormatAt, book);
	}

	public IRow GetRow(int rowIndex)
	{
		if (!rows.ContainsKey(rowIndex))
		{
			return null;
		}
		return (HSSFRow)rows[rowIndex];
	}

	public List<IDataValidation> GetDataValidations()
	{
		DataValidityTable orCreateDataValidityTable = _sheet.GetOrCreateDataValidityTable();
		List<IDataValidation> list = new List<IDataValidation>();
		RecordVisitor rv = new RecordVisitor1(list, Workbook);
		orCreateDataValidityTable.VisitContainedRecords(rv);
		return list;
	}

	public void AddValidationData(IDataValidation dataValidation)
	{
		if (dataValidation == null)
		{
			throw new ArgumentException("objValidation must not be null");
		}
		HSSFDataValidation hSSFDataValidation = (HSSFDataValidation)dataValidation;
		DataValidityTable orCreateDataValidityTable = _sheet.GetOrCreateDataValidityTable();
		DVRecord dvRecord = hSSFDataValidation.CreateDVRecord(this);
		orCreateDataValidityTable.AddDataValidation(dvRecord);
	}

	public void SetColumnHidden(int column, bool hidden)
	{
		_sheet.SetColumnHidden(column, hidden);
	}

	public bool IsColumnHidden(int column)
	{
		return _sheet.IsColumnHidden(column);
	}

	public void SetColumnWidth(int column, int width)
	{
		_sheet.SetColumnWidth(column, width);
	}

	public int GetColumnWidth(int column)
	{
		return _sheet.GetColumnWidth(column);
	}

	public float GetColumnWidthInPixels(int column)
	{
		int columnWidth = GetColumnWidth(column);
		int num = DefaultColumnWidth * 256;
		float num2 = ((columnWidth == num) ? PX_DEFAULT : PX_MODIFIED);
		return (float)columnWidth / num2;
	}

	public int AddMergedRegion(CellRangeAddress region)
	{
		return AddMergedRegion(region, validate: true);
	}

	public int AddMergedRegionUnsafe(CellRangeAddress region)
	{
		return AddMergedRegion(region, validate: false);
	}

	public void ValidateMergedRegions()
	{
		CheckForMergedRegionsIntersectingArrayFormulas();
		CheckForIntersectingMergedRegions();
	}

	private int AddMergedRegion(CellRangeAddress region, bool validate)
	{
		if (region.NumberOfCells < 2)
		{
			throw new ArgumentException("Merged region " + region.FormatAsString() + " must contain 2 or more cells");
		}
		if (validate)
		{
			region.Validate(SpreadsheetVersion.EXCEL97);
			ValidateArrayFormulas(region);
			ValidateMergedRegions(region);
		}
		return _sheet.AddMergedRegion(region.FirstRow, region.FirstColumn, region.LastRow, region.LastColumn);
	}

	private void ValidateArrayFormulas(CellRangeAddress region)
	{
		int firstRow = region.FirstRow;
		int firstColumn = region.FirstColumn;
		int lastRow = region.LastRow;
		int lastColumn = region.LastColumn;
		for (int i = firstRow; i <= lastRow; i++)
		{
			HSSFRow hSSFRow = (HSSFRow)GetRow(i);
			if (hSSFRow == null)
			{
				continue;
			}
			for (int j = firstColumn; j <= lastColumn; j++)
			{
				HSSFCell hSSFCell = (HSSFCell)hSSFRow.GetCell(j);
				if (hSSFCell != null && hSSFCell.IsPartOfArrayFormulaGroup)
				{
					CellRangeAddress arrayFormulaRange = hSSFCell.ArrayFormulaRange;
					if (arrayFormulaRange.NumberOfCells > 1 && region.Intersects(arrayFormulaRange))
					{
						throw new InvalidOperationException("The range " + region.FormatAsString() + " intersects with a multi-cell array formula. You cannot merge cells of an array.");
					}
				}
			}
		}
	}

	private void CheckForMergedRegionsIntersectingArrayFormulas()
	{
		foreach (CellRangeAddress mergedRegion in MergedRegions)
		{
			ValidateArrayFormulas(mergedRegion);
		}
	}

	private void ValidateMergedRegions(CellRangeAddress candidateRegion)
	{
		foreach (CellRangeAddress mergedRegion in MergedRegions)
		{
			if (mergedRegion.Intersects(candidateRegion))
			{
				throw new InvalidOperationException("Cannot add merged region " + candidateRegion.FormatAsString() + " to sheet because it overlaps with an existing merged region (" + mergedRegion.FormatAsString() + ").");
			}
		}
	}

	private void CheckForIntersectingMergedRegions()
	{
		List<CellRangeAddress> mergedRegions = MergedRegions;
		int count = mergedRegions.Count;
		for (int i = 0; i < count; i++)
		{
			CellRangeAddress cellRangeAddress = mergedRegions[i];
			foreach (CellRangeAddress item in mergedRegions.GetRange(i + 1, mergedRegions.Count - i - 1))
			{
				if (cellRangeAddress.Intersects(item))
				{
					throw new InvalidOperationException("The range " + cellRangeAddress.FormatAsString() + " intersects with another merged region " + item.FormatAsString() + " in this sheet");
				}
			}
		}
	}

	public void RemoveMergedRegion(int index)
	{
		_sheet.RemoveMergedRegion(index);
	}

	public void RemoveMergedRegions(IList<int> indices)
	{
		foreach (int item in new SortedSet<int>(indices, new Int32Comparer()))
		{
			_sheet.RemoveMergedRegion(item);
		}
	}

	public IEnumerator GetRowEnumerator()
	{
		return GetEnumerator();
	}

	public IEnumerator GetEnumerator()
	{
		return rows.Values.GetEnumerator();
	}

	public void SetActiveCell(int row, int column)
	{
		_sheet.SetActiveCellRange(row, row, column, column);
	}

	public void SetActiveCellRange(int firstRow, int lastRow, int firstColumn, int lastColumn)
	{
		_sheet.SetActiveCellRange(firstRow, lastRow, firstColumn, lastColumn);
	}

	public void SetActiveCellRange(List<CellRangeAddress8Bit> cellranges, int activeRange, int activeRow, int activeColumn)
	{
		_sheet.SetActiveCellRange(cellranges, activeRange, activeRow, activeColumn);
	}

	public void SetActive(bool sel)
	{
		Sheet.WindowTwo.IsActive = sel;
	}

	public void ProtectSheet(string password)
	{
		ProtectionBlock.ProtectSheet(password, shouldProtectObjects: true, shouldProtectScenarios: true);
	}

	[Obsolete("deprecated 2015-11-23 (circa POI 3.14beta1). Use {@link #setZoom(int)} instead.")]
	public void SetZoom(int numerator, int denominator)
	{
		if (numerator < 1 || numerator > 65535)
		{
			throw new ArgumentException("Numerator must be greater than 0 and less than 65536");
		}
		if (denominator < 1 || denominator > 65535)
		{
			throw new ArgumentException("Denominator must be greater than 0 and less than 65536");
		}
		SCLRecord sCLRecord = new SCLRecord();
		sCLRecord.Numerator = (short)numerator;
		sCLRecord.Denominator = (short)denominator;
		Sheet.SetSCLRecord(sCLRecord);
	}

	public void SetZoom(int scale)
	{
		SetZoom(scale, 100);
	}

	public void SetEnclosedBorderOfRegion(CellRangeAddress region, BorderStyle borderType, short color)
	{
		HSSFRegionUtil.SetRightBorderColor(color, region, this, _workbook);
		HSSFRegionUtil.SetBorderRight(borderType, region, this, _workbook);
		HSSFRegionUtil.SetLeftBorderColor(color, region, this, _workbook);
		HSSFRegionUtil.SetBorderLeft(borderType, region, this, _workbook);
		HSSFRegionUtil.SetTopBorderColor(color, region, this, _workbook);
		HSSFRegionUtil.SetBorderTop(borderType, region, this, _workbook);
		HSSFRegionUtil.SetBottomBorderColor(color, region, this, _workbook);
		HSSFRegionUtil.SetBorderBottom(borderType, region, this, _workbook);
	}

	public void SetBorderRightOfRegion(CellRangeAddress region, BorderStyle borderType, short color)
	{
		HSSFRegionUtil.SetRightBorderColor(color, region, this, _workbook);
		HSSFRegionUtil.SetBorderRight(borderType, region, this, _workbook);
	}

	public void SetBorderLeftOfRegion(CellRangeAddress region, BorderStyle borderType, short color)
	{
		HSSFRegionUtil.SetLeftBorderColor(color, region, this, _workbook);
		HSSFRegionUtil.SetBorderLeft(borderType, region, this, _workbook);
	}

	public void SetBorderTopOfRegion(CellRangeAddress region, BorderStyle borderType, short color)
	{
		HSSFRegionUtil.SetTopBorderColor(color, region, this, _workbook);
		HSSFRegionUtil.SetBorderTop(borderType, region, this, _workbook);
	}

	public void SetBorderBottomOfRegion(CellRangeAddress region, BorderStyle borderType, short color)
	{
		HSSFRegionUtil.SetBottomBorderColor(color, region, this, _workbook);
		HSSFRegionUtil.SetBorderBottom(borderType, region, this, _workbook);
	}

	public void ShowInPane(int toprow, int leftcol)
	{
		int lastRowIndex = SpreadsheetVersion.EXCEL97.LastRowIndex;
		if (toprow > lastRowIndex)
		{
			throw new ArgumentException("Maximum row number is " + lastRowIndex);
		}
		ShowInPane((short)toprow, (short)leftcol);
	}

	public void ShowInPane(short toprow, short leftcol)
	{
		_sheet.TopRow = toprow;
		_sheet.LeftCol = leftcol;
	}

	[Obsolete("deprecated POI 3.15 beta 2. This will be made private in future releases.")]
	protected void ShiftMerged(int startRow, int endRow, int n, bool IsRow)
	{
		new HSSFRowShifter(this).ShiftMergedRegions(startRow, endRow, n);
	}

	public void ShiftRows(int startRow, int endRow, int n)
	{
		ShiftRows(startRow, endRow, n, copyRowHeight: false, resetOriginalRowHeight: false);
	}

	public void ShiftRows(int startRow, int endRow, int n, bool copyRowHeight, bool resetOriginalRowHeight)
	{
		ShiftRows(startRow, endRow, n, copyRowHeight, resetOriginalRowHeight, moveComments: true);
	}

	public void ShiftRows(int startRow, int endRow, int n, bool copyRowHeight, bool resetOriginalRowHeight, bool moveComments)
	{
		if (endRow < startRow)
		{
			throw new ArgumentException("startRow must be less than or equal to endRow. To shift rows up, use n<0.");
		}
		int num;
		int num2;
		if (n < 0)
		{
			num = startRow;
			num2 = 1;
		}
		else
		{
			if (n <= 0)
			{
				return;
			}
			num = endRow;
			num2 = -1;
		}
		if (moveComments)
		{
			_sheet.GetNoteRecords();
		}
		else
		{
			_ = NoteRecord.EMPTY_ARRAY;
		}
		new HSSFRowShifter(this).ShiftMergedRegions(startRow, endRow, n);
		_sheet.PageSettings.ShiftRowBreaks(startRow, endRow, n);
		int num3 = startRow + n;
		int num4 = endRow + n;
		foreach (HSSFHyperlink hyperlink2 in GetHyperlinkList())
		{
			if (num3 <= hyperlink2.FirstRow && hyperlink2.FirstRow <= num4 && num4 <= hyperlink2.LastRow && hyperlink2.LastRow <= num4)
			{
				RemoveHyperlink(hyperlink2);
			}
		}
		for (int i = num; i >= startRow && i <= endRow && i >= 0 && i < 65536; i += num2)
		{
			HSSFRow hSSFRow = (HSSFRow)GetRow(i);
			if (hSSFRow != null)
			{
				NotifyRowShifting(hSSFRow);
			}
			HSSFRow hSSFRow2 = (HSSFRow)GetRow(i + n);
			if (hSSFRow2 == null)
			{
				hSSFRow2 = (HSSFRow)CreateRow(i + n);
			}
			hSSFRow2.RemoveAllCells();
			if (hSSFRow == null)
			{
				continue;
			}
			if (copyRowHeight)
			{
				hSSFRow2.Height = hSSFRow.Height;
			}
			if (resetOriginalRowHeight)
			{
				hSSFRow.Height = 255;
			}
			foreach (ICell cell in hSSFRow.Cells)
			{
				hSSFRow.RemoveCell(cell);
				IHyperlink hyperlink = cell.Hyperlink;
				CellValueRecordInterface cellValueRecord = ((HSSFCell)cell).CellValueRecord;
				cellValueRecord.Row = i + n;
				hSSFRow2.CreateCellFromRecord(cellValueRecord);
				_sheet.AddValueRecord(i + n, cellValueRecord);
				if (hyperlink != null)
				{
					hyperlink.FirstRow += n;
					hyperlink.LastRow += n;
				}
			}
			hSSFRow.RemoveAllCells();
			if (!moveComments)
			{
				continue;
			}
			HSSFPatriarch hSSFPatriarch = CreateDrawingPatriarch() as HSSFPatriarch;
			for (int num5 = hSSFPatriarch.Children.Count - 1; num5 >= 0; num5--)
			{
				HSSFShape hSSFShape = hSSFPatriarch.Children[num5];
				if (hSSFShape is HSSFComment)
				{
					HSSFComment hSSFComment = (HSSFComment)hSSFShape;
					if (hSSFComment.Row == i)
					{
						hSSFComment.Row = i + n;
					}
				}
			}
		}
		if (n > 0)
		{
			if (startRow == firstrow)
			{
				firstrow = Math.Max(startRow + n, 0);
				for (int j = startRow + 1; j < startRow + n; j++)
				{
					if (GetRow(j) != null)
					{
						firstrow = j;
						break;
					}
				}
			}
			if (endRow + n > lastrow)
			{
				lastrow = Math.Min(endRow + n, SpreadsheetVersion.EXCEL97.LastRowIndex);
			}
		}
		else
		{
			if (startRow + n < firstrow)
			{
				firstrow = Math.Max(startRow + n, 0);
			}
			if (endRow == lastrow)
			{
				lastrow = Math.Min(endRow + n, SpreadsheetVersion.EXCEL97.LastRowIndex);
				for (int k = endRow - 1; k > endRow + n; k++)
				{
					if (GetRow(k) != null)
					{
						lastrow = k;
						break;
					}
				}
			}
		}
		int sheetIndex = _workbook.GetSheetIndex(this);
		string sheetName = _workbook.GetSheetName(sheetIndex);
		int externSheetIndex = book.CheckExternSheet(sheetIndex);
		FormulaShifter shifter = FormulaShifter.CreateForRowShift(externSheetIndex, sheetName, startRow, endRow, n, SpreadsheetVersion.EXCEL97);
		_sheet.UpdateFormulasAfterCellShift(shifter, externSheetIndex);
		int numberOfSheets = _workbook.NumberOfSheets;
		for (int l = 0; l < numberOfSheets; l++)
		{
			InternalSheet sheet = ((HSSFSheet)_workbook.GetSheetAt(l)).Sheet;
			if (sheet != _sheet)
			{
				int externSheetIndex2 = book.CheckExternSheet(l);
				sheet.UpdateFormulasAfterCellShift(shifter, externSheetIndex2);
			}
		}
		_workbook.Workbook.UpdateNamesAfterCellShift(shifter);
	}

	public void InsertChartRecords(List<RecordBase> records)
	{
		int index = _sheet.FindFirstRecordLocBySid(574);
		_sheet.Records.InsertRange(index, records);
	}

	private void NotifyRowShifting(HSSFRow row)
	{
		string msg = "Row[rownum=" + row.RowNum + "] contains cell(s) included in a multi-cell array formula. You cannot change part of an array.";
		foreach (HSSFCell cell in row.Cells)
		{
			if (cell.IsPartOfArrayFormulaGroup)
			{
				cell.NotifyArrayFormulaChanging(msg);
			}
		}
	}

	public void CreateFreezePane(int colSplit, int rowSplit, int leftmostColumn, int topRow)
	{
		ValidateColumn(colSplit);
		ValidateRow(rowSplit);
		if (leftmostColumn < colSplit)
		{
			throw new ArgumentException("leftmostColumn parameter must not be less than colSplit parameter");
		}
		if (topRow < rowSplit)
		{
			throw new ArgumentException("topRow parameter must not be less than leftmostColumn parameter");
		}
		Sheet.CreateFreezePane(colSplit, rowSplit, topRow, leftmostColumn);
	}

	public void CreateFreezePane(int colSplit, int rowSplit)
	{
		CreateFreezePane(colSplit, rowSplit, colSplit, rowSplit);
	}

	public void CreateSplitPane(int xSplitPos, int ySplitPos, int leftmostColumn, int topRow, PanePosition activePane)
	{
		Sheet.CreateSplitPane(xSplitPos, ySplitPos, topRow, leftmostColumn, activePane);
	}

	public double GetMargin(MarginType margin)
	{
		return margin switch
		{
			MarginType.FooterMargin => _sheet.PageSettings.PrintSetup.FooterMargin, 
			MarginType.HeaderMargin => _sheet.PageSettings.PrintSetup.HeaderMargin, 
			_ => _sheet.PageSettings.GetMargin(margin), 
		};
	}

	public void SetMargin(MarginType margin, double size)
	{
		switch (margin)
		{
		case MarginType.FooterMargin:
			_sheet.PageSettings.PrintSetup.FooterMargin = size;
			break;
		case MarginType.HeaderMargin:
			_sheet.PageSettings.PrintSetup.HeaderMargin = size;
			break;
		default:
			_sheet.PageSettings.SetMargin(margin, size);
			break;
		}
	}

	public void SetRowBreak(int row)
	{
		ValidateRow(row);
		_sheet.PageSettings.SetRowBreak(row, 0, 255);
	}

	public bool IsRowBroken(int row)
	{
		return _sheet.PageSettings.IsRowBroken(row);
	}

	public void RemoveRowBreak(int row)
	{
		_sheet.PageSettings.RemoveRowBreak(row);
	}

	public void SetColumnBreak(int column)
	{
		ValidateColumn(column);
		_sheet.PageSettings.SetColumnBreak(column, 0, -1);
	}

	public bool IsColumnBroken(int column)
	{
		return _sheet.PageSettings.IsColumnBroken(column);
	}

	public void RemoveColumnBreak(int column)
	{
		_sheet.PageSettings.RemoveColumnBreak(column);
	}

	protected void ValidateRow(int row)
	{
		int lastRowIndex = SpreadsheetVersion.EXCEL97.LastRowIndex;
		if (row > lastRowIndex)
		{
			throw new ArgumentException("Maximum row number is " + lastRowIndex.ToString(CultureInfo.CurrentCulture));
		}
		if (row < 0)
		{
			throw new ArgumentException("Minumum row number is 0");
		}
	}

	protected void ValidateColumn(int column)
	{
		int lastColumnIndex = SpreadsheetVersion.EXCEL97.LastColumnIndex;
		if (column > lastColumnIndex)
		{
			throw new ArgumentException("Maximum column number is " + lastColumnIndex.ToString(CultureInfo.CurrentCulture));
		}
		if (column < 0)
		{
			throw new ArgumentException("Minimum column number is 0");
		}
	}

	public void DumpDrawingRecords(bool fat)
	{
		_sheet.AggregateDrawingRecords(book.DrawingManager, CreateIfMissing: false);
		foreach (EscherRecord escherRecord in ((EscherAggregate)Sheet.FindFirstRecordBySid(9876)).EscherRecords)
		{
			if (fat)
			{
				Console.WriteLine(escherRecord.ToString());
			}
			else
			{
				escherRecord.Display(0);
			}
		}
	}

	public IDrawing CreateDrawingPatriarch()
	{
		_patriarch = GetPatriarch(createIfMissing: true);
		return _patriarch;
	}

	private HSSFPatriarch GetPatriarch(bool createIfMissing)
	{
		if (_patriarch != null)
		{
			return _patriarch;
		}
		DrawingManager2 drawingManager = book.FindDrawingGroup();
		if (drawingManager == null)
		{
			if (!createIfMissing)
			{
				return null;
			}
			book.CreateDrawingGroup();
			drawingManager = book.DrawingManager;
		}
		EscherAggregate escherAggregate = (EscherAggregate)_sheet.FindFirstRecordBySid(9876);
		if (escherAggregate == null || escherAggregate.GetEscherContainer() == null)
		{
			int num = _sheet.AggregateDrawingRecords(drawingManager, CreateIfMissing: false);
			if (-1 == num || (escherAggregate = (EscherAggregate)_sheet.Records[num]) == null || escherAggregate.GetEscherContainer() == null)
			{
				if (createIfMissing)
				{
					num = _sheet.AggregateDrawingRecords(drawingManager, CreateIfMissing: true);
					escherAggregate = (EscherAggregate)_sheet.Records[num];
					HSSFPatriarch hSSFPatriarch = new HSSFPatriarch(this, escherAggregate);
					hSSFPatriarch.AfterCreate();
					return hSSFPatriarch;
				}
				return null;
			}
		}
		return new HSSFPatriarch(this, escherAggregate);
	}

	public void SetColumnGroupCollapsed(int columnNumber, bool collapsed)
	{
		_sheet.SetColumnGroupCollapsed(columnNumber, collapsed);
	}

	public void GroupColumn(int fromColumn, int toColumn)
	{
		_sheet.GroupColumnRange(fromColumn, toColumn, indent: true);
	}

	public void UngroupColumn(int fromColumn, int toColumn)
	{
		_sheet.GroupColumnRange(fromColumn, toColumn, indent: false);
	}

	public void GroupRow(int fromRow, int toRow)
	{
		_sheet.GroupRowRange(fromRow, toRow, indent: true);
	}

	public ICellRange<ICell> RemoveArrayFormula(ICell cell)
	{
		if (cell.Sheet != this)
		{
			throw new ArgumentException("Specified cell does not belong to this sheet.");
		}
		CellValueRecordInterface cellValueRecord = ((HSSFCell)cell).CellValueRecord;
		if (!(cellValueRecord is FormulaRecordAggregate))
		{
			string text = new CellReference(cell).FormatAsString();
			throw new ArgumentException("Cell " + text + " is not part of an array formula.");
		}
		CellRangeAddress range = ((FormulaRecordAggregate)cellValueRecord).RemoveArrayFormula(cell.RowIndex, cell.ColumnIndex);
		ICellRange<ICell> cellRange = GetCellRange(range);
		foreach (ICell item in cellRange)
		{
			item.SetCellType(CellType.Blank);
		}
		return cellRange;
	}

	private ICellRange<ICell> GetCellRange(CellRangeAddress range)
	{
		int firstRow = range.FirstRow;
		int firstColumn = range.FirstColumn;
		int lastRow = range.LastRow;
		int lastColumn = range.LastColumn;
		int num = lastRow - firstRow + 1;
		int num2 = lastColumn - firstColumn + 1;
		List<ICell> list = new List<ICell>(num * num2);
		for (int i = firstRow; i <= lastRow; i++)
		{
			for (int j = firstColumn; j <= lastColumn; j++)
			{
				IRow row = GetRow(i);
				if (row == null)
				{
					row = CreateRow(i);
				}
				ICell cell = row.GetCell(j);
				if (cell == null)
				{
					cell = row.CreateCell(j);
				}
				list.Add(cell);
			}
		}
		return SSCellRange<ICell>.Create(firstRow, firstColumn, num, num2, list, typeof(HSSFCell));
	}

	public ICellRange<ICell> SetArrayFormula(string formula, CellRangeAddress range)
	{
		int sheetIndex = _workbook.GetSheetIndex(this);
		Ptg[] ptgs = HSSFFormulaParser.Parse(formula, _workbook, FormulaType.Array, sheetIndex);
		ICellRange<ICell> cellRange = GetCellRange(range);
		foreach (HSSFCell item in cellRange)
		{
			item.SetCellArrayFormula(range);
		}
		((FormulaRecordAggregate)((HSSFCell)cellRange.TopLeftCell).CellValueRecord).SetArrayFormula(range, ptgs);
		return cellRange;
	}

	public void UngroupRow(int fromRow, int toRow)
	{
		_sheet.GroupRowRange(fromRow, toRow, indent: false);
	}

	public void SetRowGroupCollapsed(int row, bool collapse)
	{
		if (collapse)
		{
			_sheet.RowsAggregate.CollapseRow(row);
		}
		else
		{
			_sheet.RowsAggregate.ExpandRow(row);
		}
	}

	public void SetDefaultColumnStyle(int column, ICellStyle style)
	{
		_sheet.SetDefaultColumnStyle(column, style.Index);
	}

	public void AutoSizeColumn(int column)
	{
		AutoSizeColumn(column, useMergedCells: false);
	}

	public void AutoSizeColumn(int column, bool useMergedCells)
	{
		double columnWidth = SheetUtil.GetColumnWidth(this, column, useMergedCells);
		if (columnWidth != -1.0)
		{
			columnWidth *= 256.0;
			int num = 65280;
			if (columnWidth > (double)num)
			{
				columnWidth = num;
			}
			SetColumnWidth(column, (int)columnWidth);
		}
	}

	public bool IsMergedRegion(CellRangeAddress mergedRegion)
	{
		foreach (CellRangeAddress mergedRegion2 in _sheet.MergedRecords.MergedRegions)
		{
			if (mergedRegion2.FirstColumn <= mergedRegion.FirstColumn && mergedRegion2.LastColumn >= mergedRegion.LastColumn && mergedRegion2.FirstRow <= mergedRegion.FirstRow && mergedRegion2.LastRow >= mergedRegion.LastRow)
			{
				return true;
			}
		}
		return false;
	}

	public CellRangeAddress GetMergedRegion(int index)
	{
		return _sheet.GetMergedRegionAt(index);
	}

	public Font HSSFFont2Font(HSSFFont font1)
	{
		return new Font(font1.FontName, (float)font1.FontHeightInPoints);
	}

	[Obsolete("deprecated as of 2015-11-23 (circa POI 3.14beta1). Use {@link #getCellComment(CellAddress)} instead.")]
	public IComment GetCellComment(int row, int column)
	{
		return FindCellComment(row, column);
	}

	public IComment GetCellComment(CellAddress ref1)
	{
		return FindCellComment(ref1.Row, ref1.Column);
	}

	public IHyperlink GetHyperlink(int row, int column)
	{
		foreach (RecordBase record in _sheet.Records)
		{
			if (record is HyperlinkRecord)
			{
				HyperlinkRecord hyperlinkRecord = (HyperlinkRecord)record;
				if (hyperlinkRecord.FirstColumn == column && hyperlinkRecord.FirstRow == row)
				{
					return new HSSFHyperlink(hyperlinkRecord);
				}
			}
		}
		return null;
	}

	public IHyperlink GetHyperlink(CellAddress addr)
	{
		return GetHyperlink(addr.Row, addr.Column);
	}

	public List<IHyperlink> GetHyperlinkList()
	{
		List<IHyperlink> list = new List<IHyperlink>();
		foreach (RecordBase record2 in _sheet.Records)
		{
			if (record2 is HyperlinkRecord)
			{
				HyperlinkRecord record = (HyperlinkRecord)record2;
				list.Add(new HSSFHyperlink(record));
			}
		}
		return list;
	}

	protected void RemoveHyperlink(HSSFHyperlink link)
	{
		RemoveHyperlink(link.record);
	}

	protected void RemoveHyperlink(HyperlinkRecord link)
	{
		for (int i = 0; i < _sheet.Records.Count; i++)
		{
			RecordBase recordBase = _sheet.Records[i];
			if (recordBase is HyperlinkRecord)
			{
				HyperlinkRecord hyperlinkRecord = (HyperlinkRecord)recordBase;
				if (link == hyperlinkRecord)
				{
					_sheet.Records.RemoveAt(i);
					break;
				}
			}
		}
	}

	public IDataValidationHelper GetDataValidationHelper()
	{
		return new HSSFDataValidationHelper(this);
	}

	public IAutoFilter SetAutoFilter(CellRangeAddress range)
	{
		InternalWorkbook workbook = _workbook.Workbook;
		int sheetIndex = _workbook.GetSheetIndex(this);
		NameRecord nameRecord = workbook.GetSpecificBuiltinRecord(13, sheetIndex + 1);
		if (nameRecord == null)
		{
			nameRecord = workbook.CreateBuiltInName(13, sheetIndex + 1);
		}
		int num = range.FirstRow;
		if (num == -1)
		{
			num = 0;
		}
		Area3DPtg area3DPtg = new Area3DPtg(num, range.LastRow, range.FirstColumn, range.LastColumn, firstRowRelative: false, lastRowRelative: false, firstColRelative: false, lastColRelative: false, sheetIndex);
		nameRecord.NameDefinition = new Ptg[1] { area3DPtg };
		AutoFilterInfoRecord autoFilterInfoRecord = new AutoFilterInfoRecord();
		int num2 = 1 + range.LastColumn - range.FirstColumn;
		autoFilterInfoRecord.NumEntries = (short)num2;
		int index = _sheet.FindFirstRecordLocBySid(512);
		_sheet.Records.Insert(index, autoFilterInfoRecord);
		HSSFPatriarch hSSFPatriarch = (HSSFPatriarch)CreateDrawingPatriarch();
		int firstColumn = range.FirstColumn;
		int lastColumn = range.LastColumn;
		for (int i = firstColumn; i <= lastColumn; i++)
		{
			hSSFPatriarch.CreateComboBox(new HSSFClientAnchor(0, 0, 0, 0, (short)i, num, (short)(i + 1), num + 1));
		}
		return new HSSFAutoFilter(this);
	}

	protected internal HSSFComment FindCellComment(int row, int column)
	{
		HSSFPatriarch hSSFPatriarch = DrawingPatriarch as HSSFPatriarch;
		if (hSSFPatriarch == null)
		{
			hSSFPatriarch = CreateDrawingPatriarch() as HSSFPatriarch;
		}
		return LookForComment(hSSFPatriarch, row, column);
	}

	private HSSFComment LookForComment(HSSFShapeContainer container, int row, int column)
	{
		foreach (HSSFShape child in container.Children)
		{
			if (child is HSSFShapeGroup)
			{
				HSSFShape hSSFShape2 = LookForComment((HSSFShapeContainer)child, row, column);
				if (hSSFShape2 != null)
				{
					return (HSSFComment)hSSFShape2;
				}
			}
			else if (child is HSSFComment)
			{
				HSSFComment hSSFComment = (HSSFComment)child;
				if (hSSFComment.HasPosition && hSSFComment.Column == column && hSSFComment.Row == row)
				{
					return hSSFComment;
				}
			}
		}
		return null;
	}

	public Dictionary<CellAddress, IComment> GetCellComments()
	{
		HSSFPatriarch hSSFPatriarch = DrawingPatriarch as HSSFPatriarch;
		if (hSSFPatriarch == null)
		{
			hSSFPatriarch = CreateDrawingPatriarch() as HSSFPatriarch;
		}
		Dictionary<CellAddress, IComment> dictionary = new Dictionary<CellAddress, IComment>();
		FindCellCommentLocations(hSSFPatriarch, dictionary);
		return dictionary;
	}

	private void FindCellCommentLocations(HSSFShapeContainer container, Dictionary<CellAddress, IComment> locations)
	{
		foreach (HSSFShape child in container.Children)
		{
			if (child is HSSFShapeGroup)
			{
				FindCellCommentLocations((HSSFShapeGroup)child, locations);
			}
			else if (child is HSSFComment)
			{
				HSSFComment hSSFComment = (HSSFComment)child;
				if (hSSFComment.HasPosition)
				{
					locations.Add(new CellAddress(hSSFComment.Row, hSSFComment.Column), hSSFComment);
				}
			}
		}
	}

	private void SetRepeatingRowsAndColumns(CellRangeAddress rowDef, CellRangeAddress colDef)
	{
		int sheetIndex = _workbook.GetSheetIndex(this);
		int lastRowIndex = SpreadsheetVersion.EXCEL97.LastRowIndex;
		int lastColumnIndex = SpreadsheetVersion.EXCEL97.LastColumnIndex;
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		int num4 = -1;
		if (rowDef != null)
		{
			num3 = rowDef.FirstRow;
			num4 = rowDef.LastRow;
			if ((num3 == -1 && num4 != -1) || num3 > num4 || num3 < 0 || num3 > lastRowIndex || num4 < 0 || num4 > lastRowIndex)
			{
				throw new ArgumentException("Invalid row range specification");
			}
		}
		if (colDef != null)
		{
			num = colDef.FirstColumn;
			num2 = colDef.LastColumn;
			if ((num == -1 && num2 != -1) || num > num2 || num < 0 || num > lastColumnIndex || num2 < 0 || num2 > lastColumnIndex)
			{
				throw new ArgumentException("Invalid column range specification");
			}
		}
		short externalSheetIndex = (short)_workbook.Workbook.CheckExternSheet(sheetIndex);
		bool flag = rowDef != null && colDef != null;
		bool num5 = rowDef == null && colDef == null;
		HSSFName hSSFName = _workbook.GetBuiltInName(7, sheetIndex);
		if (num5)
		{
			if (hSSFName != null)
			{
				_workbook.RemoveName(hSSFName);
			}
			return;
		}
		if (hSSFName == null)
		{
			hSSFName = _workbook.CreateBuiltInName(7, sheetIndex);
		}
		List<Ptg> list = new List<Ptg>();
		if (flag)
		{
			int subExprLen = 23;
			list.Add(new MemFuncPtg(subExprLen));
		}
		if (colDef != null)
		{
			Area3DPtg item = new Area3DPtg(0, lastRowIndex, num, num2, firstRowRelative: false, lastRowRelative: false, firstColRelative: false, lastColRelative: false, externalSheetIndex);
			list.Add(item);
		}
		if (rowDef != null)
		{
			Area3DPtg item2 = new Area3DPtg(num3, num4, 0, lastColumnIndex, firstRowRelative: false, lastRowRelative: false, firstColRelative: false, lastColRelative: false, externalSheetIndex);
			list.Add(item2);
		}
		if (flag)
		{
			list.Add(UnionPtg.instance);
		}
		Ptg[] nameDefinition = list.ToArray();
		hSSFName.SetNameDefinition(nameDefinition);
		((HSSFPrintSetup)PrintSetup).ValidSettings = false;
		SetActive(sel: true);
	}

	private CellRangeAddress GetRepeatingRowsOrColums(bool rows)
	{
		NameRecord builtinNameRecord = GetBuiltinNameRecord(7);
		if (builtinNameRecord == null)
		{
			return null;
		}
		Ptg[] nameDefinition = builtinNameRecord.NameDefinition;
		if (builtinNameRecord.NameDefinition == null)
		{
			return null;
		}
		int lastRowIndex = SpreadsheetVersion.EXCEL97.LastRowIndex;
		int lastColumnIndex = SpreadsheetVersion.EXCEL97.LastColumnIndex;
		Ptg[] array = nameDefinition;
		foreach (Ptg ptg in array)
		{
			if (!(ptg is Area3DPtg))
			{
				continue;
			}
			Area3DPtg area3DPtg = (Area3DPtg)ptg;
			if (area3DPtg.FirstColumn == 0 && area3DPtg.LastColumn == lastColumnIndex)
			{
				if (rows)
				{
					return new CellRangeAddress(area3DPtg.FirstRow, area3DPtg.LastRow, -1, -1);
				}
			}
			else if (area3DPtg.FirstRow == 0 && area3DPtg.LastRow == lastRowIndex && !rows)
			{
				return new CellRangeAddress(-1, -1, area3DPtg.FirstColumn, area3DPtg.LastColumn);
			}
		}
		return null;
	}

	private NameRecord GetBuiltinNameRecord(byte builtinCode)
	{
		int sheetIndex = _workbook.GetSheetIndex(this);
		int num = _workbook.FindExistingBuiltinNameRecordIdx(sheetIndex, builtinCode);
		if (num == -1)
		{
			return null;
		}
		return _workbook.GetNameRecord(num);
	}

	public int GetColumnOutlineLevel(int columnIndex)
	{
		return _sheet.GetColumnOutlineLevel(columnIndex);
	}

	public ISheet CopySheet()
	{
		return CopySheet(SheetName + " - Copy", copyStyle: true);
	}

	public ISheet CopySheet(bool CopyStyle)
	{
		return CopySheet(SheetName + " - Copy", CopyStyle);
	}

	public ISheet CopySheet(string Name)
	{
		return CopySheet(Name, copyStyle: true);
	}

	public ISheet CopySheet(string Name, bool copyStyle)
	{
		int num = 0;
		HSSFSheet hSSFSheet = (HSSFSheet)Workbook.CreateSheet(Name);
		hSSFSheet._sheet = Sheet.CloneSheet();
		IDictionary<int, HSSFCellStyle> styleMap = (copyStyle ? new Dictionary<int, HSSFCellStyle>() : null);
		for (int i = FirstRowNum; i <= LastRowNum; i++)
		{
			HSSFRow hSSFRow = (HSSFRow)GetRow(i);
			HSSFRow destRow = (HSSFRow)hSSFSheet.CreateRow(i);
			if (hSSFRow != null)
			{
				CopyRow(this, hSSFSheet, hSSFRow, destRow, styleMap, new Dictionary<short, short>(), keepFormulas: true);
				if (hSSFRow.LastCellNum > num)
				{
					num = hSSFRow.LastCellNum;
				}
			}
		}
		for (int j = 0; j <= num; j++)
		{
			hSSFSheet.SetColumnWidth(j, GetColumnWidth(j));
		}
		hSSFSheet.ForceFormulaRecalculation = true;
		hSSFSheet.PrintSetup.Landscape = PrintSetup.Landscape;
		hSSFSheet.PrintSetup.HResolution = PrintSetup.HResolution;
		hSSFSheet.PrintSetup.VResolution = PrintSetup.VResolution;
		hSSFSheet.SetMargin(MarginType.LeftMargin, GetMargin(MarginType.LeftMargin));
		hSSFSheet.SetMargin(MarginType.RightMargin, GetMargin(MarginType.RightMargin));
		hSSFSheet.SetMargin(MarginType.TopMargin, GetMargin(MarginType.TopMargin));
		hSSFSheet.SetMargin(MarginType.BottomMargin, GetMargin(MarginType.BottomMargin));
		hSSFSheet.PrintSetup.HeaderMargin = PrintSetup.HeaderMargin;
		hSSFSheet.PrintSetup.FooterMargin = PrintSetup.FooterMargin;
		hSSFSheet.Header.Left = Header.Left;
		hSSFSheet.Header.Center = Header.Center;
		hSSFSheet.Header.Right = Header.Right;
		hSSFSheet.Footer.Left = Footer.Left;
		hSSFSheet.Footer.Center = Footer.Center;
		hSSFSheet.Footer.Right = Footer.Right;
		hSSFSheet.PrintSetup.Scale = PrintSetup.Scale;
		hSSFSheet.PrintSetup.FitHeight = PrintSetup.FitHeight;
		hSSFSheet.PrintSetup.FitWidth = PrintSetup.FitWidth;
		return hSSFSheet;
	}

	public void CopyTo(IWorkbook dest, string name, bool copyStyle, bool keepFormulas)
	{
		int num = 0;
		HSSFSheet hSSFSheet = (HSSFSheet)dest.CreateSheet(name);
		hSSFSheet._sheet = Sheet.CloneSheet();
		InternalWorkbook workbook = ((HSSFWorkbook)dest).Workbook;
		Dictionary<short, short> paletteMap = new Dictionary<short, short>();
		if (dest.NumberOfSheets == 1)
		{
			workbook.CustomPalette.ClearColors();
			paletteMap = MergePalettes(Workbook as HSSFWorkbook, dest as HSSFWorkbook);
		}
		else if (dest != Workbook)
		{
			paletteMap = MergePalettes(Workbook as HSSFWorkbook, dest as HSSFWorkbook);
		}
		IDictionary<int, HSSFCellStyle> styleMap = (copyStyle ? new Dictionary<int, HSSFCellStyle>() : null);
		for (int i = FirstRowNum; i <= LastRowNum; i++)
		{
			HSSFRow hSSFRow = (HSSFRow)GetRow(i);
			HSSFRow destRow = (HSSFRow)hSSFSheet.CreateRow(i);
			if (hSSFRow != null)
			{
				CopyRow(this, hSSFSheet, hSSFRow, destRow, styleMap, paletteMap, keepFormulas);
				if (hSSFRow.LastCellNum > num)
				{
					num = hSSFRow.LastCellNum;
				}
			}
		}
		for (int j = 0; j < num; j++)
		{
			hSSFSheet.SetColumnWidth(j, GetColumnWidth(j));
		}
		hSSFSheet.ForceFormulaRecalculation = true;
		hSSFSheet.PrintSetup.Landscape = PrintSetup.Landscape;
		hSSFSheet.PrintSetup.HResolution = PrintSetup.HResolution;
		hSSFSheet.PrintSetup.VResolution = PrintSetup.VResolution;
		hSSFSheet.SetMargin(MarginType.LeftMargin, GetMargin(MarginType.LeftMargin));
		hSSFSheet.SetMargin(MarginType.RightMargin, GetMargin(MarginType.RightMargin));
		hSSFSheet.SetMargin(MarginType.TopMargin, GetMargin(MarginType.TopMargin));
		hSSFSheet.SetMargin(MarginType.BottomMargin, GetMargin(MarginType.BottomMargin));
		hSSFSheet.PrintSetup.HeaderMargin = PrintSetup.HeaderMargin;
		hSSFSheet.PrintSetup.FooterMargin = PrintSetup.FooterMargin;
		hSSFSheet.Header.Left = Header.Left;
		hSSFSheet.Header.Center = Header.Center;
		hSSFSheet.Header.Right = Header.Right;
		hSSFSheet.Footer.Left = Footer.Left;
		hSSFSheet.Footer.Center = Footer.Center;
		hSSFSheet.Footer.Right = Footer.Right;
		hSSFSheet.PrintSetup.Scale = PrintSetup.Scale;
		hSSFSheet.PrintSetup.FitHeight = PrintSetup.FitHeight;
		hSSFSheet.PrintSetup.FitWidth = PrintSetup.FitWidth;
		EscherAggregate drawingEscherAggregate = DrawingEscherAggregate;
		if (drawingEscherAggregate == null)
		{
			return;
		}
		if (workbook.DrawingManager == null)
		{
			workbook.CreateDrawingGroup();
		}
		EscherAggregate drawingEscherAggregate2 = hSSFSheet.DrawingEscherAggregate;
		IEnumerable<int> enumerable = FindUsedPictures(drawingEscherAggregate.EscherRecords);
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		IList allPictures = Workbook.GetAllPictures();
		foreach (int item in enumerable)
		{
			if (item <= allPictures.Count)
			{
				HSSFPictureData hSSFPictureData = (HSSFPictureData)allPictures[item - 1];
				int value = dest.AddPicture(hSSFPictureData.Data, (PictureType)hSSFPictureData.Format);
				dictionary.Add(item, value);
			}
		}
		foreach (EscherRecord escherRecord in drawingEscherAggregate2.EscherRecords)
		{
			ApplyEscherRemap(escherRecord, dictionary);
		}
	}

	private IEnumerable<int> FindUsedPictures(IEnumerable<EscherRecord> escherRecords)
	{
		List<int> list = new List<int>();
		foreach (EscherRecord escherRecord in escherRecords)
		{
			GetSheetImageIds(escherRecord, list);
		}
		return list;
	}

	private void GetSheetImageIds(EscherRecord parent, List<int> usedIds)
	{
		foreach (EscherRecord childRecord in parent.ChildRecords)
		{
			if (childRecord is EscherOptRecord)
			{
				foreach (EscherProperty escherProperty in ((EscherOptRecord)childRecord).EscherProperties)
				{
					if (escherProperty.PropertyNumber == 260)
					{
						int propertyValue = ((EscherSimpleProperty)escherProperty).PropertyValue;
						if (!usedIds.Contains(propertyValue))
						{
							usedIds.Add(propertyValue);
						}
						break;
					}
				}
			}
			if (childRecord.ChildRecords.Count <= 0)
			{
				continue;
			}
			foreach (EscherRecord childRecord2 in childRecord.ChildRecords)
			{
				GetSheetImageIds(childRecord2, usedIds);
			}
		}
	}

	private void ApplyEscherRemap(EscherRecord parent, Dictionary<int, int> mappings)
	{
		foreach (EscherRecord childRecord in parent.ChildRecords)
		{
			if (childRecord is EscherOptRecord)
			{
				foreach (EscherProperty escherProperty in ((EscherOptRecord)childRecord).EscherProperties)
				{
					if (escherProperty.PropertyNumber == 260)
					{
						int propertyValue = ((EscherSimpleProperty)escherProperty).PropertyValue;
						if (mappings.ContainsKey(propertyValue))
						{
							((EscherSimpleProperty)escherProperty).PropertyValue = mappings[propertyValue];
						}
						break;
					}
				}
			}
			if (childRecord.ChildRecords.Count <= 0)
			{
				continue;
			}
			foreach (EscherRecord childRecord2 in childRecord.ChildRecords)
			{
				ApplyEscherRemap(childRecord2, mappings);
			}
		}
	}

	private static Dictionary<short, short> MergePalettes(HSSFWorkbook source, HSSFWorkbook dest)
	{
		Dictionary<short, short> dictionary = new Dictionary<short, short>();
		for (short num = 0; num < source.Workbook.CustomPalette.NumColors; num++)
		{
			byte[] color = source.Workbook.CustomPalette.GetColor((short)(num + 8));
			bool flag = false;
			for (short num2 = 0; num2 < dest.Workbook.CustomPalette.NumColors; num2++)
			{
				byte[] color2 = dest.Workbook.CustomPalette.GetColor((short)(num2 + 8));
				if (color[0] == color2[0] && color[1] == color2[1] && color[2] == color2[2])
				{
					flag = true;
					dictionary.Add((short)(num + 8), (short)(num2 + 8));
					break;
				}
			}
			if (!flag)
			{
				short numColors = dest.Workbook.CustomPalette.NumColors;
				dest.Workbook.CustomPalette.SetColor((short)(numColors + 8), color[0], color[1], color[2]);
				dictionary.Add((short)(num + 8), (short)(numColors + 8));
			}
		}
		return dictionary;
	}

	private static void CopyRow(HSSFSheet srcSheet, HSSFSheet destSheet, HSSFRow srcRow, HSSFRow destRow, IDictionary<int, HSSFCellStyle> styleMap, Dictionary<short, short> paletteMap, bool keepFormulas)
	{
		List<CellRangeAddress> mergedRegions = destSheet.Sheet.MergedRecords.MergedRegions;
		destRow.Height = srcRow.Height;
		destRow.IsHidden = srcRow.IsHidden;
		destRow.RowRecord.OptionFlags = srcRow.RowRecord.OptionFlags;
		for (int i = srcRow.FirstCellNum; i <= srcRow.LastCellNum; i++)
		{
			HSSFCell hSSFCell = (HSSFCell)srcRow.GetCell(i);
			HSSFCell hSSFCell2 = (HSSFCell)destRow.GetCell(i);
			if (srcSheet.Workbook == destSheet.Workbook)
			{
				hSSFCell2 = (HSSFCell)destRow.GetCell(i);
			}
			if (hSSFCell == null)
			{
				continue;
			}
			if (hSSFCell2 == null)
			{
				hSSFCell2 = (HSSFCell)destRow.CreateCell(i);
			}
			HSSFCellUtil.CopyCell(hSSFCell, hSSFCell2, styleMap, paletteMap, keepFormulas);
			CellRangeAddress mergedRegion = GetMergedRegion(srcSheet, srcRow.RowNum, (short)hSSFCell.ColumnIndex);
			if (mergedRegion != null)
			{
				CellRangeAddress cellRangeAddress = new CellRangeAddress(mergedRegion.FirstRow, mergedRegion.LastRow, mergedRegion.FirstColumn, mergedRegion.LastColumn);
				if (IsNewMergedRegion(cellRangeAddress, mergedRegions))
				{
					mergedRegions.Add(cellRangeAddress);
				}
			}
		}
	}

	public static CellRangeAddress GetMergedRegion(HSSFSheet sheet, int rowNum, short cellNum)
	{
		for (int i = 0; i < sheet.NumMergedRegions; i++)
		{
			CellRangeAddress mergedRegion = sheet.GetMergedRegion(i);
			if (rowNum >= mergedRegion.FirstRow && rowNum <= mergedRegion.LastRow && cellNum >= mergedRegion.FirstColumn && cellNum <= mergedRegion.LastColumn)
			{
				return mergedRegion;
			}
		}
		return null;
	}

	private static bool AreAllTrue(params bool[] values)
	{
		for (int i = 0; i < values.Length; i++)
		{
			if (!values[i])
			{
				return false;
			}
		}
		return true;
	}

	private static bool IsNewMergedRegion(CellRangeAddress newMergedRegion, List<CellRangeAddress> mergedRegions)
	{
		bool result = true;
		foreach (CellRangeAddress mergedRegion in mergedRegions)
		{
			bool flag = mergedRegion.FirstRow == newMergedRegion.FirstRow;
			bool flag2 = mergedRegion.LastRow == newMergedRegion.LastRow;
			bool flag3 = mergedRegion.FirstColumn == newMergedRegion.FirstColumn;
			bool flag4 = mergedRegion.LastColumn == newMergedRegion.LastColumn;
			if (AreAllTrue(flag, flag2, flag3, flag4))
			{
				result = false;
			}
		}
		return result;
	}

	public bool IsDate1904()
	{
		throw new NotImplementedException();
	}
}
