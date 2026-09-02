using System;

namespace NPOI.SS.Formula.Functions;

public class Asin : OneArg
{
	public override double Evaluate(double d)
	{
		return Math.Asin(d);
	}
}
