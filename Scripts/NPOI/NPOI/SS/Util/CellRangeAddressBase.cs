using System;

namespace NPOI.SS.Util;

public abstract class CellRangeAddressBase
{
	private int _firstRow;

	private int _firstCol;

	private int _lastRow;

	private int _lastCol;

	public bool IsFullColumnRange
	{
		get
		{
			if (_firstRow != 0 || _lastRow != SpreadsheetVersion.EXCEL97.LastRowIndex)
			{
				if (_firstRow == -1)
				{
					return _lastRow == -1;
				}
				return false;
			}
			return true;
		}
	}

	public bool IsFullRowRange
	{
		get
		{
			if (_firstCol != 0 || _lastCol != SpreadsheetVersion.EXCEL97.LastColumnIndex)
			{
				if (_firstCol == -1)
				{
					return _lastCol == -1;
				}
				return false;
			}
			return true;
		}
	}

	public int FirstColumn
	{
		get
		{
			return _firstCol;
		}
		set
		{
			_firstCol = value;
		}
	}

	public int FirstRow
	{
		get
		{
			return _firstRow;
		}
		set
		{
			_firstRow = value;
		}
	}

	public int LastColumn
	{
		get
		{
			return _lastCol;
		}
		set
		{
			_lastCol = value;
		}
	}

	public int LastRow
	{
		get
		{
			return _lastRow;
		}
		set
		{
			_lastRow = value;
		}
	}

	public int NumberOfCells => (_lastRow - _firstRow + 1) * (_lastCol - _firstCol + 1);

	public int MinRow => Math.Min(_firstRow, _lastRow);

	public int MaxRow => Math.Max(_firstRow, _lastRow);

	public int MinColumn => Math.Min(_firstCol, _lastCol);

	public int MaxColumn => Math.Max(_firstCol, _lastCol);

	protected CellRangeAddressBase(int firstRow, int lastRow, int firstCol, int lastCol)
	{
		_firstRow = firstRow;
		_lastRow = lastRow;
		_firstCol = firstCol;
		_lastCol = lastCol;
	}

	public void Validate(SpreadsheetVersion ssVersion)
	{
		ValidateRow(_firstRow, ssVersion);
		ValidateRow(_lastRow, ssVersion);
		ValidateColumn(_firstCol, ssVersion);
		ValidateColumn(_lastCol, ssVersion);
	}

	private static void ValidateRow(int row, SpreadsheetVersion ssVersion)
	{
		int lastRowIndex = ssVersion.LastRowIndex;
		if (row > lastRowIndex)
		{
			throw new ArgumentException("Maximum row number is " + lastRowIndex);
		}
		if (row < 0)
		{
			throw new ArgumentException("Minumum row number is 0");
		}
	}

	private static void ValidateColumn(int column, SpreadsheetVersion ssVersion)
	{
		int lastColumnIndex = ssVersion.LastColumnIndex;
		if (column > lastColumnIndex)
		{
			throw new ArgumentException("Maximum column number is " + lastColumnIndex);
		}
		if (column < 0)
		{
			throw new ArgumentException("Minimum column number is 0");
		}
	}

	public bool IsInRange(int rowInd, int colInd)
	{
		if (_firstRow <= rowInd && rowInd <= _lastRow && _firstCol <= colInd)
		{
			return colInd <= _lastCol;
		}
		return false;
	}

	public bool ContainsRow(int rowInd)
	{
		if (_firstRow <= rowInd)
		{
			return rowInd <= _lastRow;
		}
		return false;
	}

	public bool ContainsColumn(int colInd)
	{
		if (_firstCol <= colInd)
		{
			return colInd <= _lastCol;
		}
		return false;
	}

	public bool Intersects(CellRangeAddressBase other)
	{
		if (_firstRow <= other._lastRow && _firstCol <= other._lastCol && other._firstRow <= _lastRow)
		{
			return other._firstCol <= _lastCol;
		}
		return false;
	}

	public override string ToString()
	{
		CellReference cellReference = new CellReference(_firstRow, _firstCol);
		CellReference cellReference2 = new CellReference(_lastRow, _lastCol);
		return GetType().Name + " [" + cellReference.FormatAsString() + ":" + cellReference2.FormatAsString() + "]";
	}

	public override bool Equals(object other)
	{
		if (other is CellRangeAddressBase)
		{
			CellRangeAddressBase cellRangeAddressBase = (CellRangeAddressBase)other;
			if (MinRow == cellRangeAddressBase.MinRow && MaxRow == cellRangeAddressBase.MaxRow && MinColumn == cellRangeAddressBase.MinColumn)
			{
				return MaxColumn == cellRangeAddressBase.MaxColumn;
			}
			return false;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return MinColumn + (MaxColumn << 8) + (MinRow << 16) + (MaxRow << 24);
	}
}
