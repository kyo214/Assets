using System;

namespace NPOI.SS.Formula.Functions;

public class Radians : OneArg
{
	public override double Evaluate(double d)
	{
		return d * Math.PI / 180.0;
	}
}
