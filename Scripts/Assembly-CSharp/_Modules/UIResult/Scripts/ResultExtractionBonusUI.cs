using System.Collections.Generic;
using DG.Tweening;
using Toked.Crafting;
using Toked.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace _Modules.UIResult.Scripts;

public class ResultExtractionBonusUI : MonoBehaviour
{
	[SerializeField]
	private ExtractionBonusItemUI _extractionBonusItemUIPrefab;

	[SerializeField]
	private ScrollRect _scrollRect;

	[SerializeField]
	private MapUnlockUI _mapUnlockUI;

	[SerializeField]
	private HorizontalOrVerticalLayoutGroup _layoutGroup;

	[SerializeField]
	private ExtractionContainerAnimation _extractionContainerAnimation;

	[SerializeField]
	private float _animationTime = 0.6f;

	[SerializeField]
	private bool _customValue;

	[SerializeField]
	private float _spacingAnimationFrom = -240f;

	[SerializeField]
	private float _spacingAnimationTo = 20f;

	private float _defaultSpacing = 20f;

	public bool Init(List<ItemToCraftMaterialConverter.ConvertMaterialItemData> convertMaterialItemList)
	{
		_defaultSpacing = _layoutGroup.spacing;
		Hide();
		bool flag = false;
		if (convertMaterialItemList.Count > 0)
		{
			InitUI(convertMaterialItemList);
			flag = true;
		}
		flag = _mapUnlockUI.InitUI() | flag;
		if (flag)
		{
			Show();
		}
		else
		{
			Hide();
		}
		return flag;
	}

	public (bool show, Dictionary<string, MaterialInventoryData> totalMaterial) Init()
	{
		_defaultSpacing = _layoutGroup.spacing;
		Hide();
		List<ItemToCraftMaterialConverter.ConvertMaterialItemData> list = new List<ItemToCraftMaterialConverter.ConvertMaterialItemData>();
		foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			if (item2 != null)
			{
				list.AddRange(ItemToCraftMaterialConverter.ConvertItemToCraftMaterial(item2));
			}
		}
		if (GetWinCondition())
		{
			AddBonusMapItem(list);
		}
		bool flag = false;
		Dictionary<string, MaterialInventoryData> item = new Dictionary<string, MaterialInventoryData>();
		if (list.Count > 0)
		{
			InitUI(list);
			item = CalculateTotalMaterial(list);
			flag = true;
		}
		flag = _mapUnlockUI.InitUI() | flag;
		if (flag)
		{
			Show();
		}
		else
		{
			Hide();
		}
		return (show: flag, totalMaterial: item);
	}

	public void Show()
	{
		float spacing = (_customValue ? _spacingAnimationFrom : (0f - _extractionBonusItemUIPrefab.RectTransform.sizeDelta.x));
		float to = (_customValue ? _spacingAnimationTo : _defaultSpacing);
		_layoutGroup.spacing = spacing;
		if ((bool)_extractionContainerAnimation)
		{
			_extractionContainerAnimation.Init(Show);
		}
		else
		{
			Show();
		}
		void Show()
		{
			_scrollRect.gameObject.SetActive(value: true);
			DOTween.To(() => _layoutGroup.spacing, (float x) =>
			{
				_layoutGroup.spacing = x;
			}, to, _animationTime);
		}
	}

	public void Hide()
	{
		_scrollRect.gameObject.SetActive(value: false);
		_layoutGroup.spacing = _defaultSpacing;
	}

	private void InitUI(List<ItemToCraftMaterialConverter.ConvertMaterialItemData> materialItemList)
	{
		foreach (ItemToCraftMaterialConverter.ConvertMaterialItemData materialItem in materialItemList)
		{
			if (materialItem != null)
			{
				Object.Instantiate(_extractionBonusItemUIPrefab, _scrollRect.content).Init(materialItem);
			}
		}
	}

	private Dictionary<string, MaterialInventoryData> CalculateTotalMaterial(List<ItemToCraftMaterialConverter.ConvertMaterialItemData> convertMaterialItemList)
	{
		Dictionary<string, MaterialInventoryData> dictionary = new Dictionary<string, MaterialInventoryData>();
		foreach (ItemToCraftMaterialConverter.ConvertMaterialItemData item in new List<ItemToCraftMaterialConverter.ConvertMaterialItemData>(convertMaterialItemList))
		{
			foreach (KeyValuePair<string, MaterialInventoryData> item2 in item.Material)
			{
				if (dictionary.ContainsKey(item2.Key))
				{
					dictionary[item2.Key].Amount += item2.Value.Amount;
				}
				else
				{
					dictionary.Add(item2.Key, new MaterialInventoryData(item2.Value));
				}
			}
		}
		return dictionary;
	}

	private bool GetPlayerAliveStatus(int index)
	{
		return !NetworkGameManager.Instance.GetPlayer(index).network.isDeadResult;
	}

	private void AddBonusMapItem(List<ItemToCraftMaterialConverter.ConvertMaterialItemData> convertMaterialItemList)
	{
		ItemToCraftMaterialConverter.ConvertMaterialItemData convertMaterialItemData = ItemToCraftMaterialConverter.ConvertItemToCraftMaterial(GetResultBonusItem());
		if (convertMaterialItemData != null)
		{
			convertMaterialItemList.Add(convertMaterialItemData);
		}
	}

	private int GetResultBonusItem()
	{
		return UIResultManager.Instance._resultMission.MissionObjective.MissionKeyItem;
	}

	private bool GetWinCondition()
	{
		return UIResultManager.Instance.WinCondition;
	}

	private void PlayAnimation()
	{
		Hide();
		Show();
	}
}
