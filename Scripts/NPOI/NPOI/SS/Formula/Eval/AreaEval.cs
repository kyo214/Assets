namespace NPOI.SS.Formula.Eval;

public interface AreaEval : TwoDEval, ValueEval, ThreeDEval, ISheetRange
{
	int FirstRow { get; }

	int LastRow { get; }

	int FirstColumn { get; }

	int LastColumn { get; }

	bool Contains(int row, int col);

	bool ContainsColumn(int col);

	bool ContainsRow(int row);

	ValueEval GetAbsoluteValue(int row, int col);

	ValueEval GetRelativeValue(int relativeRowIndex, int relativeColumnIndex);

	AreaEval Offset(int relFirstRowIx, int relLastRowIx, int relFirstColIx, int relLastColIx);
}
