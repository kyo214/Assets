using System.Collections;
using System.Text;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula;

internal class CellEvaluationFrame
{
	private FormulaCellCacheEntry _cce;

	private ArrayList _sensitiveInputCells;

	private FormulaUsedBlankCellSet _usedBlankCellGroup;

	public CellEvaluationFrame(FormulaCellCacheEntry cce)
	{
		_cce = cce;
		_sensitiveInputCells = new ArrayList();
	}

	public CellCacheEntry GetCCE()
	{
		return _cce;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		stringBuilder.Append(GetType().Name).Append(" [");
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public void AddSensitiveInputCell(CellCacheEntry inputCell)
	{
		_sensitiveInputCells.Add(inputCell);
	}

	private CellCacheEntry[] GetSensitiveInputCells()
	{
		int count = _sensitiveInputCells.Count;
		if (count < 1)
		{
			return CellCacheEntry.EMPTY_ARRAY;
		}
		_ = new CellCacheEntry[count];
		return (CellCacheEntry[])_sensitiveInputCells.ToArray(typeof(CellCacheEntry));
	}

	public void AddUsedBlankCell(int bookIndex, int sheetIndex, int rowIndex, int columnIndex)
	{
		if (_usedBlankCellGroup == null)
		{
			_usedBlankCellGroup = new FormulaUsedBlankCellSet();
		}
		_usedBlankCellGroup.AddCell(bookIndex, sheetIndex, rowIndex, columnIndex);
	}

	public void UpdateFormulaResult(ValueEval result)
	{
		_cce.UpdateFormulaResult(result, GetSensitiveInputCells(), _usedBlankCellGroup);
	}
}
