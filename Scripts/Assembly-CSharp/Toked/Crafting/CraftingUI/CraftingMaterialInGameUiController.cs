using Toked.Inventory;

namespace Toked.Crafting.CraftingUI;

public class CraftingMaterialInGameUiController : CraftingMaterialsUIController
{
	public override void Init()
	{
		if (!_initialize)
		{
			for (int i = 0; i < _craftingMaterialUiList.Count; i++)
			{
				CraftingMaterialUI craftingMaterialUI = _craftingMaterialUiList[i];
				CraftMaterialScriptableObject craftingMaterialSo = _craftingMaterialUiList[i].CraftingMaterialSo;
				int amount = (_craftingManager.PlayerData ? GetMaterialAmount(craftingMaterialSo) : 0);
				craftingMaterialUI.Init(craftingMaterialSo, amount, hideUiIfZero: true);
				_craftingMaterialUis.Add(craftingMaterialSo.ID, craftingMaterialUI);
			}
			if ((bool)_view)
			{
				_view.OnShowCallback.Event.AddListener(RefreshUI);
			}
			_initialize = true;
		}
		InitOnMaterialChangeEvent();
	}

	protected override MaterialInventory GetMaterialInventory()
	{
		return _craftingManager.PlayerData?.MaterialInventory;
	}

	public override void RefreshUI()
	{
		_ = _craftingManager.PlayerData.MaterialInventory;
		for (int i = 0; i < _craftingMaterialUiList.Count; i++)
		{
			CraftingMaterialUI craftingMaterialUI = _craftingMaterialUiList[i];
			CraftMaterialScriptableObject craftingMaterialSo = _craftingMaterialUiList[i].CraftingMaterialSo;
			int materialAmount = GetMaterialAmount(craftingMaterialSo);
			craftingMaterialUI.SetText(materialAmount, withAnimation: false, hideUiIfZero: true);
		}
	}

	protected override int GetMaterialAmount(CraftMaterialScriptableObject craftMaterialScriptableObject)
	{
		return craftMaterialScriptableObject.GetMaterialAmount(_craftingManager.PlayerData, MaterialInventoryManager.InventoryType.InGame);
	}
}
