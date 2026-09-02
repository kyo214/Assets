using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Pi : Fixed0ArgFunction
{
	private static readonly NumberEval PI_EVAL = new NumberEval(Math.PI);

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex)
	{
		return PI_EVAL;
	}
}
