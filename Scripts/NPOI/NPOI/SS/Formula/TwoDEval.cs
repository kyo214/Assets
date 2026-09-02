using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula;

public interface TwoDEval : ValueEval
{
	int Width { get; }

	int Height { get; }

	bool IsRow { get; }

	bool IsColumn { get; }

	ValueEval GetValue(int rowIndex, int columnIndex);

	TwoDEval GetRow(int rowIndex);

	TwoDEval GetColumn(int columnIndex);

	bool IsSubTotal(int rowIndex, int columnIndex);
}
