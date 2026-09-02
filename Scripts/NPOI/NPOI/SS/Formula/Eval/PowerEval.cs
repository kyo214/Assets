using System;

namespace NPOI.SS.Formula.Eval;

public class PowerEval : TwoOperandNumericOperation
{
	public override double Evaluate(double d0, double d1)
	{
		return Math.Pow(d0, d1);
	}
}
