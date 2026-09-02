using System.Collections.Generic;
using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "CraftMaterialLibraryScriptableObject", menuName = "WMO/ScriptableObjects/Crafting/Crafting Material Library", order = 0)]
public class CraftMaterialLibraryScriptableObject : ScriptableObjectLibraryDictionaryBase<string, CraftMaterialScriptableObject>
{
	public CraftMaterialScriptableObject FindByItemIdKey(string key)
	{
		foreach (CraftMaterialScriptableObject data in base.DataList)
		{
			if (data.ItemInventoryId.ToString() == key)
			{
				return data;
			}
		}
		return null;
	}

	protected override void AddDataDictionary(Dictionary<string, CraftMaterialScriptableObject> dic, CraftMaterialScriptableObject data)
	{
		if (!dic.ContainsKey(data.ID))
		{
			dic.Add(data.ID, data);
		}
	}

	protected override void UpdateData(CraftMaterialScriptableObject data)
	{
		BGDatabase_CraftMaterial entity = BGDatabase_CraftMaterial.GetEntity(data.ID);
		if (entity != null)
		{
			data.SortOrderId = entity.Index;
			data.Name = entity.Name;
			data.Type = entity.MaterialType;
			data.ItemInventoryId = entity.ItemInventoryId;
			data.MinMaxDropAmount = new Vector2(entity.MinDropAmount, entity.MaxDropAmount);
			data.MaterialNameLocalizeId = entity.MaterialNameLocalizeId;
			data.MaterialDescriptionLocalizeId = entity.MaterialDescriptionLocalizeId;
		}
	}
}
