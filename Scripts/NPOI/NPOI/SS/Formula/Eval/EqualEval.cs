namespace NPOI.SS.Formula.Eval;

public class EqualEval : RelationalOperationEval
{
	public override bool ConvertComparisonResult(int cmpResult)
	{
		return cmpResult == 0;
	}
}
