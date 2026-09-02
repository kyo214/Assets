using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public abstract class Fixed0ArgFunction : Function0Arg, Function
{
	public ValueEval Evaluate(ValueEval[] args, int srcRowIndex, int srcColumnIndex)
	{
		if (args.Length != 0)
		{
			return ErrorEval.VALUE_INVALID;
		}
		return Evaluate(srcRowIndex, srcColumnIndex);
	}

	public abstract ValueEval Evaluate(int srcRowIndex, int srcColumnIndex);
}
