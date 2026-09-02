using System;

namespace NPOI.SS.Formula.Functions;

public class Abs : OneArg
{
	public override double Evaluate(double d)
	{
		return Math.Abs(d);
	}
}
