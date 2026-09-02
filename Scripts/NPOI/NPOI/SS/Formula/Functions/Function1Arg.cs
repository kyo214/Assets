using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public interface Function1Arg : Function
{
	ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0);
}
