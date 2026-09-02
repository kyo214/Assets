using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Mod : TwoArg
{
	public override double Evaluate(double d0, double d1)
	{
		if (d1 == 0.0)
		{
			throw new EvaluationException(ErrorEval.DIV_ZERO);
		}
		return MathX.Mod(d0, d1);
	}
}
