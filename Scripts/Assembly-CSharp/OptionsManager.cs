using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using I2.Loc;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class OptionsManager : MonoBehaviour
{
	public GameObject uiGameplay;

	public GameObject uiAudio;

	public GameObject uiDisplay;

	public GameObject uiControl;

	public GameObject uiControlTutorial;

	private UIView UIOptions;

	public Localize txtTermShakeLevel;

	public Localize txtTermIntroDialogue;

	public Localize txtTermIntroControl;

	[FormerlySerializedAs("txtTermEnableTutorial")]
	public Localize txtTermSkipTutorial;

	public TextMeshProUGUI txtVolMaster;

	public TextMeshProUGUI txtVolMusic;

	public TextMeshProUGUI txtVolSFX;

	public TextMeshProUGUI txtVolVoice;

	public TextMeshProUGUI txtVolAmbient;

	public Localize txtTermVoiceChat;

	public Localize txtSprintMode;

	public TextMeshProUGUI txtMicrophone;

	public Localize txtTermFullscreen;

	public Localize txtTermGraphic;

	public Localize txtTermVsync;

	public TextMeshProUGUI txtLimitFPS;

	public Localize txtTermMinimap;

	public TextMeshProUGUI txtResolution;

	public Localize txtTermShowFPS;

	public Localize txtTermShowChatLog;

	public bool isKeyRebinding;

	public int selectVertical;

	[SerializeField]
	public Transform selector;

	[SerializeField]
	private List<Transform> posSelectionGameplay = new List<Transform>();

	[SerializeField]
	private List<Transform> posSelectionAudio = new List<Transform>();

	[SerializeField]
	private List<Transform> posSelectionDisplay = new List<Transform>();

	private List<Transform> posSelectionControl = new List<Transform>();

	[SerializeField]
	private UIButton btnBack;

	public UIButton btnGameplay;

	public UIButton btnAudio;

	public UIButton btnDisplay;

	[SerializeField]
	private UIButton btnControl;

	[SerializeField]
	private RectTransform _scrollViewContent;

	[SerializeField]
	private RebindUI _rebindUIChild;

	public List<Resolution> resolutions = new List<Resolution>();

	public List<string> listResolutions = new List<string>();

	public int idxResolution;

	public PlayerInputActions input;

	public bool IsShowControlOnly;

	protected bool isNavigatePress;

	private int _tabIndex;

	public bool TabControlNavMode;

	public GameObject TabButtonObject;

	public static OptionsManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	public void Start()
	{
		isNavigatePress = false;
		UIOptions = GetComponent<UIView>();
		if (GlobalSaveData.instance.optionData.fullscreen)
		{
			if (GlobalSaveData.instance.optionData.windowMode == 0)
			{
				txtTermFullscreen.SetTerm("Menu/Borderless");
			}
			else
			{
				txtTermFullscreen.SetTerm("Menu/Fullscreen");
			}
		}
		else
		{
			txtTermFullscreen.SetTerm("Menu/Windowed");
		}
		Resolution[] array = Screen.resolutions;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].width * 3 == array[i].height * 4 || array[i].width * 4 == array[i].height * 5 || array[i].width * 2 == array[i].height * 3)
			{
				continue;
			}
			bool flag = false;
			for (int j = 0; j < listResolutions.Count; j++)
			{
				if (listResolutions[j] == array[i].width + " x " + array[i].height)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				resolutions.Add(array[i]);
				listResolutions.Add(array[i].width + " x " + array[i].height);
				if (GlobalSaveData.instance.optionData.resWidth == array[i].width && GlobalSaveData.instance.optionData.resHeight == array[i].height)
				{
					idxResolution = i;
				}
			}
		}
		txtResolution.text = GlobalSaveData.instance.optionData.resWidth + " x " + GlobalSaveData.instance.optionData.resHeight;
		if (GlobalSaveData.instance.optionData.showFpsRtt)
		{
			txtTermShowFPS.SetTerm("Menu/Show");
		}
		else
		{
			txtTermShowFPS.SetTerm("Menu/Hide");
		}
		if (GlobalSaveData.instance.optionData.vsync)
		{
			txtTermVsync.SetTerm("Menu/On");
			txtLimitFPS.text = "-";
		}
		else
		{
			txtTermVsync.SetTerm("Menu/Off");
			if (GlobalSaveData.instance.optionData.limitFPS == GlobalOptionsManager.Instance.ListFPS.Length - 1)
			{
				txtLimitFPS.text = LocalizationManager.GetTranslation("Menu/Unlimited");
			}
			else
			{
				txtLimitFPS.text = GlobalOptionsManager.Instance.ListFPS[GlobalSaveData.instance.optionData.limitFPS].ToString();
			}
		}
		if (GlobalSaveData.instance.optionData.chatLog)
		{
			txtTermShowChatLog.SetTerm("Menu/Show");
		}
		else
		{
			txtTermShowChatLog.SetTerm("Menu/Hide");
		}
		txtTermShakeLevel.SetTerm("Menu/" + GlobalOptionsManager.Instance.shakelevel[GlobalSaveData.instance.optionData.shakeLevel]);
		txtTermGraphic.SetTerm("Menu/" + GlobalOptionsManager.Instance.qualityLevel[GlobalSaveData.instance.optionData.graphic]);
		txtTermMinimap.SetTerm("Menu/" + GlobalOptionsManager.Instance.fixedAutoLevel[GlobalSaveData.instance.optionData.autoMinimap]);
		if (GlobalSaveData.instance.optionData.showFpsRtt)
		{
			txtTermShowFPS.SetTerm("Menu/Show");
		}
		else
		{
			txtTermShowFPS.SetTerm("Menu/Hide");
		}
		if (GlobalSaveData.instance.optionData.chatLog)
		{
			txtTermShowChatLog.SetTerm("Menu/Show");
		}
		else
		{
			txtTermShowChatLog.SetTerm("Menu/Hide");
		}
		txtTermVoiceChat.SetTerm("Menu/VoiceMode" + GlobalSaveData.instance.optionData.voiceChatMode);
		if (GlobalSaveData.instance.optionData.sprintModeToggle)
		{
			txtSprintMode.SetTerm("Menu/VoiceMode1");
		}
		else
		{
			txtSprintMode.SetTerm("Menu/Hold");
		}
		if (Microphone.devices.Length != 0)
		{
			GlobalOptionsManager.Instance.microphoneName = Microphone.devices[0];
			txtMicrophone.text = Microphone.devices[0];
		}
		if (GlobalSaveData.instance.optionData.SkipIntroControl)
		{
			txtTermIntroControl.SetTerm("Menu/" + GlobalOptionsManager.Instance.OnOff[1]);
		}
		if (GlobalSaveData.instance.optionData.SkipIntroDialogue)
		{
			txtTermIntroDialogue.SetTerm("Menu/" + GlobalOptionsManager.Instance.OnOff[1]);
		}
		if (!GlobalSaveData.instance.optionData.EnableTutorial)
		{
			txtTermSkipTutorial.SetTerm("Menu/" + GlobalOptionsManager.Instance.OnOff[1]);
		}
		if (!GameModes.Instance.isEvent)
		{
			return;
		}
		CanvasGroup component = txtTermIntroDialogue.transform.parent.GetComponent<CanvasGroup>();
		component.interactable = false;
		component.blocksRaycasts = false;
		component.alpha = 0f;
		CanvasGroup component2 = txtTermIntroControl.transform.parent.GetComponent<CanvasGroup>();
		component2.interactable = false;
		component2.blocksRaycasts = false;
		component2.alpha = 0f;
		CanvasGroup component3 = txtTermSkipTutorial.transform.parent.GetComponent<CanvasGroup>();
		component3.interactable = false;
		component3.blocksRaycasts = false;
		component3.alpha = 0f;
		foreach (Transform item in posSelectionGameplay.ToList())
		{
			CanvasGroup component4 = item.transform.parent.GetComponent<CanvasGroup>();
			if (component4 != null && Mathf.Approximately(component4.alpha, 0f))
			{
				posSelectionGameplay.Remove(item);
			}
		}
	}

	private void OnEnable()
	{
		input = new PlayerInputActions();
		input.UI.Enable();
		input.UI.Navigate.performed += OnInputNavigate;
		input.UI.Navigate.canceled += OnReleaseNavigate;
		input.UI.LeftTab.performed += OnInputLeftTab;
		input.UI.RightTab.performed += OnInputRightTab;
		input.UI.Submit.performed += OnSubmitPerformed;
		if (GameModes.Instance.isDebug)
		{
			input.UI.ChangeLanguage.performed += OnChangeLangPerformed;
		}
	}

	private void OnDisable()
	{
		if (input != null)
		{
			input.UI.Navigate.performed -= OnInputNavigate;
			input.UI.Navigate.canceled -= OnReleaseNavigate;
			input.UI.LeftTab.performed -= OnInputLeftTab;
			input.UI.RightTab.performed -= OnInputRightTab;
			input.UI.Submit.performed -= OnSubmitPerformed;
			if (GameModes.Instance.isDebug)
			{
				input.UI.ChangeLanguage.performed -= OnChangeLangPerformed;
			}
			input.UI.Disable();
		}
	}

	public void OnShow()
	{
		if (UITitleMenuManager.Instance != null)
		{
			UITitleMenuManager.Instance.HideTitle();
			UITitleMenuManager.Instance.HideInfoBot();
		}
		btnDisplay.gameObject.SetActive(value: true);
		btnGameplay.gameObject.SetActive(value: true);
		btnAudio.gameObject.SetActive(value: true);
		ShowGameplay();
	}

	public void ChangeLang(bool isIncrease)
	{
		AudioManager.PlaySFX("ui_select");
		int num = 0;
		for (int i = 0; i < GlobalSaveData.instance.arrLang.Count; i++)
		{
			if (GlobalSaveData.instance.optionData.lang == GlobalSaveData.instance.arrLang[i].LangCode)
			{
				num = i;
			}
		}
		int num2 = -1;
		int num3 = -1;
		if (isIncrease)
		{
			num++;
			if (num >= GlobalSaveData.instance.arrLang.Count)
			{
				num = 0;
			}
			for (int j = 0; j < GlobalSaveData.instance.arrLang.Count; j++)
			{
				if (GlobalSaveData.instance.arrLang[j].Enable && num2 == -1)
				{
					num2 = j;
				}
				if (j == num)
				{
					if (GlobalSaveData.instance.arrLang[j].Enable)
					{
						break;
					}
					num++;
				}
			}
		}
		else
		{
			num--;
			if (num < 0)
			{
				num = GlobalSaveData.instance.arrLang.Count - 1;
			}
			for (int num4 = GlobalSaveData.instance.arrLang.Count - 1; num4 >= 0; num4--)
			{
				if (GlobalSaveData.instance.arrLang[num4].Enable && num3 == -1)
				{
					num3 = num4;
				}
				if (num4 == num)
				{
					if (GlobalSaveData.instance.arrLang[num4].Enable)
					{
						break;
					}
					num--;
				}
			}
		}
		if (num < 0)
		{
			num = num3;
		}
		else if (num >= GlobalSaveData.instance.arrLang.Count)
		{
			num = num2;
		}
		GlobalSaveData.instance.optionData.lang = GlobalSaveData.instance.arrLang[num].LangCode;
		LocalizationManager.CurrentLanguageCode = GlobalSaveData.instance.optionData.lang;
		if (LobbyManager.Instance != null)
		{
			if (NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo)
			{
				UIGameManager.Instance.sessionName.transform.parent.gameObject.SetActive(value: false);
				UIGameManager.Instance.sessionName.text = "";
			}
			else
			{
				UIGameManager.Instance.sessionName.text = "******";
			}
		}
		if ((bool)UIMissionObjective.Instance)
		{
			UIMissionObjective.Instance.SetUIMapText();
		}
		if (UIRegionDifficulty.Instance != null)
		{
			UIRegionDifficulty.Instance.Translate();
		}
	}

	public void ChangeShakeLevel(bool isIncrease)
	{
		AudioManager.PlaySFX("ui_select");
		if (isIncrease)
		{
			GlobalSaveData.instance.optionData.shakeLevel++;
		}
		else
		{
			GlobalSaveData.instance.optionData.shakeLevel--;
		}
		if (GlobalSaveData.instance.optionData.shakeLevel < 0)
		{
			GlobalSaveData.instance.optionData.shakeLevel = 3;
		}
		else if (GlobalSaveData.instance.optionData.shakeLevel > 3)
		{
			GlobalSaveData.instance.optionData.shakeLevel = 0;
		}
		txtTermShakeLevel.SetTerm("Menu/" + GlobalOptionsManager.Instance.shakelevel[GlobalSaveData.instance.optionData.shakeLevel]);
	}

	public void ChangeIntroDialogue()
	{
		AudioManager.PlaySFX("ui_select");
		GlobalSaveData.instance.optionData.SkipIntroDialogue = !GlobalSaveData.instance.optionData.SkipIntroDialogue;
		if (GlobalSaveData.instance.optionData.SkipIntroDialogue)
		{
			txtTermIntroDialogue.SetTerm("Menu/" + GlobalOptionsManager.Instance.OnOff[1]);
		}
		else
		{
			txtTermIntroDialogue.SetTerm("Menu/" + GlobalOptionsManager.Instance.OnOff[0]);
		}
	}

	public void ChangeIntroControl()
	{
		AudioManager.PlaySFX("ui_select");
		GlobalSaveData.instance.optionData.SkipIntroControl = !GlobalSaveData.instance.optionData.SkipIntroControl;
		if (GlobalSaveData.instance.optionData.SkipIntroControl)
		{
			txtTermIntroControl.SetTerm("Menu/" + GlobalOptionsManager.Instance.OnOff[1]);
		}
		else
		{
			txtTermIntroControl.SetTerm("Menu/" + GlobalOptionsManager.Instance.OnOff[0]);
		}
	}

	public void ChangeEnableTutorial()
	{
		AudioManager.PlaySFX("ui_select");
		GlobalSaveData.instance.optionData.EnableTutorial = !GlobalSaveData.instance.optionData.EnableTutorial;
		if (GlobalSaveData.instance.optionData.EnableTutorial)
		{
			txtTermSkipTutorial.SetTerm("Menu/" + GlobalOptionsManager.Instance.OnOff[0]);
		}
		else
		{
			txtTermSkipTutorial.SetTerm("Menu/" + GlobalOptionsManager.Instance.OnOff[1]);
		}
		if (GlobalSaveData.instance.optionData.EnableTutorial)
		{
			GlobalSaveData.instance.optionData.IsTutorialMoveCleared = false;
			GlobalSaveData.instance.optionData.IsTutorialSprintCleared = false;
			GlobalSaveData.instance.optionData.IsTutorialDashCleared = false;
			GlobalSaveData.instance.optionData.IsTutorialMeleeCleared = false;
			GlobalSaveData.instance.optionData.IsTutorialShootCleared = false;
			if (!LobbyManager.Instance)
			{
				return;
			}
			foreach (TriggerEvent item in LobbyManager.Instance.ListAreaTutorial1)
			{
				item.gameObject.SetActive(value: true);
				item.ResetProgress();
			}
			{
				foreach (TriggerEvent item2 in LobbyManager.Instance.ListAreaTutorial2)
				{
					item2.gameObject.SetActive(value: false);
					item2.ResetProgress();
				}
				return;
			}
		}
		GlobalSaveData.instance.optionData.IsTutorialMoveCleared = true;
		GlobalSaveData.instance.optionData.IsTutorialSprintCleared = true;
		GlobalSaveData.instance.optionData.IsTutorialDashCleared = true;
		GlobalSaveData.instance.optionData.IsTutorialMeleeCleared = true;
		GlobalSaveData.instance.optionData.IsTutorialShootCleared = true;
		if (!LobbyManager.Instance)
		{
			return;
		}
		foreach (TriggerEvent item3 in LobbyManager.Instance.ListAreaTutorial1)
		{
			item3.gameObject.SetActive(value: false);
		}
		foreach (TriggerEvent item4 in LobbyManager.Instance.ListAreaTutorial2)
		{
			item4.gameObject.SetActive(value: false);
		}
	}

	public void ChangeVolMaster(bool isIncrease)
	{
		AudioManager.PlaySFX("ui_select");
		if (isIncrease)
		{
			GlobalSaveData.instance.optionData.volMaster += 10;
		}
		else
		{
			GlobalSaveData.instance.optionData.volMaster -= 10;
		}
		if (GlobalSaveData.instance.optionData.volMaster < 0)
		{
			GlobalSaveData.instance.optionData.volMaster = 0;
		}
		else if (GlobalSaveData.instance.optionData.volMaster > 100)
		{
			GlobalSaveData.instance.optionData.volMaster = 100;
		}
		txtVolMaster.text = GlobalSaveData.instance.optionData.volMaster.ToString();
		AudioManager.ChangeVolumeMaster((float)GlobalSaveData.instance.optionData.volMaster / 100f);
	}

	public void ChangeVolBGM(bool isIncrease)
	{
		AudioManager.PlaySFX("ui_select");
		if (isIncrease)
		{
			GlobalSaveData.instance.optionData.volMusic += 10;
		}
		else
		{
			GlobalSaveData.instance.optionData.volMusic -= 10;
		}
		if (GlobalSaveData.instance.optionData.volMusic < 0)
		{
			GlobalSaveData.instance.optionData.volMusic = 0;
		}
		else if (GlobalSaveData.instance.optionData.volMusic > 100)
		{
			GlobalSaveData.instance.optionData.volMusic = 100;
		}
		txtVolMusic.text = GlobalSaveData.instance.optionData.volMusic.ToString();
		AudioManager.ChangeVolumeBGM((float)GlobalSaveData.instance.optionData.volMusic / 100f);
	}

	public void ChangeVolSFX(bool isIncrease)
	{
		AudioManager.PlaySFX("ui_select");
		if (isIncrease)
		{
			GlobalSaveData.instance.optionData.volSFX += 10;
		}
		else
		{
			GlobalSaveData.instance.optionData.volSFX -= 10;
		}
		if (GlobalSaveData.instance.optionData.volSFX < 0)
		{
			GlobalSaveData.instance.optionData.volSFX = 0;
		}
		else if (GlobalSaveData.instance.optionData.volSFX > 100)
		{
			GlobalSaveData.instance.optionData.volSFX = 100;
		}
		txtVolSFX.text = GlobalSaveData.instance.optionData.volSFX.ToString();
		AudioManager.ChangeVolumeSFX((float)GlobalSaveData.instance.optionData.volSFX / 100f);
	}

	public void ChangeVolVoice(bool isIncrease)
	{
		AudioManager.PlaySFX("ui_select");
		if (isIncrease)
		{
			GlobalSaveData.instance.optionData.volVoice += 10;
		}
		else
		{
			GlobalSaveData.instance.optionData.volVoice -= 10;
		}
		if (GlobalSaveData.instance.optionData.volVoice < 0)
		{
			GlobalSaveData.instance.optionData.volVoice = 0;
		}
		else if (GlobalSaveData.instance.optionData.volVoice > 100)
		{
			GlobalSaveData.instance.optionData.volVoice = 100;
		}
		txtVolVoice.text = GlobalSaveData.instance.optionData.volVoice.ToString();
		AudioManager.ChangeVolumeVoice((float)GlobalSaveData.instance.optionData.volVoice / 100f);
		AudioManager.PlaySFX("test");
	}

	public void ChangeVolAmbient(bool isIncrease)
	{
		AudioManager.PlaySFX("ui_select");
		if (isIncrease)
		{
			GlobalSaveData.instance.optionData.volAmbient += 10;
		}
		else
		{
			GlobalSaveData.instance.optionData.volAmbient -= 10;
		}
		if (GlobalSaveData.instance.optionData.volAmbient < 0)
		{
			GlobalSaveData.instance.optionData.volAmbient = 0;
		}
		else if (GlobalSaveData.instance.optionData.volAmbient > 100)
		{
			GlobalSaveData.instance.optionData.volAmbient = 100;
		}
		txtVolAmbient.text = GlobalSaveData.instance.optionData.volAmbient.ToString();
		AudioManager.ChangeVolumeAmbient((float)GlobalSaveData.instance.optionData.volAmbient / 100f);
	}

	public void ChangeVoiceChatMode(bool isIncrease)
	{
		AudioManager.PlaySFX("ui_select");
		if (isIncrease)
		{
			GlobalSaveData.instance.optionData.voiceChatMode++;
		}
		else
		{
			GlobalSaveData.instance.optionData.voiceChatMode--;
		}
		if (GlobalSaveData.instance.optionData.voiceChatMode < 0)
		{
			GlobalSaveData.instance.optionData.voiceChatMode = 1;
		}
		else if (GlobalSaveData.instance.optionData.voiceChatMode > 1)
		{
			GlobalSaveData.instance.optionData.voiceChatMode = 0;
		}
		txtTermVoiceChat.SetTerm("Menu/VoiceMode" + GlobalSaveData.instance.optionData.voiceChatMode);
		if (VoiceChatGlobalController.Instance != null)
		{
			VoiceChatGlobalController.Instance.SetMuted(Value: true);
			UIGameManager.Instance.micOn.SetActive(value: false);
			UIGameManager.Instance.micOff.SetActive(value: true);
		}
	}

	public void ChangeSprintMode()
	{
		AudioManager.PlaySFX("ui_select");
		GlobalSaveData.instance.optionData.sprintModeToggle = !GlobalSaveData.instance.optionData.sprintModeToggle;
		if (GlobalSaveData.instance.optionData.sprintModeToggle)
		{
			txtSprintMode.SetTerm("Menu/VoiceMode1");
		}
		else
		{
			txtSprintMode.SetTerm("Menu/Hold");
		}
	}

	public void ChangeMicrophone(bool isIncrease)
	{
		AudioManager.PlaySFX("ui_select");
		int num = -1;
		for (int i = 0; i < Microphone.devices.Length; i++)
		{
			if (GlobalOptionsManager.Instance.microphoneName == Microphone.devices[i])
			{
				num = i;
			}
		}
		if (num >= 0)
		{
			num = ((!isIncrease) ? (num - 1) : (num + 1));
			if (num < 0)
			{
				num = Microphone.devices.Length - 1;
			}
			else if (num >= Microphone.devices.Length)
			{
				num = 0;
			}
		}
		else
		{
			num = 0;
		}
		GlobalOptionsManager.Instance.microphoneName = Microphone.devices[num];
		txtMicrophone.text = Microphone.devices[num];
		if ((bool)VoiceChatGlobalController.Instance)
		{
			VoiceChatGlobalController.Instance.VoiceComms.MicrophoneName = Microphone.devices[num];
		}
	}

	public void ShowDisplay()
	{
		_tabIndex = 2;
		_scrollViewContent.localPosition = Vector3.zero;
		float y = uiDisplay.GetComponent<RectTransform>().sizeDelta.y;
		_scrollViewContent.sizeDelta = new Vector2(_scrollViewContent.sizeDelta.x, y);
		selector.transform.DOKill();
		selector.gameObject.SetActive(value: true);
		uiGameplay.SetActive(value: false);
		uiAudio.SetActive(value: false);
		uiControl.SetActive(value: false);
		uiDisplay.SetActive(value: true);
		uiControlTutorial.SetActive(value: false);
		selectVertical = 0;
		selector.transform.position = new Vector2(selector.transform.position.x, posSelectionDisplay[0].position.y);
		EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
		btnDisplay.interactable = false;
		btnAudio.interactable = true;
		btnGameplay.interactable = true;
		btnControl.interactable = true;
		if (GlobalSaveData.instance.optionData.vsync)
		{
			txtLimitFPS.text = "-";
		}
		else if (GlobalSaveData.instance.optionData.limitFPS == GlobalOptionsManager.Instance.ListFPS.Length - 1)
		{
			txtLimitFPS.text = LocalizationManager.GetTranslation("Menu/Unlimited");
		}
		else
		{
			txtLimitFPS.text = GlobalOptionsManager.Instance.ListFPS[GlobalSaveData.instance.optionData.limitFPS].ToString();
		}
	}

	public void ShowGameplay()
	{
		btnDisplay.gameObject.SetActive(value: true);
		btnGameplay.gameObject.SetActive(value: true);
		btnAudio.gameObject.SetActive(value: true);
		_tabIndex = 0;
		_scrollViewContent.localPosition = Vector3.zero;
		float y = uiGameplay.GetComponent<RectTransform>().sizeDelta.y;
		_scrollViewContent.sizeDelta = new Vector2(_scrollViewContent.sizeDelta.x, y);
		selector.transform.DOKill();
		selector.gameObject.SetActive(value: true);
		uiAudio.SetActive(value: false);
		uiDisplay.SetActive(value: false);
		uiControl.SetActive(value: false);
		uiGameplay.SetActive(value: true);
		uiControlTutorial.SetActive(value: false);
		selectVertical = 0;
		EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
		btnGameplay.interactable = false;
		btnAudio.interactable = true;
		btnDisplay.interactable = true;
		btnControl.interactable = true;
		UniTaskUtil.DelayedCall(this, 0.1f, () =>
		{
			selector.transform.DOMoveY(posSelectionGameplay[0].position.y, 0f);
		}).Forget();
	}

	public void ShowAudio()
	{
		btnDisplay.gameObject.SetActive(value: true);
		btnGameplay.gameObject.SetActive(value: true);
		btnAudio.gameObject.SetActive(value: true);
		_tabIndex = 1;
		_scrollViewContent.localPosition = Vector3.zero;
		float y = uiGameplay.GetComponent<RectTransform>().sizeDelta.y;
		_scrollViewContent.sizeDelta = new Vector2(_scrollViewContent.sizeDelta.x, y);
		selector.transform.DOKill();
		selector.gameObject.SetActive(value: true);
		uiAudio.SetActive(value: true);
		uiDisplay.SetActive(value: false);
		uiControl.SetActive(value: false);
		uiGameplay.SetActive(value: false);
		uiControlTutorial.SetActive(value: false);
		selectVertical = 0;
		EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
		btnGameplay.interactable = true;
		btnAudio.interactable = false;
		btnDisplay.interactable = true;
		btnControl.interactable = true;
		txtVolMaster.text = GlobalSaveData.instance.optionData.volMaster.ToString();
		txtVolMusic.text = GlobalSaveData.instance.optionData.volMusic.ToString();
		txtVolSFX.text = GlobalSaveData.instance.optionData.volSFX.ToString();
		txtVolVoice.text = GlobalSaveData.instance.optionData.volVoice.ToString();
		txtVolAmbient.text = GlobalSaveData.instance.optionData.volAmbient.ToString();
		UniTaskUtil.DelayedCall(this, 0.1f, () =>
		{
			selector.transform.DOMoveY(posSelectionAudio[0].position.y, 0f);
		}).Forget();
	}

	public void ShowControlStatic()
	{
		_tabIndex = 3;
		selector.transform.DOKill();
		selector.gameObject.SetActive(value: false);
		uiDisplay.SetActive(value: false);
		uiGameplay.SetActive(value: false);
		uiControlTutorial.SetActive(value: true);
		for (int i = 0; i < uiControlTutorial.transform.childCount; i++)
		{
			if (GlobalOptionsManager.Instance.bindingIndex == i)
			{
				uiControlTutorial.transform.GetChild(i).gameObject.SetActive(value: true);
			}
			else
			{
				uiControlTutorial.transform.GetChild(i).gameObject.SetActive(value: false);
			}
		}
		selectVertical = 0;
		EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
		btnGameplay.interactable = true;
		btnDisplay.interactable = true;
		btnControl.interactable = false;
		btnBack.Select();
	}

	public void ShowControl()
	{
		_tabIndex = 3;
		selector.transform.DOKill();
		selector.gameObject.SetActive(value: true);
		uiAudio.SetActive(value: false);
		uiDisplay.SetActive(value: false);
		uiGameplay.SetActive(value: false);
		uiControl.SetActive(value: true);
		selectVertical = 0;
		EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
		btnGameplay.interactable = true;
		btnAudio.interactable = true;
		btnDisplay.interactable = true;
		btnControl.interactable = false;
		UniTaskUtil.DelayedCall(this, 0.1f, () =>
		{
			ShowRebindControl();
		}).Forget();
	}

	private void ShowRebindControl()
	{
		_scrollViewContent.localPosition = Vector3.zero;
		float y = uiControl.GetComponent<RectTransform>().sizeDelta.y;
		_scrollViewContent.sizeDelta = new Vector2(_scrollViewContent.sizeDelta.x, y);
		selector.transform.DOMoveY(posSelectionControl[0].position.y, 0f);
	}

	public void ChangeResolutions(bool isIncrease)
	{
		AudioManager.PlaySFX("ui_select");
		if (isIncrease)
		{
			idxResolution++;
		}
		else
		{
			idxResolution--;
		}
		if (idxResolution < 0)
		{
			idxResolution = 0;
		}
		else if (idxResolution >= listResolutions.Count)
		{
			idxResolution = listResolutions.Count - 1;
		}
		txtResolution.text = listResolutions[idxResolution];
		GlobalSaveData.instance.optionData.resWidth = resolutions[idxResolution].width;
		GlobalSaveData.instance.optionData.resHeight = resolutions[idxResolution].height;
		if (GlobalSaveData.instance.optionData.fullscreen)
		{
			Screen.SetResolution(resolutions[idxResolution].width, resolutions[idxResolution].height, FullScreenMode.ExclusiveFullScreen);
		}
		else
		{
			Screen.SetResolution(resolutions[idxResolution].width, resolutions[idxResolution].height, FullScreenMode.Windowed);
		}
	}

	public void ChangeFullscreen(bool isIncrease)
	{
		if (isIncrease)
		{
			GlobalSaveData.instance.optionData.windowMode++;
		}
		else
		{
			GlobalSaveData.instance.optionData.windowMode--;
		}
		if (GlobalSaveData.instance.optionData.windowMode > 2)
		{
			GlobalSaveData.instance.optionData.windowMode = 0;
		}
		else if (GlobalSaveData.instance.optionData.windowMode < 0)
		{
			GlobalSaveData.instance.optionData.windowMode = 2;
		}
		AudioManager.PlaySFX("ui_select");
		if (GlobalSaveData.instance.optionData.windowMode < 2)
		{
			GlobalSaveData.instance.optionData.fullscreen = true;
			if (GlobalSaveData.instance.optionData.windowMode == 0)
			{
				txtTermFullscreen.SetTerm("Menu/Borderless");
				Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
			}
			else
			{
				txtTermFullscreen.SetTerm("Menu/Fullscreen");
				Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
			}
		}
		else
		{
			GlobalSaveData.instance.optionData.fullscreen = false;
			txtTermFullscreen.SetTerm("Menu/Windowed");
			Screen.fullScreenMode = FullScreenMode.Windowed;
		}
		if (GlobalSaveData.instance.optionData.vsync)
		{
			QualitySettings.vSyncCount = 1;
			return;
		}
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = GlobalOptionsManager.Instance.ListFPS[GlobalSaveData.instance.optionData.limitFPS];
	}

	public void ChangeQuality(bool isIncrease)
	{
		AudioManager.PlaySFX("ui_select");
		if (isIncrease)
		{
			GlobalSaveData.instance.optionData.graphic++;
		}
		else
		{
			GlobalSaveData.instance.optionData.graphic--;
		}
		if (GlobalSaveData.instance.optionData.graphic < 0)
		{
			GlobalSaveData.instance.optionData.graphic = 2;
		}
		else if (GlobalSaveData.instance.optionData.graphic > 2)
		{
			GlobalSaveData.instance.optionData.graphic = 0;
		}
		txtTermGraphic.SetTerm("Menu/" + GlobalOptionsManager.Instance.qualityLevel[GlobalSaveData.instance.optionData.graphic]);
		QualitySettings.SetQualityLevel(GlobalSaveData.instance.optionData.graphic);
		if ((bool)CameraGame.Instance)
		{
			if (GlobalSaveData.instance.optionData.graphic >= 1)
			{
				CameraGame.Instance.Bloom.active = true;
			}
			else
			{
				CameraGame.Instance.Bloom.active = false;
			}
		}
		if (GameManager.Instance != null)
		{
			foreach (RoomCollider item in GameManager.Instance.arrRoom)
			{
				foreach (Animator animatedObject in item.animatedObjectList)
				{
					if (animatedObject.isActiveAndEnabled)
					{
						if (GlobalSaveData.instance.optionData.graphic == 2)
						{
							animatedObject.Play("Animated");
						}
						else
						{
							animatedObject.Play("Idle");
						}
					}
				}
			}
		}
		if (GlobalSaveData.instance.optionData.graphic >= 2)
		{
			Shader.EnableKeyword("BACKLIGHT_ENABLED");
		}
		else
		{
			Shader.DisableKeyword("BACKLIGHT_ENABLED");
		}
		if (GlobalSaveData.instance.optionData.vsync)
		{
			QualitySettings.vSyncCount = 1;
		}
		else
		{
			QualitySettings.vSyncCount = 0;
		}
		Application.targetFrameRate = GlobalOptionsManager.Instance.ListFPS[GlobalSaveData.instance.optionData.limitFPS];
	}

	public void ChangeAutoMinimap(bool isIncrease)
	{
		AudioManager.PlaySFX("ui_select");
		if (GlobalSaveData.instance.optionData.autoMinimap == 0)
		{
			GlobalSaveData.instance.optionData.autoMinimap = 1;
		}
		else
		{
			GlobalSaveData.instance.optionData.autoMinimap = 0;
		}
		txtTermMinimap.SetTerm("Menu/" + GlobalOptionsManager.Instance.fixedAutoLevel[GlobalSaveData.instance.optionData.autoMinimap]);
		if (!(CameraMiniMap.Instance != null))
		{
			return;
		}
		if (GlobalSaveData.instance.optionData.autoMinimap == 1)
		{
			CameraMiniMap.Instance.transform.DOLocalRotate(new Vector3(90f, CameraGame.Instance.camRotate, 0f), 0f);
			{
				foreach (ItemPickable item in GameManager.Instance.arrItemPickable)
				{
					if (item.itemMap != null)
					{
						item.itemMap.transform.DOLocalRotate(new Vector3(90f, 0f, -CameraGame.Instance.camRotate), 0f);
					}
				}
				return;
			}
		}
		CameraMiniMap.Instance.transform.DOLocalRotate(new Vector3(90f, 0f, 0f), 0f);
		foreach (ItemPickable item2 in GameManager.Instance.arrItemPickable)
		{
			if (item2.itemMap != null)
			{
				item2.itemMap.transform.DOLocalRotate(new Vector3(90f, 0f, 0f), 0f);
			}
		}
		foreach (PlayerController item3 in NetworkGameManager.Instance.arrPlayerNetworkController)
		{
			if (item3 != null && !item3.network.isLocalPlayer)
			{
				item3.iconCharMap.DORotate(new Vector3(90f, 0f, 0f), 0f);
			}
		}
		CameraGame.Instance.SetFixedMinimapRoomText();
	}

	public void ChangeShowFPSRTT(bool isIncrease)
	{
		AudioManager.PlaySFX("ui_select");
		if (!GlobalSaveData.instance.optionData.showFpsRtt)
		{
			GlobalSaveData.instance.optionData.showFpsRtt = true;
			txtTermShowFPS.SetTerm("Menu/Show");
			if (UIGameManager.Instance != null)
			{
				UIGameManager.Instance.fpsObject.SetActive(value: true);
			}
		}
		else
		{
			GlobalSaveData.instance.optionData.showFpsRtt = false;
			txtTermShowFPS.SetTerm("Menu/Hide");
			if (UIGameManager.Instance != null)
			{
				UIGameManager.Instance.fpsObject.SetActive(value: false);
			}
		}
	}

	public void ChangeChatLog(bool isIncrease)
	{
		AudioManager.PlaySFX("ui_select");
		if (!GlobalSaveData.instance.optionData.chatLog)
		{
			GlobalSaveData.instance.optionData.chatLog = true;
			txtTermShowChatLog.SetTerm("Menu/Show");
		}
		else
		{
			GlobalSaveData.instance.optionData.chatLog = false;
			txtTermShowChatLog.SetTerm("Menu/Hide");
		}
	}

	public void OnInputNavigate(InputAction.CallbackContext value)
	{
		TabControlNavMode = true;
		bool flag = false;
		if (UIOptions.isVisible && !isNavigatePress)
		{
			if (value.ReadValue<Vector2>().y > 0.5f && selectVertical > 0)
			{
				selector.gameObject.SetActive(value: true);
				selectVertical--;
				EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
				if (uiGameplay.activeSelf)
				{
					selector.transform.DOMoveY(posSelectionGameplay[selectVertical].position.y, 0.15f);
				}
				else if (uiAudio.activeSelf)
				{
					selector.transform.DOMoveY(posSelectionAudio[selectVertical].position.y, 0.15f);
				}
				else if (uiDisplay.activeSelf)
				{
					selector.transform.DOMoveY(posSelectionDisplay[selectVertical].position.y, 0.15f);
				}
				else if (uiControl.activeSelf)
				{
					selector.transform.DOLocalMoveY(posSelectionControl[selectVertical].localPosition.y, 0.15f).OnComplete(() =>
					{
						_rebindUIChild.ShiftScroll(selectVertical, posSelectionControl[selectVertical].localPosition.y);
					});
				}
				AudioManager.PlaySFX("ui_select");
				isNavigatePress = true;
			}
			else if (value.ReadValue<Vector2>().y < -0.5f)
			{
				int num = 0;
				if (selectVertical < posSelectionGameplay.Count - 1 && uiGameplay.activeSelf)
				{
					num = 1;
				}
				else if (selectVertical < posSelectionDisplay.Count - 1 && uiDisplay.activeSelf)
				{
					num = 1;
				}
				else if (selectVertical < posSelectionControl.Count - 1 && uiControl.activeSelf)
				{
					num = 1;
				}
				else if (selectVertical < posSelectionAudio.Count - 1 && uiAudio.activeSelf)
				{
					num = 1;
				}
				else if (selectVertical == posSelectionGameplay.Count - 1 && uiGameplay.activeSelf)
				{
					num = 2;
				}
				else if (selectVertical == posSelectionDisplay.Count - 1 && uiDisplay.activeSelf)
				{
					num = 2;
				}
				else if (selectVertical == posSelectionControl.Count - 1 && uiControl.activeSelf)
				{
					num = 2;
				}
				else if (selectVertical == posSelectionAudio.Count - 1 && uiAudio.activeSelf)
				{
					num = 2;
				}
				switch (num)
				{
				case 1:
					selector.gameObject.SetActive(value: true);
					selectVertical++;
					EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(null);
					if (uiGameplay.activeSelf)
					{
						selector.transform.DOMoveY(posSelectionGameplay[selectVertical].position.y, 0.15f);
					}
					else if (uiAudio.activeSelf)
					{
						selector.transform.DOMoveY(posSelectionAudio[selectVertical].position.y, 0.15f);
					}
					else if (uiDisplay.activeSelf)
					{
						selector.transform.DOMoveY(posSelectionDisplay[selectVertical].position.y, 0.15f);
					}
					else if (uiControl.activeSelf)
					{
						selector.transform.DOLocalMoveY(posSelectionControl[selectVertical].localPosition.y, 0.15f).OnComplete(() =>
						{
							_rebindUIChild.ShiftScroll(selectVertical, posSelectionControl[selectVertical].localPosition.y);
						});
					}
					AudioManager.PlaySFX("ui_select");
					isNavigatePress = true;
					break;
				case 2:
					selector.gameObject.SetActive(value: false);
					if (uiGameplay.activeSelf)
					{
						selectVertical = posSelectionGameplay.Count;
					}
					else if (uiDisplay.activeSelf)
					{
						selectVertical = posSelectionDisplay.Count;
					}
					else if (uiControl.activeSelf)
					{
						selectVertical = posSelectionControl.Count;
					}
					btnBack.Select();
					AudioManager.PlaySFX("ui_select");
					isNavigatePress = true;
					break;
				}
			}
			bool isIncrease = false;
			if (value.ReadValue<Vector2>().x > 0.5f)
			{
				isIncrease = true;
				flag = true;
			}
			else if (value.ReadValue<Vector2>().x < -0.5f)
			{
				isIncrease = false;
				flag = true;
			}
			if (flag)
			{
				isNavigatePress = true;
				if (uiGameplay.activeSelf)
				{
					if (selectVertical == 0)
					{
						ChangeLang(isIncrease);
					}
					else if (selectVertical == 1)
					{
						ChangeShakeLevel(isIncrease);
					}
					else if (selectVertical == 2)
					{
						ChangeSprintMode();
					}
					else if (selectVertical == 3)
					{
						ChangeIntroDialogue();
					}
					else if (selectVertical == 4)
					{
						ChangeIntroControl();
					}
					else if (selectVertical == 5)
					{
						ChangeEnableTutorial();
					}
				}
				else if (uiAudio.activeSelf)
				{
					if (selectVertical == 0)
					{
						ChangeVolMaster(isIncrease);
					}
					else if (selectVertical == 1)
					{
						ChangeVolBGM(isIncrease);
					}
					else if (selectVertical == 2)
					{
						ChangeVolSFX(isIncrease);
					}
					else if (selectVertical == 3)
					{
						ChangeVolAmbient(isIncrease);
					}
					else if (selectVertical == 4)
					{
						ChangeVolVoice(isIncrease);
					}
					else if (selectVertical == 5)
					{
						ChangeVoiceChatMode(isIncrease);
					}
					else if (selectVertical == 6)
					{
						ChangeMicrophone(isIncrease);
					}
				}
				else if (uiDisplay.activeSelf)
				{
					if (selectVertical == 0)
					{
						ChangeResolutions(isIncrease);
					}
					else if (selectVertical == 1)
					{
						ChangeFullscreen(isIncrease);
					}
					else if (selectVertical == 2)
					{
						ChangeQuality(isIncrease);
					}
					else if (selectVertical == 3)
					{
						ChangeVsync(isIncrease);
					}
					else if (selectVertical == 4)
					{
						ChangeLimitFPS(isIncrease);
					}
					else if (selectVertical == 5)
					{
						ChangeAutoMinimap(isIncrease);
					}
					else if (selectVertical == 6)
					{
						ChangeShowFPSRTT(isIncrease);
					}
					else if (selectVertical == 7)
					{
						ChangeChatLog(isIncrease);
					}
				}
			}
		}
		if (value.ReadValue<Vector2>().y <= 0.5f && value.ReadValue<Vector2>().y >= -0.5f && value.ReadValue<Vector2>().x <= 0.5f && value.ReadValue<Vector2>().x >= -0.5f)
		{
			isNavigatePress = false;
		}
	}

	public void OnReleaseNavigate(InputAction.CallbackContext value)
	{
		isNavigatePress = false;
	}

	public void OnInputLeftTab(InputAction.CallbackContext value)
	{
		if (!isKeyRebinding && UIOptions.isVisible && !IsShowControlOnly)
		{
			AudioManager.PlaySFX("ui_select");
			switch (_tabIndex)
			{
			case 0:
				ShowControl();
				break;
			case 1:
				ShowGameplay();
				break;
			case 2:
				ShowAudio();
				break;
			case 3:
				ShowDisplay();
				break;
			}
		}
	}

	public void OnInputRightTab(InputAction.CallbackContext value)
	{
		if (!isKeyRebinding && UIOptions.isVisible && !IsShowControlOnly)
		{
			AudioManager.PlaySFX("ui_select");
			switch (_tabIndex)
			{
			case 0:
				ShowAudio();
				break;
			case 1:
				ShowDisplay();
				break;
			case 2:
				ShowControl();
				break;
			case 3:
				ShowGameplay();
				break;
			}
		}
	}

	public void ChangeVsync(bool isIncrease)
	{
		AudioManager.PlaySFX("ui_select");
		if (!GlobalSaveData.instance.optionData.vsync)
		{
			Application.targetFrameRate = 60;
			QualitySettings.vSyncCount = 1;
			GlobalSaveData.instance.optionData.vsync = true;
			txtTermVsync.SetTerm("Menu/On");
			txtLimitFPS.text = "-";
			return;
		}
		QualitySettings.vSyncCount = 0;
		GlobalSaveData.instance.optionData.vsync = false;
		txtTermVsync.SetTerm("Menu/Off");
		if (GlobalSaveData.instance.optionData.limitFPS == GlobalOptionsManager.Instance.ListFPS.Length - 1)
		{
			txtLimitFPS.text = LocalizationManager.GetTranslation("Menu/Unlimited");
		}
		else
		{
			txtLimitFPS.text = GlobalOptionsManager.Instance.ListFPS[GlobalSaveData.instance.optionData.limitFPS].ToString();
		}
		Application.targetFrameRate = GlobalOptionsManager.Instance.ListFPS[GlobalSaveData.instance.optionData.limitFPS];
	}

	public void ChangeLimitFPS(bool isIncrease)
	{
		if (!GlobalSaveData.instance.optionData.vsync)
		{
			AudioManager.PlaySFX("ui_select");
			if (isIncrease)
			{
				GlobalSaveData.instance.optionData.limitFPS++;
			}
			else
			{
				GlobalSaveData.instance.optionData.limitFPS--;
			}
			if (GlobalSaveData.instance.optionData.limitFPS < 0)
			{
				GlobalSaveData.instance.optionData.limitFPS = GlobalOptionsManager.Instance.ListFPS.Length - 1;
			}
			else if (GlobalSaveData.instance.optionData.limitFPS >= GlobalOptionsManager.Instance.ListFPS.Length)
			{
				GlobalSaveData.instance.optionData.limitFPS = 0;
			}
			if (GlobalSaveData.instance.optionData.limitFPS == GlobalOptionsManager.Instance.ListFPS.Length - 1)
			{
				txtLimitFPS.text = LocalizationManager.GetTranslation("Menu/Unlimited");
			}
			else
			{
				txtLimitFPS.text = GlobalOptionsManager.Instance.ListFPS[GlobalSaveData.instance.optionData.limitFPS].ToString();
			}
			Application.targetFrameRate = GlobalOptionsManager.Instance.ListFPS[GlobalSaveData.instance.optionData.limitFPS];
		}
	}

	private void OnSubmitPerformed(InputAction.CallbackContext value)
	{
		if (UIOptions.isVisible && uiControl.activeSelf && selectVertical < posSelectionControl.Count)
		{
			int actionIndex = posSelectionControl[selectVertical].GetSiblingIndex() - 1;
			_rebindUIChild.BeginRebind(actionIndex);
		}
	}

	public void OnHide()
	{
		UITitleMenuManager.Instance?.BackToTitleMenu();
		if (UIGameManager.Instance != null)
		{
			foreach (ConvertNote item in UIGameManager.Instance.arrConvertedText)
			{
				item.textMesh.text = UIGameManager.Instance.ConvertNote(item.initText);
			}
		}
		GlobalSaveData.instance.SaveOptionData();
	}

	public void SaveData()
	{
		GlobalSaveData.instance.SaveOptionData();
	}

	public void ResetPosSelectionControl()
	{
		posSelectionControl.Clear();
	}

	public void AddToPosSelectionControl(Transform uiGroup)
	{
		posSelectionControl.Add(uiGroup);
	}

	public void SnapSelectorPosition(int idx, float globalPosition)
	{
		selectVertical = idx;
		selector.transform.DOMoveY(globalPosition, 0f);
	}

	public void OnChangeLangPerformed(InputAction.CallbackContext value)
	{
		ChangeLang(isIncrease: true);
	}
}
