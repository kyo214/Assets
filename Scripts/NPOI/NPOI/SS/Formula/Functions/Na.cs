using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Na : Fixed0ArgFunction
{
	public override ValueEval Evaluate(int srcCellRow, int srcCellCol)
	{
		return ErrorEval.NA;
	}
}
