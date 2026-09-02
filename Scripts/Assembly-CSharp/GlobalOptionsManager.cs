using System;
using System.Collections.Generic;
using I2.Loc;
using Toked;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.Rendering;

public class GlobalOptionsManager : MonoBehaviour
{
	public int seed;

	public int screenSelect;

	public string[] shakelevel = new string[4] { "Off", "Low", "Normal", "High" };

	public string[] qualityLevel = new string[3] { "Low", "Normal", "High" };

	public string[] fixedAutoLevel = new string[2] { "Fixed", "Auto" };

	public string[] OnOff = new string[2] { "Off", "On" };

	public int[] ListFPS = new int[8] { 24, 30, 40, 60, 90, 120, 144, 360 };

	public bool usingWeaponSelect;

	public bool enableVOItem = true;

	public bool usingGamepad;

	public bool fixedGamepadType;

	public bool minimapAutoRotation;

	public GamepadType defaultGamepadType;

	public GamepadType currentGamepadType;

	public List<KeyButtonInfo> arrKeyButtonInfo = new List<KeyButtonInfo>();

	public int bindingIndex;

	public int variantScene = -1;

	public int gameScene = 1;

	public DateTime UtcDateTime;

	public DateTime PdtDateTime;

	public DateTime LocalDateTime;

	[SerializeField]
	private InputActionReference leftTab;

	[SerializeField]
	private InputActionReference rightTab;

	[SerializeField]
	private InputActionReference inventoryAction;

	[SerializeField]
	private InputActionReference throwAction;

	[SerializeField]
	private InputActionReference healAction;

	[SerializeField]
	private InputActionReference tabKillAction;

	[SerializeField]
	private InputActionReference interactionAction;

	[SerializeField]
	private InputActionReference readyAction;

	[SerializeField]
	private InputActionReference cancelAction;

	[SerializeField]
	private InputActionReference moveAction;

	[SerializeField]
	private InputActionReference copyAction;

	[SerializeField]
	private InputActionReference showCodeAction;

	[SerializeField]
	private InputActionReference voiceAction;

	[SerializeField]
	private InputActionReference shootAction;

	[SerializeField]
	private InputActionReference aimModeAction;

	[SerializeField]
	private InputActionReference rotateLeftAction;

	[SerializeField]
	private InputActionReference rotateRightAction;

	[SerializeField]
	private InputActionReference openMapAction;

	[SerializeField]
	private InputActionReference openMenuAction;

	[SerializeField]
	private InputActionReference dashAction;

	[SerializeField]
	private InputActionReference runAction;

	[SerializeField]
	private InputActionReference reloadAction;

	[SerializeField]
	private InputActionReference movementKeysAction;

	[SerializeField]
	private InputActionReference changeWeapon;

	[SerializeField]
	private InputActionReference chatWheel;

	[SerializeField]
	private InputActionReference combineItemAction;

	[SerializeField]
	private InputActionReference dropItemAction;

	[SerializeField]
	private InputActionReference leaderboardAction;

	public string cancelName;

	public string upName;

	public string downName;

	public string leftName;

	public string rightName;

	public string leftTabActionName;

	public string rightTabActionName;

	public string inventoryActionName;

	public string throwActionName;

	public string healActionName;

	public string tabKillActionName;

	public string interactionActionName;

	public string readActionName;

	public string copyActionName;

	public string showCodeActionName;

	public string openMapActionName;

	public string voiceActionName;

	public string changeWeaponActionName;

	public string chatWheelActionName;

	public string microphoneName;

	public string AttackActionName;

	public string AimActionName;

	public string RotateLeftActionName;

	public string RotateRightActionName;

	public string ReloadActionName;

	public string SprintActionName;

	public string DashActionName;

	public string DropItemActionName;

	public string CombineItemActionName;

	public string LeaderboardName;

	private static readonly Dictionary<string, string> PlayStationMapping = new Dictionary<string, string>
	{
		{ "Left Trigger", "L2" },
		{ "Right Trigger", "R2" },
		{ "Left Shoulder", "L1" },
		{ "Right Shoulder", "R1" },
		{ "Button South", "X" },
		{ "Button East", "O" },
		{ "Button West", "*Square" },
		{ "Button North", "*Triangle" },
		{ "Left Stick Press", "L3" },
		{ "Right Stick Press", "R3" },
		{ "Left Stick", "*Left Stick" },
		{ "Right Stick", "*Right Stick" },
		{ "Start", "Options" },
		{ "Select", "Share" }
	};

	private static readonly Dictionary<string, string> XboxMapping = new Dictionary<string, string>
	{
		{ "Left Trigger", "LT" },
		{ "Right Trigger", "RT" },
		{ "Left Shoulder", "LB" },
		{ "Right Shoulder", "RB" },
		{ "Button South", "A" },
		{ "Button East", "B" },
		{ "Button West", "X" },
		{ "Button North", "Y" },
		{ "Left Stick Press", "LS" },
		{ "Right Stick Press", "RS" },
		{ "Left Stick", "*Left Stick" },
		{ "Right Stick", "*Right Stick" },
		{ "Start", "Menu" },
		{ "Select", "View" }
	};

	private static readonly Dictionary<string, string> SwitchMapping = new Dictionary<string, string>
	{
		{ "Left Trigger", "ZL" },
		{ "Right Trigger", "ZR" },
		{ "Left Shoulder", "L" },
		{ "Right Shoulder", "R" },
		{ "Button South", "B" },
		{ "Button East", "A" },
		{ "Button West", "X" },
		{ "Button North", "Y" },
		{ "Left Stick Press", "LS" },
		{ "Right Stick Press", "RS" },
		{ "Left Stick", "*Left Stick" },
		{ "Right Stick", "*Right Stick" },
		{ "Start", "+" },
		{ "Select", "-" }
	};

	private readonly Dictionary<string, string> _regionNameList = new Dictionary<string, string>
	{
		{ "asia", "Southeast Asia" },
		{ "jp", "Japan" },
		{ "kr", "South Korea" },
		{ "eu", "Europe" },
		{ "us", "USA East" },
		{ "usw", "USA West" },
		{ "sa", "South America" }
	};

	private string currentScheme;

	public float lastCheck;

	public bool IsDisableDeviceChange;

	public static GlobalOptionsManager Instance { get; private set; }

	public static event Action<GlobalOptionsManager> OnDeviceChangedEvent;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		DebugManager.instance.enableRuntimeUI = false;
		Instance = this;
		QualitySettings.vSyncCount = 0;
	}

	private void Start()
	{
		AudioManager.ChangeVolumeMaster((float)GlobalSaveData.instance.optionData.volMaster / 100f);
		AudioManager.ChangeVolumeSFX((float)GlobalSaveData.instance.optionData.volSFX / 100f);
		AudioManager.ChangeVolumeBGM((float)GlobalSaveData.instance.optionData.volMusic / 100f);
		GetCurrentTime();
	}

	public void GetCurrentTime()
	{
		UtcDateTime = DateTime.Now.ToUniversalTime();
		LocalDateTime = UtcDateTime.ToLocalTime();
		TimeZoneInfo destinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
		PdtDateTime = TimeZoneInfo.ConvertTimeFromUtc(UtcDateTime, destinationTimeZone);
		Debug.Log("UTC Date Time : " + UtcDateTime);
		GameModes.Instance.ApplyModifier();
	}

	public void InitUI()
	{
		OptionsManager.Instance.txtTermShakeLevel.SetTerm("Menu/" + shakelevel[GlobalSaveData.instance.optionData.shakeLevel]);
		OptionsManager.Instance.txtVolMusic.text = GlobalSaveData.instance.optionData.volMusic.ToString();
		OptionsManager.Instance.txtVolSFX.text = GlobalSaveData.instance.optionData.volSFX.ToString();
		OptionsManager.Instance.txtVolVoice.text = GlobalSaveData.instance.optionData.volVoice.ToString();
		OptionsManager.Instance.txtVolAmbient.text = GlobalSaveData.instance.optionData.volAmbient.ToString();
	}

	public string ConvertInputName(string inputName)
	{
		string text = "";
		text = ((currentGamepadType == GamepadType.Playstation) ? PlayStationMapping.GetValueOrDefault(inputName, inputName) : ((currentGamepadType != GamepadType.Switch) ? XboxMapping.GetValueOrDefault(inputName, inputName) : SwitchMapping.GetValueOrDefault(inputName, inputName)));
		if (text.Contains("*"))
		{
			text = text.Replace("*", "");
			text = LocalizationManager.GetTranslation("Controls/" + text);
		}
		return text;
	}

	public void ChangeLang(int value)
	{
		GlobalSaveData.instance.optionData.lang = GlobalSaveData.instance.arrLang[value].LangCode;
		LocalizationManager.CurrentLanguageCode = GlobalSaveData.instance.optionData.lang;
	}

	public string GetLangName()
	{
		string result = "";
		for (int i = 0; i < GlobalSaveData.instance.arrLang.Count; i++)
		{
			if (GlobalSaveData.instance.optionData.lang == GlobalSaveData.instance.arrLang[i].LangCode)
			{
				result = GlobalSaveData.instance.arrLang[i].LangName;
			}
		}
		return result;
	}

	public void ChangeShakeLevel(LevelOptionsShake value)
	{
		GlobalSaveData.instance.optionData.shakeLevel = (byte)value;
	}

	public void ChangeVolBGM(byte value)
	{
		GlobalSaveData.instance.optionData.volMusic = value;
		OptionsManager.Instance.txtVolMusic.text = GlobalSaveData.instance.optionData.volMusic.ToString();
		AudioManager.ChangeVolumeBGM((float)GlobalSaveData.instance.optionData.volMusic / 100f);
	}

	public void ChangeVolSFX(byte value)
	{
		GlobalSaveData.instance.optionData.volSFX = value;
		OptionsManager.Instance.txtVolSFX.text = GlobalSaveData.instance.optionData.volSFX.ToString();
		AudioManager.ChangeVolumeSFX((float)GlobalSaveData.instance.optionData.volSFX / 100f);
	}

	public void ChangeVolVoice(byte value)
	{
		GlobalSaveData.instance.optionData.volVoice = value;
		OptionsManager.Instance.txtVolVoice.text = GlobalSaveData.instance.optionData.volVoice.ToString();
		AudioManager.ChangeVolumeVoice((float)GlobalSaveData.instance.optionData.volVoice / 100f);
	}

	public void ChangeVolAmbient(byte value)
	{
		GlobalSaveData.instance.optionData.volAmbient = value;
		OptionsManager.Instance.txtVolAmbient.text = GlobalSaveData.instance.optionData.volAmbient.ToString();
		AudioManager.ChangeVolumeAmbient((float)GlobalSaveData.instance.optionData.volAmbient / 100f);
	}

	public void ChangeFullscreen(int value)
	{
		GlobalSaveData.instance.optionData.windowMode = value;
		switch (value)
		{
		case 0:
			GlobalSaveData.instance.optionData.fullscreen = true;
			break;
		case 2:
			GlobalSaveData.instance.optionData.fullscreen = false;
			break;
		}
	}

	private void Update()
	{
		if (Time.unscaledTime - lastCheck < 1f)
		{
			return;
		}
		if (Gamepad.current != null && (Gamepad.current.buttonSouth.wasPressedThisFrame || Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.3f))
		{
			if (!(currentScheme == "Gamepad") || !(currentScheme != "") || !usingGamepad)
			{
				SetScheme("Gamepad", Gamepad.current);
				lastCheck = Time.unscaledTime;
			}
		}
		else if (Keyboard.current != null && (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame) && (!(currentScheme == "Keyboard") || !(currentScheme != "")))
		{
			SetScheme("Keyboard", Keyboard.current, Mouse.current);
			lastCheck = Time.unscaledTime;
		}
	}

	public void SetScheme(string scheme, params InputDevice[] devices)
	{
		currentScheme = scheme;
		if ((bool)NetworkGameManager.Instance && (bool)NetworkGameManager.Instance.ownPlayer)
		{
			NetworkGameManager.Instance.ownPlayer.playerInput.SwitchCurrentControlScheme(scheme, devices);
		}
	}

	public void DeviceChange(PlayerInput myPlayerInput)
	{
		if (fixedGamepadType || IsDisableDeviceChange)
		{
			return;
		}
		if (usingGamepad)
		{
			if (Gamepad.current is DualShockGamepad || Gamepad.current is DualSenseGamepadHID)
			{
				currentGamepadType = GamepadType.Playstation;
			}
			else if (Gamepad.current is SwitchProControllerHID)
			{
				currentGamepadType = GamepadType.Switch;
			}
			else
			{
				currentGamepadType = GamepadType.Xbox;
			}
		}
		if (inventoryAction.action.controls.Count > 0)
		{
			bindingIndex = inventoryAction.action.GetBindingIndexForControl(inventoryAction.action.controls[0]);
		}
		inventoryActionName = InputControlPath.ToHumanReadableString(inventoryAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		throwActionName = InputControlPath.ToHumanReadableString(throwAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		healActionName = InputControlPath.ToHumanReadableString(healAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		tabKillActionName = InputControlPath.ToHumanReadableString(tabKillAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		interactionActionName = InputControlPath.ToHumanReadableString(interactionAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		readActionName = InputControlPath.ToHumanReadableString(readyAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		cancelName = InputControlPath.ToHumanReadableString(cancelAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		upName = InputControlPath.ToHumanReadableString(moveAction.action.bindings[1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		downName = InputControlPath.ToHumanReadableString(moveAction.action.bindings[2].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		leftName = InputControlPath.ToHumanReadableString(moveAction.action.bindings[3].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		rightName = InputControlPath.ToHumanReadableString(moveAction.action.bindings[4].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		copyActionName = InputControlPath.ToHumanReadableString(copyAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		showCodeActionName = InputControlPath.ToHumanReadableString(showCodeAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		openMapActionName = InputControlPath.ToHumanReadableString(openMapAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		voiceActionName = InputControlPath.ToHumanReadableString(voiceAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		changeWeaponActionName = InputControlPath.ToHumanReadableString(changeWeapon.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		chatWheelActionName = InputControlPath.ToHumanReadableString(chatWheel.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		leftTabActionName = InputControlPath.ToHumanReadableString(leftTab.action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		rightTabActionName = InputControlPath.ToHumanReadableString(rightTab.action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		AttackActionName = InputControlPath.ToHumanReadableString(shootAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		AimActionName = InputControlPath.ToHumanReadableString(aimModeAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		DashActionName = InputControlPath.ToHumanReadableString(dashAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		SprintActionName = InputControlPath.ToHumanReadableString(runAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		RotateLeftActionName = InputControlPath.ToHumanReadableString(rotateLeftAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		RotateRightActionName = InputControlPath.ToHumanReadableString(rotateRightAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		ReloadActionName = InputControlPath.ToHumanReadableString(reloadAction.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		CombineItemActionName = InputControlPath.ToHumanReadableString(combineItemAction.action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		DropItemActionName = InputControlPath.ToHumanReadableString(dropItemAction.action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		LeaderboardName = InputControlPath.ToHumanReadableString(leaderboardAction.action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
		if (cancelName == "Escape")
		{
			cancelName = "Esc";
		}
		if (bindingIndex == 0)
		{
			usingGamepad = false;
		}
		else
		{
			usingGamepad = true;
		}
		foreach (KeyButtonInfo item in arrKeyButtonInfo)
		{
			item.DeviceChange();
		}
		OnDeviceChangedEvent?.Invoke(this);
	}

	public InputActionReference GetActionInventory()
	{
		return inventoryAction;
	}

	public InputActionReference GetActionThrow()
	{
		return throwAction;
	}

	public InputActionReference GetActionHeal()
	{
		return healAction;
	}

	public InputActionReference GetActionTabKill()
	{
		return tabKillAction;
	}

	public InputActionReference GetActionInteract()
	{
		return interactionAction;
	}

	public InputActionReference GetActionReady()
	{
		return readyAction;
	}

	public InputActionReference GetActionShoot()
	{
		return shootAction;
	}

	public InputActionReference GetActionAim()
	{
		return aimModeAction;
	}

	public InputActionReference GetActionRotateLeft()
	{
		return rotateLeftAction;
	}

	public InputActionReference GetActionRotateRight()
	{
		return rotateRightAction;
	}

	public InputActionReference GetActionOpenMap()
	{
		return openMapAction;
	}

	public InputActionReference GetActionVoice()
	{
		return voiceAction;
	}

	public InputActionReference GetActionOpenMenu()
	{
		return openMenuAction;
	}

	public InputActionReference GetActionDash()
	{
		return dashAction;
	}

	public InputActionReference GetActionRun()
	{
		return runAction;
	}

	public InputActionReference GetActionReload()
	{
		return reloadAction;
	}

	public InputActionReference GetMovementKeyActions()
	{
		return movementKeysAction;
	}

	public InputActionReference GetChangeWeapon()
	{
		return changeWeapon;
	}

	public InputActionReference GetChatWheel()
	{
		return chatWheel;
	}

	public InputActionReference GetActionDropItem()
	{
		return dropItemAction;
	}

	public InputActionReference GetActionCombineItem()
	{
		return combineItemAction;
	}

	public InputActionReference GetActionLeaderboard()
	{
		return leaderboardAction;
	}

	public int GetSeedCombineWithMissionID()
	{
		int result = 0;
		if ((bool)GameManagerPhoton.Instance)
		{
			result = GameManagerPhoton.Instance.Mission + GameManagerPhoton.Instance.SeedPuzzle;
		}
		return result;
	}

	public string GetRegionName(string codeRegion)
	{
		return _regionNameList[codeRegion];
	}
}
