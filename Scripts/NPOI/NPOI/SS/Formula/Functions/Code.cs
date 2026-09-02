using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Code : Fixed1ArgFunction
{
	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval textArg)
	{
		ValueEval singleValue;
		try
		{
			singleValue = OperandResolver.GetSingleValue(textArg, srcRowIndex, srcColumnIndex);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		string text = OperandResolver.CoerceValueToString(singleValue);
		if (text.Length == 0)
		{
			return ErrorEval.VALUE_INVALID;
		}
		return new StringEval(((int)text[0]).ToString());
	}
}
