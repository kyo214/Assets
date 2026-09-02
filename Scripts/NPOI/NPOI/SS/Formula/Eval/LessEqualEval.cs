namespace NPOI.SS.Formula.Eval;

public class LessEqualEval : RelationalOperationEval
{
	public override bool ConvertComparisonResult(int cmpResult)
	{
		return cmpResult <= 0;
	}
}
