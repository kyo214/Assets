using System;
using System.Text;
using NPOI.SS.Formula.Functions;

namespace NPOI.SS.Formula.Eval;

public class ConcatEval : Fixed2ArgFunction
{
	public static NPOI.SS.Formula.Functions.Function instance = new ConcatEval();

	private ConcatEval()
	{
	}

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1)
	{
		ValueEval singleValue;
		ValueEval singleValue2;
		try
		{
			singleValue = OperandResolver.GetSingleValue(arg0, srcRowIndex, srcColumnIndex);
			singleValue2 = OperandResolver.GetSingleValue(arg1, srcRowIndex, srcColumnIndex);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GetText(singleValue));
		stringBuilder.Append(GetText(singleValue2));
		return new StringEval(stringBuilder.ToString());
	}

	private object GetText(ValueEval ve)
	{
		if (ve is StringValueEval)
		{
			return ((StringValueEval)ve).StringValue;
		}
		if (ve == BlankEval.instance)
		{
			return "";
		}
		throw new InvalidOperationException("Unexpected value type (" + ve.GetType().Name + ")");
	}
}
