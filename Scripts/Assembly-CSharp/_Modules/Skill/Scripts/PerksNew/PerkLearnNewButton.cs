using System;
using Doozy.Runtime.UIManager.Components;
using TMPro;
using Toked;
using Toked.Skill;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Modules.Skill.Scripts.PerksNew;

public class PerkLearnNewButton : MonoBehaviour
{
	[SerializeField]
	private SkillScriptableObject _skillScriptableObject;

	[SerializeField]
	private UIButton _uiButton;

	[SerializeField]
	private Image _skillImage;

	[SerializeField]
	private Image _highLightImage;

	[SerializeField]
	private TMP_Text _playerNameText;

	[SerializeField]
	private Image _playerIconImage;

	private bool _init;

	private PerkSelectorManager _perkSelectorManager;

	public SkillScriptableObject SkillScriptableObject => _skillScriptableObject;

	public UIButton UIButton => _uiButton;

	public event Action<PerkLearnNewButton> OnClickEvents;

	public event Action<PerkLearnNewButton> OnHoverButtonEvents;

	public event Action<PerkLearnNewButton> OnUnhoverButtonEvents;

	private void Init()
	{
		_uiButton.onClickEvent.AddListener(OnClickButton);
		_uiButton.normalState.stateEvent.Event.AddListener(UnHoverAction);
		_uiButton.highlightedState.stateEvent.Event.AddListener(HoverAction);
		_uiButton.selectedState.stateEvent.Event.AddListener(HoverAction);
		_init = true;
	}

	public void Init(PerkSelectorManager perkSelectorManager, SkillScriptableObject skillScriptableObject, Action<PerkLearnNewButton> onClickAction, Action<PerkLearnNewButton> onHoverAction, Action<PerkLearnNewButton> onUnhoverAction)
	{
		if (!(skillScriptableObject == null))
		{
			_skillScriptableObject = skillScriptableObject;
			_skillImage.sprite = skillScriptableObject.SkillSprite;
			_skillImage.gameObject.SetActive(value: true);
			if (!_init)
			{
				Init();
			}
			OnClickEvents = onClickAction;
			OnHoverButtonEvents = onHoverAction;
			OnUnhoverButtonEvents = onUnhoverAction;
			_perkSelectorManager = perkSelectorManager;
			_perkSelectorManager.UpdatePerkUI(this);
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
		AudioManager.PlaySFX("ui_confirm");
		if (!_perkSelectorManager || !_perkSelectorManager.CheckIsTaken(_skillScriptableObject))
		{
			OnClickEvents?.Invoke(this);
		}
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

	public void SetPlayer(PlayerController player)
	{
		ResetPlayer();
		if (!(player == null))
		{
			_playerIconImage.sprite = player.data.PlayerSkinData.GetHeadSkinMiniAvatar();
			_playerNameText.text = player.network.GetPlayerName();
			_playerIconImage.gameObject.SetActive(value: true);
			_uiButton.interactable = false;
		}
	}

	public void ResetPlayer()
	{
		_playerIconImage.gameObject.SetActive(value: false);
		_playerNameText.text = "";
		_uiButton.interactable = true;
	}
}
