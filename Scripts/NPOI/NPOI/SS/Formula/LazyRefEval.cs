using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.PTG;
using NPOI.SS.Util;

namespace NPOI.SS.Formula;

public class LazyRefEval : RefEvalBase
{
	private SheetRangeEvaluator _evaluator;

	public bool IsSubTotal => _evaluator.GetSheetEvaluator(base.FirstSheetIndex).IsSubTotal(base.Row, base.Column);

	public LazyRefEval(int rowIndex, int columnIndex, SheetRangeEvaluator sre)
		: base(sre, rowIndex, columnIndex)
	{
		if (sre == null)
		{
			throw new ArgumentException("sre must not be null");
		}
		_evaluator = sre;
	}

	public override ValueEval GetInnerValueEval(int sheetIndex)
	{
		return _evaluator.GetEvalForCell(sheetIndex, base.Row, base.Column);
	}

	public override AreaEval Offset(int relFirstRowIx, int relLastRowIx, int relFirstColIx, int relLastColIx)
	{
		return new LazyAreaEval(new OffsetArea(base.Row, base.Column, relFirstRowIx, relLastRowIx, relFirstColIx, relLastColIx), _evaluator);
	}

	public override string ToString()
	{
		CellReference cellReference = new CellReference(base.Row, base.Column);
		return GetType().Name + "[" + _evaluator.SheetNameRange + "!" + cellReference.FormatAsString() + "]";
	}
}
