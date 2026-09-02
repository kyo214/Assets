using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public interface IDStarAlgorithm
{
	ValueEval Result { get; }

	bool ProcessMatch(ValueEval Eval);
}
