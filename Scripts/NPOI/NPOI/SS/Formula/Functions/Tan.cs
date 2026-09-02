using System;

namespace NPOI.SS.Formula.Functions;

public class Tan : OneArg
{
	public override double Evaluate(double d)
	{
		return Math.Tan(d);
	}
}
