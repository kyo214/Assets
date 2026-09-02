using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public interface LookupValueComparer
{
	CompareResult CompareTo(ValueEval other);
}
