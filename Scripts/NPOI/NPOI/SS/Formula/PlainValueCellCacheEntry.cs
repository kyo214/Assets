using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula;

public class PlainValueCellCacheEntry : CellCacheEntry
{
	public PlainValueCellCacheEntry(ValueEval value)
	{
		UpdateValue(value);
	}
}
