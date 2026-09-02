using System;

namespace NPOI.SS.Formula.Functions;

public class Atan : OneArg
{
	public override double Evaluate(double d)
	{
		return Math.Atan(d);
	}
}
