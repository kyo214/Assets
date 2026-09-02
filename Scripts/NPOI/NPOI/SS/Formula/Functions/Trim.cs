using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Trim : SingleArgTextFunc
{
	public override ValueEval Evaluate(string arg)
	{
		return new StringEval(arg.Trim());
	}
}
