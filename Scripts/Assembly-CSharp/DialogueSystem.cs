using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Doozy.Runtime.UIManager.Containers;
using I2.Loc;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
	[SerializeField]
	private UIView _uiView;

	[SerializeField]
	private bool _isShowBlackBar;

	[SerializeField]
	private int _playerFacingAngle;

	[SerializeField]
	private DialogSO _dialogue;

	public DialogSO ShowPerksAction;

	[SerializeField]
	private int _idxActionDialogue;

	[SerializeField]
	private bool _waitPress;

	[SerializeField]
	private List<CharAvatarDialogID> _listCharDialog = new List<CharAvatarDialogID>();

	[SerializeField]
	private float holdTimeSkip;

	[SerializeField]
	private bool isHoldingSkip;

	[SerializeField]
	private float _speedBlackbar = 1f;

	[SerializeField]
	private Transform _blackBarTop;

	[SerializeField]
	private Transform _blackBarBot;

	[SerializeField]
	private TextMeshProUGUI _name;

	[SerializeField]
	private GameObject _dialogueBox;

	[SerializeField]
	private TextMeshProUGUI _dialogueText;

	[SerializeField]
	private Localize _dialogueTerm;

	[SerializeField]
	private Image _spriteAvatar;

	[SerializeField]
	private GameObject _iconDown;

	[SerializeField]
	private string _sfxShowAllMessage;

	[SerializeField]
	private string _sfxNextMessage;

	[SerializeField]
	private string _sfxMessagePerChar;

	private string _prevText;

	public bool IsFinishedIntroDialogue;

	private bool _cantSkip;

	public static DialogueSystem Instance { get; private set; }

	public UIView GetUIView => _uiView;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
		}
		else
		{
			Instance = this;
		}
	}

	public void ShowUI(DialogSO newDialogue, float speedBlackbar = 1f)
	{
		Debug.Log("ShowUI");
		UIGameManager.Instance.ArrPlayerInfo[NetworkGameManager.Instance.ownPlayer.network.GetIDX()].DialogueObject.SetActive(value: false);
		UIGameManager.Instance.ArrPlayerInfo[NetworkGameManager.Instance.ownPlayer.network.GetIDX()].BotDialogueObject.SetActive(value: false);
		if ((bool)newDialogue)
		{
			_dialogue = newDialogue;
		}
		_speedBlackbar = speedBlackbar;
		_uiView.Show();
	}

	public void OnShow()
	{
		Debug.Log("ShowUIDialog");
		UIGameManager.Instance.ArrPlayerInfo[NetworkGameManager.Instance.ownPlayer.network.GetIDX()].DialogueObject.SetActive(value: false);
		UIGameManager.Instance.ArrPlayerInfo[NetworkGameManager.Instance.ownPlayer.network.GetIDX()].BotDialogueObject.SetActive(value: false);
		UIGameManager.Instance.uiInGame.Hide();
		_iconDown.SetActive(value: false);
		if (_isShowBlackBar)
		{
			_blackBarTop.DOScaleY(1.2f, _speedBlackbar);
			_blackBarBot.DOScaleY(1.2f, _speedBlackbar);
		}
		NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: false);
		UniTaskUtil.DelayedCall(this, _speedBlackbar, () =>
		{
			_idxActionDialogue = 0;
			TriggerActionDialogue();
			UIGameManager.Instance.ArrPlayerInfo[NetworkGameManager.Instance.ownPlayer.network.GetIDX()].ProgressBarTutorialObject.SetActive(value: false);
		}).Forget();
	}

	private CharAvatarDialogID GetCharDialogID(CharDialogueEnum charNameDialogue)
	{
		foreach (CharAvatarDialogID item in _listCharDialog)
		{
			if (item.CharName == charNameDialogue)
			{
				return item;
			}
		}
		return null;
	}

	public void TriggerActionDialogue()
	{
		if (_idxActionDialogue <= _dialogue.ListActionDialogue.Count - 1)
		{
			ActionDialogue actionDialogue = _dialogue.ListActionDialogue[_idxActionDialogue];
			_dialogueText.text = "";
			_dialogueBox.SetActive(!actionDialogue.HideDialogueBox);
			_cantSkip = actionDialogue.CantSkip;
			switch (actionDialogue.ActionType)
			{
			case ActionDialogueType.SHOW_TERM_TEXT:
			{
				_name.text = LocalizationManager.GetTranslation("Interaction/" + actionDialogue.CharName);
				_spriteAvatar.sprite = GetCharDialogID(actionDialogue.CharName).SpriteChar;
				_dialogueText.text = "";
				string text = "";
				if (!string.IsNullOrWhiteSpace(actionDialogue.TermTextDialogue))
				{
					text = LocalizationManager.GetTranslation(actionDialogue.TermTextDialogue);
				}
				_prevText = "";
				_dialogueText.DOText(text, (float)text.Length * 0.04f).SetEase(Ease.Linear).SetDelay(0.1f)
					.OnComplete(() =>
					{
						_idxActionDialogue++;
						_waitPress = true;
						_iconDown.SetActive(value: true);
					})
					.OnUpdate(() =>
					{
						if (_dialogueText.text != _prevText)
						{
							AudioManager.PlaySFX(_sfxMessagePerChar);
							_prevText = _dialogueText.text;
						}
					});
				break;
			}
			case ActionDialogueType.CHANGE_PLAYER_DIRECTION:
				if (actionDialogue.IsDelayTrigger)
				{
					UniTaskUtil.DelayedCall(this, actionDialogue.Delay, () =>
					{
						NetworkGameManager.Instance.ownPlayer.angleRot = actionDialogue.PlayerDirection;
					}).Forget();
				}
				else
				{
					NetworkGameManager.Instance.ownPlayer.angleRot = actionDialogue.PlayerDirection;
				}
				NextDialogue(actionDialogue);
				break;
			case ActionDialogueType.CHANGE_CAMERA_TARGET_TO_NPC:
				if ((bool)GetCharDialogID(actionDialogue.CharName).TransformPosition)
				{
					CameraGame.Instance.RemoveAllMember();
					CameraGame.Instance.CinemachineTarget.AddMember(GetCharDialogID(actionDialogue.CharName).TransformPosition, 1f, 3f);
				}
				NextDialogue(actionDialogue);
				break;
			case ActionDialogueType.CHANGE_CAMERA_TARGET_TO_PLAYER:
				CameraGame.Instance.RemoveAllMember();
				CameraGame.Instance.CinemachineTarget.AddMember(NetworkGameManager.Instance.ownPlayer.transform, 1f, 3f);
				NextDialogue(actionDialogue);
				break;
			case ActionDialogueType.CUSTOM_ACTION:
				if ((bool)actionDialogue.CustomAction)
				{
					actionDialogue.CustomAction?.Invoke(() =>
					{
						NextDialogue(actionDialogue);
					});
				}
				else
				{
					NextDialogue(actionDialogue);
				}
				break;
			case ActionDialogueType.SHOW_UI:
				break;
			}
		}
		else
		{
			EndDialogue();
		}
		void NextDialogue(ActionDialogue actionDialogue2)
		{
			_idxActionDialogue++;
			if (_idxActionDialogue <= _dialogue.ListActionDialogue.Count - 1)
			{
				_name.text = LocalizationManager.GetTranslation("Interaction/" + actionDialogue2.CharName);
				if (!actionDialogue2.IsDelayTrigger)
				{
					UniTaskUtil.DelayedCall(this, actionDialogue2.Delay, TriggerActionDialogue).Forget();
				}
				else
				{
					TriggerActionDialogue();
				}
			}
			else
			{
				EndDialogue();
			}
		}
	}

	private void SkipDialogue()
	{
		_iconDown.SetActive(value: false);
		for (int i = _idxActionDialogue; i < _dialogue.ListActionDialogue.Count; i++)
		{
			if (_dialogue.ListActionDialogue[i].ActionType == ActionDialogueType.CHANGE_CAMERA_TARGET_TO_NPC)
			{
				if ((bool)GetCharDialogID(_dialogue.ListActionDialogue[i].CharName).TransformPosition)
				{
					CameraGame.Instance.RemoveAllMember();
					CameraGame.Instance.CinemachineTarget.AddMember(GetCharDialogID(_dialogue.ListActionDialogue[i].CharName).TransformPosition, 1f, 3f);
				}
			}
			else if (_dialogue.ListActionDialogue[i].ActionType == ActionDialogueType.CHANGE_CAMERA_TARGET_TO_PLAYER)
			{
				CameraGame.Instance.RemoveAllMember();
				CameraGame.Instance.CinemachineTarget.AddMember(NetworkGameManager.Instance.ownPlayer.transform, 1f, 3f);
			}
			else if (_dialogue.ListActionDialogue[i].CantSkip || _dialogue.ListActionDialogue[i].ActionType == ActionDialogueType.CUSTOM_ACTION)
			{
				AudioManager.PlaySFX(_sfxShowAllMessage);
				_dialogueText.DOKill(complete: true);
				_idxActionDialogue = i;
				TriggerActionDialogue();
				break;
			}
			_idxActionDialogue = i + 1;
		}
		if (_idxActionDialogue >= _dialogue.ListActionDialogue.Count)
		{
			if (DOTween.IsTweening(_dialogueText))
			{
				AudioManager.PlaySFX(_sfxShowAllMessage);
				_dialogueText.DOKill(complete: true);
			}
			EndDialogue();
		}
	}

	private void EndDialogue()
	{
		Debug.Log("EndDialog");
		if (!NetworkGameManager.Instance.ownPlayer.network.playerPhoton.IsDialogueOnboardingNPCShowed)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				NetworkGameManager.Instance.ownPlayer.network.playerPhoton.IsDialogueOnboardingNPCShowed = true;
			}
			else
			{
				NetworkGameManager.Instance.ownPlayer.network.playerPhoton.RPCDialogueOnboardingNPCShowed();
			}
		}
		holdTimeSkip = 0f;
		_waitPress = false;
		GlobalSaveData.instance.dialogueOnboardingShowed = true;
		_iconDown.SetActive(value: false);
		_dialogueBox.SetActive(value: false);
		if (_isShowBlackBar)
		{
			_blackBarTop.DOScaleY(0f, _speedBlackbar / 2f);
			_blackBarBot.DOScaleY(0f, _speedBlackbar / 2f);
		}
		UniTaskUtil.DelayedCall(this, _speedBlackbar / 2f, () =>
		{
			IsFinishedIntroDialogue = true;
			NetworkGameManager.Instance.ownPlayer.DelayInputTimer.StartDuration(0.5f);
			UIGameManager.Instance.uiInGame.Show();
			NetworkGameManager.Instance.ownPlayer.network.SetEnableControl(value: true);
			_uiView.Hide();
			if (!GlobalSaveData.instance.optionData.SkipIntroControl || !GlobalSaveData.instance.optionData.IsFirstTimeControlShowed)
			{
				LobbyManager.Instance.UIHintControl.Show();
			}
			UIGameManager.Instance.ArrPlayerInfo[NetworkGameManager.Instance.ownPlayer.network.GetIDX()].DialogueObject.SetActive(value: true);
			UIGameManager.Instance.ArrPlayerInfo[NetworkGameManager.Instance.ownPlayer.network.GetIDX()].BotDialogueObject.SetActive(value: true);
		}).Forget();
	}

	private void Update()
	{
		if (_uiView.isHidden || _cantSkip)
		{
			return;
		}
		if ((Gamepad.current != null && (Gamepad.current.buttonWest.wasPressedThisFrame || Gamepad.current.buttonNorth.wasPressedThisFrame || Gamepad.current.buttonSouth.wasPressedThisFrame || Gamepad.current.buttonEast.wasPressedThisFrame)) || Input.anyKeyDown)
		{
			isHoldingSkip = true;
			holdTimeSkip = 0f;
			if (!_waitPress && DOTween.IsTweening(_dialogueText))
			{
				AudioManager.PlaySFX(_sfxShowAllMessage);
				_dialogueText.DOKill(complete: true);
				_iconDown.SetActive(value: true);
			}
			else if (_waitPress)
			{
				AudioManager.PlaySFX(_sfxNextMessage);
				if (!_dialogue.ListActionDialogue[_idxActionDialogue].IsDelayTrigger)
				{
					UniTaskUtil.DelayedCall(this, _dialogue.ListActionDialogue[_idxActionDialogue].Delay, TriggerActionDialogue).Forget();
				}
				else
				{
					TriggerActionDialogue();
				}
				_waitPress = false;
				_iconDown.SetActive(value: false);
			}
		}
		else if ((Gamepad.current != null && (Gamepad.current.buttonWest.wasReleasedThisFrame || Gamepad.current.buttonNorth.wasPressedThisFrame || Gamepad.current.buttonSouth.wasReleasedThisFrame || Gamepad.current.buttonEast.wasReleasedThisFrame)) || !Input.anyKey)
		{
			isHoldingSkip = false;
		}
		if (isHoldingSkip)
		{
			holdTimeSkip += Time.deltaTime;
			if (holdTimeSkip >= 1.2f)
			{
				SkipDialogue();
				isHoldingSkip = false;
			}
		}
	}
}
