using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public interface FreeRefFunction
{
	ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec);
}
