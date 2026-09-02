using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class DMin : IDStarAlgorithm
{
	private ValueEval minimumValue;

	public ValueEval Result
	{
		get
		{
			if (minimumValue == null)
			{
				return NumberEval.ZERO;
			}
			return minimumValue;
		}
	}

	public bool ProcessMatch(ValueEval eval)
	{
		if (eval is NumericValueEval)
		{
			if (minimumValue == null)
			{
				minimumValue = eval;
			}
			else
			{
				double numberValue = ((NumericValueEval)eval).NumberValue;
				double numberValue2 = ((NumericValueEval)minimumValue).NumberValue;
				if (numberValue < numberValue2)
				{
					minimumValue = eval;
				}
			}
		}
		return true;
	}
}
