using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public abstract class Fixed2ArgFunction : Function2Arg, Function
{
	public ValueEval Evaluate(ValueEval[] args, int srcRowIndex, int srcColumnIndex)
	{
		if (args.Length != 2)
		{
			return ErrorEval.VALUE_INVALID;
		}
		return Evaluate(srcRowIndex, srcColumnIndex, args[0], args[1]);
	}

	public abstract ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1);
}
