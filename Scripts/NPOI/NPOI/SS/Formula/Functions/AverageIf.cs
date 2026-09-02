using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class AverageIf : FreeRefFunction
{
	public static FreeRefFunction instance = new AverageIf();

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length > 3 || args.Length < 2)
		{
			return ErrorEval.VALUE_INVALID;
		}
		return AverageIfs.instance.Evaluate(new ValueEval[3]
		{
			GetSumRange(args),
			args[0],
			args[1]
		}, ec);
	}

	private ValueEval GetSumRange(ValueEval[] args)
	{
		try
		{
			return args[2];
		}
		catch
		{
			return args[0];
		}
	}
}
