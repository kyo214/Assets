using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.SS.UserModel;

namespace BansheeGz.BGDatabase;

public abstract class BGExcelSheetWriterART
{
	protected readonly BGLogger logger;

	protected readonly BGRepo repo;

	protected readonly IWorkbook book;

	protected readonly BGBookInfo bookInfo;

	private readonly HashSet<int> usedRows = new HashSet<int>();

	protected int currentRow;

	protected ISheet sheet;

	protected IRow row;

	protected BGSheetInfoA sheetInfo;

	public IRow IRow => row;

	public int CurrentRow
	{
		get
		{
			return currentRow;
		}
		set
		{
			currentRow = value;
		}
	}

	public int NewRow
	{
		get
		{
			for (int i = 1; i < 1048575; i++)
			{
				if (!usedRows.Contains(i))
				{
					IRow row = sheet.GetRow(i);
					if (row == null)
					{
						usedRows.Add(i);
						return i;
					}
					if (IsRowEmpty(row))
					{
						usedRows.Add(i);
						return i;
					}
				}
			}
			throw new Exception("It looks like sheet " + sheet.SheetName + " reached the maximum number of rows = 1048575!");
		}
	}

	public int NewCellIndex
	{
		get
		{
			int num = -1;
			foreach (ICell cell in row.Cells)
			{
				if (num < cell.ColumnIndex)
				{
					num = cell.ColumnIndex;
				}
			}
			return num + 1;
		}
	}

	protected BGExcelSheetWriterART(BGLogger logger, BGRepo repo, IWorkbook book, BGBookInfo bookInfo)
	{
		this.logger = logger;
		this.repo = repo;
		this.book = book;
		this.bookInfo = bookInfo;
	}

	public void Row(Action action)
	{
		Row(currentRow, action);
	}

	public void Row(int index, Action action)
	{
		row = GetRow(index) ?? sheet.CreateRow(index);
		currentRow = index;
		action();
		currentRow++;
		row.Height = -1;
	}

	public IRow GetRow(int index)
	{
		return sheet.GetRow(index);
	}

	protected void Delete(List<int> rows)
	{
		if (BGUtil.IsEmpty(rows))
		{
			return;
		}
		rows.Sort();
		int lastRowNum = sheet.LastRowNum;
		for (int i = 0; i < rows.Count; i++)
		{
			int num = rows[i] - i;
			if (num < lastRowNum)
			{
				sheet.ShiftRows(num + 1, lastRowNum, -1);
			}
		}
		for (int num2 = lastRowNum; num2 > lastRowNum - rows.Count; num2--)
		{
			IRow row = sheet.GetRow(num2);
			if (row != null)
			{
				sheet.RemoveRow(row);
			}
		}
	}

	protected void Cell(int index, Action<ICell> cellAction)
	{
		ICell obj = row.GetCell(index) ?? row.CreateCell(index);
		cellAction(obj);
	}

	protected void Cell(int index, bool value)
	{
		Cell(index, (ICell cell) =>
		{
			cell.SetCellValue(value);
		});
	}

	protected void Cell(int index, double value)
	{
		Cell(index, (ICell cell) =>
		{
			cell.SetCellValue(value);
		});
	}

	protected void Cell(int index, string value)
	{
		Cell(index, (ICell cell) =>
		{
			cell.SetCellValue(value);
		});
	}

	protected static void Clear(ISheet sheet)
	{
		List<IRow> list = new List<IRow>();
		IEnumerator rowEnumerator = sheet.GetRowEnumerator();
		while (rowEnumerator.MoveNext())
		{
			list.Add((IRow)rowEnumerator.Current);
		}
		foreach (IRow item in list)
		{
			sheet.RemoveRow(item);
		}
	}

	protected int MapHeader(string header, int index)
	{
		if (index < 0)
		{
			index = NewCellIndex;
			logger.AppendLine("$ column not found. Created new column at index $", header, index);
		}
		else
		{
			logger.AppendLine("$ column found at index $", header, index);
		}
		Cell(index, header);
		return index;
	}

	protected void Sheet<T>(string name, bool @override, Func<T> provider, Func<T> factory, Action<T> action) where T : BGSheetInfoA
	{
		currentRow = 0;
		T val = provider();
		if (val == null)
		{
			logger.AppendLine("Sheet with name $ not found. Creating a new sheet..", name);
			string duplicateSheetName = GetDuplicateSheetName(name);
			if (duplicateSheetName != null)
			{
				throw new BGException("Can not create an Excel sheet with name=$, cause a sheet with the same name=$ already exists (comparison is case insensitive)", name, duplicateSheetName);
			}
			sheet = book.CreateSheet(name);
			val = factory();
		}
		else
		{
			logger.AppendLine("Found existing sheet with name $", name);
			sheet = book.GetSheetAt(val.SheetNumber);
			if (@override)
			{
				Clear(sheet);
				val.Clear();
			}
		}
		sheetInfo = val;
		usedRows.Clear();
		val.ForEachRow((BGId id, int index) =>
		{
			usedRows.Add(index);
		});
		action(val);
	}

	protected void Remove(BGSheetInfoA info, Predicate<BGId> predicate)
	{
		List<int> rowsToRemove = new List<int>();
		info.ForEachRow((BGId id, int rowIndex) =>
		{
			if (predicate(id))
			{
				rowsToRemove.Add(rowIndex);
			}
		});
		Delete(rowsToRemove);
	}

	protected bool GetRowIndex(BGSheetInfoA info, BGId id, bool isAdding, bool isUpdating, out int rowIndex)
	{
		rowIndex = info.GetRow(id);
		if (rowIndex == -1)
		{
			if (!isAdding)
			{
				return false;
			}
			rowIndex = NewRow;
		}
		else if (!isUpdating)
		{
			return false;
		}
		return true;
	}

	private string GetDuplicateSheetName(string name)
	{
		for (int i = 0; i < book.NumberOfSheets; i++)
		{
			string sheetName = book.GetSheetName(i);
			if (name.Equals(sheetName, StringComparison.InvariantCultureIgnoreCase))
			{
				return sheetName;
			}
		}
		return null;
	}

	public static bool IsCellEmpty(IRow row, int index)
	{
		return IsCellEmpty(row.GetCell(index));
	}

	public static bool IsCellEmpty(ICell cell)
	{
		if (cell == null)
		{
			return true;
		}
		bool flag = false;
		switch (cell.CellType)
		{
		case CellType.Numeric:
			flag = cell.NumericCellValue != 0.0;
			break;
		case CellType.String:
			flag = !string.IsNullOrEmpty(cell.StringCellValue);
			break;
		case CellType.Formula:
			flag = true;
			break;
		case CellType.Boolean:
			flag = cell.BooleanCellValue;
			break;
		case CellType.Error:
			flag = true;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		case CellType.Unknown:
		case CellType.Blank:
			break;
		}
		return !flag;
	}

	private static bool IsRowEmpty(IRow row)
	{
		List<ICell> cells = row.Cells;
		if (cells == null || cells.Count == 0)
		{
			return true;
		}
		foreach (ICell item in cells)
		{
			if (!IsCellEmpty(item))
			{
				return false;
			}
		}
		return true;
	}
}
