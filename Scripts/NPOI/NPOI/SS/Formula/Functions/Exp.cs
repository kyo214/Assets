using System;

namespace NPOI.SS.Formula.Functions;

public class Exp : OneArg
{
	public override double Evaluate(double d)
	{
		return Math.Pow(Math.E, d);
	}
}
