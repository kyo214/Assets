using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class True : Fixed0ArgFunction
{
	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex)
	{
		return BoolEval.TRUE;
	}
}
