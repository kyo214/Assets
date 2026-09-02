using System;

namespace NPOI.SS.Formula.Functions;

public class Cos : OneArg
{
	public override double Evaluate(double d)
	{
		return Math.Cos(d);
	}
}
