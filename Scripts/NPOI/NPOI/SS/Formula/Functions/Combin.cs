using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Combin : TwoArg
{
	public override double Evaluate(double d0, double d1)
	{
		if (d0 > 2147483647.0 || d1 > 2147483647.0)
		{
			throw new EvaluationException(ErrorEval.NUM_ERROR);
		}
		return MathX.NChooseK((int)d0, (int)d1);
	}
}
