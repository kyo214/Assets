using System.Collections.Generic;
using NPOI.SS.Formula.Eval;
using NPOI.Util;

namespace NPOI.SS.Formula.Functions;

public class FactDouble : Fixed1ArgFunction, FreeRefFunction
{
	public static FreeRefFunction instance = new FactDouble();

	private static Dictionary<int, BigInteger> cache = new Dictionary<int, BigInteger>();

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval numberVE)
	{
		int num;
		try
		{
			num = OperandResolver.CoerceValueToInt(numberVE);
		}
		catch (EvaluationException)
		{
			return ErrorEval.VALUE_INVALID;
		}
		if (num < 0)
		{
			return ErrorEval.NUM_ERROR;
		}
		return new NumberEval(factorial(num).LongValue());
	}

	public static BigInteger factorial(int n)
	{
		if (n == 0 || n < 0)
		{
			return BigInteger.One;
		}
		if (cache.ContainsKey(n))
		{
			return cache[n];
		}
		BigInteger bigInteger = BigInteger.ValueOf(n).Multiply(factorial(n - 2));
		cache.Add(n, bigInteger);
		return bigInteger;
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length != 1)
		{
			return ErrorEval.VALUE_INVALID;
		}
		return Evaluate(ec.RowIndex, ec.ColumnIndex, args[0]);
	}
}
