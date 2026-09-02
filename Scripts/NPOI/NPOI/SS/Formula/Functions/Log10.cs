using System;

namespace NPOI.SS.Formula.Functions;

public class Log10 : OneArg
{
	public override double Evaluate(double d)
	{
		return Math.Log(d) / NumericFunction.LOG_10_TO_BASE_e;
	}
}
