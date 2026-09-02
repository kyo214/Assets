using Toked.Crafting;
using UnityEngine;
using UnityEngine.UI;

namespace _Modules.Skill.Scripts.PerksNew;

public class ItemSlotInventoryPerkUI : MonoBehaviour
{
	[SerializeField]
	private ItemScriptableObject _itemScriptableObject;

	[SerializeField]
	private Image _itemImage;

	[SerializeField]
	private Button _button;

	public void Init(ItemScriptableObject itemScriptableObject)
	{
		_itemImage.gameObject.SetActive(value: false);
		if (!(itemScriptableObject == null))
		{
			_itemScriptableObject = itemScriptableObject;
			_itemImage.sprite = itemScriptableObject.ItemSprite;
			_itemImage.gameObject.SetActive(_itemImage.sprite);
		}
	}

	public void Reset()
	{
		_itemImage.gameObject.SetActive(value: false);
		_itemImage.sprite = null;
		_itemScriptableObject = null;
	}
}
