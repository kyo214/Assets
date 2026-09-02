using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula;

public interface IEvaluationListener
{
	void OnCacheHit(int sheetIndex, int rowIndex, int columnIndex, ValueEval result);

	void OnReadPlainValue(int sheetIndex, int rowIndex, int columnIndex, ICacheEntry entry);

	void OnStartEvaluate(IEvaluationCell cell, ICacheEntry entry);

	void OnEndEvaluate(ICacheEntry entry, ValueEval result);

	void OnClearWholeCache();

	void OnClearCachedValue(ICacheEntry entry);

	void SortDependentCachedValues(ICacheEntry[] formulaCells);

	void OnClearDependentCachedValue(ICacheEntry formulaCell, int depth);

	void OnChangeFromBlankValue(int sheetIndex, int rowIndex, int columnIndex, IEvaluationCell cell, ICacheEntry entry);
}
