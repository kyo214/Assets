namespace NPOI.SS.Formula.Eval;

public class GreaterEqualEval : RelationalOperationEval
{
	public override bool ConvertComparisonResult(int cmpResult)
	{
		return cmpResult >= 0;
	}
}
