using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Log : Var1or2ArgFunction
{
	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0)
	{
		double num;
		try
		{
			num = Math.Log(NumericFunction.SingleOperandEvaluate(arg0, srcRowIndex, srcColumnIndex)) / NumericFunction.LOG_10_TO_BASE_e;
			NumericFunction.CheckValue(num);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		return new NumberEval(num);
	}

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1)
	{
		double num4;
		try
		{
			double d = NumericFunction.SingleOperandEvaluate(arg0, srcRowIndex, srcColumnIndex);
			double num = NumericFunction.SingleOperandEvaluate(arg1, srcRowIndex, srcColumnIndex);
			double num2 = Math.Log(d);
			double num3 = num;
			num4 = ((num3 != Math.E) ? (num2 / Math.Log(num3)) : num2);
			NumericFunction.CheckValue(num4);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		return new NumberEval(num4);
	}
}
