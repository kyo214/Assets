using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public abstract class Fixed1ArgFunction : Function1Arg, Function
{
	public ValueEval Evaluate(ValueEval[] args, int srcRowIndex, int srcColumnIndex)
	{
		if (args.Length != 1)
		{
			return ErrorEval.VALUE_INVALID;
		}
		return Evaluate(srcRowIndex, srcColumnIndex, args[0]);
	}

	public abstract ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0);
}
