using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using I2.Loc;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.UI;

public class ChatSystem : MonoBehaviour
{
	public Localize LabelTermItemCommand;

	public TextMeshProUGUI TextItemCommand;

	public GameObject ItemCommand;

	public GameObject ObjectEscape;

	public Localize TextNameEscape;

	public TextMeshProUGUI TextCountEscape;

	[SerializeField]
	private List<Image> _dialogueBox = new List<Image>();

	[SerializeField]
	private List<Image> _botDialogueBox = new List<Image>();

	public List<GameObject> IconSpeaking = new List<GameObject>();

	public List<TextMeshProUGUI> ListChatPlayers = new List<TextMeshProUGUI>();

	[SerializeField]
	private GameObject _chatLog;

	[SerializeField]
	private List<TextMeshProUGUI> _listChatLog = new List<TextMeshProUGUI>();

	[SerializeField]
	private XTimer timerInvisible;

	[SerializeField]
	private XTimer timerDelay;

	public XTimer timerCountdown;

	public ItemInteractable ItemInteractableCountdown;

	[SerializeField]
	private TextMeshProUGUI _txtDialogue;

	[SerializeField]
	private ChatType prevChatType;

	[SerializeField]
	private Image iconItem;

	[SerializeField]
	private Sprite _iconSpritePickable;

	[SerializeField]
	private Sprite _iconSpriteInteractable;

	[SerializeField]
	private Sprite _iconSpriteNotes;

	[SerializeField]
	private Sprite _iconSpriteMaterial;

	[SerializeField]
	private Sprite _iconSpriteInspect;

	[SerializeField]
	private Sprite _iconSpriteCrafting;

	[SerializeField]
	private Sprite _iconSpriteWardrobe;

	[SerializeField]
	private Sprite _iconSpriteItemBox;

	[SerializeField]
	private Sprite _iconSpriteTransporter;

	[SerializeField]
	private Sprite _iconSpriteMission;

	[SerializeField]
	private bool _isStaticChat;

	public int SyncingTimerCountdown;

	public static ChatSystem Instance { get; private set; }

	public bool IsStaticChat
	{
		get
		{
			return _isStaticChat;
		}
		set
		{
			_isStaticChat = value;
		}
	}

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
		_chatLog.SetActive(value: false);
	}

	private void FixedUpdate()
	{
		if (timerInvisible.isCompleted())
		{
			_chatLog.SetActive(value: false);
			for (int i = 0; i < _listChatLog.Count; i++)
			{
				_listChatLog[i].text = "";
			}
		}
		if (timerCountdown.isRunning)
		{
			TextCountEscape.text = MathFunc.GetMinute(timerCountdown.interval).ToString("D2") + ":" + MathFunc.GetSecond(timerCountdown.interval).ToString("D2");
			if ((bool)GameManagerPhoton.Instance.CurrentMission && GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsCarRepairingOnStart)
			{
				UIMissionObjective.Instance.TextEscape.text = LocalizationManager.GetTranslation("Menu/RepairingEngine") + " <color=white>(" + TextCountEscape.text + ")</color>";
			}
			if (NetworkGameManager.Instance.isServer && timerCountdown.interval <= (float)SyncingTimerCountdown && timerCountdown.interval > 0f)
			{
				SyncingTimerCountdown = Mathf.RoundToInt(timerCountdown.interval) - 10;
				GameManagerPhoton.Instance?.RpcSyncTimeIntervalCountdown((short)(timerCountdown.interval * 10f));
			}
		}
		if (!timerCountdown.isCompleted())
		{
			return;
		}
		if ((bool)ItemInteractableCountdown.ObjectActiveAfterComplete)
		{
			ItemInteractable itemInteractable = ItemInteractableCountdown.ObjectActiveAfterComplete.GetComponentInChildren<ItemInteractable>();
			if (itemInteractable == null)
			{
				itemInteractable = ItemInteractableCountdown.ObjectActiveAfterComplete.GetComponent<ItemInteractable>();
			}
			if (itemInteractable != null)
			{
				ItemInteractableCountdown.ObjectActiveAfterComplete.SetActive(value: true);
				itemInteractable.boxCollider.enabled = true;
				ItemInteractableCountdown.boxCollider.enabled = false;
			}
			else
			{
				ItemInteractableCountdown.ObjectActiveAfterComplete.SetActive(value: true);
			}
			if ((bool)ItemInteractableCountdown.ObjectActiveSpecial)
			{
				ItemInteractableCountdown.ObjectActiveSpecial.SetActive(value: false);
			}
			if (ItemInteractableCountdown.ShowCountdownLabelBeforeComplete)
			{
				if (NetworkGameManager.Instance.arrPlayerController.Count > 1)
				{
					TextNameEscape.SetTerm("Menu/AgentsInCircle");
					MissionManager.Instance.IsCountAgentInCircle = true;
				}
				else
				{
					ObjectEscape.SetActive(value: false);
				}
			}
		}
		if ((bool)ItemInteractableCountdown.ObjectInactiveAfterComplete)
		{
			ItemInteractableCountdown.ObjectInactiveAfterComplete.layer = 1;
			ItemInteractableCountdown.ObjectInactiveAfterComplete.SetActive(value: false);
		}
	}

	public void InstantHideDialogueBox(int playerIdx)
	{
		_dialogueBox[playerIdx].DOKill();
		_dialogueBox[playerIdx].DOFade(0f, 0f);
		_botDialogueBox[playerIdx].DOKill();
		_botDialogueBox[playerIdx].DOFade(0f, 0f);
	}

	public void HideBaloonChatMonologue(PlayerController player, float timeFading = 0.2f)
	{
		if (ListChatPlayers[player.network.GetIDX()].gameObject.activeSelf)
		{
			_dialogueBox[player.network.GetIDX()].DOKill();
			_dialogueBox[player.network.GetIDX()].DOFade(0f, timeFading).OnComplete(() =>
			{
				ListChatPlayers[player.network.GetIDX()].gameObject.SetActive(value: false);
			});
			_botDialogueBox[player.network.GetIDX()].DOKill();
			_botDialogueBox[player.network.GetIDX()].DOFade(0f, timeFading);
			_isStaticChat = false;
		}
	}

	public void HideBaloonChat(PlayerController player, ItemInteractable item = null)
	{
		if (!(item != null))
		{
			return;
		}
		item.timerDelay.StartDuration(1f);
		if (player != null)
		{
			if (player.itemCollision != null)
			{
				player.itemCollision = null;
			}
			player.functionItemCollision = "";
		}
		item.labelItemCommandOff = false;
		ItemCommand.SetActive(value: false);
	}

	public void ShowBaloonChat(int playerID, ChatType chatType, short itemID, short itemID2 = -1, short itemID3 = -1, short UIDItem1 = -1, int targetPlayerID = 10, bool alwaysShowChat = false)
	{
		if (UIGameManager.Instance.isUIInvisible || timerDelay.isRunning || !(!_isStaticChat | alwaysShowChat))
		{
			return;
		}
		if (prevChatType == chatType && chatType >= ChatType.CHAT_W_REGROUP)
		{
			timerDelay.StartDuration(2f);
			prevChatType = ChatType.EMPTY;
			return;
		}
		if (prevChatType == chatType && chatType >= ChatType.CHAT_W_REGROUP)
		{
			timerDelay.StartDuration(2f);
		}
		prevChatType = chatType;
		string itemType = DataManager.Instance.GetItemType(itemID);
		string itemType2 = DataManager.Instance.GetItemType(itemID2);
		string itemType3 = DataManager.Instance.GetItemType(itemID3);
		string text = "";
		string text2 = "";
		string text3 = "";
		PlayerController player = NetworkGameManager.Instance.GetPlayer(playerID);
		string roomName = player.RoomName;
		roomName = LocalizationManager.GetTranslation("Locations/" + roomName);
		switch (chatType)
		{
		case ChatType.NEED_2_ITEM:
		case ChatType.NEED_3_ITEM:
		case ChatType.USE_ITEM:
			if (itemID != -1)
			{
				text = ((DataManager.Instance.GetValueDatabase(itemType, itemID, "Alias") == null) ? LocalizationManager.GetTranslation(itemType + "/" + itemType + itemID) : LocalizationManager.GetTranslation(itemType + "/" + itemType + "Alias" + itemID));
			}
			if (itemID2 != -1)
			{
				text2 = ((DataManager.Instance.GetValueDatabase(itemType, itemID2, "Alias") == null) ? LocalizationManager.GetTranslation(itemType2 + "/" + itemType2 + itemID2) : LocalizationManager.GetTranslation(itemType2 + "/" + itemType2 + "Alias" + itemID2));
			}
			if (itemID3 != -1 && chatType == ChatType.NEED_3_ITEM)
			{
				text3 = ((DataManager.Instance.GetValueDatabase(itemType, itemID3, "Alias") == null) ? LocalizationManager.GetTranslation(itemType3 + "/" + itemType3 + itemID3) : LocalizationManager.GetTranslation(itemType3 + "/" + itemType3 + "Alias" + itemID3));
			}
			break;
		case ChatType.LOCKED:
		{
			ItemInteractable itemInteractable = GameManager.Instance.GetItemInteractable(UIDItem1);
			if (itemInteractable != null && itemInteractable.objectName != "")
			{
				text = LocalizationManager.GetTranslation("Interaction/" + itemInteractable.objectName);
			}
			break;
		}
		default:
			if (itemID != -1)
			{
				text = LocalizationManager.GetTranslation(itemType + "/" + itemType + itemID);
			}
			if (itemID2 != -1)
			{
				text2 = LocalizationManager.GetTranslation(itemType2 + "/" + itemType2 + itemID2);
			}
			break;
		}
		if (text == null)
		{
			text = "";
		}
		if (text2 == null)
		{
			text2 = "";
		}
		if (text3 == null)
		{
			text3 = "";
		}
		string text4 = "";
		string text5 = "";
		switch (chatType)
		{
		case ChatType.GOT_ITEM:
			text4 = LocalizationManager.GetTranslation("Interaction/IntGotItem");
			if (itemType == "Weapon")
			{
				text = LocalizationManager.GetTranslation("Weapon/Weapon" + DataManager.Instance.GetBaseWeapon(itemID));
				string attachedWeaponName = UIGameManager.Instance.GetAttachedWeaponName(itemID);
				if (attachedWeaponName != "")
				{
					text += attachedWeaponName;
				}
			}
			text4 = text4.Replace("[-]", text);
			if (UIDItem1 >= 0)
			{
				ItemPickable itemPickable = GameManager.Instance.GetItemPickable(UIDItem1);
				if (itemPickable.IsCursedItem)
				{
					text4 = text4 + " " + itemPickable.ItemIntractableStatusEffect.GetEffectLocalization("", isNewLine: false, IsusingBrackets: true);
				}
			}
			break;
		case ChatType.LOCKED:
			if (text != "")
			{
				text4 = LocalizationManager.GetTranslation("Interaction/IntLocked");
				text4 = text4.Replace("[-]", text);
			}
			else
			{
				text4 = LocalizationManager.GetTranslation("Interaction/IntLocked2");
			}
			break;
		case ChatType.UNLOCKED:
			if (text3 != "")
			{
				text4 = LocalizationManager.GetTranslation("Interaction/IntUnlocked3");
				text4 = text4.Replace("[1]", text);
				text4 = text4.Replace("[2]", text2);
				text4 = text4.Replace("[3]", text3);
			}
			else if (text2 != "")
			{
				text4 = LocalizationManager.GetTranslation("Interaction/IntUnlocked2");
				text4 = text4.Replace("[1]", text);
				text4 = text4.Replace("[2]", text2);
			}
			else
			{
				text4 = LocalizationManager.GetTranslation("Interaction/IntUnlocked");
				text4 = text4.Replace("[1]", text);
			}
			text5 = text4;
			break;
		case ChatType.NOT_ENOUGH_RES:
			text4 = LocalizationManager.GetTranslation("Interaction/IntNotEnoughRes");
			text4 = text4.Replace("[-]", text);
			break;
		case ChatType.ONVENTORY_FULL:
			text4 = LocalizationManager.GetTranslation("Interaction/IntInvFull");
			break;
		case ChatType.LOCKED_OTHER_SIDE:
			text4 = LocalizationManager.GetTranslation("Interaction/IntLockedOtherSide");
			break;
		case ChatType.ELECTRICTY_OBSTACLE:
			text4 = LocalizationManager.GetTranslation("Interaction/IntElectricity");
			break;
		case ChatType.MONOLOGUE:
			text4 = LocalizationManager.GetTranslation("Interaction/Monologue-" + itemID);
			break;
		case ChatType.TUTORIAL:
		{
			text4 = LocalizationManager.GetTranslation("Interaction/Monologue-" + itemID);
			if (GlobalOptionsManager.Instance.usingGamepad)
			{
				string text6 = GlobalOptionsManager.Instance.ConvertInputName(GlobalOptionsManager.Instance.AttackActionName);
				string text7 = GlobalOptionsManager.Instance.ConvertInputName(GlobalOptionsManager.Instance.AimActionName);
				string text8 = GlobalOptionsManager.Instance.ConvertInputName("Left Stick");
				string text9 = GlobalOptionsManager.Instance.ConvertInputName(GlobalOptionsManager.Instance.SprintActionName);
				string text10 = GlobalOptionsManager.Instance.ConvertInputName(GlobalOptionsManager.Instance.DashActionName);
				text4 = text4.Replace("[1]", "<color=blue>[" + text6 + "]</color>");
				text4 = text4.Replace("[2]", "<color=blue>[" + text7 + "]</color>");
				text4 = text4.Replace("[3]", "<color=blue>[" + text8 + "]</color>");
				text4 = text4.Replace("[4]", "<color=blue>[" + text9 + "]</color>");
				text4 = text4.Replace("[5]", "<color=blue>[" + text10 + "]</color>");
				break;
			}
			string text11 = GlobalOptionsManager.Instance.ConvertInputName(GlobalOptionsManager.Instance.upName);
			string text12 = GlobalOptionsManager.Instance.ConvertInputName(GlobalOptionsManager.Instance.leftName);
			string text13 = GlobalOptionsManager.Instance.ConvertInputName(GlobalOptionsManager.Instance.downName);
			string text14 = GlobalOptionsManager.Instance.ConvertInputName(GlobalOptionsManager.Instance.rightName);
			string text15 = "[" + text11 + "] [" + text12 + "] [" + text13 + "] [" + text14 + "]";
			string text16 = GlobalOptionsManager.Instance.ConvertInputName(GlobalOptionsManager.Instance.SprintActionName);
			string text17 = GlobalOptionsManager.Instance.ConvertInputName(GlobalOptionsManager.Instance.DashActionName);
			text4 = text4.Replace("[1]", "<color=blue>[LMB]</color>");
			text4 = text4.Replace("[2]", "<color=blue>[RMB]</color>");
			text4 = text4.Replace("[3]", "<color=blue>" + text15 + "</color>");
			text4 = text4.Replace("[4]", "<color=blue>[" + text16 + "]</color>");
			text4 = text4.Replace("[5]", "<color=blue>[" + text17 + "]</color>");
			break;
		}
		case ChatType.REPAIR_TIME_DECREASED:
			text4 = LocalizationManager.GetTranslation("Interaction/IntRepairTimeDecreased");
			break;
		case ChatType.HELP_ME:
			text4 = LocalizationManager.GetTranslation("Interaction/IntHelp");
			break;
		case ChatType.NEED_2_ITEM:
			if (text2 == "")
			{
				text4 = LocalizationManager.GetTranslation("Interaction/IntNeedItem");
				text4 = text4.Replace("[1]", text);
			}
			else
			{
				text4 = LocalizationManager.GetTranslation("Interaction/IntNeed2Item");
				text4 = text4.Replace("[1]", text);
				text4 = text4.Replace("[2]", text2);
			}
			break;
		case ChatType.USE_ITEM:
			if (text2 == "")
			{
				text4 = LocalizationManager.GetTranslation("Interaction/IntUse");
				text4 = text4.Replace("[1]", text);
			}
			else
			{
				text4 = LocalizationManager.GetTranslation("Interaction/IntUse2");
				text4 = text4.Replace("[1]", text);
				text4 = text4.Replace("[2]", text2);
			}
			break;
		case ChatType.NEED_3_ITEM:
			text4 = LocalizationManager.GetTranslation("Interaction/IntNeed3Item");
			text4 = text4.Replace("[1]", text);
			text4 = text4.Replace("[2]", text2);
			text4 = text4.Replace("[3]", text3);
			break;
		case ChatType.CHAT_W_REGROUP:
			StartCoroutine(ChatSpeak("regroup", playerID));
			text4 = LocalizationManager.GetTranslation("ChatWheel/CWRegroup");
			text5 = ((!(roomName == "") && roomName != null) ? (text4 + " (" + roomName + ")") : text4);
			break;
		case ChatType.CHAT_W_INJURED:
			StartCoroutine(ChatSpeak("medic", playerID));
			text4 = LocalizationManager.GetTranslation("ChatWheel/CWInjured");
			text5 = ((!(roomName == "") && roomName != null) ? (text4 + " (" + roomName + ")") : text4);
			break;
		case ChatType.CHAT_W_ONMYWAY:
			StartCoroutine(ChatSpeak("onmyway", playerID));
			text4 = LocalizationManager.GetTranslation("ChatWheel/CWOnMyWay");
			break;
		case ChatType.CHAT_W_HELP:
			StartCoroutine(ChatSpeak("help", playerID));
			text4 = LocalizationManager.GetTranslation("ChatWheel/CWHelp");
			text5 = ((!(roomName == "") && roomName != null) ? (text4 + " (" + roomName + ")") : text4);
			break;
		case ChatType.CHAT_W_SPREAD:
			StartCoroutine(ChatSpeak("spread", playerID));
			text4 = LocalizationManager.GetTranslation("ChatWheel/CWSpread");
			break;
		case ChatType.CHAT_W_AMMO:
			StartCoroutine(ChatSpeak("ammo", playerID));
			text4 = LocalizationManager.GetTranslation("ChatWheel/CWAmmo");
			text5 = ((!(text == "")) ? (text4 + " (" + text + ")") : text4);
			break;
		case ChatType.CHAT_W_RUNN:
			StartCoroutine(ChatSpeak("run", playerID));
			text4 = LocalizationManager.GetTranslation("ChatWheel/CWRun");
			break;
		case ChatType.CHAT_W_THANKS:
			StartCoroutine(ChatSpeak("thanks", playerID));
			text4 = LocalizationManager.GetTranslation("ChatWheel/CWThanks");
			break;
		case ChatType.CHAT_W_HURRY_UP:
			StartCoroutine(ChatSpeak("getready", playerID));
			text4 = NetworkGameManager.Instance.GetPlayer(targetPlayerID).network.GetPlayerName() + ", " + LocalizationManager.GetTranslation("ChatWheel/CWTHurrUp");
			break;
		}
		if (text5 == "")
		{
			text5 = text4;
		}
		if (chatType == ChatType.HELP_ME)
		{
			ListChatPlayers[playerID].text = text4;
			_dialogueBox[playerID].DOKill();
			_botDialogueBox[playerID].DOKill();
			UniTaskUtil.DelayedCall(this, 1f, () =>
			{
				if (NetworkGameManager.Instance.GetPlayer(playerID).network.GetHealth() <= 0f)
				{
					ListChatPlayers[playerID].gameObject.SetActive(value: true);
					_dialogueBox[playerID].DOFade(1f, 0.1f);
					_botDialogueBox[playerID].DOFade(1f, 0.1f);
				}
			}).Forget();
			_dialogueBox[playerID].DOFade(0f, 0.2f).SetDelay((int)player.network.playerPhoton.reviveTimer).OnComplete(() =>
			{
				ListChatPlayers[playerID].gameObject.SetActive(value: false);
			});
			_botDialogueBox[playerID].DOFade(0f, 0.2f).SetDelay((int)player.network.playerPhoton.reviveTimer).OnComplete(() =>
			{
				ListChatPlayers[playerID].gameObject.SetActive(value: false);
			});
		}
		else
		{
			ListChatPlayers[playerID].text = text4;
			_dialogueBox[playerID].DOKill();
			_botDialogueBox[playerID].DOKill();
			ListChatPlayers[playerID].gameObject.SetActive(value: true);
			_dialogueBox[playerID].DOFade(1f, 0.1f);
			_botDialogueBox[playerID].DOFade(1f, 0.1f);
			if (!alwaysShowChat)
			{
				_dialogueBox[playerID].DOFade(0f, 0.2f).SetDelay(3.5f).OnComplete(() =>
				{
					ListChatPlayers[playerID].gameObject.SetActive(value: false);
				});
				_botDialogueBox[playerID].DOFade(0f, 0.2f).SetDelay(3.5f).OnComplete(() =>
				{
					ListChatPlayers[playerID].gameObject.SetActive(value: false);
				});
			}
			else
			{
				_isStaticChat = true;
			}
		}
		if ((chatType >= ChatType.CHAT_W_REGROUP && chatType < (ChatType)30) || chatType == ChatType.UNLOCKED || chatType == ChatType.GOT_ITEM || chatType == ChatType.NEED_2_ITEM || chatType == ChatType.NEED_3_ITEM)
		{
			ShowChatLog(playerID, text5);
		}
	}

	public void ShowChatLog(int playerID, string chatStr, bool logCanRepeat = true)
	{
		if (GlobalSaveData.instance.optionData.chatLog && !UIGameManager.Instance.isUIInvisible)
		{
			_chatLog.SetActive(value: true);
		}
		if (logCanRepeat || (!logCanRepeat && _listChatLog[0].text != "<color=#ffee34>" + NetworkGameManager.Instance.GetPlayer(playerID).network.GetPlayerName() + "</color>: " + chatStr))
		{
			for (int num = _listChatLog.Count - 1; num > 0; num--)
			{
				_listChatLog[num].text = _listChatLog[num - 1].text;
			}
			string text = "";
			if (chatStr.Contains("[username]"))
			{
				chatStr = chatStr.Replace("[username]", "<color=#ffee34>" + NetworkGameManager.Instance.GetPlayer(playerID).network.GetPlayerName() + "</color> ");
				text = chatStr;
			}
			else
			{
				text = "<color=#ffee34>" + NetworkGameManager.Instance.GetPlayer(playerID).network.GetPlayerName() + "</color>: " + chatStr;
			}
			_listChatLog[0].text = text.Replace("<br>", " ");
		}
		timerInvisible.StartDuration(8f);
	}

	private IEnumerator ChatSpeak(string audioName, int playerID)
	{
		AudioManager.PlaySFX("ui-chatwheel-confirm");
		yield return new WaitForSeconds(0.2f);
		if (!NetworkGameManager.Instance.GetPlayer(playerID).IsMale)
		{
			AudioManager.PlaySFX("hero1-chatwheel-" + audioName);
		}
		else
		{
			AudioManager.PlaySFX("hero0-chatwheel-" + audioName);
		}
	}

	public void ShowMonologue(string id)
	{
		_txtDialogue.DOKill();
		_txtDialogue.gameObject.SetActive(value: true);
		_txtDialogue.DOFade(1f, 0f);
		_txtDialogue.text = LocalizationManager.GetTranslation("Interaction/" + id);
		_txtDialogue.DOFade(0f, 0.5f).SetDelay(5f).OnComplete(() =>
		{
			_txtDialogue.gameObject.SetActive(value: false);
		});
	}

	public void SetIcon(IconItemType iconItemType)
	{
		switch (iconItemType)
		{
		case IconItemType.NOTE:
			iconItem.sprite = _iconSpriteNotes;
			break;
		case IconItemType.PICKABLE:
			iconItem.sprite = _iconSpritePickable;
			break;
		case IconItemType.INSPECT:
			iconItem.sprite = _iconSpriteInspect;
			break;
		case IconItemType.MATERIAL:
			iconItem.sprite = _iconSpriteMaterial;
			break;
		case IconItemType.INTERACTABLE:
			iconItem.sprite = _iconSpriteInteractable;
			break;
		case IconItemType.MISSION:
			iconItem.sprite = _iconSpriteMission;
			break;
		case IconItemType.CRAFTING:
			iconItem.sprite = _iconSpriteCrafting;
			break;
		case IconItemType.ITEM_BOX:
			iconItem.sprite = _iconSpriteItemBox;
			break;
		case IconItemType.TRANSPORTER:
			iconItem.sprite = _iconSpriteTransporter;
			break;
		case IconItemType.WARDROBE:
			iconItem.sprite = _iconSpriteWardrobe;
			break;
		}
	}
}
