using System.Collections.Generic;
using BansheeGz.BGDatabase;
using Sirenix.OdinInspector;
using Toked;
using UnityEngine;

namespace Database.Tools;

[CreateAssetMenu(fileName = "DatabaseUpdaterConfig", menuName = "WMO/ScriptableObjects/Tools/Database Updater", order = 0)]
public class DatabaseUpdaterConfig : SerializedScriptableObject
{
	[SerializeField]
	private Dictionary<string, IScriptableObjectLibrary> _scriptableObjectDictionary = new Dictionary<string, IScriptableObjectLibrary>();

	[SerializeField]
	private string[] _scriptableObjectLocationPath = new string[1] { FilePath.SO_PATH };

	public T GetData<T>() where T : IScriptableObjectLibrary
	{
		string key = typeof(T).Name;
		T result = null;
		if (_scriptableObjectDictionary.TryGetValue(key, out var value))
		{
			return value as T;
		}
		return result;
	}

	protected virtual string[] FindAssets()
	{
		return null;
	}

	private void UpdateDatabase()
	{
		BGAddonLiveUpdate.LoadDefault();
		UpdateLibrary();
	}

	public void UpdateLibrary()
	{
		foreach (KeyValuePair<string, IScriptableObjectLibrary> item in _scriptableObjectDictionary)
		{
			item.Value.UpdateLibrary();
		}
	}
}
