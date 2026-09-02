using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.SS.Util;

namespace NPOI.SS.UserModel;

public interface ISheet
{
	int PhysicalNumberOfRows { get; }

	int FirstRowNum { get; }

	int LastRowNum { get; }

	bool ForceFormulaRecalculation { get; set; }

	int DefaultColumnWidth { get; set; }

	short DefaultRowHeight { get; set; }

	float DefaultRowHeightInPoints { get; set; }

	bool HorizontallyCenter { get; set; }

	bool VerticallyCenter { get; set; }

	int NumMergedRegions { get; }

	List<CellRangeAddress> MergedRegions { get; }

	bool DisplayZeros { get; set; }

	bool Autobreaks { get; set; }

	bool DisplayGuts { get; set; }

	bool FitToPage { get; set; }

	bool RowSumsBelow { get; set; }

	bool RowSumsRight { get; set; }

	bool IsPrintGridlines { get; set; }

	bool IsPrintRowAndColumnHeadings { get; set; }

	IPrintSetup PrintSetup { get; }

	IHeader Header { get; }

	IFooter Footer { get; }

	bool Protect { get; }

	bool ScenarioProtect { get; }

	short TabColorIndex { get; set; }

	IDrawing DrawingPatriarch { get; }

	short TopRow { get; set; }

	short LeftCol { get; set; }

	PaneInformation PaneInformation { get; }

	bool DisplayGridlines { get; set; }

	bool DisplayFormulas { get; set; }

	bool DisplayRowColHeadings { get; set; }

	bool IsActive { get; set; }

	int[] RowBreaks { get; }

	int[] ColumnBreaks { get; }

	IWorkbook Workbook { get; }

	string SheetName { get; }

	bool IsSelected { get; set; }

	ISheetConditionalFormatting SheetConditionalFormatting { get; }

	bool IsRightToLeft { get; set; }

	CellRangeAddress RepeatingRows { get; set; }

	CellRangeAddress RepeatingColumns { get; set; }

	CellAddress ActiveCell { get; set; }

	IRow CreateRow(int rownum);

	void RemoveRow(IRow row);

	IRow GetRow(int rownum);

	void SetColumnHidden(int columnIndex, bool hidden);

	bool IsColumnHidden(int columnIndex);

	IRow CopyRow(int sourceIndex, int targetIndex);

	void SetColumnWidth(int columnIndex, int width);

	int GetColumnWidth(int columnIndex);

	float GetColumnWidthInPixels(int columnIndex);

	ICellStyle GetColumnStyle(int column);

	int AddMergedRegion(CellRangeAddress region);

	int AddMergedRegionUnsafe(CellRangeAddress region);

	void ValidateMergedRegions();

	void RemoveMergedRegion(int index);

	void RemoveMergedRegions(IList<int> indices);

	CellRangeAddress GetMergedRegion(int index);

	IEnumerator GetRowEnumerator();

	IEnumerator GetEnumerator();

	double GetMargin(MarginType margin);

	void SetMargin(MarginType margin, double size);

	void ProtectSheet(string password);

	[Obsolete("deprecated 2015-11-23 (circa POI 3.14beta1). Use {@link #setZoom(int)} instead.")]
	void SetZoom(int numerator, int denominator);

	void SetZoom(int scale);

	void ShowInPane(int toprow, int leftcol);

	void ShiftRows(int startRow, int endRow, int n);

	void ShiftRows(int startRow, int endRow, int n, bool copyRowHeight, bool resetOriginalRowHeight);

	void CreateFreezePane(int colSplit, int rowSplit, int leftmostColumn, int topRow);

	void CreateFreezePane(int colSplit, int rowSplit);

	void CreateSplitPane(int xSplitPos, int ySplitPos, int leftmostColumn, int topRow, PanePosition activePane);

	bool IsRowBroken(int row);

	void RemoveRowBreak(int row);

	void SetActiveCellRange(int firstRow, int lastRow, int firstColumn, int lastColumn);

	void SetActiveCellRange(List<CellRangeAddress8Bit> cellranges, int activeRange, int activeRow, int activeColumn);

	void SetColumnBreak(int column);

	void SetRowBreak(int row);

	bool IsColumnBroken(int column);

	void RemoveColumnBreak(int column);

	void SetColumnGroupCollapsed(int columnNumber, bool collapsed);

	void GroupColumn(int fromColumn, int toColumn);

	void UngroupColumn(int fromColumn, int toColumn);

	void GroupRow(int fromRow, int toRow);

	void UngroupRow(int fromRow, int toRow);

	void SetRowGroupCollapsed(int row, bool collapse);

	void SetDefaultColumnStyle(int column, ICellStyle style);

	void AutoSizeColumn(int column);

	void AutoSizeColumn(int column, bool useMergedCells);

	[Obsolete("deprecated as of 2015-11-23 (circa POI 3.14beta1). Use {@link #getCellComment(CellAddress)} instead.")]
	IComment GetCellComment(int row, int column);

	IComment GetCellComment(CellAddress ref1);

	Dictionary<CellAddress, IComment> GetCellComments();

	IDrawing CreateDrawingPatriarch();

	void SetActive(bool value);

	ICellRange<ICell> SetArrayFormula(string formula, CellRangeAddress range);

	ICellRange<ICell> RemoveArrayFormula(ICell cell);

	bool IsMergedRegion(CellRangeAddress mergedRegion);

	IDataValidationHelper GetDataValidationHelper();

	List<IDataValidation> GetDataValidations();

	void AddValidationData(IDataValidation dataValidation);

	IAutoFilter SetAutoFilter(CellRangeAddress range);

	ISheet CopySheet(string Name);

	ISheet CopySheet(string Name, bool copyStyle);

	int GetColumnOutlineLevel(int columnIndex);

	bool IsDate1904();

	IHyperlink GetHyperlink(int row, int column);

	IHyperlink GetHyperlink(CellAddress addr);

	List<IHyperlink> GetHyperlinkList();

	void CopyTo(IWorkbook dest, string name, bool copyStyle, bool keepFormulas);
}
