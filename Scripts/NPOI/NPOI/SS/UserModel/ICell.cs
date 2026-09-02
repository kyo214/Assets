using System;
using NPOI.SS.Util;

namespace NPOI.SS.UserModel;

public interface ICell
{
	int ColumnIndex { get; }

	int RowIndex { get; }

	ISheet Sheet { get; }

	IRow Row { get; }

	CellType CellType { get; }

	CellType CachedFormulaResultType { get; }

	string CellFormula { get; set; }

	double NumericCellValue { get; }

	DateTime DateCellValue { get; }

	IRichTextString RichStringCellValue { get; }

	byte ErrorCellValue { get; }

	string StringCellValue { get; }

	bool BooleanCellValue { get; }

	ICellStyle CellStyle { get; set; }

	CellAddress Address { get; }

	IComment CellComment { get; set; }

	IHyperlink Hyperlink { get; set; }

	CellRangeAddress ArrayFormulaRange { get; }

	bool IsPartOfArrayFormulaGroup { get; }

	bool IsMergedCell { get; }

	void SetCellType(CellType cellType);

	void SetBlank();

	void SetCellValue(double value);

	void SetCellErrorValue(byte value);

	void SetCellValue(DateTime value);

	void SetCellValue(IRichTextString value);

	void SetCellValue(string value);

	ICell CopyCellTo(int targetIndex);

	void SetCellFormula(string formula);

	void SetCellValue(bool value);

	void SetAsActiveCell();

	void RemoveCellComment();

	void RemoveHyperlink();

	CellType GetCachedFormulaResultTypeEnum();
}
