using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;

namespace NPOI.SS.Formula.Atp;

internal class Ifs : FreeRefFunction
{
	public static FreeRefFunction Instance = new Ifs();

	private Ifs()
	{
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length % 2 != 0)
		{
			return ErrorEval.VALUE_INVALID;
		}
		for (int i = 0; i < args.Length; i += 2)
		{
			if (((BoolEval)args[i]).BooleanValue)
			{
				return args[i + 1];
			}
		}
		return ErrorEval.NA;
	}
}
