using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Toked.Skill;

public class PerkLearnPopupUI : GenericSingleton<PerkLearnPopupUI>
{
	[SerializeField]
	private UIView _uiview;

	[SerializeField]
	private RectTransform _panel;

	[SerializeField]
	private PerkLearnButton _perkButtonPrefab;

	[SerializeField]
	private RectTransform _contentPanel;

	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private HorizontalLayoutGroup _layoutPerkGroup;

	[SerializeField]
	private List<PerkLearnButton> _perkLearnButtonList = new List<PerkLearnButton>();

	[SerializeField]
	private int _chooseOptionPerk = 3;

	[SerializeField]
	private RoomCode _roomCode;

	private int _lastActiveButtonIndex = 2;

	private Action OnHideEvent;

	private bool _initOnDeviceChanged;

	public bool IsShow => _panel.gameObject.activeSelf;

	public void Init(Action onHideAction = null)
	{
		CreateSkillButton(GetRandomPerks());
		Show();
		OnHideEvent = onHideAction;
	}

	public void Show()
	{
		_layoutPerkGroup.spacing = -282f;
		DOTween.To(() => _layoutPerkGroup.spacing, (float x) =>
		{
			_layoutPerkGroup.spacing = x;
		}, -20f, 0.5f);
		_canvasGroup.interactable = false;
		InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.DISABLE_PLAYER_INPUT, InputManager.inputActions.UI);
		EventSystem.current.SetSelectedGameObject(null);
		_panel.gameObject.SetActive(value: true);
		_uiview.Show();
		UniTaskUtil.DelayedCall(this, 0.6f, () =>
		{
			_canvasGroup.interactable = true;
			SelectedFirstButton();
		}).Forget();
		_roomCode.enabled = true;
		InitOnDeviceChanged();
	}

	public void Hide()
	{
		RemoveOnDeviceChanged();
		EventSystem.current.SetSelectedGameObject(null);
		_uiview.Hide();
		_panel.gameObject.SetActive(value: false);
		InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.ENABLE_PLAYER_INPUT, InputManager.inputActions.UI, InputManager.inputActions.Player);
		OnHideEvent?.Invoke();
		OnHideEvent = null;
		_roomCode.enabled = false;
	}

	public void SelectedFirstButton()
	{
		if (GlobalOptionsManager.Instance.usingGamepad)
		{
			UniTaskUtil.DelayedCall(this, 0.2f, () =>
			{
				_perkLearnButtonList[0].SelectButton();
			}).Forget();
		}
	}

	private void OnClickAction(PerkLearnButton skillLearnButton)
	{
		Hide();
	}

	private void OnHoverAction(PerkLearnButton skillLearnButton)
	{
	}

	private void OnUnHoverAction(PerkLearnButton skillLearnButton)
	{
	}

	private void OnDestroy()
	{
		RemoveOnDeviceChanged();
	}

	private void InitOnDeviceChanged()
	{
		if (!_initOnDeviceChanged)
		{
			GlobalOptionsManager.OnDeviceChangedEvent += OnDeviceChanged;
			_initOnDeviceChanged = true;
		}
	}

	private void RemoveOnDeviceChanged()
	{
		if (_initOnDeviceChanged)
		{
			GlobalOptionsManager.OnDeviceChangedEvent -= OnDeviceChanged;
			_initOnDeviceChanged = false;
		}
	}

	private void OnDeviceChanged(GlobalOptionsManager manager)
	{
		if (manager.usingGamepad && _uiview.isVisible)
		{
			SelectedFirstButton();
		}
	}

	private void CreateSkillButton(List<SkillScriptableObject> skillScriptableObjects)
	{
		int count = _perkLearnButtonList.Count;
		int count2 = skillScriptableObjects.Count;
		bool flag = count < count2;
		int num = Mathf.Max(count, count2);
		_lastActiveButtonIndex = count2 - 1;
		for (int i = 0; i < num; i++)
		{
			if (flag)
			{
				SkillScriptableObject skillScriptableObject = skillScriptableObjects[i];
				PerkLearnButton perkLearnButton;
				if (i >= count)
				{
					perkLearnButton = UnityEngine.Object.Instantiate(_perkButtonPrefab, _contentPanel);
					_perkLearnButtonList.Add(perkLearnButton);
				}
				else
				{
					perkLearnButton = _perkLearnButtonList[i];
				}
				perkLearnButton.gameObject.SetActive(value: true);
				perkLearnButton.Init(skillScriptableObject, OnClickAction, OnHoverAction, OnUnHoverAction);
			}
			else
			{
				PerkLearnButton perkLearnButton = _perkLearnButtonList[i];
				if (i < count2)
				{
					SkillScriptableObject skillScriptableObject = skillScriptableObjects[i];
					perkLearnButton.Init(skillScriptableObject, OnClickAction, OnHoverAction, OnUnHoverAction);
					perkLearnButton.gameObject.SetActive(value: true);
				}
				else
				{
					perkLearnButton.gameObject.SetActive(value: false);
					perkLearnButton.Reset();
				}
			}
		}
		SetNavigation();
	}

	private void SetNavigation()
	{
		for (int i = 0; i < _perkLearnButtonList.Count; i++)
		{
			int num = i - 1;
			UIButton selectOnLeft = ((num < 0) ? _perkLearnButtonList[_lastActiveButtonIndex].UIButton : _perkLearnButtonList[num].UIButton);
			UIButton uIButton = _perkLearnButtonList[i].UIButton;
			int num2 = i + 1;
			UIButton selectOnRight = ((num2 > _lastActiveButtonIndex) ? _perkLearnButtonList[0].UIButton : _perkLearnButtonList[num2].UIButton);
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

	private List<SkillScriptableObject> GetRandomPerks()
	{
		List<SkillScriptableObject> list = new List<SkillScriptableObject>(DataManager.Instance.Get<PerkLibraryScriptableObject>().DataList);
		list.Shuffle();
		List<SkillScriptableObject> list2 = new List<SkillScriptableObject>();
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			SkillScriptableObject skillScriptableObject = list[i];
			if (skillScriptableObject.CheckRequirementUnlock())
			{
				list2.Add(skillScriptableObject);
				num++;
			}
			if (num == _chooseOptionPerk)
			{
				break;
			}
		}
		return list2;
	}
}
