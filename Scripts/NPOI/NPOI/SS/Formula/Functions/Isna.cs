using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Isna : LogicalFunction
{
	protected override bool Evaluate(ValueEval arg)
	{
		return arg == ErrorEval.NA;
	}
}
