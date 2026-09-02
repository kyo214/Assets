using System;
using DG.Tweening;
using Doozy.Runtime.UIManager.Components;
using I2.Loc;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Modules.CharacterSkin.Scripts;

public class SkinChangerPanelUI : MonoBehaviour
{
	[SerializeField]
	private UISelectable _uiSelectable;

	[SerializeField]
	private UIButton _prevButton;

	[SerializeField]
	private UIButton _nextButton;

	[SerializeField]
	private TMP_Text _valueText;

	[SerializeField]
	private Localize _valueLocalize;

	[SerializeField]
	private Image _valueImage;

	[SerializeField]
	private Image _highlightImage;

	private bool _isSelected;

	private Action _onDeselectEvent;

	public void InitButton(Action prevAction, Action nextAction, Action onDeselectedAction)
	{
		_prevButton.onClickEvent.AddListener(() =>
		{
			prevAction();
		});
		_nextButton.onClickEvent.AddListener(() =>
		{
			nextAction();
		});
		_onDeselectEvent = onDeselectedAction;
	}

	public void UpdateFunction()
	{
		if (_isSelected)
		{
			if (InputManager.inputActions.UI.Navigate.WasPressedThisFrame() && InputManager.inputActions.UI.Navigate.ReadValue<Vector2>().x > 0.9f)
			{
				InvokeNextButton();
			}
			else if (InputManager.inputActions.UI.Navigate.WasPressedThisFrame() && InputManager.inputActions.UI.Navigate.ReadValue<Vector2>().x < -0.9f)
			{
				InvokePrevButton();
			}
		}
	}

	public void SetLocalizeTerm(string term)
	{
		SetValueText("");
		if (!(_valueLocalize == null))
		{
			_valueLocalize.SetTerm(term);
		}
	}

	public void ChangeTextColor(Color color)
	{
		if (!(_valueText == null))
		{
			_valueText.DOColor(color, 0f);
		}
	}

	public void SetValueText(string text)
	{
		if (!(_valueText == null))
		{
			_valueText.text = text;
		}
	}

	public void SetValueImage(Color color)
	{
		if (!(_valueImage == null))
		{
			_valueImage.color = color;
		}
	}

	public void OnHoverPanel()
	{
		_onDeselectEvent?.Invoke();
		_highlightImage.gameObject.SetActive(value: true);
		_isSelected = true;
	}

	public void OnUnHoverPanel()
	{
		_highlightImage.gameObject.SetActive(value: false);
		_isSelected = false;
	}

	public void Selected()
	{
		EventSystem.current.SetSelectedGameObject(null);
		_uiSelectable.Select();
	}

	public void InvokePrevButton()
	{
		_prevButton.onClickEvent.Invoke();
	}

	public void InvokeNextButton()
	{
		_nextButton.onClickEvent.Invoke();
	}
}
