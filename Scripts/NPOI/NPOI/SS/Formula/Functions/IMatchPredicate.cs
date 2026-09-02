using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public interface IMatchPredicate
{
	bool Matches(ValueEval x);
}
