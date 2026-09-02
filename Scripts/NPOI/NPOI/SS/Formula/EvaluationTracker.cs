using System;
using System.Collections;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula;

public class EvaluationTracker
{
	private IList _evaluationFrames;

	private IList _currentlyEvaluatingCells;

	private EvaluationCache _cache;

	public EvaluationTracker(EvaluationCache cache)
	{
		_cache = cache;
		_evaluationFrames = new ArrayList();
		_currentlyEvaluatingCells = new ArrayList();
	}

	public bool StartEvaluate(FormulaCellCacheEntry cce)
	{
		if (cce == null)
		{
			throw new ArgumentException("cellLoc must not be null");
		}
		if (_currentlyEvaluatingCells.Contains(cce))
		{
			return false;
		}
		_currentlyEvaluatingCells.Add(cce);
		_evaluationFrames.Add(new CellEvaluationFrame(cce));
		return true;
	}

	public void UpdateCacheResult(ValueEval result)
	{
		int count = _evaluationFrames.Count;
		if (count < 1)
		{
			throw new InvalidOperationException("Call To endEvaluate without matching call To startEvaluate");
		}
		((CellEvaluationFrame)_evaluationFrames[count - 1]).UpdateFormulaResult(result);
	}

	public void EndEvaluate(CellCacheEntry cce)
	{
		int count = _evaluationFrames.Count;
		if (count < 1)
		{
			throw new InvalidOperationException("Call To endEvaluate without matching call To startEvaluate");
		}
		count--;
		CellEvaluationFrame cellEvaluationFrame = (CellEvaluationFrame)_evaluationFrames[count];
		if (cce != cellEvaluationFrame.GetCCE())
		{
			throw new InvalidOperationException("Wrong cell specified. ");
		}
		_evaluationFrames.RemoveAt(count);
		_currentlyEvaluatingCells.Remove(cce);
	}

	public void AcceptFormulaDependency(CellCacheEntry cce)
	{
		int num = _evaluationFrames.Count - 1;
		if (num >= 0)
		{
			((CellEvaluationFrame)_evaluationFrames[num]).AddSensitiveInputCell(cce);
		}
	}

	public void AcceptPlainValueDependency(int bookIndex, int sheetIndex, int rowIndex, int columnIndex, ValueEval value)
	{
		int num = _evaluationFrames.Count - 1;
		if (num >= 0)
		{
			CellEvaluationFrame cellEvaluationFrame = (CellEvaluationFrame)_evaluationFrames[num];
			if (value == BlankEval.instance)
			{
				cellEvaluationFrame.AddUsedBlankCell(bookIndex, sheetIndex, rowIndex, columnIndex);
				return;
			}
			PlainValueCellCacheEntry plainValueEntry = _cache.GetPlainValueEntry(bookIndex, sheetIndex, rowIndex, columnIndex, value);
			cellEvaluationFrame.AddSensitiveInputCell(plainValueEntry);
		}
	}
}
