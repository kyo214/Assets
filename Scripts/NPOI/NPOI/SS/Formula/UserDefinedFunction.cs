using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;

namespace NPOI.SS.Formula;

public class UserDefinedFunction : FreeRefFunction
{
	public static FreeRefFunction instance = new UserDefinedFunction();

	private UserDefinedFunction()
	{
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		int num = args.Length;
		if (num < 1)
		{
			throw new Exception("function name argument missing");
		}
		ValueEval valueEval = args[0];
		string empty = string.Empty;
		if (valueEval is FunctionNameEval)
		{
			empty = ((FunctionNameEval)valueEval).FunctionName;
			FreeRefFunction obj = ec.FindUserDefinedFunction(empty) ?? throw new NotImplementedFunctionException(empty);
			int num2 = num - 1;
			ValueEval[] array = new ValueEval[num2];
			Array.Copy(args, 1, array, 0, num2);
			return obj.Evaluate(array, ec);
		}
		throw new Exception("First argument should be a NameEval, but got (" + valueEval.GetType().Name + ")");
	}
}
