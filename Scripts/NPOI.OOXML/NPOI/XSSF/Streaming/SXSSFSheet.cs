using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.UserModel;

namespace NPOI.XSSF.Streaming;

public class SXSSFSheet : ISheet
{
	internal XSSFSheet _sh;

	private SXSSFWorkbook _workbook;

	private IDictionary<int, SXSSFRow> _rows = new Dictionary<int, SXSSFRow>();

	private SheetDataWriter _writer;

	private int _randomAccessWindowSize = 100;

	private Lazy<AutoSizeColumnTracker> _autoSizeColumnTracker;

	private int outlineLevelRow;

	private int lastFlushedRowNumber = -1;

	private bool allFlushed;

	private int _FirstRowNum = -1;

	private int _LastRowNum = -1;

	public bool Autobreaks
	{
		get
		{
			return _sh.Autobreaks;
		}
		set
		{
			_sh.Autobreaks = value;
		}
	}

	public int[] ColumnBreaks => _sh.ColumnBreaks;

	public int DefaultColumnWidth
	{
		get
		{
			return _sh.DefaultColumnWidth;
		}
		set
		{
			_sh.DefaultColumnWidth = value;
		}
	}

	public short DefaultRowHeight
	{
		get
		{
			return _sh.DefaultRowHeight;
		}
		set
		{
			_sh.DefaultRowHeight = value;
		}
	}

	public float DefaultRowHeightInPoints
	{
		get
		{
			return _sh.DefaultRowHeightInPoints;
		}
		set
		{
			_sh.DefaultRowHeightInPoints = value;
		}
	}

	public bool DisplayFormulas
	{
		get
		{
			return _sh.DisplayFormulas;
		}
		set
		{
			_sh.DisplayFormulas = value;
		}
	}

	public bool DisplayGridlines
	{
		get
		{
			return _sh.DisplayGridlines;
		}
		set
		{
			_sh.DisplayGridlines = value;
		}
	}

	public bool DisplayGuts
	{
		get
		{
			return _sh.DisplayGuts;
		}
		set
		{
			_sh.DisplayGuts = value;
		}
	}

	public bool DisplayRowColHeadings
	{
		get
		{
			return _sh.DisplayRowColHeadings;
		}
		set
		{
			_sh.DisplayRowColHeadings = value;
		}
	}

	public bool DisplayZeros
	{
		get
		{
			return _sh.DisplayZeros;
		}
		set
		{
			_sh.DisplayZeros = value;
		}
	}

	public IDrawing DrawingPatriarch => _sh.DrawingPatriarch;

	public int FirstRowNum
	{
		get
		{
			if (_writer.NumberOfFlushedRows > 0)
			{
				return _writer.LowestIndexOfFlushedRows;
			}
			if (_rows.Count != 0)
			{
				return _FirstRowNum;
			}
			return 0;
		}
	}

	public bool FitToPage
	{
		get
		{
			return _sh.FitToPage;
		}
		set
		{
			_sh.FitToPage = value;
		}
	}

	public IFooter Footer => _sh.Footer;

	public bool ForceFormulaRecalculation
	{
		get
		{
			return _sh.ForceFormulaRecalculation;
		}
		set
		{
			_sh.ForceFormulaRecalculation = value;
		}
	}

	public IHeader Header => _sh.Header;

	public bool HorizontallyCenter
	{
		get
		{
			return _sh.HorizontallyCenter;
		}
		set
		{
			_sh.HorizontallyCenter = value;
		}
	}

	public bool IsActive
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public bool IsPrintGridlines
	{
		get
		{
			return _sh.IsPrintGridlines;
		}
		set
		{
			_sh.IsPrintGridlines = value;
		}
	}

	public bool IsPrintRowAndColumnHeadings
	{
		get
		{
			return _sh.IsPrintRowAndColumnHeadings;
		}
		set
		{
			_sh.IsPrintRowAndColumnHeadings = value;
		}
	}

	public bool IsRightToLeft
	{
		get
		{
			return _sh.IsRightToLeft;
		}
		set
		{
			_sh.IsRightToLeft = value;
		}
	}

	public bool IsSelected
	{
		get
		{
			return _sh.IsSelected;
		}
		set
		{
			_sh.IsSelected = value;
		}
	}

	public int LastRowNum
	{
		get
		{
			if (_rows.Count == 0)
			{
				if (_writer.NumberOfFlushedRows <= 0)
				{
					return 0;
				}
				return LastFlushedRowNumber;
			}
			return _LastRowNum;
		}
	}

	public short LeftCol
	{
		get
		{
			return _sh.LeftCol;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public int NumMergedRegions => _sh.NumMergedRegions;

	public List<CellRangeAddress> MergedRegions => _sh.MergedRegions;

	public PaneInformation PaneInformation => _sh.PaneInformation;

	public int PhysicalNumberOfRows => _rows.Count + _writer.NumberOfFlushedRows;

	public IPrintSetup PrintSetup => _sh.PrintSetup;

	public bool Protect => _sh.Protect;

	public CellRangeAddress RepeatingColumns
	{
		get
		{
			return _sh.RepeatingColumns;
		}
		set
		{
			_sh.RepeatingColumns = value;
		}
	}

	public CellRangeAddress RepeatingRows
	{
		get
		{
			return _sh.RepeatingRows;
		}
		set
		{
			_sh.RepeatingRows = value;
		}
	}

	public int[] RowBreaks => _sh.RowBreaks;

	public bool RowSumsBelow
	{
		get
		{
			return _sh.RowSumsBelow;
		}
		set
		{
			_sh.RowSumsBelow = value;
		}
	}

	public bool RowSumsRight
	{
		get
		{
			return _sh.RowSumsRight;
		}
		set
		{
			_sh.RowSumsRight = value;
		}
	}

	public bool ScenarioProtect => _sh.ScenarioProtect;

	public ISheetConditionalFormatting SheetConditionalFormatting => _sh.SheetConditionalFormatting;

	public string SheetName => _sh.SheetName;

	public short TabColorIndex
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public short TopRow
	{
		get
		{
			return _sh.TopRow;
		}
		set
		{
			_sh.TopRow = value;
		}
	}

	public bool VerticallyCenter
	{
		get
		{
			return _sh.VerticallyCenter;
		}
		set
		{
			_sh.VerticallyCenter = value;
		}
	}

	public IWorkbook Workbook => _workbook;

	public ISet<int> TrackedColumnsForAutoSizing => _autoSizeColumnTracker.Value.TrackedColumns;

	public bool AllRowsFlushed => allFlushed;

	public int LastFlushedRowNumber => lastFlushedRowNumber;

	public SheetDataWriter SheetDataWriter => _writer;

	public CellAddress ActiveCell
	{
		get
		{
			return _sh.ActiveCell;
		}
		set
		{
			_sh.ActiveCell = value;
		}
	}

	public XSSFColor TabColor
	{
		get
		{
			return _sh.TabColor;
		}
		set
		{
			_sh.TabColor = value;
		}
	}

	public SXSSFSheet(SXSSFWorkbook workbook, XSSFSheet xSheet)
	{
		_workbook = workbook;
		_sh = xSheet;
		_writer = workbook.CreateSheetDataWriter();
		SetRandomAccessWindowSize(_workbook.RandomAccessWindowSize);
		_autoSizeColumnTracker = new Lazy<AutoSizeColumnTracker>(() => new AutoSizeColumnTracker(this));
	}

	public void SetRandomAccessWindowSize(int value)
	{
		if (value == 0 || value < -1)
		{
			throw new ArgumentException("RandomAccessWindowSize must be either -1 or a positive integer");
		}
		_randomAccessWindowSize = value;
	}

	public int AddMergedRegion(CellRangeAddress region)
	{
		return _sh.AddMergedRegion(region);
	}

	public int AddMergedRegionUnsafe(CellRangeAddress region)
	{
		return _sh.AddMergedRegionUnsafe(region);
	}

	public void ValidateMergedRegions()
	{
		_sh.ValidateMergedRegions();
	}

	public void AddValidationData(IDataValidation dataValidation)
	{
		_sh.AddValidationData(dataValidation);
	}

	public void AutoSizeColumn(int column)
	{
		AutoSizeColumn(column, useMergedCells: false);
	}

	public void AutoSizeColumn(int column, bool useMergedCells)
	{
		int bestFitColumnWidth;
		try
		{
			bestFitColumnWidth = _autoSizeColumnTracker.Value.GetBestFitColumnWidth(column, useMergedCells);
		}
		catch (Exception innerException)
		{
			throw new InvalidOperationException("Could not auto-size column. Make sure the column was tracked prior to auto-sizing the column.", innerException);
		}
		int val = (int)(256.0 * SheetUtil.GetColumnWidth(this, column, useMergedCells));
		int num = Math.Max(bestFitColumnWidth, val);
		if (num > 0)
		{
			int val2 = 65280;
			int width = Math.Min(num, val2);
			SetColumnWidth(column, width);
		}
	}

	public IRow CopyRow(int sourceIndex, int targetIndex)
	{
		throw new NotImplementedException();
	}

	public ISheet CopySheet(string Name)
	{
		throw new NotImplementedException();
	}

	public ISheet CopySheet(string Name, string newName, bool copyStyle)
	{
		throw new NotImplementedException();
	}

	public ISheet CopySheet(string Name, bool copyStyle)
	{
		throw new NotImplementedException();
	}

	public IHyperlink GetHyperlink(int row, int column)
	{
		return _sh.GetHyperlink(row, column);
	}

	public IHyperlink GetHyperlink(CellAddress addr)
	{
		return _sh.GetHyperlink(addr);
	}

	public List<IHyperlink> GetHyperlinkList()
	{
		return _sh.GetHyperlinkList();
	}

	public IDrawing CreateDrawingPatriarch()
	{
		return _sh.CreateDrawingPatriarch();
	}

	public void CreateFreezePane(int colSplit, int rowSplit)
	{
		_sh.CreateFreezePane(colSplit, rowSplit);
	}

	public void CreateFreezePane(int colSplit, int rowSplit, int leftmostColumn, int topRow)
	{
		_sh.CreateFreezePane(colSplit, rowSplit, leftmostColumn, topRow);
	}

	public IRow CreateRow(int rownum)
	{
		int lastRowIndex = SpreadsheetVersion.EXCEL2007.LastRowIndex;
		if (rownum < 0 || rownum > lastRowIndex)
		{
			throw new ArgumentException("Invalid row number (" + rownum + ") outside allowable range (0.." + lastRowIndex + ")");
		}
		if (rownum <= _writer.NumberLastFlushedRow)
		{
			throw new ArgumentException("Attempting to write a row[" + rownum + "] in the range [0," + _writer.NumberLastFlushedRow + "] that is already written to disk.");
		}
		if (_sh.PhysicalNumberOfRows > 0 && rownum <= _sh.LastRowNum)
		{
			throw new ArgumentException("Attempting to write a row[" + rownum + "] in the range [0," + _sh.LastRowNum + "] that is already written to disk.");
		}
		SXSSFRow sXSSFRow = new SXSSFRow(this);
		_rows[rownum] = sXSSFRow;
		UpdateIndexWhenAdd(rownum);
		allFlushed = false;
		if (_randomAccessWindowSize >= 0 && _rows.Count > _randomAccessWindowSize)
		{
			try
			{
				FlushRows(_randomAccessWindowSize, flushOnDisk: false);
			}
			catch (IOException e)
			{
				throw new RuntimeException(e);
			}
		}
		return sXSSFRow;
	}

	private void UpdateIndexWhenAdd(int rownum)
	{
		if (_FirstRowNum == -1 || rownum < _FirstRowNum)
		{
			_FirstRowNum = rownum;
		}
		if (rownum > _LastRowNum)
		{
			_LastRowNum = rownum;
		}
	}

	public void CreateSplitPane(int xSplitPos, int ySplitPos, int leftmostColumn, int topRow, PanePosition activePane)
	{
		_sh.CreateSplitPane(xSplitPos, ySplitPos, leftmostColumn, topRow, activePane);
	}

	[Obsolete("deprecated as of 2015-11-23 (circa POI 3.14beta1). Use {@link #getCellComment(CellAddress)} instead.")]
	public IComment GetCellComment(int row, int column)
	{
		return GetCellComment(new CellAddress(row, column));
	}

	public IComment GetCellComment(CellAddress ref1)
	{
		return _sh.GetCellComment(ref1);
	}

	public Dictionary<CellAddress, IComment> GetCellComments()
	{
		return _sh.GetCellComments();
	}

	public int GetColumnOutlineLevel(int columnIndex)
	{
		return _sh.GetColumnOutlineLevel(columnIndex);
	}

	public ICellStyle GetColumnStyle(int column)
	{
		return _sh.GetColumnStyle(column);
	}

	public int GetColumnWidth(int columnIndex)
	{
		return _sh.GetColumnWidth(columnIndex);
	}

	public float GetColumnWidthInPixels(int columnIndex)
	{
		return _sh.GetColumnWidthInPixels(columnIndex);
	}

	public IDataValidationHelper GetDataValidationHelper()
	{
		return _sh.GetDataValidationHelper();
	}

	public List<IDataValidation> GetDataValidations()
	{
		return _sh.GetDataValidations();
	}

	public IEnumerator GetEnumerator()
	{
		return new SortedDictionary<int, SXSSFRow>(_rows).Values.GetEnumerator();
	}

	public double GetMargin(MarginType margin)
	{
		return _sh.GetMargin(margin);
	}

	public CellRangeAddress GetMergedRegion(int index)
	{
		return _sh.GetMergedRegion(index);
	}

	public IRow GetRow(int rownum)
	{
		if (_rows.ContainsKey(rownum))
		{
			return _rows[rownum];
		}
		return null;
	}

	public IEnumerator GetRowEnumerator()
	{
		return GetEnumerator();
	}

	public void GroupColumn(int fromColumn, int toColumn)
	{
		_sh.GroupColumn(fromColumn, toColumn);
	}

	public void GroupRow(int fromRow, int toRow)
	{
		foreach (SXSSFRow item in from r in _rows
			where r.Key >= fromRow && r.Key <= toRow + 1
			select r.Value)
		{
			int num = ++item.OutlineLevel;
			if (num > outlineLevelRow)
			{
				outlineLevelRow = num;
			}
		}
		SetWorksheetOutlineLevelRow();
	}

	public void SetRowOutlineLevel(int rownum, int level)
	{
		_rows[rownum].OutlineLevel = level;
		if (level > 0 && level > outlineLevelRow)
		{
			outlineLevelRow = level;
			SetWorksheetOutlineLevelRow();
		}
	}

	private void SetWorksheetOutlineLevelRow()
	{
		CT_Worksheet cTWorksheet = _sh.GetCTWorksheet();
		CT_SheetFormatPr cT_SheetFormatPr = (cTWorksheet.IsSetSheetFormatPr() ? cTWorksheet.sheetFormatPr : cTWorksheet.AddNewSheetFormatPr());
		if (outlineLevelRow > 0)
		{
			cT_SheetFormatPr.outlineLevelRow = (byte)outlineLevelRow;
		}
	}

	public bool IsColumnBroken(int column)
	{
		return _sh.IsColumnBroken(column);
	}

	public bool IsColumnHidden(int columnIndex)
	{
		return _sh.IsColumnHidden(columnIndex);
	}

	public bool IsMergedRegion(CellRangeAddress mergedRegion)
	{
		throw new NotImplementedException();
	}

	public bool IsRowBroken(int row)
	{
		return _sh.IsRowBroken(row);
	}

	public void ProtectSheet(string password)
	{
		_sh.ProtectSheet(password);
	}

	public ICellRange<ICell> RemoveArrayFormula(ICell cell)
	{
		return _sh.RemoveArrayFormula(cell);
	}

	public void RemoveColumnBreak(int column)
	{
		_sh.RemoveColumnBreak(column);
	}

	public void RemoveMergedRegion(int index)
	{
		_sh.RemoveMergedRegion(index);
	}

	public void RemoveMergedRegions(IList<int> indices)
	{
		_sh.RemoveMergedRegions(indices);
	}

	public void RemoveRow(IRow row)
	{
		if (row == null)
		{
			throw new ArgumentException("Invalid row (null)");
		}
		if (row.Sheet != this)
		{
			throw new ArgumentException("Specified row does not belong to this sheet");
		}
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, SXSSFRow> row2 in _rows)
		{
			if (row2.Value == row)
			{
				list.Add(row2.Key);
			}
		}
		bool flag = false;
		bool flag2 = false;
		foreach (int item in list)
		{
			if (item == _FirstRowNum)
			{
				flag = true;
			}
			if (item >= _LastRowNum - 1)
			{
				flag2 = true;
			}
			_rows.Remove(item);
		}
		if (flag)
		{
			InvalidateFirstRowNum();
		}
		if (flag2)
		{
			InvalidateLastRowNum();
		}
	}

	private void InvalidateFirstRowNum()
	{
		if (_rows.Count == 0)
		{
			_FirstRowNum = -1;
		}
		else
		{
			_FirstRowNum = _rows.Keys.Min();
		}
	}

	private void InvalidateLastRowNum()
	{
		if (_rows.Count == 0)
		{
			_LastRowNum = -1;
		}
		else
		{
			_LastRowNum = _rows.Keys.Max();
		}
	}

	public void RemoveRowBreak(int row)
	{
		_sh.RemoveRowBreak(row);
	}

	public void SetActive(bool value)
	{
		throw new NotImplementedException();
	}

	public void SetActiveCell(int row, int column)
	{
		throw new NotImplementedException();
	}

	public void SetActiveCellRange(List<CellRangeAddress8Bit> cellranges, int activeRange, int activeRow, int activeColumn)
	{
		throw new NotImplementedException();
	}

	public void SetActiveCellRange(int firstRow, int lastRow, int firstColumn, int lastColumn)
	{
		throw new NotImplementedException();
	}

	public ICellRange<ICell> SetArrayFormula(string formula, CellRangeAddress range)
	{
		return _sh.SetArrayFormula(formula, range);
	}

	public IAutoFilter SetAutoFilter(CellRangeAddress range)
	{
		return _sh.SetAutoFilter(range);
	}

	public void SetColumnBreak(int column)
	{
		_sh.SetColumnBreak(column);
	}

	public void SetColumnGroupCollapsed(int columnNumber, bool collapsed)
	{
		_sh.SetColumnGroupCollapsed(columnNumber, collapsed);
	}

	public void SetColumnHidden(int columnIndex, bool hidden)
	{
		_sh.SetColumnHidden(columnIndex, hidden);
	}

	public void SetColumnWidth(int columnIndex, int width)
	{
		_sh.SetColumnWidth(columnIndex, width);
	}

	public void SetDefaultColumnStyle(int column, ICellStyle style)
	{
		_sh.SetDefaultColumnStyle(column, style);
	}

	public void TrackColumnForAutoSizing(int column)
	{
		_autoSizeColumnTracker.Value.TrackColumn(column);
	}

	public void TrackColumnsForAutoSizing(ICollection<int> columns)
	{
		_autoSizeColumnTracker.Value.TrackColumns(columns);
	}

	public void TrackAllColumnsForAutoSizing()
	{
		_autoSizeColumnTracker.Value.TrackAllColumns();
	}

	public bool UntrackColumnForAutoSizing(int column)
	{
		return _autoSizeColumnTracker.Value.UntrackColumn(column);
	}

	public bool UntrackColumnsForAutoSizing(ICollection<int> columns)
	{
		return _autoSizeColumnTracker.Value.UntrackColumns(columns);
	}

	public void UntrackAllColumnsForAutoSizing()
	{
		_autoSizeColumnTracker.Value.UntrackAllColumns();
	}

	public bool IsColumnTrackedForAutoSizing(int column)
	{
		return _autoSizeColumnTracker.Value.IsColumnTracked(column);
	}

	public void SetMargin(MarginType margin, double size)
	{
		_sh.SetMargin(margin, size);
	}

	public void SetRowBreak(int row)
	{
		_sh.SetRowBreak(row);
	}

	public void SetRowGroupCollapsed(int row, bool collapse)
	{
		if (collapse)
		{
			collapseRow(row);
			return;
		}
		throw new RuntimeException("Unable to expand row: Not Implemented");
	}

	private void collapseRow(int rowIndex)
	{
		SXSSFRow sXSSFRow = (SXSSFRow)GetRow(rowIndex);
		if (sXSSFRow == null)
		{
			throw new InvalidOperationException("Invalid row number(" + rowIndex + "). Row does not exist.");
		}
		int rowIndex2 = FindStartOfRowOutlineGroup(rowIndex);
		int rownum = WriteHidden(sXSSFRow, rowIndex2, hidden: true);
		SXSSFRow sXSSFRow2 = (SXSSFRow)GetRow(rownum);
		if (sXSSFRow2 != null)
		{
			sXSSFRow2.Collapsed = true;
		}
		else
		{
			((SXSSFRow)CreateRow(rownum)).Collapsed = true;
		}
	}

	private int FindStartOfRowOutlineGroup(int rowIndex)
	{
		int outlineLevel = ((SXSSFRow)GetRow(rowIndex)).OutlineLevel;
		if (outlineLevel == 0)
		{
			throw new InvalidOperationException("Outline level is zero for the row (" + rowIndex + ").");
		}
		int num = rowIndex;
		while (GetRow(num) != null)
		{
			if (GetRow(num).OutlineLevel < outlineLevel)
			{
				return num + 1;
			}
			num--;
		}
		return num + 1;
	}

	private int WriteHidden(SXSSFRow xRow, int rowIndex, bool hidden)
	{
		int outlineLevel = xRow.OutlineLevel;
		SXSSFRow sXSSFRow = (SXSSFRow)GetRow(rowIndex);
		while (sXSSFRow != null && sXSSFRow.OutlineLevel >= outlineLevel)
		{
			sXSSFRow.Hidden = hidden;
			rowIndex++;
			sXSSFRow = (SXSSFRow)GetRow(rowIndex);
		}
		return rowIndex;
	}

	[Obsolete("deprecated 2015-11-23 (circa POI 3.14beta1). Use {@link #setZoom(int)} instead.")]
	public void SetZoom(int numerator, int denominator)
	{
		_sh.SetZoom(numerator, denominator);
	}

	public void SetZoom(int scale)
	{
		_sh.SetZoom(scale);
	}

	public void ShiftRows(int startRow, int endRow, int n)
	{
		throw new NotImplementedException();
	}

	public void ShiftRows(int startRow, int endRow, int n, bool copyRowHeight, bool resetOriginalRowHeight)
	{
		throw new NotImplementedException();
	}

	public void ShowInPane(int toprow, int leftcol)
	{
		_sh.ShowInPane(toprow, leftcol);
	}

	public void UngroupColumn(int fromColumn, int toColumn)
	{
		_sh.UngroupColumn(fromColumn, toColumn);
	}

	public void UngroupRow(int fromRow, int toRow)
	{
		_sh.UngroupRow(fromRow, toRow);
	}

	public bool IsDate1904()
	{
		return _workbook.IsDate1904();
	}

	public int GetRowNum(SXSSFRow row)
	{
		foreach (KeyValuePair<int, SXSSFRow> row2 in _rows)
		{
			if (row2.Value == row)
			{
				return row2.Key;
			}
		}
		return -1;
	}

	public void ChangeRowNum(SXSSFRow row, int newRowNum)
	{
		RemoveRow(row);
		_rows.Add(newRowNum, row);
		UpdateIndexWhenAdd(newRowNum);
	}

	public bool Dispose()
	{
		if (!allFlushed)
		{
			FlushRows();
		}
		return _writer.Dispose();
	}

	private void FlushRows(int remaining, bool flushOnDisk)
	{
		KeyValuePair<int, SXSSFRow>? keyValuePair = null;
		int num = 0;
		while (_rows.Count > remaining)
		{
			num++;
			keyValuePair = flushOneRow();
		}
		InvalidateFirstRowNum();
		InvalidateLastRowNum();
		if (remaining == 0)
		{
			allFlushed = true;
		}
		if (keyValuePair.HasValue & flushOnDisk)
		{
			_writer.FlushRows(num, keyValuePair.Value.Key, keyValuePair.Value.Value.LastCellNum);
		}
	}

	public void FlushRows()
	{
		FlushRows(0, flushOnDisk: true);
	}

	private KeyValuePair<int, SXSSFRow>? flushOneRow()
	{
		if (_rows.Count == 0)
		{
			return null;
		}
		int num = _rows.Keys.Min();
		SXSSFRow sXSSFRow = _rows[num];
		_writer.WriteRow(num, sXSSFRow);
		_rows.Remove(num);
		lastFlushedRowNumber = num;
		return new KeyValuePair<int, SXSSFRow>(num, sXSSFRow);
	}

	public Stream GetWorksheetXMLInputStream()
	{
		FlushRows(0, flushOnDisk: true);
		_writer.Close();
		return _writer.GetWorksheetXmlInputStream();
	}

	public void CopyTo(IWorkbook dest, string name, bool copyStyle, bool keepFormulas)
	{
		throw new NotImplementedException();
	}
}
