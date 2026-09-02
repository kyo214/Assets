using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.UI;

public class RebindUI : MonoBehaviour
{
	[Header("UI Component Factory")]
	[SerializeField]
	private GameObject _keyBindUIGroup;

	[SerializeField]
	private GameObject _resetKeyUIGroup;

	[Header("Component Reference")]
	[SerializeField]
	private Transform _scrollViewContent;

	[SerializeField]
	private RectTransform _selector;

	public bool PressAnyKeyDebug;

	private List<TextMeshProUGUI> _actionNameTexts;

	private List<Localize> _actionNameLocs;

	private List<TextMeshProUGUI> _actionBindTexts;

	private List<Image> _actionBindIcons;

	private List<TextMeshProUGUI> _actionBindNotice;

	private List<Animator> _iconAnimators;

	private PlayerInput _playerInput;

	private List<InputActionReference> _inputActionRefs;

	private InputActionRebindingExtensions.RebindingOperation _rebindOperation;

	private int _bindingIndex;

	private float _shiftHeight = 41f;

	private int _showedScrollIndex;

	private bool _elementInstantiated;

	private bool _validated;

	private string _gamepadLayout;

	private string SAVEJSON_PATH => GlobalSaveData.ROOT_PATH + "InputConfig.sav";

	private void OnEnable()
	{
		if ((bool)OptionsManager.Instance)
		{
			OptionsManager.Instance.isKeyRebinding = false;
		}
		_showedScrollIndex = 0;
		if (_validated)
		{
			LoadKeyBind();
		}
		if (!_elementInstantiated)
		{
			CreateOptionsElement();
		}
		if ((bool)GlobalOptionsManager.Instance && _elementInstantiated)
		{
			if (GlobalOptionsManager.Instance.bindingIndex == 0)
			{
				EnableCompositeButtons(enabled: true);
			}
			else
			{
				EnableCompositeButtons(enabled: false);
			}
		}
	}

	private void Start()
	{
		StartCoroutine(FirstTimeLoadKeyBind());
	}

	private IEnumerator FirstTimeLoadKeyBind()
	{
		yield return new WaitUntil(() => _validated);
		LoadKeyBind();
	}

	private void OnDisable()
	{
		_scrollViewContent.transform.DOKill();
	}

	private void OnDestroy()
	{
		_scrollViewContent.transform.DOKill();
	}

	private void LateUpdate()
	{
		if (!_elementInstantiated)
		{
			CreateOptionsElement();
		}
	}

	private void CreateOptionsElement()
	{
		if (!GlobalOptionsManager.Instance)
		{
			return;
		}
		ListInputBindings();
		if ((bool)UITitleMenuManager.Instance)
		{
			_playerInput = UITitleMenuManager.Instance.GetComponent<PlayerInput>();
		}
		else
		{
			if (!NetworkGameManager.Instance || !NetworkGameManager.Instance.ownPlayer)
			{
				return;
			}
			_playerInput = NetworkGameManager.Instance.ownPlayer.playerInput;
		}
		_actionNameTexts = new List<TextMeshProUGUI>();
		_actionNameLocs = new List<Localize>();
		_actionBindTexts = new List<TextMeshProUGUI>();
		_actionBindIcons = new List<Image>();
		_actionBindNotice = new List<TextMeshProUGUI>();
		_iconAnimators = new List<Animator>();
		GameObject gameObject = Object.Instantiate(_resetKeyUIGroup, base.transform);
		OptionsManager.Instance.AddToPosSelectionControl(gameObject.transform);
		gameObject.GetComponent<Button>().onClick.AddListener(() =>
		{
			ResetKeyBind();
		});
		gameObject.GetComponent<HoverHighlight>().SetIndex(_inputActionRefs.Count);
		for (int num = 0; num < _inputActionRefs.Count; num++)
		{
			GameObject gameObject2 = Object.Instantiate(_keyBindUIGroup, base.transform);
			OptionsManager.Instance.AddToPosSelectionControl(gameObject2.transform);
			_actionNameTexts.Add(gameObject2.transform.GetChild(0).GetComponent<TextMeshProUGUI>());
			_actionNameLocs.Add(gameObject2.transform.GetChild(0).GetComponent<Localize>());
			_actionBindTexts.Add(gameObject2.transform.GetChild(4).GetComponent<TextMeshProUGUI>());
			_actionBindIcons.Add(gameObject2.transform.GetChild(5).GetComponent<Image>());
			_iconAnimators.Add(gameObject2.transform.GetChild(5).GetComponent<Animator>());
			_actionBindNotice.Add(gameObject2.transform.GetChild(6).GetComponent<TextMeshProUGUI>());
			int uiIndex = num;
			gameObject2.GetComponent<Button>().onClick.AddListener(() =>
			{
				BeginRebind(uiIndex + 1);
			});
			gameObject2.GetComponent<HoverHighlight>().SetIndex(num);
		}
		if (_actionNameTexts.Count == _inputActionRefs.Count && _actionBindTexts.Count == _inputActionRefs.Count)
		{
			_validated = true;
		}
		_elementInstantiated = true;
	}

	private void ListInputBindings()
	{
		_inputActionRefs = new List<InputActionReference>
		{
			GlobalOptionsManager.Instance.GetMovementKeyActions(),
			GlobalOptionsManager.Instance.GetMovementKeyActions(),
			GlobalOptionsManager.Instance.GetMovementKeyActions(),
			GlobalOptionsManager.Instance.GetMovementKeyActions(),
			GlobalOptionsManager.Instance.GetActionInventory(),
			GlobalOptionsManager.Instance.GetActionThrow(),
			GlobalOptionsManager.Instance.GetActionHeal(),
			GlobalOptionsManager.Instance.GetActionInteract(),
			GlobalOptionsManager.Instance.GetActionShoot(),
			GlobalOptionsManager.Instance.GetActionAim(),
			GlobalOptionsManager.Instance.GetActionRotateLeft(),
			GlobalOptionsManager.Instance.GetActionRotateRight(),
			GlobalOptionsManager.Instance.GetActionOpenMap(),
			GlobalOptionsManager.Instance.GetActionDash(),
			GlobalOptionsManager.Instance.GetActionRun(),
			GlobalOptionsManager.Instance.GetActionReload(),
			GlobalOptionsManager.Instance.GetActionVoice(),
			GlobalOptionsManager.Instance.GetChatWheel()
		};
	}

	private void EnableCompositeButtons(bool enabled)
	{
		OptionsManager.Instance.ResetPosSelectionControl();
		for (int i = 1; i < base.transform.childCount; i++)
		{
			if (i >= 2 && i <= 5)
			{
				base.transform.GetChild(i).gameObject.SetActive(enabled);
				if (base.transform.GetChild(i).gameObject.activeSelf)
				{
					OptionsManager.Instance.AddToPosSelectionControl(base.transform.GetChild(i));
				}
			}
			else
			{
				OptionsManager.Instance.AddToPosSelectionControl(base.transform.GetChild(i));
			}
		}
		if ((bool)OptionsManager.Instance.selector)
		{
			OptionsManager.Instance.selector.transform.position = new Vector2(OptionsManager.Instance.selector.transform.position.x, 400f);
		}
	}

	private void RefreshKeyBind(bool atStart)
	{
		if (atStart)
		{
			_bindingIndex = GlobalOptionsManager.Instance.bindingIndex;
		}
		int num = 0;
		string[] array = new string[4] { "Up", "Down", "Left", "Right" };
		for (int i = 0; i < _inputActionRefs.Count; i++)
		{
			string text = _inputActionRefs[i].action.name;
			string text2 = "_";
			if (_bindingIndex < _inputActionRefs[i].action.bindings.Count)
			{
				if (_inputActionRefs[i].action.bindings[0].isComposite)
				{
					text = text + " " + array[num];
					num++;
					text2 = InputControlPath.ToHumanReadableString(_inputActionRefs[i].action.bindings[num].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
				}
				else
				{
					text2 = InputControlPath.ToHumanReadableString(_inputActionRefs[i].action.bindings[_bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
				}
			}
			_actionNameLocs[i].SetTerm("Menu/" + text);
			_actionBindTexts[i].text = text2;
			if (Gamepad.current is DualShockGamepad)
			{
				_gamepadLayout = " 0";
			}
			else
			{
				_gamepadLayout = "";
			}
			bool flag = true;
			if (_bindingIndex < 1)
			{
				flag = false;
			}
			if (text2 == "_")
			{
				flag = false;
			}
			if (flag)
			{
				_actionBindIcons[i].gameObject.SetActive(value: true);
				text2 += _gamepadLayout;
				if (!_iconAnimators[i].HasState(0, Animator.StringToHash(text2)))
				{
					flag = false;
				}
			}
			if (flag)
			{
				_actionBindTexts[i].gameObject.SetActive(value: false);
				_actionBindIcons[i].gameObject.SetActive(value: true);
				_iconAnimators[i].CrossFade(text2, 0f);
			}
			else
			{
				_actionBindIcons[i].gameObject.SetActive(value: false);
				_actionBindTexts[i].gameObject.SetActive(value: true);
			}
			_actionBindNotice[i].gameObject.SetActive(value: false);
		}
		GlobalOptionsManager.Instance.IsDisableDeviceChange = false;
		if (GlobalOptionsManager.Instance.usingGamepad)
		{
			GlobalOptionsManager.Instance.SetScheme("Gamepad", Gamepad.current);
		}
		else
		{
			GlobalOptionsManager.Instance.SetScheme("Keyboard", Keyboard.current, Mouse.current);
		}
	}

	public void BeginRebind(int actionIndex)
	{
		if (actionIndex == 0)
		{
			ResetKeyBind();
			return;
		}
		GlobalOptionsManager.Instance.IsDisableDeviceChange = true;
		actionIndex--;
		_playerInput.enabled = false;
		_bindingIndex = GlobalOptionsManager.Instance.bindingIndex;
		string tempInputPath = _inputActionRefs[actionIndex].action.bindings[_bindingIndex].effectivePath;
		int bindingIndex = _bindingIndex;
		if (_inputActionRefs[actionIndex].action.bindings[_bindingIndex].isComposite)
		{
			bindingIndex = actionIndex + 1;
			tempInputPath = _inputActionRefs[actionIndex].action.bindings[bindingIndex].effectivePath;
		}
		PressAnyKeyDebug = true;
		_playerInput.actions.Disable();
		_rebindOperation = _inputActionRefs[actionIndex].action.PerformInteractiveRebinding(bindingIndex).WithControlsExcluding("<Pointer>/position").WithControlsExcluding("<Pointer>/delta")
			.WithControlsExcluding("<Gamepad>/leftStick")
			.WithControlsExcluding("<Gamepad>/rightStick")
			.WithControlsExcluding("<DualSenseGamepadHID>/rightTriggerButton")
			.WithControlsExcluding("<DualSenseGamepadHID>/leftTriggerButton")
			.WithCancelingThrough("<Keyboard>/escape")
			.OnMatchWaitForAnother(0.1f)
			.OnComplete((InputActionRebindingExtensions.RebindingOperation operation) =>
			{
				RebindComplete(actionIndex, tempInputPath, bindingIndex);
			})
			.OnCancel((InputActionRebindingExtensions.RebindingOperation operation) =>
			{
				CancelRebind(actionIndex);
			})
			.Start();
		_actionBindTexts[actionIndex].color = new Color(1f, 1f, 1f, 0f);
		_actionBindIcons[actionIndex].color = new Color(1f, 1f, 1f, 0f);
		_actionBindNotice[actionIndex].gameObject.SetActive(value: true);
		OptionsManager.Instance.isKeyRebinding = true;
	}

	private void CancelRebind(int actionIndex)
	{
		_actionBindTexts[actionIndex].color = Color.white;
		_actionBindIcons[actionIndex].color = Color.white;
		_actionBindNotice[actionIndex].gameObject.SetActive(value: false);
		_playerInput.enabled = true;
		_playerInput.actions.Enable();
		PressAnyKeyDebug = false;
		OptionsManager.Instance.isKeyRebinding = false;
	}

	private void RebindComplete(int actionIndex, string tempInputPath, int correctBindingIndex)
	{
		Debug.Log(tempInputPath);
		for (int i = 0; i < _inputActionRefs.Count; i++)
		{
			if (i != actionIndex && _bindingIndex < _inputActionRefs[i].action.bindings.Count)
			{
				InputBinding inputBinding = _inputActionRefs[i].action.bindings[_bindingIndex];
				int num = _bindingIndex;
				if (inputBinding.isComposite)
				{
					num = i + 1;
					inputBinding = _inputActionRefs[i].action.bindings[num];
				}
				InputBinding inputBinding2 = _inputActionRefs[actionIndex].action.bindings[correctBindingIndex];
				if (inputBinding.effectivePath == inputBinding2.effectivePath)
				{
					_inputActionRefs[i].action.ApplyBindingOverride(num, tempInputPath);
					break;
				}
			}
		}
		_rebindOperation.Dispose();
		_playerInput.enabled = true;
		_playerInput.actions.Enable();
		PressAnyKeyDebug = false;
		_actionBindTexts[actionIndex].color = Color.white;
		_actionBindIcons[actionIndex].color = Color.white;
		_actionBindNotice[actionIndex].gameObject.SetActive(value: false);
		SaveKeyBind();
		RefreshKeyBind(atStart: false);
		OptionsManager.Instance.isKeyRebinding = false;
		foreach (KeyButtonInfo item in GlobalOptionsManager.Instance.arrKeyButtonInfo)
		{
			item.DeviceChange();
		}
	}

	private void LoadKeyBind()
	{
		string text = "";
		try
		{
			text = ES3.LoadRawString(SAVEJSON_PATH);
		}
		catch
		{
			text = "";
		}
		if ((bool)_playerInput && !string.IsNullOrWhiteSpace(text) && !GameModes.Instance.isEvent)
		{
			_playerInput.actions.LoadBindingOverridesFromJson(text);
		}
		RefreshKeyBind(atStart: true);
	}

	private void SaveKeyBind()
	{
		if ((bool)_playerInput)
		{
			ES3.SaveRaw(_playerInput.actions.SaveBindingOverridesAsJson(), SAVEJSON_PATH);
		}
	}

	public void ShiftScroll(int selectedIndex, float yPos)
	{
		_scrollViewContent.transform.DOKill();
		int num = 7;
		if (selectedIndex > _showedScrollIndex + num)
		{
			_showedScrollIndex = selectedIndex - num;
		}
		else if (selectedIndex < _showedScrollIndex)
		{
			_showedScrollIndex = selectedIndex;
		}
		if (selectedIndex >= 0 && selectedIndex < _inputActionRefs.Count)
		{
			_scrollViewContent.transform.DOLocalMoveY(_shiftHeight * (float)_showedScrollIndex, 0.1f).OnComplete(() =>
			{
				SnapPos(yPos);
			});
		}
	}

	public void ResetKeyBind()
	{
		_playerInput.actions.RemoveAllBindingOverrides();
		SaveKeyBind();
		RefreshKeyBind(atStart: false);
	}

	public void SnapPos(float yPos)
	{
		_selector.localPosition = new Vector3(_selector.localPosition.x, yPos, _selector.localPosition.z);
	}
}
