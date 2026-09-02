using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public abstract class Var2or3ArgFunction : Function2Arg, Function, Function3Arg
{
	public ValueEval Evaluate(ValueEval[] args, int srcRowIndex, int srcColumnIndex)
	{
		return args.Length switch
		{
			2 => Evaluate(srcRowIndex, srcColumnIndex, args[0], args[1]), 
			3 => Evaluate(srcRowIndex, srcColumnIndex, args[0], args[1], args[2]), 
			_ => ErrorEval.VALUE_INVALID, 
		};
	}

	public abstract ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1);

	public abstract ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1, ValueEval arg2);
}
