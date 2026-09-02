using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Not : Fixed1ArgFunction
{
	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0)
	{
		bool flag2;
		try
		{
			bool? flag = OperandResolver.CoerceValueToBoolean(OperandResolver.GetSingleValue(arg0, srcRowIndex, srcColumnIndex), stringsAreBlanks: false);
			flag2 = flag.HasValue && flag.Value;
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		return BoolEval.ValueOf(!flag2);
	}
}
