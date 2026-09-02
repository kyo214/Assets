using System;
using System.Globalization;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Fixed : Function1Arg, Function, Function2Arg, Function3Arg
{
	public ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1, ValueEval arg2)
	{
		return doFixed(arg0, arg1, arg2, srcRowIndex, srcColumnIndex);
	}

	public ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1)
	{
		return doFixed(arg0, arg1, BoolEval.FALSE, srcRowIndex, srcColumnIndex);
	}

	public ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0)
	{
		return doFixed(arg0, new NumberEval(2.0), BoolEval.FALSE, srcRowIndex, srcColumnIndex);
	}

	public ValueEval Evaluate(ValueEval[] args, int srcRowIndex, int srcColumnIndex)
	{
		return args.Length switch
		{
			1 => doFixed(args[0], new NumberEval(2.0), BoolEval.FALSE, srcRowIndex, srcColumnIndex), 
			2 => doFixed(args[0], args[1], BoolEval.FALSE, srcRowIndex, srcColumnIndex), 
			3 => doFixed(args[0], args[1], args[2], srcRowIndex, srcColumnIndex), 
			_ => ErrorEval.VALUE_INVALID, 
		};
	}

	private ValueEval doFixed(ValueEval numberParam, ValueEval placesParam, ValueEval skipThousandsSeparatorParam, int srcRowIndex, int srcColumnIndex)
	{
		try
		{
			decimal d = (decimal)OperandResolver.CoerceValueToDouble(OperandResolver.GetSingleValue(numberParam, srcRowIndex, srcColumnIndex));
			int num = OperandResolver.CoerceValueToInt(OperandResolver.GetSingleValue(placesParam, srcRowIndex, srcColumnIndex));
			bool? flag = OperandResolver.CoerceValueToBoolean(OperandResolver.GetSingleValue(skipThousandsSeparatorParam, srcRowIndex, srcColumnIndex), stringsAreBlanks: false);
			if (num < 0)
			{
				d /= (decimal)Math.Pow(10.0, -num);
				d = Math.Round(d, 0);
				d *= (decimal)Math.Pow(10.0, -num);
			}
			else
			{
				d = Math.Round(d, num);
			}
			return new StringEval((flag.HasValue && flag.Value) ? d.ToString((num > 0) ? ("F" + num) : "F0") : d.ToString((num > 0) ? ("N" + num) : "N0", CultureInfo.InvariantCulture));
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}
}
