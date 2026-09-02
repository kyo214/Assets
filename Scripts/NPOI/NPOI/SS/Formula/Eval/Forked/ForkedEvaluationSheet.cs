using System;
using System.Collections.Generic;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.SS.Formula.Eval.Forked;

internal class ForkedEvaluationSheet : IEvaluationSheet
{
	private class RowColKey : IComparable<RowColKey>
	{
		private int _rowIndex;

		private int _columnIndex;

		public int RowIndex => _rowIndex;

		public int ColumnIndex => _columnIndex;

		public RowColKey(int rowIndex, int columnIndex)
		{
			_rowIndex = rowIndex;
			_columnIndex = columnIndex;
		}

		public override bool Equals(object obj)
		{
			RowColKey rowColKey = (RowColKey)obj;
			if (_rowIndex == rowColKey._rowIndex)
			{
				return _columnIndex == rowColKey._columnIndex;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _rowIndex ^ _columnIndex;
		}

		public int CompareTo(RowColKey o)
		{
			int num = _rowIndex - o._rowIndex;
			if (num != 0)
			{
				return num;
			}
			return _columnIndex - o._columnIndex;
		}
	}

	private IEvaluationSheet _masterSheet;

	private Dictionary<RowColKey, ForkedEvaluationCell> _sharedCellsByRowCol;

	public ForkedEvaluationSheet(IEvaluationSheet masterSheet)
	{
		_masterSheet = masterSheet;
		_sharedCellsByRowCol = new Dictionary<RowColKey, ForkedEvaluationCell>();
	}

	public IEvaluationCell GetCell(int rowIndex, int columnIndex)
	{
		RowColKey key = new RowColKey(rowIndex, columnIndex);
		ForkedEvaluationCell forkedEvaluationCell = null;
		if (_sharedCellsByRowCol.ContainsKey(key))
		{
			forkedEvaluationCell = _sharedCellsByRowCol[key];
		}
		if (forkedEvaluationCell == null)
		{
			return _masterSheet.GetCell(rowIndex, columnIndex);
		}
		return forkedEvaluationCell;
	}

	public ForkedEvaluationCell GetOrCreateUpdatableCell(int rowIndex, int columnIndex)
	{
		RowColKey key = new RowColKey(rowIndex, columnIndex);
		ForkedEvaluationCell forkedEvaluationCell = null;
		if (_sharedCellsByRowCol.ContainsKey(key))
		{
			forkedEvaluationCell = _sharedCellsByRowCol[key];
		}
		if (forkedEvaluationCell == null)
		{
			IEvaluationCell cell = _masterSheet.GetCell(rowIndex, columnIndex);
			if (cell == null)
			{
				CellReference cellReference = new CellReference(rowIndex, columnIndex);
				throw new InvalidOperationException("Underlying cell '" + cellReference.FormatAsString() + "' is missing in master sheet.");
			}
			forkedEvaluationCell = new ForkedEvaluationCell(this, cell);
			if (_sharedCellsByRowCol.ContainsKey(key))
			{
				_sharedCellsByRowCol[key] = forkedEvaluationCell;
			}
			else
			{
				_sharedCellsByRowCol.Add(key, forkedEvaluationCell);
			}
		}
		return forkedEvaluationCell;
	}

	public void CopyUpdatedCells(ISheet sheet)
	{
		RowColKey[] array = new RowColKey[_sharedCellsByRowCol.Count];
		_sharedCellsByRowCol.Keys.CopyTo(array, 0);
		Array.Sort(array);
		foreach (RowColKey rowColKey in array)
		{
			IRow row = sheet.GetRow(rowColKey.RowIndex);
			if (row == null)
			{
				row = sheet.CreateRow(rowColKey.RowIndex);
			}
			ICell cell = row.GetCell(rowColKey.ColumnIndex);
			if (cell == null)
			{
				cell = row.CreateCell(rowColKey.ColumnIndex);
			}
			_sharedCellsByRowCol[rowColKey].CopyValue(cell);
		}
	}

	public int GetSheetIndex(IEvaluationWorkbook mewb)
	{
		return mewb.GetSheetIndex(_masterSheet);
	}

	public void ClearAllCachedResultValues()
	{
		_masterSheet.ClearAllCachedResultValues();
	}
}
