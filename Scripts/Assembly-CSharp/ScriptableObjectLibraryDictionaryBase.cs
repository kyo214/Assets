using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ScriptableObjectLibraryDictionaryBase<TKey, TSo> : IScriptableObjectLibrary where TSo : ScriptableObject
{
	[SerializeField]
	protected Dictionary<TKey, TSo> _dataDictionary = new Dictionary<TKey, TSo>();

	protected List<TSo> _dataList = new List<TSo>();

	[SerializeField]
	private string[] _scriptableObjectLocationPath;

	public List<TKey> KeyList => _dataDictionary.Keys.Where((TKey x) => x != null).ToList();

	public List<TSo> DataList
	{
		get
		{
			if (_dataList.Count != _dataDictionary.Count)
			{
				_dataList = SortList();
			}
			return _dataList;
		}
	}

	protected abstract void AddDataDictionary(Dictionary<TKey, TSo> dic, TSo data);

	public TSo GetDataByIndex(int index)
	{
		if (index >= 0 && index < _dataDictionary.Count)
		{
			return DataList[index];
		}
		return null;
	}

	public Dictionary<TKey, TSo> GetData()
	{
		return _dataDictionary;
	}

	public TSo GetData(TKey key)
	{
		_dataDictionary.TryGetValue(key, out var value);
		return value;
	}

	protected override string GetFilterString()
	{
		return "t:" + typeof(TSo).Name;
	}

	protected virtual string[] FindAssets()
	{
		return null;
	}

	public override void RefreshLibrary(Update_Type updateType = Update_Type.FINDASSETS)
	{
	}

	public virtual void RefreshLibraryDatabase()
	{
	}

	public virtual void RefreshLibraryFindAssets()
	{
	}

	protected void RefreshAndSortLibrary()
	{
		RefreshLibrary();
		SortLibrary();
	}

	protected void SortLibrary()
	{
		_dataList = SortList();
	}

	protected virtual List<TSo> SortList()
	{
		return _dataDictionary.Values.OrderBy((TSo o) => o.name).ToList();
	}

	public override void UpdateLibrary()
	{
		foreach (TSo data in DataList)
		{
			UpdateData(data);
		}
	}

	protected abstract void UpdateData(TSo data);

	protected virtual TSo CreateSo(string soName)
	{
		return null;
	}
}
