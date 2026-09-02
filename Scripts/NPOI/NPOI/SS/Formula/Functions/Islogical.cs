using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Islogical : LogicalFunction
{
	protected override bool Evaluate(ValueEval arg)
	{
		return arg is BoolEval;
	}
}
