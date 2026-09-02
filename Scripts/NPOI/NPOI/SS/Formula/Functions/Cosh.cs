using System;

namespace NPOI.SS.Formula.Functions;

public class Cosh : OneArg
{
	public override double Evaluate(double d)
	{
		return Math.Cosh(d);
	}
}
