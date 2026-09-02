using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Lower : SingleArgTextFunc
{
	public override ValueEval Evaluate(string arg)
	{
		return new StringEval(arg.ToLower());
	}
}
