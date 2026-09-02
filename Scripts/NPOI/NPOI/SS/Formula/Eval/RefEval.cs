namespace NPOI.SS.Formula.Eval;

public interface RefEval : ValueEval, ISheetRange
{
	int Column { get; }

	int Row { get; }

	int NumberOfSheets { get; }

	ValueEval GetInnerValueEval(int sheetIndex);

	AreaEval Offset(int relFirstRowIx, int relLastRowIx, int relFirstColIx, int relLastColIx);
}
