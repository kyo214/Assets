namespace NPOI.SS.Formula.Eval;

public class GreaterThanEval : RelationalOperationEval
{
	public override bool ConvertComparisonResult(int cmpResult)
	{
		return cmpResult > 0;
	}
}
