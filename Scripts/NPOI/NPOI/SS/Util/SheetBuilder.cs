using System;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.SS.Util;

public class SheetBuilder
{
	private IWorkbook workbook;

	private object[][] cells;

	private bool shouldCreateEmptyCells;

	private string sheetName;

	public SheetBuilder(IWorkbook workbook, object[][] cells)
	{
		this.workbook = workbook;
		this.cells = (object[][])cells.Clone();
	}

	public bool GetCreateEmptyCells()
	{
		return shouldCreateEmptyCells;
	}

	public SheetBuilder SetCreateEmptyCells(bool shouldCreateEmptyCells)
	{
		this.shouldCreateEmptyCells = shouldCreateEmptyCells;
		return this;
	}

	public SheetBuilder SetSheetName(string sheetName)
	{
		this.sheetName = sheetName;
		return this;
	}

	public ISheet Build()
	{
		ISheet sheet = ((sheetName == null) ? workbook.CreateSheet() : workbook.CreateSheet(sheetName));
		IRow row = null;
		ICell cell = null;
		for (int i = 0; i < cells.Length; i++)
		{
			object[] array = cells[i];
			row = sheet.CreateRow(i);
			for (int j = 0; j < array.Length; j++)
			{
				object obj = array[j];
				if (obj != null || shouldCreateEmptyCells)
				{
					cell = row.CreateCell(j);
					SetCellValue(cell, obj);
				}
			}
		}
		return sheet;
	}

	private void SetCellValue(ICell cell, object value)
	{
		if (value != null && cell != null)
		{
			if (Number.IsNumber(value))
			{
				double.TryParse(value.ToString(), out var result);
				cell.SetCellValue(result);
			}
			else if (value is DateTime)
			{
				cell.SetCellValue((DateTime)value);
			}
			else if (IsFormulaDefinition(value))
			{
				cell.CellFormula = GetFormula(value);
			}
			else
			{
				cell.SetCellValue(value.ToString());
			}
		}
	}

	private bool IsFormulaDefinition(object obj)
	{
		if (obj is string)
		{
			if (((string)obj).Length < 2)
			{
				return false;
			}
			return ((string)obj)[0] == '=';
		}
		return false;
	}

	private string GetFormula(object obj)
	{
		return ((string)obj).Substring(1);
	}
}
