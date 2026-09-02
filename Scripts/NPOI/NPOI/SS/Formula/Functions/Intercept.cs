using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Intercept : Fixed2ArgFunction
{
	private LinearRegressionFunction func;

	public Intercept()
	{
		func = new LinearRegressionFunction(LinearRegressionFunction.FUNCTION.INTERCEPT);
	}

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1)
	{
		return func.Evaluate(srcRowIndex, srcColumnIndex, arg0, arg1);
	}
}
