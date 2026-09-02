namespace NPOI.SS.Formula.Eval;

public class NotEqualEval : RelationalOperationEval
{
	public override bool ConvertComparisonResult(int cmpResult)
	{
		return cmpResult != 0;
	}
}
