using System;

namespace NPOI.SS.Formula.Functions;

public class Sqrt : OneArg
{
	public override double Evaluate(double d)
	{
		return Math.Sqrt(d);
	}
}
