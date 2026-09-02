using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public interface ValueVector
{
	int Size { get; }

	ValueEval GetItem(int index);
}
