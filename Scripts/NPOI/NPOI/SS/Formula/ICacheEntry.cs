using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula;

public interface ICacheEntry
{
	ValueEval GetValue();
}
