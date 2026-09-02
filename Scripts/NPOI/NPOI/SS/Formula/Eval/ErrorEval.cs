using System.Collections.Generic;
using System.Text;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.SS.Formula.Eval;

public class ErrorEval : ValueEval
{
	private static Dictionary<FormulaError, ErrorEval> evals = new Dictionary<FormulaError, ErrorEval>();

	public static readonly ErrorEval NULL_INTERSECTION = new ErrorEval(FormulaError.NULL);

	public static readonly ErrorEval DIV_ZERO = new ErrorEval(FormulaError.DIV0);

	public static readonly ErrorEval VALUE_INVALID = new ErrorEval(FormulaError.VALUE);

	public static readonly ErrorEval REF_INVALID = new ErrorEval(FormulaError.REF);

	public static readonly ErrorEval NAME_INVALID = new ErrorEval(FormulaError.NAME);

	public static readonly ErrorEval NUM_ERROR = new ErrorEval(FormulaError.NUM);

	public static readonly ErrorEval NA = new ErrorEval(FormulaError.NA);

	public static ErrorEval FUNCTION_NOT_IMPLEMENTED = new ErrorEval(FormulaError.FUNCTION_NOT_IMPLEMENTED);

	public static ErrorEval CIRCULAR_REF_ERROR = new ErrorEval(FormulaError.CIRCULAR_REF);

	private FormulaError _error;

	public int ErrorCode => _error.LongCode;

	public string ErrorString => _error.String;

	public static ErrorEval ValueOf(int errorCode)
	{
		FormulaError key = FormulaError.ForInt(errorCode);
		if (evals.ContainsKey(key))
		{
			return evals[key];
		}
		throw new RuntimeException("Unhandled error type  for code " + errorCode);
	}

	public static string GetText(int errorCode)
	{
		if (FormulaError.IsValidCode(errorCode))
		{
			return FormulaError.ForInt(errorCode).String;
		}
		return "~non~std~err(" + errorCode + ")~";
	}

	private ErrorEval(FormulaError error)
	{
		_error = error;
		if (!evals.ContainsKey(error))
		{
			evals.Add(error, this);
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder(64);
		stringBuilder.Append(GetType().Name).Append(" [");
		stringBuilder.Append(_error.String);
		stringBuilder.Append("]");
		return stringBuilder.ToString();
	}
}
