using System;

namespace NPOI.SS.Formula.Functions;

public class Tanh : OneArg
{
	public override double Evaluate(double d)
	{
		return Math.Tanh(d);
	}
}
