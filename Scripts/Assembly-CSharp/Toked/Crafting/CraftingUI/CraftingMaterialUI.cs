using System;
using DG.Tweening;
using Doozy.Runtime.UIManager.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Toked.Crafting.CraftingUI;

public class CraftingMaterialUI : MonoBehaviour
{
	[SerializeField]
	private CraftMaterialScriptableObject _craftingMaterialSo;

	[SerializeField]
	private UISelectable _uiSelectable;

	[SerializeField]
	private Image _materialImage;

	[SerializeField]
	private TMP_Text _amountText;

	[SerializeField]
	private Image _hoverImage;

	[SerializeField]
	private Image _highlightImage;

	[SerializeField]
	private Color _hasIngredientsColor = Color.white;

	[SerializeField]
	private Color _noIngredientsColor = Color.red;

	private Action<CraftMaterialScriptableObject> onHoverUIEvent;

	private Action onUnHoverUIEvent;

	public CraftMaterialScriptableObject CraftingMaterialSo
	{
		get
		{
			return _craftingMaterialSo;
		}
		set
		{
			_craftingMaterialSo = value;
		}
	}

	public void Init(int amount = 0, bool withAnimation = false, bool hideUiIfZero = true)
	{
		_materialImage.sprite = _craftingMaterialSo.MaterialSprite;
		SetText(amount, withAnimation, hideUiIfZero);
	}

	public void Init(CraftMaterialScriptableObject materialScriptableObject, int amount = 0, bool hideUiIfZero = false)
	{
		_craftingMaterialSo = materialScriptableObject;
		Init(amount, withAnimation: false, hideUiIfZero);
	}

	public void SetSelectableUIEvent(Action<CraftMaterialScriptableObject> onHoverAction, Action onUnhoverAction)
	{
		onHoverUIEvent = onHoverAction;
		onUnHoverUIEvent = onUnhoverAction;
	}

	public void SetText(int amount, bool withAnimation = false, bool hideUiIfZero = false)
	{
		if (hideUiIfZero)
		{
			base.gameObject.SetActive(amount > 0);
		}
		else
		{
			base.gameObject.SetActive(value: true);
		}
		if (withAnimation)
		{
			int.TryParse(_amountText.text, out var amountScore);
			AudioManager.PlaySFX("materials-scrap");
			DOTween.To(() => amountScore, (int x) =>
			{
				amountScore = x;
			}, amount, 1f).OnUpdate(() =>
			{
				_amountText.text = amountScore.ToString();
			}).SetDelay(0.3f);
			_amountText.DOKill();
			_amountText.DOScale(1.5f, 1.2f).OnComplete(() =>
			{
				_amountText.DOKill();
				_amountText.DOScale(1f, 0.4f);
			});
		}
		else
		{
			_amountText.text = amount.ToString();
		}
	}

	public void Hover()
	{
		_hoverImage.gameObject.SetActive(value: true);
		onHoverUIEvent?.Invoke(_craftingMaterialSo);
	}

	public void UnHover()
	{
		_hoverImage.gameObject.SetActive(value: false);
		onUnHoverUIEvent?.Invoke();
	}

	public void Selected(bool hasIngredients)
	{
		SetColor(hasIngredients);
		_highlightImage.gameObject.SetActive(value: true);
	}

	public void Deselected()
	{
		ResetColor();
		_highlightImage.gameObject.SetActive(value: false);
	}

	private void SetColor(bool hasIngredients)
	{
		if (hasIngredients)
		{
			ResetColor();
			return;
		}
		_materialImage.color = _noIngredientsColor;
		_amountText.color = _noIngredientsColor;
	}

	private void ResetColor()
	{
		_materialImage.color = _hasIngredientsColor;
		_amountText.color = _hasIngredientsColor;
	}
}
