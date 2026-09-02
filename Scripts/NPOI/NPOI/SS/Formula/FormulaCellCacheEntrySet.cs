using System;

namespace NPOI.SS.Formula;

internal class FormulaCellCacheEntrySet
{
	private int _size;

	private FormulaCellCacheEntry[] _arr;

	public FormulaCellCacheEntrySet()
	{
		_arr = FormulaCellCacheEntry.EMPTY_ARRAY;
	}

	public FormulaCellCacheEntry[] ToArray()
	{
		int size = _size;
		if (size < 1)
		{
			return FormulaCellCacheEntry.EMPTY_ARRAY;
		}
		FormulaCellCacheEntry[] array = new FormulaCellCacheEntry[size];
		int num = 0;
		for (int i = 0; i < _arr.Length; i++)
		{
			FormulaCellCacheEntry formulaCellCacheEntry = _arr[i];
			if (formulaCellCacheEntry != null)
			{
				array[num++] = formulaCellCacheEntry;
			}
		}
		if (num != size)
		{
			throw new InvalidOperationException("size mismatch");
		}
		return array;
	}

	public void Add(CellCacheEntry cce)
	{
		CellCacheEntry[] arr2;
		if (_size * 3 >= _arr.Length * 2)
		{
			FormulaCellCacheEntry[] arr = _arr;
			FormulaCellCacheEntry[] array = new FormulaCellCacheEntry[4 + _arr.Length * 3 / 2];
			for (int i = 0; i < arr.Length; i++)
			{
				FormulaCellCacheEntry formulaCellCacheEntry = _arr[i];
				if (formulaCellCacheEntry != null)
				{
					arr2 = array;
					AddInternal(arr2, formulaCellCacheEntry);
				}
			}
			_arr = array;
		}
		arr2 = _arr;
		if (AddInternal(arr2, cce))
		{
			_size++;
		}
	}

	private static bool AddInternal(CellCacheEntry[] arr, CellCacheEntry cce)
	{
		int num = cce.GetHashCode() % arr.Length;
		for (int i = num; i < arr.Length; i++)
		{
			CellCacheEntry cellCacheEntry = arr[i];
			if (cellCacheEntry == cce)
			{
				return false;
			}
			if (cellCacheEntry == null)
			{
				arr[i] = cce;
				return true;
			}
		}
		for (int j = 0; j < num; j++)
		{
			CellCacheEntry cellCacheEntry2 = arr[j];
			if (cellCacheEntry2 == cce)
			{
				return false;
			}
			if (cellCacheEntry2 == null)
			{
				arr[j] = cce;
				return true;
			}
		}
		throw new InvalidOperationException("No empty space found");
	}

	public bool Remove(CellCacheEntry cce)
	{
		FormulaCellCacheEntry[] arr = _arr;
		if (_size * 3 < _arr.Length && _arr.Length > 8)
		{
			bool result = false;
			FormulaCellCacheEntry[] arr2 = _arr;
			FormulaCellCacheEntry[] array = new FormulaCellCacheEntry[_arr.Length / 2];
			for (int i = 0; i < arr2.Length; i++)
			{
				FormulaCellCacheEntry formulaCellCacheEntry = _arr[i];
				if (formulaCellCacheEntry != null)
				{
					if (formulaCellCacheEntry == cce)
					{
						result = true;
						_size--;
					}
					else
					{
						CellCacheEntry[] arr3 = array;
						AddInternal(arr3, formulaCellCacheEntry);
					}
				}
			}
			_arr = array;
			return result;
		}
		int num = cce.GetHashCode() % arr.Length;
		for (int j = num; j < arr.Length; j++)
		{
			if (arr[j] == cce)
			{
				arr[j] = null;
				_size--;
				return true;
			}
		}
		for (int k = 0; k < num; k++)
		{
			if (arr[k] == cce)
			{
				arr[k] = null;
				_size--;
				return true;
			}
		}
		return false;
	}
}
