using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Iserror : LogicalFunction
{
	protected override bool Evaluate(ValueEval arg)
	{
		return arg is ErrorEval;
	}
}
