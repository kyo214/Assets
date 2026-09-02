using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Rand : Fixed0ArgFunction
{
	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex)
	{
		return new NumberEval(new Random().NextDouble());
	}
}
