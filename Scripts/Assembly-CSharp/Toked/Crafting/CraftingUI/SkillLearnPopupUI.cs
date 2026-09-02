using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Doozy.Runtime.UIManager.Components;
using I2.Loc;
using TMPro;
using Toked.Skill;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Toked.Crafting.CraftingUI;

public class SkillLearnPopupUI : MonoBehaviour
{
	[SerializeField]
	private CraftingUIController _craftingUIController;

	[SerializeField]
	private RectTransform _panel;

	[SerializeField]
	private SkillLearnButton _skillButtonPrefab;

	[SerializeField]
	private RectTransform _contentPanel;

	[SerializeField]
	private TMP_Text _skillNameText;

	[SerializeField]
	private Localize _skillNameLocalize;

	[SerializeField]
	private TMP_Text _skillDescriptionText;

	[SerializeField]
	private Localize _skillDescriptionLocalize;

	[SerializeField]
	private List<SkillLearnButton> _skillLearnButtonList = new List<SkillLearnButton>();

	private int _lastActiveButtonIndex = 2;

	public bool IsShow => _panel.gameObject.activeSelf;

	public void Init(List<SkillScriptableObject> skillScriptableObjects, Action onClickAction = null)
	{
		CreateSkillButton(skillScriptableObjects, onClickAction);
		Show();
	}

	public void Show()
	{
		InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.DISABLE_PLAYER_INPUT, InputManager.inputActions.UI);
		EventSystem.current.SetSelectedGameObject(null);
		_panel.gameObject.SetActive(value: true);
		SelectedFirstButton();
	}

	public void Hide()
	{
		EventSystem.current.SetSelectedGameObject(null);
		_panel.gameObject.SetActive(value: false);
		_craftingUIController.Init();
		ResetText();
	}

	public void SelectedFirstButton()
	{
		if (GlobalOptionsManager.Instance.usingGamepad)
		{
			UniTaskUtil.DelayedCall(this, 0.2f, () =>
			{
				_skillLearnButtonList[0].SelectButton();
			}).Forget();
		}
	}

	private void OnClickAction(SkillLearnButton skillLearnButton)
	{
		Hide();
	}

	private void OnHoverAction(SkillLearnButton skillLearnButton)
	{
		_skillNameLocalize.SetTerm(skillLearnButton.SkillScriptableObject.SkillNameLocalizeId);
		_skillDescriptionLocalize.SetTerm(skillLearnButton.SkillScriptableObject.SkillDescriptionLocalizeId);
	}

	private void OnUnHoverAction(SkillLearnButton skillLearnButton)
	{
		ResetText();
	}

	private void ResetText()
	{
		_skillNameText.text = "";
		_skillDescriptionText.text = "";
		_skillNameLocalize.SetTerm("");
		_skillDescriptionLocalize.SetTerm("");
	}

	private void CreateSkillButton(List<SkillScriptableObject> skillScriptableObjects, Action onClickAction = null)
	{
		int count = _skillLearnButtonList.Count;
		int count2 = skillScriptableObjects.Count;
		bool flag = count < count2;
		int num = Mathf.Max(count, count2);
		_lastActiveButtonIndex = num - 1;
		for (int i = 0; i < num; i++)
		{
			if (flag)
			{
				SkillScriptableObject skillScriptableObject = skillScriptableObjects[i];
				SkillLearnButton skillLearnButton;
				if (i >= count)
				{
					skillLearnButton = UnityEngine.Object.Instantiate(_skillButtonPrefab, _contentPanel);
					_skillLearnButtonList.Add(skillLearnButton);
				}
				else
				{
					skillLearnButton = _skillLearnButtonList[i];
				}
				skillLearnButton.gameObject.SetActive(value: true);
				skillLearnButton.Init(skillScriptableObject, OnClickActionEvent, OnHoverAction, OnUnHoverAction);
			}
			else
			{
				SkillLearnButton skillLearnButton = _skillLearnButtonList[i];
				if (i < count2)
				{
					SkillScriptableObject skillScriptableObject = skillScriptableObjects[i];
					skillLearnButton.Init(skillScriptableObject, OnClickActionEvent, OnHoverAction, OnUnHoverAction);
					skillLearnButton.gameObject.SetActive(value: true);
				}
				else
				{
					skillLearnButton.gameObject.SetActive(value: false);
					skillLearnButton.Reset();
				}
			}
		}
		SetNavigation();
		void OnClickActionEvent(SkillLearnButton skillLearnButton2)
		{
			onClickAction?.Invoke();
			OnClickAction(skillLearnButton2);
		}
	}

	private void SetNavigation()
	{
		for (int i = 0; i < _skillLearnButtonList.Count; i++)
		{
			int num = i - 1;
			UIButton selectOnLeft = ((num < 0) ? _skillLearnButtonList[_lastActiveButtonIndex].UIButton : _skillLearnButtonList[num].UIButton);
			UIButton uIButton = _skillLearnButtonList[i].UIButton;
			int num2 = i + 1;
			UIButton selectOnRight = ((num2 > _lastActiveButtonIndex) ? _skillLearnButtonList[0].UIButton : _skillLearnButtonList[num2].UIButton);
			uIButton.navigation = new Navigation
			{
				mode = Navigation.Mode.Explicit,
				selectOnDown = null,
				selectOnLeft = selectOnLeft,
				selectOnRight = selectOnRight,
				selectOnUp = null
			};
		}
	}
}
