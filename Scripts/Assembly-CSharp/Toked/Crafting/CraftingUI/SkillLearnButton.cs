using System;
using Doozy.Runtime.UIManager.Components;
using Toked.Skill;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Toked.Crafting.CraftingUI;

public class SkillLearnButton : MonoBehaviour
{
	[SerializeField]
	private SkillScriptableObject _skillScriptableObject;

	[SerializeField]
	private UIButton _uiButton;

	[SerializeField]
	private Image _skillImage;

	[SerializeField]
	private Image _highLightImage;

	private bool _init;

	public SkillScriptableObject SkillScriptableObject => _skillScriptableObject;

	public UIButton UIButton => _uiButton;

	public event Action<SkillLearnButton> OnClickEvents;

	public event Action<SkillLearnButton> OnHoverButtonEvents;

	public event Action<SkillLearnButton> OnUnhoverButtonEvents;

	private void Init()
	{
		_uiButton.onClickEvent.AddListener(OnClickButton);
		_uiButton.normalState.stateEvent.Event.AddListener(UnHoverAction);
		_uiButton.highlightedState.stateEvent.Event.AddListener(HoverAction);
		_uiButton.selectedState.stateEvent.Event.AddListener(HoverAction);
		_init = true;
	}

	public void Init(SkillScriptableObject skillScriptableObject, Action<SkillLearnButton> onClickAction, Action<SkillLearnButton> onHoverAction, Action<SkillLearnButton> onUnhoverAction)
	{
		if (!(skillScriptableObject == null))
		{
			if (!_init)
			{
				Init();
			}
			_skillScriptableObject = skillScriptableObject;
			_skillImage.sprite = skillScriptableObject.SkillSprite;
			OnClickEvents = onClickAction;
			OnHoverButtonEvents = onHoverAction;
			OnUnhoverButtonEvents = onUnhoverAction;
		}
	}

	public void Reset()
	{
		_skillScriptableObject = null;
		_skillImage.sprite = null;
		OnClickEvents = null;
		OnHoverButtonEvents = null;
		OnUnhoverButtonEvents = null;
	}

	private void OnClickButton()
	{
		_skillScriptableObject?.ExecuteEffectSkill(NetworkGameManager.Instance.ownPlayer);
		AudioManager.PlaySFX("ui_confirm");
		OnClickEvents?.Invoke(this);
	}

	private void UnHoverAction()
	{
		_highLightImage.gameObject.SetActive(value: false);
		OnUnhoverButtonEvents?.Invoke(this);
	}

	private void HoverAction()
	{
		_highLightImage.gameObject.SetActive(value: true);
		AudioManager.PlaySFX("ui_select");
		OnHoverButtonEvents?.Invoke(this);
	}

	public void SelectButton()
	{
		if (EventSystem.current.currentSelectedGameObject == _uiButton.gameObject)
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
		_uiButton.Select();
	}
}
