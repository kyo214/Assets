using System.Text;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Rept : Fixed2ArgFunction
{
	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval text, ValueEval number_times)
	{
		ValueEval singleValue;
		try
		{
			singleValue = OperandResolver.GetSingleValue(text, srcRowIndex, srcColumnIndex);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		string text2 = OperandResolver.CoerceValueToString(singleValue);
		double num = 0.0;
		try
		{
			num = OperandResolver.CoerceValueToDouble(number_times);
		}
		catch (EvaluationException)
		{
			return ErrorEval.VALUE_INVALID;
		}
		int num2 = (int)num;
		StringBuilder stringBuilder = new StringBuilder(text2.Length * num2);
		for (int i = 0; i < num2; i++)
		{
			stringBuilder.Append(text2);
		}
		if (stringBuilder.ToString().Length > 32767)
		{
			return ErrorEval.VALUE_INVALID;
		}
		return new StringEval(stringBuilder.ToString());
	}
}
