using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Iserr : LogicalFunction
{
	protected override bool Evaluate(ValueEval arg)
	{
		if (arg is ErrorEval)
		{
			return arg != ErrorEval.NA;
		}
		return false;
	}
}
