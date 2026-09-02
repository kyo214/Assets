using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Functions;

public class Text : Fixed2ArgFunction
{
	public static DataFormatter Formatter = new DataFormatter();

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1)
	{
		double value;
		string formatString;
		try
		{
			value = TextFunction.EvaluateDoubleArg(arg0, srcRowIndex, srcColumnIndex);
			formatString = TextFunction.EvaluateStringArg(arg1, srcRowIndex, srcColumnIndex);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		try
		{
			return new StringEval(Formatter.FormatRawCellContents(value, -1, formatString));
		}
		catch (Exception)
		{
			return ErrorEval.VALUE_INVALID;
		}
	}
}
