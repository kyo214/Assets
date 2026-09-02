using System;
using Doozy.Runtime.UIManager.Components;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Toked.Skill;

public class PerkLearnButton : MonoBehaviour
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
	private TMP_Text _perkNameText;

	[SerializeField]
	private Localize _perkNameLocalize;

	[SerializeField]
	private LocalizationParamsManager _localizationParamsManager;

	[SerializeField]
	private TMP_Text _perkDescriptionText;

	[SerializeField]
	private Localize _perkDescriptionLocalize;

	private bool _init;

	public SkillScriptableObject SkillScriptableObject => _skillScriptableObject;

	public UIButton UIButton => _uiButton;

	public event Action<PerkLearnButton> OnClickEvents;

	public event Action<PerkLearnButton> OnHoverButtonEvents;

	public event Action<PerkLearnButton> OnUnhoverButtonEvents;

	private void Init()
	{
		_uiButton.onClickEvent.AddListener(OnClickButton);
		_uiButton.normalState.stateEvent.Event.AddListener(UnHoverAction);
		_uiButton.highlightedState.stateEvent.Event.AddListener(HoverAction);
		_uiButton.selectedState.stateEvent.Event.AddListener(HoverAction);
		_init = true;
	}

	public void Init(SkillScriptableObject skillScriptableObject, Action<PerkLearnButton> onClickAction, Action<PerkLearnButton> onHoverAction, Action<PerkLearnButton> onUnhoverAction)
	{
		if (!(skillScriptableObject == null))
		{
			if (!_init)
			{
				Init();
			}
			_skillScriptableObject = skillScriptableObject;
			_skillImage.sprite = skillScriptableObject.SkillSprite;
			_perkNameText.text = "";
			_perkDescriptionText.text = "";
			_perkNameLocalize.SetTerm(skillScriptableObject.SkillNameLocalizeId);
			_perkDescriptionLocalize.SetTerm(skillScriptableObject.SkillDescriptionLocalizeId);
			skillScriptableObject.SetStatsValueLocalization(_localizationParamsManager);
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
		NetworkGameManager.Instance.ownPlayer.data.SkillData.SetPerk(_skillScriptableObject.ID ?? "");
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
