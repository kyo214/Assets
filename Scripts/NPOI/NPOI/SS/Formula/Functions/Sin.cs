using System;

namespace NPOI.SS.Formula.Functions;

public class Sin : OneArg
{
	public override double Evaluate(double d)
	{
		return Math.Sin(d);
	}
}
