using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Floor : TwoArg
{
	private static double ZERO;

	public override double Evaluate(double d0, double d1)
	{
		if (d1 == ZERO)
		{
			if (d0 == ZERO)
			{
				return ZERO;
			}
			throw new EvaluationException(ErrorEval.DIV_ZERO);
		}
		return MathX.Floor(d0, d1);
	}
}
