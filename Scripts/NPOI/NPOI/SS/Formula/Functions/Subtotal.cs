using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.Formula.Eval;
using NPOI.Util;

namespace NPOI.SS.Formula.Functions;

public class Subtotal : Function
{
	private static Function FindFunction(int functionCode)
	{
		switch (functionCode)
		{
		case 1:
			return AggregateFunction.SubtotalInstance(AggregateFunction.AVERAGE);
		case 2:
			return Count.SubtotalInstance();
		case 3:
			return Counta.SubtotalInstance();
		case 4:
			return AggregateFunction.SubtotalInstance(AggregateFunction.MAX);
		case 5:
			return AggregateFunction.SubtotalInstance(AggregateFunction.MIN);
		case 6:
			return AggregateFunction.SubtotalInstance(AggregateFunction.PRODUCT);
		case 7:
			return AggregateFunction.SubtotalInstance(AggregateFunction.STDEV);
		case 8:
			throw new NotImplementedFunctionException("STDEVP");
		case 9:
			return AggregateFunction.SubtotalInstance(AggregateFunction.SUM);
		case 10:
			throw new NotImplementedFunctionException("VAR");
		case 11:
			throw new NotImplementedFunctionException("VARP");
		case 101:
		case 102:
		case 103:
		case 104:
		case 105:
		case 106:
		case 107:
		case 108:
		case 109:
		case 110:
		case 111:
			throw new NotImplementedException("SUBTOTAL - with 'exclude hidden values' option");
		default:
			throw EvaluationException.InvalidValue();
		}
	}

	public ValueEval Evaluate(ValueEval[] args, int srcRowIndex, int srcColumnIndex)
	{
		if (args.Length - 1 < 1)
		{
			return ErrorEval.VALUE_INVALID;
		}
		Function function;
		try
		{
			function = FindFunction(OperandResolver.CoerceValueToInt(OperandResolver.GetSingleValue(args[0], srcRowIndex, srcColumnIndex)));
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		IList<ValueEval> list = new List<ValueEval>(Arrays.AsList(args).GetRange(1, args.Length - 1));
		IEnumerator<ValueEval> enumerator = list.GetEnumerator();
		IList<ValueEval> list2 = new List<ValueEval>();
		while (enumerator.MoveNext())
		{
			ValueEval current = enumerator.Current;
			if (current is LazyRefEval && ((LazyRefEval)current).IsSubTotal)
			{
				list2.Add(current);
			}
		}
		foreach (ValueEval item in list2)
		{
			list.Remove(item);
		}
		return function.Evaluate(list.ToArray(), srcRowIndex, srcColumnIndex);
	}
}
