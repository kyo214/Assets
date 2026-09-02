using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public interface Function4Arg : Function
{
	ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1, ValueEval arg2, ValueEval arg3);
}
