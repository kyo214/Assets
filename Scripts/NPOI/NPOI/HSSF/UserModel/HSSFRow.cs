using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.HSSF.Record;
using NPOI.SS;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.HSSF.UserModel;

[Serializable]
public class HSSFRow : IRow, IEnumerable<ICell>, IEnumerable, IComparable<HSSFRow>
{
	public const int INITIAL_CAPACITY = 5;

	private int rowNum;

	private SortedDictionary<int, ICell> cells = new SortedDictionary<int, ICell>();

	[NonSerialized]
	private RowRecord row;

	private HSSFWorkbook book;

	private HSSFSheet sheet;

	public bool IsHidden
	{
		get
		{
			return ZeroHeight;
		}
		set
		{
			ZeroHeight = value;
		}
	}

	public int RowNum
	{
		get
		{
			return rowNum;
		}
		set
		{
			int lastRowIndex = SpreadsheetVersion.EXCEL97.LastRowIndex;
			if (value < 0 || value > lastRowIndex)
			{
				throw new ArgumentException("Invalid row number (" + value + ") outside allowable range (0.." + lastRowIndex + ")");
			}
			rowNum = value;
			if (row != null)
			{
				row.RowNumber = value;
			}
		}
	}

	public int OutlineLevel => row.OutlineLevel;

	public ISheet Sheet => sheet;

	public short FirstCellNum
	{
		get
		{
			if (row.IsEmpty)
			{
				return -1;
			}
			return (short)row.FirstCol;
		}
	}

	public short LastCellNum
	{
		get
		{
			if (row.IsEmpty)
			{
				return -1;
			}
			return (short)row.LastCol;
		}
	}

	public int PhysicalNumberOfCells => cells.Count;

	public bool ZeroHeight
	{
		get
		{
			return row.ZeroHeight;
		}
		set
		{
			row.ZeroHeight = value;
		}
	}

	public short Height
	{
		get
		{
			short height = row.Height;
			if ((height & 0x8000) != 0)
			{
				return sheet.Sheet.DefaultRowHeight;
			}
			return (short)(height & 0x7FFF);
		}
		set
		{
			if (value == -1)
			{
				row.Height = -32513;
				row.BadFontHeight = false;
			}
			else
			{
				row.BadFontHeight = true;
				row.Height = value;
			}
		}
	}

	public bool IsFormatted => row.Formatted;

	public ICellStyle RowStyle
	{
		get
		{
			if (!IsFormatted)
			{
				return null;
			}
			short xFIndex = row.XFIndex;
			ExtendedFormatRecord exFormatAt = book.Workbook.GetExFormatAt(xFIndex);
			return new HSSFCellStyle(xFIndex, exFormatAt, book);
		}
		set
		{
			row.Formatted = true;
			row.XFIndex = value.Index;
		}
	}

	public float HeightInPoints
	{
		get
		{
			return (float)Height / 20f;
		}
		set
		{
			if (value == -1f)
			{
				row.Height = -32513;
				return;
			}
			row.BadFontHeight = true;
			row.Height = (short)(value * 20f);
		}
	}

	public RowRecord RowRecord => row;

	public List<ICell> Cells => new List<ICell>(cells.Values);

	public bool? Hidden
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

	public bool? Collapsed
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

	[Obsolete]
	public HSSFRow()
	{
	}

	public HSSFRow(HSSFWorkbook book, HSSFSheet sheet, int rowNum)
		: this(book, sheet, new RowRecord(rowNum))
	{
	}

	public HSSFRow(HSSFWorkbook book, HSSFSheet sheet, RowRecord record)
	{
		this.book = book;
		this.sheet = sheet;
		row = record;
		RowNum = record.RowNumber;
		record.SetEmpty();
	}

	public ICell CreateCell(int column)
	{
		return CreateCell(column, CellType.Blank);
	}

	public ICell CreateCell(int columnIndex, CellType type)
	{
		_ = 32767;
		ICell cell = new HSSFCell(book, sheet, RowNum, (short)columnIndex, type);
		AddCell(cell);
		sheet.Sheet.AddValueRecord(RowNum, ((HSSFCell)cell).CellValueRecord);
		return cell;
	}

	public IRow CopyRowTo(int targetIndex)
	{
		return sheet.CopyRow(RowNum, targetIndex);
	}

	public ICell CopyCell(int sourceIndex, int targetIndex)
	{
		return CellUtil.CopyCell(this, sourceIndex, targetIndex);
	}

	public void RemoveCell(ICell cell)
	{
		if (cell == null)
		{
			throw new ArgumentException("cell must not be null");
		}
		RemoveCell((HSSFCell)cell, alsoRemoveRecords: true);
	}

	private void RemoveCell(ICell cell, bool alsoRemoveRecords)
	{
		int columnIndex = cell.ColumnIndex;
		if (columnIndex < 0)
		{
			throw new Exception("Negative cell indexes not allowed");
		}
		if (!cells.ContainsKey(columnIndex) || cell != cells[columnIndex])
		{
			throw new Exception("Specified cell is not from this row");
		}
		if (cell.IsPartOfArrayFormulaGroup)
		{
			((HSSFCell)cell).NotifyArrayFormulaChanging();
		}
		cells.Remove(columnIndex);
		if (alsoRemoveRecords)
		{
			CellValueRecordInterface cellValueRecord = ((HSSFCell)cell).CellValueRecord;
			sheet.Sheet.RemoveValueRecord(RowNum, cellValueRecord);
		}
		if (cell.ColumnIndex + 1 == row.LastCol)
		{
			row.LastCol = CalculateNewLastCellPlusOne(row.LastCol);
		}
		if (cell.ColumnIndex == row.FirstCol)
		{
			row.FirstCol = CalculateNewFirstCell(row.FirstCol);
		}
	}

	private int CalculateNewLastCellPlusOne(int lastcell)
	{
		int num = lastcell - 1;
		for (ICell cell = RetrieveCell(num); cell == null; cell = RetrieveCell(--num))
		{
			if (num < 0)
			{
				return 0;
			}
		}
		return num + 1;
	}

	private int CalculateNewFirstCell(int firstcell)
	{
		int num = firstcell + 1;
		ICell cell = RetrieveCell(num);
		if (cells.Count == 0)
		{
			return 0;
		}
		while (cell == null)
		{
			if (num <= cells.Count)
			{
				return 0;
			}
			cell = RetrieveCell(++num);
		}
		return num;
	}

	public ICell CreateCellFromRecord(CellValueRecordInterface cell)
	{
		ICell cell2 = new HSSFCell(book, sheet, cell);
		AddCell(cell2);
		int column = cell.Column;
		if (row.IsEmpty)
		{
			row.FirstCol = column;
			row.LastCol = column + 1;
		}
		else if (column < row.FirstCol)
		{
			row.FirstCol = column;
		}
		else if (column > row.LastCol)
		{
			row.LastCol = column + 1;
		}
		return cell2;
	}

	public void RemoveAllCells()
	{
		ICell[] array = new ICell[cells.Values.Count];
		cells.Values.CopyTo(array, 0);
		ICell[] array2 = array;
		foreach (ICell cell in array2)
		{
			RemoveCell(cell, alsoRemoveRecords: true);
		}
	}

	public void MoveCell(ICell cell, int newColumn)
	{
		if (cells.ContainsKey(newColumn))
		{
			throw new ArgumentException("Asked to move cell to column " + newColumn + " but there's already a cell there");
		}
		bool flag = false;
		foreach (ICell value in cells.Values)
		{
			if (value.Equals(cell))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			throw new ArgumentException("Asked to move a cell, but it didn't belong to our row");
		}
		RemoveCell(cell, alsoRemoveRecords: false);
		((HSSFCell)cell).UpdateCellNum(newColumn);
		AddCell(cell);
	}

	private void AddCell(ICell cell)
	{
		int columnIndex = cell.ColumnIndex;
		if (cells.ContainsKey(columnIndex))
		{
			cells.Remove(columnIndex);
		}
		cells.Add(columnIndex, cell);
		if (row.IsEmpty || columnIndex < row.FirstCol)
		{
			row.FirstCol = columnIndex;
		}
		if (row.IsEmpty || columnIndex >= row.LastCol)
		{
			row.LastCol = (short)(columnIndex + 1);
		}
	}

	private ICell RetrieveCell(int cellnum)
	{
		if (!cells.ContainsKey(cellnum))
		{
			return null;
		}
		return cells[cellnum];
	}

	public ICell GetCell(int cellnum)
	{
		return GetCell(cellnum, book.MissingCellPolicy);
	}

	public ICell GetCell(int cellnum, MissingCellPolicy policy)
	{
		ICell cell = RetrieveCell(cellnum);
		switch (policy)
		{
		case MissingCellPolicy.RETURN_NULL_AND_BLANK:
			return cell;
		case MissingCellPolicy.RETURN_BLANK_AS_NULL:
			if (cell == null || cell.CellType != CellType.Blank)
			{
				return cell;
			}
			return null;
		case MissingCellPolicy.CREATE_NULL_AS_BLANK:
			if (cell != null)
			{
				return cell;
			}
			return CreateCell(cellnum, CellType.Blank);
		default:
			throw new ArgumentException("Illegal policy " + policy.ToString() + " (" + policy.ToString() + ")");
		}
	}

	[Obsolete]
	private short FindFirstCell(int firstcell)
	{
		int num = firstcell + 1;
		ICell cell = GetCell(num);
		while (cell == null && num <= LastCellNum)
		{
			cell = GetCell(++num);
		}
		if (num > LastCellNum)
		{
			return -1;
		}
		return (short)num;
	}

	public IEnumerator<ICell> GetEnumerator()
	{
		return cells.Values.GetEnumerator();
	}

	public int CompareTo(HSSFRow other)
	{
		if (Sheet != other.Sheet)
		{
			throw new ArgumentException("The compared rows must belong to the same sheet");
		}
		return RowNum.CompareTo(other.RowNum);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is HSSFRow))
		{
			return false;
		}
		HSSFRow hSSFRow = (HSSFRow)obj;
		if (RowNum == hSSFRow.RowNum)
		{
			return Sheet == hSSFRow.Sheet;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return row.GetHashCode();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public bool HasCustomHeight()
	{
		throw new NotImplementedException();
	}
}
