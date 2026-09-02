using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public interface Function0Arg : Function
{
	ValueEval Evaluate(int srcRowIndex, int srcColumnIndex);
}
