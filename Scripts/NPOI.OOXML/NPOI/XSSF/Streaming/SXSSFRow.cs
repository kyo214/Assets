using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.Streaming;

public class SXSSFRow : IRow, IEnumerable<ICell>, IEnumerable, IComparable<SXSSFRow>
{
	public class FilledCellIterator : IEnumerator<ICell>, IDisposable, IEnumerator
	{
		private IEnumerator<SXSSFCell> enumerator;

		public ICell Current => enumerator.Current;

		object IEnumerator.Current => enumerator.Current;

		public FilledCellIterator(SortedDictionary<int, SXSSFCell> cells)
		{
			enumerator = cells.Values.GetEnumerator();
		}

		public void Dispose()
		{
		}

		public IEnumerator<ICell> GetEnumerator()
		{
			return enumerator;
		}

		public bool MoveNext()
		{
			return enumerator.MoveNext();
		}

		public void Reset()
		{
			enumerator.Reset();
		}
	}

	public class CellIterator : IEnumerator<ICell>, IDisposable, IEnumerator
	{
		private IDictionary<int, SXSSFCell> _cells;

		private int maxColumn;

		private int pos;

		public ICell Current
		{
			get
			{
				if (!_cells.ContainsKey(pos))
				{
					return null;
				}
				return _cells[pos];
			}
		}

		object IEnumerator.Current => Current;

		public CellIterator(int lastCellNum, IDictionary<int, SXSSFCell> cells)
		{
			maxColumn = lastCellNum;
			pos = -1;
			_cells = cells;
		}

		public void Dispose()
		{
		}

		public IEnumerator<ICell> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public bool HasNext()
		{
			return pos < maxColumn;
		}

		public bool MoveNext()
		{
			if (HasNext())
			{
				pos++;
				return true;
			}
			return false;
		}

		public ICell Next()
		{
			if (HasNext())
			{
				if (_cells.ContainsKey(pos))
				{
					return _cells[pos++];
				}
				pos++;
				return null;
			}
			throw new NullReferenceException();
		}

		public void Remove()
		{
			throw new InvalidOperationException();
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}
	}

	private SXSSFSheet _sheet;

	private IDictionary<int, SXSSFCell> _cells = new Dictionary<int, SXSSFCell>();

	private short _style = -1;

	private bool _zHeight;

	private float _height = -1f;

	private int _FirstCellNum = -1;

	private int _LastCellNum = -1;

	public bool? Hidden { get; set; }

	public bool? Collapsed { get; set; }

	public List<ICell> Cells => ((IEnumerable<SXSSFCell>)_cells.Values).Select((Func<SXSSFCell, ICell>)((SXSSFCell cell) => cell)).ToList();

	public short FirstCellNum
	{
		get
		{
			try
			{
				return (short)_FirstCellNum;
			}
			catch
			{
				return -1;
			}
		}
	}

	public short Height
	{
		get
		{
			return (short)((_height == -1f) ? (Sheet.DefaultRowHeightInPoints * 20f) : _height);
		}
		set
		{
			_height = value;
		}
	}

	public float HeightInPoints
	{
		get
		{
			return (float)((_height == -1f) ? ((double)Sheet.DefaultRowHeightInPoints) : ((double)_height / 20.0));
		}
		set
		{
			_height = ((value == -1f) ? (-1f) : (value * 20f));
		}
	}

	public bool IsFormatted => _style > -1;

	public short LastCellNum => (short)_LastCellNum;

	public int OutlineLevel { get; set; }

	public int PhysicalNumberOfCells => Cells.Count;

	public int RowNum
	{
		get
		{
			return _sheet.GetRowNum(this);
		}
		set
		{
			_sheet.ChangeRowNum(this, value);
		}
	}

	internal int RowStyleIndex => _style;

	public ICellStyle RowStyle
	{
		get
		{
			if (!IsFormatted)
			{
				return null;
			}
			return Sheet.Workbook.GetCellStyleAt(_style);
		}
		set
		{
			if (value == null)
			{
				_style = -1;
			}
			else
			{
				_style = value.Index;
			}
		}
	}

	public ISheet Sheet => _sheet;

	public bool ZeroHeight
	{
		get
		{
			return _zHeight;
		}
		set
		{
			_zHeight = value;
		}
	}

	public SXSSFRow(SXSSFSheet sheet)
	{
		_sheet = sheet;
	}

	public CellIterator AllCellsIterator()
	{
		return new CellIterator(LastCellNum, new SortedDictionary<int, SXSSFCell>(_cells));
	}

	public bool HasCustomHeight()
	{
		return Height != -1;
	}

	public int CompareTo(SXSSFRow other)
	{
		if (Sheet != other.Sheet)
		{
			throw new InvalidOperationException("The compared rows must belong to the same sheet");
		}
		int rowNum = RowNum;
		int rowNum2 = other.RowNum;
		return rowNum.CompareTo(rowNum2);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is SXSSFRow))
		{
			return false;
		}
		SXSSFRow sXSSFRow = (SXSSFRow)obj;
		if (RowNum == sXSSFRow.RowNum)
		{
			return Sheet == sXSSFRow.Sheet;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return _cells.GetHashCode();
	}

	public ICell CopyCell(int sourceIndex, int targetIndex)
	{
		throw new NotImplementedException();
	}

	public IRow CopyRowTo(int targetIndex)
	{
		throw new NotImplementedException();
	}

	public ICell CreateCell(int column)
	{
		return CreateCell(column, CellType.Blank);
	}

	public ICell CreateCell(int column, CellType type)
	{
		CheckBounds(column);
		SXSSFCell sXSSFCell = new SXSSFCell(this, type);
		_cells[column] = sXSSFCell;
		UpdateIndexWhenAdd(column);
		return sXSSFCell;
	}

	private void UpdateIndexWhenAdd(int cellnum)
	{
		if (cellnum < _FirstCellNum || _FirstCellNum == -1)
		{
			_FirstCellNum = cellnum;
		}
		if (cellnum >= _LastCellNum)
		{
			_LastCellNum = cellnum + 1;
		}
	}

	private static void CheckBounds(int cellIndex)
	{
		SpreadsheetVersion eXCEL = SpreadsheetVersion.EXCEL2007;
		int lastColumnIndex = SpreadsheetVersion.EXCEL2007.LastColumnIndex;
		if (cellIndex < 0 || cellIndex > lastColumnIndex)
		{
			throw new ArgumentException("Invalid column index (" + cellIndex + ").  Allowable column range for " + eXCEL.DefaultExtension + " is (0.." + lastColumnIndex + ") or ('A'..'" + eXCEL.LastColumnName + "')");
		}
	}

	public ICell GetCell(int cellnum)
	{
		MissingCellPolicy missingCellPolicy = _sheet.Workbook.MissingCellPolicy;
		return GetCell(cellnum, missingCellPolicy);
	}

	public ICell GetCell(int cellnum, MissingCellPolicy policy)
	{
		CheckBounds(cellnum);
		SXSSFCell sXSSFCell = null;
		if (_cells.ContainsKey(cellnum))
		{
			sXSSFCell = _cells[cellnum];
		}
		switch (policy)
		{
		case MissingCellPolicy.RETURN_NULL_AND_BLANK:
			return sXSSFCell;
		case MissingCellPolicy.RETURN_BLANK_AS_NULL:
			if (sXSSFCell == null || sXSSFCell.CellType != CellType.Blank)
			{
				return sXSSFCell;
			}
			return null;
		case MissingCellPolicy.CREATE_NULL_AS_BLANK:
			if (sXSSFCell != null)
			{
				return sXSSFCell;
			}
			return CreateCell(cellnum, CellType.Blank);
		default:
			throw new ArgumentException("Illegal policy " + policy.ToString() + " (" + policy.ToString() + ")");
		}
	}

	public IEnumerator<ICell> GetEnumerator()
	{
		return new FilledCellIterator(new SortedDictionary<int, SXSSFCell>(_cells));
	}

	public void MoveCell(ICell cell, int newColumn)
	{
		throw new NotImplementedException();
	}

	public void RemoveCell(ICell cell)
	{
		int cellIndex = GetCellIndex((SXSSFCell)cell);
		_cells.Remove(cellIndex);
		if (cellIndex == _FirstCellNum)
		{
			InvalidateFirstCellNum();
		}
		if (cellIndex >= _LastCellNum - 1)
		{
			InvalidateLastCellNum();
		}
	}

	private void InvalidateFirstCellNum()
	{
		if (_cells.Keys.Count == 0)
		{
			_FirstCellNum = 0;
		}
		else
		{
			_FirstCellNum = _cells.Keys.Min();
		}
	}

	private void InvalidateLastCellNum()
	{
		if (_cells.Count == 0)
		{
			_LastCellNum = 0;
		}
		else
		{
			_LastCellNum = _cells.Keys.Max() + 1;
		}
	}

	public int GetCellIndex(SXSSFCell cell)
	{
		foreach (KeyValuePair<int, SXSSFCell> cell2 in _cells)
		{
			if (cell2.Value == cell)
			{
				return cell2.Key;
			}
		}
		return -1;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotImplementedException();
	}
}
