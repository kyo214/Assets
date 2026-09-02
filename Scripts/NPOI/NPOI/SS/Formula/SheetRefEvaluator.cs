using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula;

public class SheetRefEvaluator
{
	private WorkbookEvaluator _bookEvaluator;

	private EvaluationTracker _tracker;

	private IEvaluationSheet _sheet;

	private int _sheetIndex;

	public string SheetName => _bookEvaluator.GetSheetName(_sheetIndex);

	private IEvaluationSheet Sheet
	{
		get
		{
			if (_sheet == null)
			{
				_sheet = _bookEvaluator.GetSheet(_sheetIndex);
			}
			return _sheet;
		}
	}

	public SheetRefEvaluator(WorkbookEvaluator bookEvaluator, EvaluationTracker tracker, int sheetIndex)
	{
		if (sheetIndex < 0)
		{
			throw new ArgumentException("Invalid sheetIndex: " + sheetIndex + ".");
		}
		_bookEvaluator = bookEvaluator;
		_tracker = tracker;
		_sheetIndex = sheetIndex;
	}

	public ValueEval GetEvalForCell(int rowIndex, int columnIndex)
	{
		return _bookEvaluator.EvaluateReference(Sheet, _sheetIndex, rowIndex, columnIndex, _tracker);
	}

	public bool IsSubTotal(int rowIndex, int columnIndex)
	{
		bool result = false;
		IEvaluationCell cell = Sheet.GetCell(rowIndex, columnIndex);
		if (cell != null && cell.CellType == CellType.Formula)
		{
			Ptg[] formulaTokens = _bookEvaluator.Workbook.GetFormulaTokens(cell);
			foreach (Ptg ptg in formulaTokens)
			{
				if (ptg is FuncVarPtg)
				{
					FuncVarPtg funcVarPtg = (FuncVarPtg)ptg;
					if ("SUBTOTAL".Equals(funcVarPtg.Name))
					{
						result = true;
						break;
					}
				}
			}
		}
		return result;
	}
}
