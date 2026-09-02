using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula;

public interface ThreeDEval : TwoDEval, ValueEval, ISheetRange
{
	ValueEval GetValue(int sheetIndex, int rowIndex, int columnIndex);
}
