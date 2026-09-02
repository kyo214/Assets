using System;

namespace NPOI.SS.Formula.Functions;

public class Ln : OneArg
{
	public override double Evaluate(double d)
	{
		return Math.Log(d);
	}
}
