using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using I2.Loc;
using TMPro;
using Toked;
using Toked.Crafting;
using Toked.Skill;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using _Modules.Player.Data;

namespace _Modules.Skill.Scripts.PerksNew;

public class PerkLearnPopupNewUI : GenericSingleton<PerkLearnPopupNewUI>
{
	[SerializeField]
	private PerkSelectorManager _perkSelectorManager;

	[SerializeField]
	private UIView _uiview;

	[SerializeField]
	private RectTransform _panel;

	[SerializeField]
	private PerkLearnNewButton _perkButtonPrefab;

	[SerializeField]
	private RectTransform _contentPanel;

	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private HorizontalLayoutGroup _layoutPerkGroup;

	[SerializeField]
	private List<PerkLearnNewButton> _perkLearnButtonList = new List<PerkLearnNewButton>();

	[SerializeField]
	private RoomCode _roomCode;

	[SerializeField]
	private GameObject _perkDescriptionPanel;

	[SerializeField]
	private TMP_Text _perkNameText;

	[SerializeField]
	private Localize _perkNameLocalize;

	[SerializeField]
	private TMP_Text _perkHealthText;

	[SerializeField]
	private TMP_Text _perkStaminaText;

	[SerializeField]
	private LocalizationParamsManager _localizationParamsManager;

	[SerializeField]
	private TMP_Text _perkDescriptionText;

	[SerializeField]
	private Localize _perkDescriptionLocalize;

	[SerializeField]
	private InventoryPerkUI _inventoryPerkUI;

	[SerializeField]
	private Light _spotlightPerk;

	private int _lastActiveButtonIndex = 2;

	private Action OnHideEvent;

	private bool _initOnDeviceChanged;

	private bool _initOnPerkChanged;

	private bool _isShow;

	public PerkSelectorManager PerkSelectorManager => _perkSelectorManager;

	public bool IsShow => _panel.gameObject.activeSelf;

	public void Init(Action onHideAction = null)
	{
		CreateSkillButton(_perkSelectorManager.GetRandomPerks());
		Show();
		OnHideEvent = onHideAction;
	}

	public void Show()
	{
		ResetUI();
		float spacing = _layoutPerkGroup.spacing;
		_layoutPerkGroup.spacing = -100f;
		DOTween.To(() => _layoutPerkGroup.spacing, (float x) =>
		{
			_layoutPerkGroup.spacing = x;
		}, spacing, 0.5f);
		_canvasGroup.interactable = false;
		InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.DISABLE_PLAYER_INPUT, InputManager.inputActions.UI);
		EventSystem.current.SetSelectedGameObject(null);
		_panel.gameObject.SetActive(value: true);
		_uiview.Show();
		UniTaskUtil.DelayedCall(this, 0.6f, () =>
		{
			Vector3 position = NetworkGameManager.Instance.ownPlayer.transform.position;
			Vector3 position2 = new Vector3(position.x + 0.5f, 2f, position.z + 0.5f);
			_spotlightPerk.gameObject.SetActive(value: true);
			_spotlightPerk.gameObject.transform.position = position2;
			_canvasGroup.interactable = true;
			SelectedFirstButton();
		}).Forget();
		_roomCode.enabled = true;
		InitOnDeviceChanged();
		InitPerkChange();
		_isShow = true;
	}

	public void Hide()
	{
		_spotlightPerk.gameObject.SetActive(value: false);
		RemoveOnDeviceChanged();
		EventSystem.current.SetSelectedGameObject(null);
		_uiview.Hide();
		_panel.gameObject.SetActive(value: false);
		InputManager.ToggleActionMap(InputManager.PlayerInputToggleAction.ENABLE_PLAYER_INPUT, InputManager.inputActions.UI, InputManager.inputActions.Player);
		OnHideEvent?.Invoke();
		OnHideEvent = null;
		_roomCode.enabled = false;
		RemovePerkChange();
		_isShow = false;
		ResetButtonUI();
	}

	public void SelectedFirstButton()
	{
		UniTaskUtil.DelayedCall(this, 0.2f, () =>
		{
			GetFirstButton()?.SelectButton();
		}).Forget();
	}

	public PerkLearnNewButton GetFirstButton()
	{
		foreach (PerkLearnNewButton perkLearnButton in _perkLearnButtonList)
		{
			if (perkLearnButton != null && perkLearnButton.UIButton.interactable && perkLearnButton.gameObject.activeInHierarchy)
			{
				return perkLearnButton;
			}
		}
		return null;
	}

	private void OnClickAction(PerkLearnNewButton skillLearnButton)
	{
		SkillScriptableObject skillScriptableObject = skillLearnButton.SkillScriptableObject;
		if (!(skillScriptableObject == null))
		{
			PlayerController ownPlayer = NetworkGameManager.Instance.ownPlayer;
			ownPlayer.data.SkillData.SetPerk(skillScriptableObject.ID ?? "");
			skillScriptableObject?.ExecuteEffectSkill(ownPlayer);
			GlobalSaveData.instance.SaveGameData(ownPlayer, GameManagerPhoton.Instance);
			Hide();
		}
	}

	private void OnHoverAction(PerkLearnNewButton skillLearnButton)
	{
		_perkNameText.text = "";
		_perkDescriptionText.text = "";
		SkillScriptableObject skillScriptableObject = skillLearnButton.SkillScriptableObject;
		if (!(skillScriptableObject == null))
		{
			_inventoryPerkUI.Init(skillScriptableObject);
			_perkNameLocalize.SetTerm(skillScriptableObject.SkillNameLocalizeId);
			_perkDescriptionLocalize.SetTerm(skillScriptableObject.SkillDescriptionLocalizeId);
			int num = 100 + StaminaEffectValue.CalculateTotalValue(skillScriptableObject.GetEffectValues<StaminaEffectValue>());
			int num2 = 100 + HealthEffectValue.CalculateTotalValue(skillScriptableObject.GetEffectValues<HealthEffectValue>());
			_perkHealthText.text = $"{num2}/{num2}";
			_perkStaminaText.text = $"{num}/{num}";
			skillScriptableObject.SetStatsValueLocalization(_localizationParamsManager);
			_perkDescriptionPanel.gameObject.SetActive(value: true);
			_inventoryPerkUI.gameObject.SetActive(value: true);
		}
	}

	private void OnUnHoverAction(PerkLearnNewButton skillLearnButton)
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
				PerkLearnNewButton perkLearnNewButton;
				if (i >= count)
				{
					perkLearnNewButton = UnityEngine.Object.Instantiate(_perkButtonPrefab, _contentPanel);
					_perkLearnButtonList.Add(perkLearnNewButton);
				}
				else
				{
					perkLearnNewButton = _perkLearnButtonList[i];
				}
				perkLearnNewButton.gameObject.SetActive(value: true);
				perkLearnNewButton.Init(_perkSelectorManager, skillScriptableObject, OnClickAction, OnHoverAction, OnUnHoverAction);
			}
			else
			{
				PerkLearnNewButton perkLearnNewButton = _perkLearnButtonList[i];
				if (i < count2)
				{
					SkillScriptableObject skillScriptableObject = skillScriptableObjects[i];
					perkLearnNewButton.Init(_perkSelectorManager, skillScriptableObject, OnClickAction, OnHoverAction, OnUnHoverAction);
					perkLearnNewButton.gameObject.SetActive(value: true);
				}
				else
				{
					perkLearnNewButton.gameObject.SetActive(value: false);
					perkLearnNewButton.Reset();
				}
			}
		}
		SetNavigation();
	}

	private void SetNavigation()
	{
		int count = _perkLearnButtonList.Count;
		for (int i = 0; i < count; i++)
		{
			_perkLearnButtonList[i].UIButton.navigation = new Navigation
			{
				mode = Navigation.Mode.Explicit,
				selectOnLeft = GetInteractableButton(i, -1),
				selectOnRight = GetInteractableButton(i, 1),
				selectOnUp = null,
				selectOnDown = null
			};
		}
		UIButton GetInteractableButton(int startIndex, int direction)
		{
			int count2 = _perkLearnButtonList.Count;
			int num = startIndex;
			do
			{
				num = (num + direction + count2) % count2;
				UIButton uIButton = _perkLearnButtonList[num].UIButton;
				if (uIButton != null && uIButton.interactable && uIButton.gameObject.activeInHierarchy)
				{
					return uIButton;
				}
			}
			while (num != startIndex);
			return null;
		}
	}

	private void ResetUI()
	{
		_perkDescriptionPanel.gameObject.SetActive(value: false);
		_inventoryPerkUI.gameObject.SetActive(value: false);
		_inventoryPerkUI.InitSlot();
		_perkNameText.text = "";
		_perkDescriptionText.text = "";
		_perkNameLocalize.SetTerm(null);
		_perkDescriptionLocalize.SetTerm(null);
	}

	private void InitPerkChange()
	{
		PlayerSkillDataNetwork.OnPerkNetworkChangedEvent += OnPerkChangeAction;
		PlayerSkillDataNetwork.OnPerkDestroyEvent += OnPerkDestroyAction;
	}

	private void RemovePerkChange()
	{
		PlayerSkillDataNetwork.OnPerkNetworkChangedEvent -= OnPerkChangeAction;
		PlayerSkillDataNetwork.OnPerkDestroyEvent -= OnPerkDestroyAction;
	}

	private void OnPerkChangeAction(PlayerController playerController, string perkId)
	{
		if (_isShow)
		{
			GetPerkLearnNewButton(perkId)?.SetPlayer(playerController);
			SetNavigation();
		}
	}

	private void OnPerkDestroyAction(string perkId)
	{
		if (_isShow)
		{
			GetPerkLearnNewButton(perkId)?.ResetPlayer();
			SetNavigation();
		}
	}

	private void ResetButtonUI()
	{
		foreach (PerkLearnNewButton perkLearnButton in _perkLearnButtonList)
		{
			perkLearnButton.ResetPlayer();
		}
	}

	private PerkLearnNewButton GetPerkLearnNewButton(string perkId)
	{
		foreach (PerkLearnNewButton perkLearnButton in _perkLearnButtonList)
		{
			if ((bool)perkLearnButton && perkLearnButton.SkillScriptableObject?.ID == perkId)
			{
				return perkLearnButton;
			}
		}
		return null;
	}
}
