using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public abstract class Var1or2ArgFunction : Function1Arg, Function, Function2Arg
{
	public ValueEval Evaluate(ValueEval[] args, int srcRowIndex, int srcColumnIndex)
	{
		return args.Length switch
		{
			1 => Evaluate(srcRowIndex, srcColumnIndex, args[0]), 
			2 => Evaluate(srcRowIndex, srcColumnIndex, args[0], args[1]), 
			_ => ErrorEval.VALUE_INVALID, 
		};
	}

	public abstract ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0);

	public abstract ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1);
}
