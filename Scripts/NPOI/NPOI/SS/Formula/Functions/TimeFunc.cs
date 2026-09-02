using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class TimeFunc : Fixed3ArgFunction
{
	private const int SECONDS_PER_MINUTE = 60;

	private const int SECONDS_PER_HOUR = 3600;

	private const int HOURS_PER_DAY = 24;

	private const int SECONDS_PER_DAY = 86400;

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1, ValueEval arg2)
	{
		double value;
		try
		{
			value = Evaluate(EvalArg(arg0, srcRowIndex, srcColumnIndex), EvalArg(arg1, srcRowIndex, srcColumnIndex), EvalArg(arg2, srcRowIndex, srcColumnIndex));
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		return new NumberEval(value);
	}

	private int EvalArg(ValueEval arg, int srcRowIndex, int srcColumnIndex)
	{
		if (arg == MissingArgEval.instance)
		{
			return 0;
		}
		return OperandResolver.CoerceValueToInt(OperandResolver.GetSingleValue(arg, srcRowIndex, srcColumnIndex));
	}

	private double Evaluate(int hours, int minutes, int seconds)
	{
		if (hours > 32767 || minutes > 32767 || seconds > 32767)
		{
			throw new EvaluationException(ErrorEval.VALUE_INVALID);
		}
		int num = hours * 3600 + minutes * 60 + seconds;
		if (num < 0)
		{
			throw new EvaluationException(ErrorEval.VALUE_INVALID);
		}
		return (double)(num % 86400) / 86400.0;
	}
}
