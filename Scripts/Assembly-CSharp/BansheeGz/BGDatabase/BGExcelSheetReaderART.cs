using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace BansheeGz.BGDatabase;

public class BGExcelSheetReaderART
{
	public class ExitException : Exception
	{
	}

	protected static int ForEachRowNoHeader(ISheet sheet, Action<IRow> action)
	{
		IEnumerator enumerator = sheet.GetEnumerator();
		int num = 0;
		while (enumerator.MoveNext())
		{
			num++;
			if (num != 1)
			{
				try
				{
					action((IRow)enumerator.Current);
				}
				catch (ExitException)
				{
				}
			}
		}
		return num;
	}

	protected static void ForEachCell(IRow row, Action<int, ICell> action)
	{
		List<ICell> list = row?.Cells;
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				ICell arg = list[i];
				action(i, arg);
			}
		}
	}

	public void ReadNotNull(IRow row, int index, Action<string> action)
	{
		ICell cell = row.GetCell(index);
		if (cell != null)
		{
			string text = ReadAsString(cell);
			if (!string.IsNullOrEmpty(text))
			{
				action(text);
			}
		}
	}

	protected void Read(IRow row, int index, Action<string> action)
	{
		ICell cell = row.GetCell(index);
		if (cell != null)
		{
			action(ReadAsString(cell));
		}
	}

	protected static string ReadAsString(ICell cell)
	{
		if (cell != null)
		{
			return ReadCell(cell.CellType, cell, allowFormula: true);
		}
		return null;
	}

	private static string ReadCell(CellType cellType, ICell cell, bool allowFormula)
	{
		switch (cellType)
		{
		case CellType.Numeric:
			return cell.NumericCellValue.ToString() ?? "";
		case CellType.String:
			return cell.StringCellValue?.Trim();
		case CellType.Boolean:
			return cell.BooleanCellValue ? "1" : "0";
		case CellType.Formula:
			if (!allowFormula)
			{
				throw new Exception("Formulas not allowed at this point");
			}
			if (string.IsNullOrEmpty(cell.CellFormula))
			{
				return null;
			}
			if (cell.IsPartOfArrayFormulaGroup)
			{
				return ReadCell(cell.Sheet.Workbook.GetCreationHelper().CreateFormulaEvaluator().Evaluate(cell)
					.CellType, cell, allowFormula: false);
				}
				return ReadCell(cell.CachedFormulaResultType, cell, allowFormula: false);
			default:
				return null;
			}
		}
	}
