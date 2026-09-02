using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Slope : Fixed2ArgFunction
{
	private LinearRegressionFunction func;

	public Slope()
	{
		func = new LinearRegressionFunction(LinearRegressionFunction.FUNCTION.SLOPE);
	}

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1)
	{
		return func.Evaluate(srcRowIndex, srcColumnIndex, arg0, arg1);
	}
}
