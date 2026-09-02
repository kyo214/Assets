namespace NPOI.SS.Formula.Eval;

public class LessThanEval : RelationalOperationEval
{
	public override bool ConvertComparisonResult(int cmpResult)
	{
		return cmpResult < 0;
	}
}
