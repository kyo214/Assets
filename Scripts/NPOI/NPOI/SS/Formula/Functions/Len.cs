using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Len : SingleArgTextFunc
{
	public override ValueEval Evaluate(string arg)
	{
		return new NumberEval(arg.Length);
	}
}
