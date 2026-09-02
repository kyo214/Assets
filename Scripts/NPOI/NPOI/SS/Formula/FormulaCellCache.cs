using System.Collections;

namespace NPOI.SS.Formula;

public class FormulaCellCache
{
	private Hashtable _formulaEntriesByCell;

	public FormulaCellCache()
	{
		_formulaEntriesByCell = new Hashtable();
	}

	public CellCacheEntry[] GetCacheEntries()
	{
		FormulaCellCacheEntry[] array = new FormulaCellCacheEntry[_formulaEntriesByCell.Count];
		_formulaEntriesByCell.Values.CopyTo(array, 0);
		return array;
	}

	public void Clear()
	{
		_formulaEntriesByCell.Clear();
	}

	public FormulaCellCacheEntry Get(IEvaluationCell cell)
	{
		if (_formulaEntriesByCell.ContainsKey(cell.IdentityKey))
		{
			return (FormulaCellCacheEntry)_formulaEntriesByCell[cell.IdentityKey];
		}
		return null;
	}

	public void Put(IEvaluationCell cell, FormulaCellCacheEntry entry)
	{
		_formulaEntriesByCell[cell.IdentityKey] = entry;
	}

	public FormulaCellCacheEntry Remove(IEvaluationCell cell)
	{
		FormulaCellCacheEntry result = (FormulaCellCacheEntry)_formulaEntriesByCell[cell.IdentityKey];
		_formulaEntriesByCell.Remove(cell);
		return result;
	}

	public void ApplyOperation(IEntryOperation operation)
	{
		IEnumerator enumerator = _formulaEntriesByCell.Values.GetEnumerator();
		while (enumerator.MoveNext())
		{
			operation.ProcessEntry((FormulaCellCacheEntry)enumerator.Current);
		}
	}
}
