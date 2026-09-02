using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Functions;

public class Errortype : Fixed1ArgFunction
{
	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0)
	{
		try
		{
			OperandResolver.GetSingleValue(arg0, srcRowIndex, srcColumnIndex);
			return ErrorEval.NA;
		}
		catch (EvaluationException ex)
		{
			return new NumberEval(TranslateErrorCodeToErrorTypeValue(ex.GetErrorEval().ErrorCode));
		}
	}

	private int TranslateErrorCodeToErrorTypeValue(int errorCode)
	{
		return (FormulaErrorEnum)errorCode switch
		{
			FormulaErrorEnum.NULL => 1, 
			FormulaErrorEnum.DIV_0 => 2, 
			FormulaErrorEnum.VALUE => 3, 
			FormulaErrorEnum.REF => 4, 
			FormulaErrorEnum.NAME => 5, 
			FormulaErrorEnum.NUM => 6, 
			FormulaErrorEnum.NA => 7, 
			_ => throw new ArgumentException("Invalid error code (" + errorCode + ")"), 
		};
	}
}
