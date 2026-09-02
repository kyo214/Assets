using System.Collections.Generic;
using UnityEngine;

public abstract class ScriptableObjectLibraryBase<T> : IScriptableObjectLibrary where T : ScriptableObject
{
	[SerializeField]
	private List<T> _dataList = new List<T>();

	public List<T> DataList => _dataList;

	public void SetDataList(List<T> newListData)
	{
		_dataList = new List<T>(newListData);
	}

	protected override string GetFilterString()
	{
		return "t:" + typeof(T).Name;
	}

	public virtual void SortData()
	{
		_dataList.Sort((T x, T y) => string.Compare(x.name, y.name));
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
		SortData();
	}

	public override void UpdateLibrary()
	{
		foreach (T data in DataList)
		{
			UpdateData(data);
		}
	}

	protected abstract void UpdateData(T data);
}
