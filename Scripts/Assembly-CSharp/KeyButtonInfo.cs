using Cysharp.Threading.Tasks;
using Doozy.Runtime.Common.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class KeyButtonInfo : MonoBehaviour
{
	[SerializeField]
	private GameObject keyboardInfo;

	[SerializeField]
	private GameObject gamepadInfo;

	[SerializeField]
	private Animator keyboardButton;

	[SerializeField]
	private Animator gamepadButton;

	[SerializeField]
	private ActionPlayerType actionName;

	[SerializeField]
	private TextMeshProUGUI txtKey;

	[SerializeField]
	private TextMeshProUGUI txtName;

	[SerializeField]
	private bool isShowOnlyGamepad;

	[SerializeField]
	private bool isShowOnlyKeyboard;

	[SerializeField]
	private bool isUIOnly;

	private void Start()
	{
		if (isShowOnlyGamepad)
		{
			keyboardInfo.SetActive(value: false);
		}
		GlobalOptionsManager.Instance.arrKeyButtonInfo.Add(this);
		DeviceChange();
	}

	private void OnEnable()
	{
		DeviceChange();
	}

	private void OnDestroy()
	{
		GlobalOptionsManager.Instance.arrKeyButtonInfo.Remove(this);
	}

	public void DeviceChange()
	{
		if (!(GlobalOptionsManager.Instance != null))
		{
			return;
		}
		if (GlobalOptionsManager.Instance.usingGamepad)
		{
			if (isShowOnlyKeyboard)
			{
				gamepadInfo.SetActive(value: false);
				txtName?.gameObject.SetActive(value: false);
			}
			else
			{
				gamepadInfo.SetActive(value: true);
				txtName?.gameObject.SetActive(value: true);
			}
			keyboardInfo.SetActive(value: false);
		}
		else
		{
			if (isShowOnlyGamepad)
			{
				keyboardInfo.SetActive(value: false);
			}
			else
			{
				keyboardInfo.SetActive(value: true);
			}
			gamepadInfo.SetActive(value: false);
			txtName?.gameObject.SetActive(value: false);
		}
		if (isUIOnly)
		{
			return;
		}
		string text = "";
		switch (actionName)
		{
		case ActionPlayerType.Left:
			text = GlobalOptionsManager.Instance.leftName;
			break;
		case ActionPlayerType.Right:
			text = GlobalOptionsManager.Instance.rightName;
			break;
		case ActionPlayerType.Up:
			text = GlobalOptionsManager.Instance.upName;
			break;
		case ActionPlayerType.Down:
			text = GlobalOptionsManager.Instance.downName;
			break;
		case ActionPlayerType.Inventory:
			text = GlobalOptionsManager.Instance.inventoryActionName;
			break;
		case ActionPlayerType.Throw:
			text = GlobalOptionsManager.Instance.throwActionName;
			break;
		case ActionPlayerType.Heal:
			text = GlobalOptionsManager.Instance.healActionName;
			break;
		case ActionPlayerType.TabKill:
			text = GlobalOptionsManager.Instance.tabKillActionName;
			break;
		case ActionPlayerType.Interact:
			text = GlobalOptionsManager.Instance.interactionActionName;
			break;
		case ActionPlayerType.Ready:
			text = GlobalOptionsManager.Instance.readActionName;
			break;
		case ActionPlayerType.Cancel:
			text = ((!GlobalOptionsManager.Instance.usingGamepad) ? GlobalOptionsManager.Instance.interactionActionName : GlobalOptionsManager.Instance.cancelName);
			break;
		case ActionPlayerType.LeftTab:
			text = GlobalOptionsManager.Instance.leftTabActionName;
			break;
		case ActionPlayerType.RightTab:
			text = GlobalOptionsManager.Instance.rightTabActionName;
			break;
		case ActionPlayerType.Copy:
			text = GlobalOptionsManager.Instance.copyActionName;
			break;
		case ActionPlayerType.ShowCode:
			text = GlobalOptionsManager.Instance.showCodeActionName;
			break;
		case ActionPlayerType.Map:
			text = GlobalOptionsManager.Instance.openMapActionName;
			break;
		case ActionPlayerType.Voice:
			text = GlobalOptionsManager.Instance.voiceActionName;
			break;
		case ActionPlayerType.Attack:
			text = GlobalOptionsManager.Instance.AttackActionName;
			break;
		case ActionPlayerType.Aim:
			text = GlobalOptionsManager.Instance.AimActionName;
			break;
		case ActionPlayerType.Dash:
			text = GlobalOptionsManager.Instance.DashActionName;
			break;
		case ActionPlayerType.Sprint:
			text = GlobalOptionsManager.Instance.SprintActionName;
			break;
		case ActionPlayerType.Reload:
			text = GlobalOptionsManager.Instance.ReloadActionName;
			break;
		case ActionPlayerType.ChatWheel:
			text = GlobalOptionsManager.Instance.chatWheelActionName;
			break;
		case ActionPlayerType.RotateLeft:
			text = GlobalOptionsManager.Instance.RotateLeftActionName;
			break;
		case ActionPlayerType.RotateRight:
			text = GlobalOptionsManager.Instance.RotateRightActionName;
			break;
		case ActionPlayerType.ChangeWeapon:
			text = GlobalOptionsManager.Instance.changeWeaponActionName;
			break;
		case ActionPlayerType.CombineItem:
			text = GlobalOptionsManager.Instance.CombineItemActionName;
			break;
		case ActionPlayerType.DropItem:
			text = GlobalOptionsManager.Instance.DropItemActionName;
			break;
		case ActionPlayerType.Skip:
			text = GlobalOptionsManager.Instance.cancelName;
			break;
		}
		if (GlobalOptionsManager.Instance.usingGamepad)
		{
			if (Gamepad.current is DualShockGamepad)
			{
				text += " 0";
			}
			gamepadButton.enabled = true;
			if (gamepadButton.isActiveAndEnabled)
			{
				gamepadButton.Play(text);
				UniTaskUtil.DelayedCall(this, 0.1f, () =>
				{
					if (gamepadButton != null)
					{
						gamepadButton.enabled = false;
					}
				}).Forget();
			}
		}
		else if (txtKey != null)
		{
			if (text.IndexOf("Shift") >= 0)
			{
				text = text.Replace("Left ", "");
				text = text.Replace("Right ", "");
			}
			else if (text.IndexOf("Button") >= 0)
			{
				if (text.IndexOf("Left") >= 0)
				{
					text = "LMB";
				}
				else if (text.IndexOf("Right") >= 0)
				{
					text = "RMB";
				}
				else if (text.IndexOf("Middle") >= 0)
				{
					text = "Mid MB";
				}
			}
			else if (text.Length > 5)
			{
				text = text.Left(3);
			}
			txtKey.text = text;
		}
		if (GlobalOptionsManager.Instance.usingGamepad || !keyboardButton || !keyboardButton.isActiveAndEnabled)
		{
			return;
		}
		keyboardButton.enabled = true;
		if (txtKey.text.Length > 1)
		{
			keyboardButton.Play("Key1");
		}
		else
		{
			keyboardButton.Play("Key0");
		}
		UniTaskUtil.DelayedCall(this, 0.1f, () =>
		{
			if (keyboardButton != null)
			{
				keyboardButton.enabled = false;
			}
		}).Forget();
	}
}
