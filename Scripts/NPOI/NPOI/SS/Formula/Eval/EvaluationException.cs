using System;

namespace NPOI.SS.Formula.Eval;

[Serializable]
public class EvaluationException : Exception
{
	private ErrorEval _errorEval;

	public EvaluationException(ErrorEval errorEval)
	{
		_errorEval = errorEval;
	}

	public static EvaluationException InvalidValue()
	{
		return new EvaluationException(ErrorEval.VALUE_INVALID);
	}

	public static EvaluationException InvalidRef()
	{
		return new EvaluationException(ErrorEval.REF_INVALID);
	}

	public static EvaluationException NumberError()
	{
		return new EvaluationException(ErrorEval.NUM_ERROR);
	}

	public ErrorEval GetErrorEval()
	{
		return _errorEval;
	}
}
