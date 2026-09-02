using System;
using NPOI.SS.Formula.PTG;

namespace NPOI.SS.Formula.Eval;

public abstract class AreaEvalBase : AreaEval, TwoDEval, ValueEval, ThreeDEval, ISheetRange
{
	private int _firstSheet;

	private int _firstColumn;

	private int _firstRow;

	private int _lastSheet;

	private int _lastColumn;

	private int _lastRow;

	private int _nColumns;

	private int _nRows;

	public int FirstColumn => _firstColumn;

	public int FirstRow => _firstRow;

	public int LastColumn => _lastColumn;

	public int LastRow => _lastRow;

	public int FirstSheetIndex => _firstSheet;

	public int LastSheetIndex => _lastSheet;

	public bool IsColumn => _firstColumn == _lastColumn;

	public bool IsRow => _firstRow == _lastRow;

	public int Width => _lastColumn - _firstColumn + 1;

	public int Height => _lastRow - _firstRow + 1;

	protected AreaEvalBase(ISheetRange sheets, int firstRow, int firstColumn, int lastRow, int lastColumn)
	{
		_firstColumn = firstColumn;
		_firstRow = firstRow;
		_lastColumn = lastColumn;
		_lastRow = lastRow;
		_nColumns = _lastColumn - _firstColumn + 1;
		_nRows = _lastRow - _firstRow + 1;
		if (sheets != null)
		{
			_firstSheet = sheets.FirstSheetIndex;
			_lastSheet = sheets.LastSheetIndex;
		}
		else
		{
			_firstSheet = -1;
			_lastSheet = -1;
		}
	}

	protected AreaEvalBase(int firstRow, int firstColumn, int lastRow, int lastColumn)
		: this(null, firstRow, firstColumn, lastRow, lastColumn)
	{
	}

	protected AreaEvalBase(AreaI ptg)
		: this(ptg, null)
	{
	}

	protected AreaEvalBase(AreaI ptg, ISheetRange sheets)
		: this(sheets, ptg.FirstRow, ptg.FirstColumn, ptg.LastRow, ptg.LastColumn)
	{
	}

	public ValueEval GetValue(int row, int col)
	{
		return GetRelativeValue(row, col);
	}

	public ValueEval GetValue(int sheetIndex, int row, int col)
	{
		return GetRelativeValue(sheetIndex, row, col);
	}

	public bool Contains(int row, int col)
	{
		if (_firstRow <= row && _lastRow >= row && _firstColumn <= col)
		{
			return _lastColumn >= col;
		}
		return false;
	}

	public bool ContainsRow(int row)
	{
		if (_firstRow <= row)
		{
			return _lastRow >= row;
		}
		return false;
	}

	public bool ContainsColumn(int col)
	{
		if (_firstColumn <= col)
		{
			return _lastColumn >= col;
		}
		return false;
	}

	public ValueEval GetAbsoluteValue(int row, int col)
	{
		int num = row - _firstRow;
		int num2 = col - _firstColumn;
		if (num < 0 || num >= _nRows)
		{
			throw new ArgumentException("Specified row index (" + row + ") is outside the allowed range (" + _firstRow + ".." + _lastRow + ")");
		}
		if (num2 < 0 || num2 >= _nColumns)
		{
			throw new ArgumentException("Specified column index (" + col + ") is outside the allowed range (" + _firstColumn + ".." + col + ")");
		}
		return GetRelativeValue(num, num2);
	}

	public abstract ValueEval GetRelativeValue(int relativeRowIndex, int relativeColumnIndex);

	public abstract ValueEval GetRelativeValue(int sheetIndex, int relativeRowIndex, int relativeColumnIndex);

	public virtual bool IsSubTotal(int rowIndex, int columnIndex)
	{
		return false;
	}

	public abstract TwoDEval GetRow(int rowIndex);

	public abstract TwoDEval GetColumn(int columnIndex);

	public abstract AreaEval Offset(int relFirstRowIx, int relLastRowIx, int relFirstColIx, int relLastColIx);
}
