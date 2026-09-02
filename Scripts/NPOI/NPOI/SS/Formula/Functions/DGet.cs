using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class DGet : IDStarAlgorithm
{
	private ValueEval result;

	public ValueEval Result
	{
		get
		{
			if (result == null)
			{
				return ErrorEval.VALUE_INVALID;
			}
			if (result is BlankEval)
			{
				return ErrorEval.VALUE_INVALID;
			}
			try
			{
				if (OperandResolver.CoerceValueToString(OperandResolver.GetSingleValue(result, 0, 0)).Equals(""))
				{
					return ErrorEval.VALUE_INVALID;
				}
				return result;
			}
			catch (EvaluationException ex)
			{
				return ex.GetErrorEval();
			}
		}
	}

	public bool ProcessMatch(ValueEval eval)
	{
		if (result == null)
		{
			result = eval;
			return true;
		}
		result = ErrorEval.NUM_ERROR;
		return false;
	}
}
