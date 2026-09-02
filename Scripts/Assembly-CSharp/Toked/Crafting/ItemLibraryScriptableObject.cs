using System.Collections.Generic;
using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "ItemLibraryScriptableObject", menuName = "WMO/ScriptableObjects/Item/Item Library", order = 0)]
public class ItemLibraryScriptableObject : ScriptableObjectLibraryDictionaryBase<string, ItemScriptableObject>
{
	protected override void AddDataDictionary(Dictionary<string, ItemScriptableObject> dic, ItemScriptableObject data)
	{
		if (!dic.ContainsKey(data.ID))
		{
			dic.Add(data.ID, data);
		}
	}

	protected override void UpdateData(ItemScriptableObject data)
	{
	}
}
