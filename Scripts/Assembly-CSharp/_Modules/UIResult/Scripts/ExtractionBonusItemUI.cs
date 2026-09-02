using I2.Loc;
using TMPro;
using Toked.Crafting;
using UnityEngine;
using UnityEngine.UI;

namespace _Modules.UIResult.Scripts;

public class ExtractionBonusItemUI : MonoBehaviour
{
	[SerializeField]
	private RectTransform _rectTransform;

	[SerializeField]
	private TMP_Text _itemNameText;

	[SerializeField]
	private Localize _itemNameLocalize;

	[SerializeField]
	private Image _itemImage;

	[SerializeField]
	private UIMaterialResultPanel _uiMaterialResultPanel;

	public RectTransform RectTransform => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());

	public void Init(ItemToCraftMaterialConverter.ConvertMaterialItemData convertMaterialItemData)
	{
		ItemScriptableObject itemData = DataManager.Instance.GetItemData(convertMaterialItemData.InventoryObject.ID.ToString());
		_itemNameText.text = "";
		_itemNameLocalize.SetTerm(convertMaterialItemData.InventoryObject.ItemType + "/" + convertMaterialItemData.InventoryObject.ItemType + convertMaterialItemData.InventoryObject.ID);
		_itemImage.sprite = itemData?.ItemSprite;
		_uiMaterialResultPanel.Set(convertMaterialItemData.Material);
	}
}
