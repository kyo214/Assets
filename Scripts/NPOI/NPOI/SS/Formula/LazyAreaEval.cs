using System;
using System.Text;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.PTG;
using NPOI.SS.Util;

namespace NPOI.SS.Formula;

public class LazyAreaEval : AreaEvalBase
{
	private SheetRangeEvaluator _evaluator;

	public LazyAreaEval(AreaI ptg, SheetRangeEvaluator evaluator)
		: base(ptg, evaluator)
	{
		_evaluator = evaluator;
	}

	public LazyAreaEval(int firstRowIndex, int firstColumnIndex, int lastRowIndex, int lastColumnIndex, SheetRangeEvaluator evaluator)
		: base(evaluator, firstRowIndex, firstColumnIndex, lastRowIndex, lastColumnIndex)
	{
		_evaluator = evaluator;
	}

	public override ValueEval GetRelativeValue(int relativeRowIndex, int relativeColumnIndex)
	{
		return GetRelativeValue(base.FirstSheetIndex, relativeRowIndex, relativeColumnIndex);
	}

	public override ValueEval GetRelativeValue(int sheetIndex, int relativeRowIndex, int relativeColumnIndex)
	{
		int rowIndex = relativeRowIndex + base.FirstRow;
		int columnIndex = relativeColumnIndex + base.FirstColumn;
		return _evaluator.GetEvalForCell(sheetIndex, rowIndex, columnIndex);
	}

	public override TwoDEval GetRow(int rowIndex)
	{
		if (rowIndex >= base.Height)
		{
			throw new ArgumentException("Invalid rowIndex " + rowIndex + ".  Allowable range is (0.." + base.Height + ").");
		}
		int num = base.FirstRow + rowIndex;
		return new LazyAreaEval(num, base.FirstColumn, num, base.LastColumn, _evaluator);
	}

	public override TwoDEval GetColumn(int columnIndex)
	{
		if (columnIndex >= base.Width)
		{
			throw new ArgumentException("Invalid columnIndex " + columnIndex + ".  Allowable range is (0.." + base.Width + ").");
		}
		int num = base.FirstColumn + columnIndex;
		return new LazyAreaEval(base.FirstRow, num, base.LastRow, num, _evaluator);
	}

	public override AreaEval Offset(int relFirstRowIx, int relLastRowIx, int relFirstColIx, int relLastColIx)
	{
		return new LazyAreaEval(new OffsetArea(base.FirstRow, base.FirstColumn, relFirstRowIx, relLastRowIx, relFirstColIx, relLastColIx), _evaluator);
	}

	public override string ToString()
	{
		CellReference cellReference = new CellReference(base.FirstRow, base.FirstColumn);
		CellReference cellReference2 = new CellReference(base.LastRow, base.LastColumn);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GetType().Name).Append("[");
		stringBuilder.Append(_evaluator.SheetNameRange);
		stringBuilder.Append('!');
		stringBuilder.Append(cellReference.FormatAsString());
		stringBuilder.Append(':');
		stringBuilder.Append(cellReference2.FormatAsString());
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}

	public override bool IsSubTotal(int rowIndex, int columnIndex)
	{
		return _evaluator.GetSheetEvaluator(_evaluator.FirstSheetIndex).IsSubTotal(base.FirstRow + rowIndex, base.FirstColumn + columnIndex);
	}
}
