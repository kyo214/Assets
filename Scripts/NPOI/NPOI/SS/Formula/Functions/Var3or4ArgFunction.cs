using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public abstract class Var3or4ArgFunction : Function3Arg, Function, Function4Arg
{
	public ValueEval Evaluate(ValueEval[] args, int srcRowIndex, int srcColumnIndex)
	{
		return args.Length switch
		{
			3 => Evaluate(srcRowIndex, srcColumnIndex, args[0], args[1], args[2]), 
			4 => Evaluate(srcRowIndex, srcColumnIndex, args[0], args[1], args[2], args[3]), 
			_ => ErrorEval.VALUE_INVALID, 
		};
	}

	public abstract ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1, ValueEval arg2);

	public abstract ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1, ValueEval arg2, ValueEval arg3);
}
