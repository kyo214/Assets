using System.Collections.Generic;
using NPOI.SS.Formula;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel;

public class XSSFEvaluationSheet : IEvaluationSheet
{
	private class CellKey
	{
		private int _row;

		private int _col;

		private int _hash = -1;

		protected internal CellKey(int row, int col)
		{
			_row = row;
			_col = col;
		}

		public override int GetHashCode()
		{
			if (_hash == -1)
			{
				_hash = (629 + _row) * 37 + _col;
			}
			return _hash;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is CellKey))
			{
				return false;
			}
			CellKey cellKey = (CellKey)obj;
			if (_row == cellKey._row)
			{
				return _col == cellKey._col;
			}
			return false;
		}
	}

	private XSSFSheet _xs;

	private Dictionary<CellKey, IEvaluationCell> _cellCache;

	public XSSFEvaluationSheet(ISheet sheet)
	{
		_xs = (XSSFSheet)sheet;
	}

	public XSSFEvaluationSheet()
	{
	}

	public void ClearAllCachedResultValues()
	{
		_cellCache = null;
	}

	public XSSFSheet GetXSSFSheet()
	{
		return _xs;
	}

	public IEvaluationCell GetCell(int rowIndex, int columnIndex)
	{
		if (_cellCache == null)
		{
			_cellCache = new Dictionary<CellKey, IEvaluationCell>(_xs.LastRowNum * 3);
			foreach (IRow x in _xs)
			{
				int rowNum = x.RowNum;
				foreach (ICell item in x)
				{
					CellKey key = new CellKey(rowNum, item.ColumnIndex);
					IEvaluationCell value = new XSSFEvaluationCell((XSSFCell)item, this);
					_cellCache.Add(key, value);
				}
			}
		}
		CellKey key2 = new CellKey(rowIndex, columnIndex);
		IEvaluationCell evaluationCell = null;
		if (_cellCache.ContainsKey(key2))
		{
			evaluationCell = _cellCache[key2];
		}
		if (evaluationCell == null)
		{
			if (!(_xs.GetRow(rowIndex) is XSSFRow xSSFRow))
			{
				return null;
			}
			if (!(xSSFRow.GetCell(columnIndex) is XSSFCell cell))
			{
				return null;
			}
			evaluationCell = new XSSFEvaluationCell(cell, this);
			_cellCache[key2] = evaluationCell;
		}
		return evaluationCell;
	}
}
