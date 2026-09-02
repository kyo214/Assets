using System.Collections.Generic;
using Doozy.Runtime.UIManager.Containers;
using Toked.Inventory;
using UnityEngine;

namespace Toked.Crafting.CraftingUI;

public class CraftingMaterialsUIController : MonoBehaviour
{
	[SerializeField]
	private CraftMaterialLibraryScriptableObject _craftMaterialLibraryScriptableObject;

	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	protected List<CraftingMaterialUI> _craftingMaterialUiList = new List<CraftingMaterialUI>();

	protected Dictionary<string, CraftingMaterialUI> _craftingMaterialUis = new Dictionary<string, CraftingMaterialUI>();

	[SerializeField]
	protected CraftingManager _craftingManager;

	protected CraftingUIController _craftingUiController;

	[SerializeField]
	protected UIView _view;

	protected bool _initialize;

	private bool _initializeEvent;

	public CraftingUIController CraftingUIController => _craftingUiController ?? (_craftingUiController = _craftingManager?.GetComponent<CraftingUIController>());

	public virtual void Init()
	{
		if (!_initialize)
		{
			for (int i = 0; i < _craftingMaterialUiList.Count; i++)
			{
				CraftingMaterialUI craftingMaterialUI = _craftingMaterialUiList[i];
				CraftMaterialScriptableObject craftingMaterialSo = _craftingMaterialUiList[i].CraftingMaterialSo;
				int amount = (_craftingManager.PlayerData ? GetMaterialAmount(craftingMaterialSo) : 0);
				craftingMaterialUI.Init(craftingMaterialSo, amount);
				craftingMaterialUI.SetSelectableUIEvent(OnHoverMaterialUI, OnUnhoverMaterialUI);
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

	protected void InitOnMaterialChangeEvent()
	{
		if (!_initializeEvent)
		{
			AddOnChangedMaterialEvent();
			_initializeEvent = true;
		}
	}

	public void SetVisibilityUi(bool show)
	{
		if (_canvasGroup == null)
		{
			_canvasGroup = GetComponent<CanvasGroup>() ?? base.gameObject.AddComponent<CanvasGroup>();
		}
		_canvasGroup.alpha = (show ? 1 : 0);
		_canvasGroup.interactable = show;
		_canvasGroup.blocksRaycasts = show;
	}

	private void OnDestroy()
	{
		if (_initializeEvent)
		{
			RemoveOnChangedMaterialEvent();
		}
	}

	public void SetHover(List<CraftingIngredient> craftingIngredientList, List<bool> hoverState)
	{
		int num = 0;
		foreach (CraftingIngredient craftingIngredient in craftingIngredientList)
		{
			GetCraftingMaterialUi(craftingIngredient.CraftMaterialScriptableObject.ID)?.Selected(hoverState[num++]);
		}
	}

	public void SetUnHover()
	{
		foreach (CraftingMaterialUI craftingMaterialUi in _craftingMaterialUiList)
		{
			craftingMaterialUi.Deselected();
		}
	}

	public virtual void SetMaterialAmount(string key, int amount)
	{
		if (GetCraftingMaterialUi(key) != null)
		{
			GetCraftingMaterialUi(key)?.SetText(amount, withAnimation: true);
		}
	}

	public CraftingMaterialUI GetCraftingMaterialUi(string key)
	{
		CraftingMaterialUI value = null;
		_craftingMaterialUis.TryGetValue(key, out value);
		return value;
	}

	protected void SetMaterialAmount(CraftMaterialScriptableObject craftMaterialScriptableObject, int amount)
	{
		SetMaterialAmount(craftMaterialScriptableObject.ID, amount);
	}

	public virtual void RefreshUI()
	{
		for (int i = 0; i < _craftingMaterialUiList.Count; i++)
		{
			CraftingMaterialUI craftingMaterialUI = _craftingMaterialUiList[i];
			CraftMaterialScriptableObject craftingMaterialSo = _craftingMaterialUiList[i].CraftingMaterialSo;
			int materialAmount = GetMaterialAmount(craftingMaterialSo);
			craftingMaterialUI.SetText(materialAmount);
		}
	}

	private void AddOnChangedMaterialEvent()
	{
		MaterialInventory materialInventory = GetMaterialInventory();
		if ((bool)materialInventory)
		{
			materialInventory.OnChangedMaterialEvent += SetMaterialAmount;
		}
		if ((bool)_craftingManager.PlayerData)
		{
			_craftingManager.PlayerData.SkillData.OnChangedSkillPointEvent += OnChangedSkillPointAction;
		}
	}

	private void RemoveOnChangedMaterialEvent()
	{
		MaterialInventory materialInventory = GetMaterialInventory();
		if ((bool)materialInventory)
		{
			materialInventory.OnChangedMaterialEvent -= SetMaterialAmount;
		}
		if ((bool)_craftingManager.PlayerData)
		{
			_craftingManager.PlayerData.SkillData.OnChangedSkillPointEvent -= OnChangedSkillPointAction;
		}
	}

	protected virtual MaterialInventory GetMaterialInventory()
	{
		return _craftingManager.PlayerData?.MaterialInventoryManager.GetMaterialInventory();
	}

	protected virtual int GetMaterialAmount(CraftMaterialScriptableObject craftMaterialScriptableObject)
	{
		return craftMaterialScriptableObject.GetMaterialAmount(_craftingManager.PlayerData);
	}

	private void OnChangedSkillPointAction(int amount)
	{
		SetMaterialAmount("SkillPoint", amount);
	}

	private void OnHoverMaterialUI(CraftMaterialScriptableObject craftMaterialScriptableObject)
	{
		CraftingUIController?.CraftingDescriptionsUI.Set(craftMaterialScriptableObject.MaterialNameLocalizeId, craftMaterialScriptableObject.MaterialDescriptionLocalizeId);
	}

	private void OnUnhoverMaterialUI()
	{
		CraftingUIController?.CraftingDescriptionsUI.Reset();
	}
}
