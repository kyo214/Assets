using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Row : Function0Arg, Function, Function1Arg
{
	public ValueEval Evaluate(int srcRowIndex, int srcColumnIndex)
	{
		return new NumberEval(srcRowIndex + 1);
	}

	public ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0)
	{
		int num;
		if (arg0 is AreaEval)
		{
			num = ((AreaEval)arg0).FirstRow;
		}
		else
		{
			if (!(arg0 is RefEval))
			{
				return ErrorEval.VALUE_INVALID;
			}
			num = ((RefEval)arg0).Row;
		}
		return new NumberEval(num + 1);
	}

	public ValueEval Evaluate(ValueEval[] args, int srcRowIndex, int srcColumnIndex)
	{
		return args.Length switch
		{
			1 => Evaluate(srcRowIndex, srcColumnIndex, args[0]), 
			0 => new NumberEval(srcRowIndex + 1), 
			_ => ErrorEval.VALUE_INVALID, 
		};
	}
}
