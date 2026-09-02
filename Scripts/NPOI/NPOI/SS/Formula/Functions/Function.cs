using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public interface Function
{
	ValueEval Evaluate(ValueEval[] args, int srcRowIndex, int srcColumnIndex);
}
