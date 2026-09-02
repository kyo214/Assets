using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Hyperlink : Var1or2ArgFunction
{
	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0)
	{
		return arg0;
	}

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1)
	{
		return arg1;
	}
}
