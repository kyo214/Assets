using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Toked.Crafting.CraftingUI;

public class CraftingItemRequirementUI : MonoBehaviour
{
	[SerializeField]
	private Image _materialImage;

	[SerializeField]
	private TMP_Text _amountText;

	[SerializeField]
	private Color hasIngredientsColor = Color.white;

	[SerializeField]
	private Color noIngredientsColor = Color.red;

	public void Set(Sprite sprite, int amount)
	{
		_materialImage.sprite = sprite;
		_amountText.text = $"{amount}";
	}

	public void SetTextColor(bool hasIngredients)
	{
		if (hasIngredients)
		{
			_amountText.color = hasIngredientsColor;
			_materialImage.color = hasIngredientsColor;
		}
		else
		{
			_amountText.color = noIngredientsColor;
			_materialImage.color = noIngredientsColor;
		}
	}
}
