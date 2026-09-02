using System;

namespace NPOI.SS.Formula.Functions;

public class Degrees : OneArg
{
	public override double Evaluate(double d)
	{
		return d * 180.0 / Math.PI;
	}
}
