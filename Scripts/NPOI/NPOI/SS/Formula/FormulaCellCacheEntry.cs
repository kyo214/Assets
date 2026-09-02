using System.Collections;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula;

public class FormulaCellCacheEntry : CellCacheEntry
{
	public new static FormulaCellCacheEntry[] EMPTY_ARRAY = new FormulaCellCacheEntry[0];

	private CellCacheEntry[] _sensitiveInputCells;

	private FormulaUsedBlankCellSet _usedBlankCellGroup;

	public bool IsInputSensitive
	{
		get
		{
			if (_sensitiveInputCells != null && _sensitiveInputCells.Length != 0)
			{
				return true;
			}
			if (_usedBlankCellGroup != null)
			{
				return !_usedBlankCellGroup.IsEmpty;
			}
			return false;
		}
	}

	public void SetSensitiveInputCells(CellCacheEntry[] sensitiveInputCells)
	{
		if (sensitiveInputCells == null)
		{
			_sensitiveInputCells = null;
			ChangeConsumingCells(CellCacheEntry.EMPTY_ARRAY);
		}
		else
		{
			_sensitiveInputCells = (CellCacheEntry[])sensitiveInputCells.Clone();
			ChangeConsumingCells(_sensitiveInputCells);
		}
	}

	public void ClearFormulaEntry()
	{
		CellCacheEntry[] sensitiveInputCells = _sensitiveInputCells;
		if (sensitiveInputCells != null)
		{
			for (int num = sensitiveInputCells.Length - 1; num >= 0; num--)
			{
				sensitiveInputCells[num].ClearConsumingCell(this);
			}
		}
		_sensitiveInputCells = null;
		ClearValue();
	}

	private void ChangeConsumingCells(CellCacheEntry[] usedCells)
	{
		CellCacheEntry[] sensitiveInputCells = _sensitiveInputCells;
		int num = usedCells.Length;
		for (int i = 0; i < num; i++)
		{
			usedCells[i].AddConsumingCell(this);
		}
		if (sensitiveInputCells == null)
		{
			return;
		}
		int num2 = sensitiveInputCells.Length;
		if (num2 < 1)
		{
			return;
		}
		ArrayList arrayList;
		if (num < 1)
		{
			arrayList = new ArrayList();
		}
		else
		{
			arrayList = new ArrayList(num * 3 / 2);
			for (int j = 0; j < num; j++)
			{
				arrayList.Add(usedCells[j]);
			}
		}
		for (int k = 0; k < num2; k++)
		{
			CellCacheEntry cellCacheEntry = sensitiveInputCells[k];
			if (!arrayList.Contains(cellCacheEntry))
			{
				cellCacheEntry.ClearConsumingCell(this);
			}
		}
	}

	public void UpdateFormulaResult(ValueEval result, CellCacheEntry[] sensitiveInputCells, FormulaUsedBlankCellSet usedBlankAreas)
	{
		UpdateValue(result);
		SetSensitiveInputCells(sensitiveInputCells);
		_usedBlankCellGroup = usedBlankAreas;
	}

	public void NotifyUpdatedBlankCell(BookSheetKey bsk, int rowIndex, int columnIndex, IEvaluationListener evaluationListener)
	{
		if (_usedBlankCellGroup != null && _usedBlankCellGroup.ContainsCell(bsk, rowIndex, columnIndex))
		{
			ClearFormulaEntry();
			RecurseClearCachedFormulaResults(evaluationListener);
		}
	}
}
