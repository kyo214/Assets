using System.Collections.Generic;
using Toked.Crafting.CraftingUI;
using Toked.Inventory;
using UnityEngine;

namespace _Modules.UIResult.Scripts;

public class UIMaterialResultPanel : MonoBehaviour
{
	[SerializeField]
	private List<CraftingMaterialUI> _craftingMaterialUiList;

	[SerializeField]
	private bool _withAnimation = true;

	public void Init()
	{
		foreach (CraftingMaterialUI craftingMaterialUi in _craftingMaterialUiList)
		{
			craftingMaterialUi.Init();
		}
	}

	public void Set(Dictionary<string, MaterialInventoryData> materialInventoryDic, bool isAlive = true)
	{
		bool flag = materialInventoryDic != null && materialInventoryDic.Count > 0;
		foreach (CraftingMaterialUI craftingMaterialUi in _craftingMaterialUiList)
		{
			if (isAlive)
			{
				if (flag)
				{
					materialInventoryDic.TryGetValue(craftingMaterialUi.CraftingMaterialSo.ID, out var value);
					int amount = value?.Amount ?? 0;
					craftingMaterialUi.Init(amount, _withAnimation);
				}
				else
				{
					craftingMaterialUi.Init();
				}
			}
			else
			{
				craftingMaterialUi.gameObject.SetActive(value: false);
			}
		}
	}

	public void Hide()
	{
		foreach (CraftingMaterialUI craftingMaterialUi in _craftingMaterialUiList)
		{
			craftingMaterialUi.gameObject.SetActive(value: false);
		}
	}
}
