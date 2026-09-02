using System;

namespace NPOI.SS.Formula.Functions;

public class Acos : OneArg
{
	public override double Evaluate(double d)
	{
		return Math.Acos(d);
	}
}
