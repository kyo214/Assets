using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Doozy.Runtime.UIManager.Components;
using Fusion;
using TMPro;
using Toked.Skill.UI;
using UnityEngine;
using UnityEngine.UI;
using _Modules.CharacterSkin.Scripts;
using _Modules.UIGlobal;
using _Modules.UIInGame.Scripts;

public class SurvivorLobby : MonoBehaviour
{
	public CharacterAvatarUIController PlayerAvatar;

	public TextMeshProUGUI PlayerName;

	[SerializeField]
	private TextMeshProUGUI _textHp;

	[SerializeField]
	private TextMeshProUGUI _textStamina;

	public Toggle CheckboxReady;

	public GameObject LabelReady;

	public GameObject LabelNotReady;

	public GameObject LabelDisconnected;

	public GameObject boardReady;

	public Image Weapon0;

	public Image Weapon1;

	public List<Image> itemInventory = new List<Image>();

	public UIButton BtnKick;

	public UIButton BtnWarn;

	public GameObject BtnWarnDisable;

	public PlayerController Player;

	[SerializeField]
	private UIPerkStatusController _perkStatusController;

	[SerializeField]
	private UISkillStatusController _uiSkillStatusController;

	public void Init(PlayerController playerController, int i)
	{
		Player = playerController;
		if (playerController.network.playerPhoton.Object == null)
		{
			return;
		}
		if (Player.network.GetReadyLobby() && !Player.network.isLocalPlayer)
		{
			BtnWarn.gameObject.SetActive(value: false);
			BtnWarnDisable.gameObject.SetActive(value: true);
		}
		PlayerName.text = playerController.network.GetPlayerName();
		_textHp.text = playerController.network.GetHealth().ToString(CultureInfo.InvariantCulture) + "/" + playerController.data.GetMaxHealth().ToString(CultureInfo.InvariantCulture);
		_textStamina.text = playerController.data.GetMaxStamina().ToString(CultureInfo.InvariantCulture) + "/" + playerController.data.GetCurrentMaxStamina().ToString(CultureInfo.InvariantCulture);
		ChangeAvatar(playerController.data.PlayerSkinData);
		bool readyLobby = playerController.network.GetReadyLobby();
		CheckboxReady.isOn = readyLobby;
		LabelReady.SetActive(readyLobby);
		boardReady.SetActive(readyLobby);
		LabelNotReady.SetActive(!readyLobby);
		if ((bool)playerController.network.playerPhoton.disconnected)
		{
			LabelReady.SetActive(value: false);
			LabelNotReady.SetActive(value: false);
			LabelDisconnected.SetActive(value: true);
		}
		else
		{
			LabelDisconnected.SetActive(value: false);
		}
		Weapon0.enabled = true;
		Weapon1.enabled = true;
		if (playerController.network.GetIdWeapon0() > 0)
		{
			Weapon0.sprite = DataManager.Instance.GetItemSprite(playerController.network.GetIdWeapon0().ToString());
		}
		else
		{
			Weapon0.enabled = false;
		}
		if (NetworkGameManager.Instance.arrPlayerNetworkController[i].network.GetIdWeapon1() > 0)
		{
			Weapon1.sprite = DataManager.Instance.GetItemSprite(playerController.network.GetIdWeapon1().ToString());
		}
		else
		{
			Weapon1.enabled = false;
		}
		for (int j = 0; j < 10; j++)
		{
			if (j < NetworkGameManager.Instance.arrPlayerNetworkController[i].data.GetMaxInventory() - 2)
			{
				itemInventory[j].gameObject.SetActive(value: false);
			}
		}
		int num = 0;
		for (int k = 2; k < NetworkGameManager.Instance.arrPlayerNetworkController[i].data.arrInventory.Count; k++)
		{
			InventoryObject inventoryObject = playerController.data.arrInventory[k];
			if (inventoryObject.Name != "Null" && inventoryObject.ID != -1)
			{
				itemInventory[num].gameObject.SetActive(value: true);
				itemInventory[num].color = new Color(1f, 1f, 1f, 1f);
				itemInventory[num].sprite = DataManager.Instance.GetItemSprite(inventoryObject.ID.ToString());
				num++;
			}
			else if (num < NetworkGameManager.Instance.arrPlayerNetworkController[i].data.GetMaxInventory() - 2)
			{
				itemInventory[num].gameObject.SetActive(value: false);
			}
		}
		_perkStatusController?.Init(Player);
		_uiSkillStatusController?.Init(Player);
	}

	public void KickPlayer()
	{
		GenericSingleton<PopupUIManager>.Instance.Show(PopupUIManager.Type.YesNo, "Menu/ConfirmKick", () =>
		{
			bool flag = false;
			foreach (PlayerRef activePlayer in PhotonMultiplayerManager.Instance._runner.ActivePlayers)
			{
				if (PhotonMultiplayerManager.Instance._runner.GetPlayerObject(activePlayer) == Player.network.networkObj)
				{
					PlayerTempInventory item = new PlayerTempInventory
					{
						DeviceID = Player.network.playerPhoton.PlayerDeviceID,
						ArrInventory = Player.data.arrInventory.ToList()
					};
					NetworkGameManager.Instance.ListPlayerTempInventory.Add(item);
					Player.network.playerPhoton.isQuitGame = true;
					NetworkGameManager.Instance.arrPlayerController.Remove(Player);
					for (int i = 0; i < NetworkGameManager.Instance.arrPlayerNetworkController.Count; i++)
					{
						if (NetworkGameManager.Instance.arrPlayerNetworkController[i] == Player)
						{
							NetworkGameManager.Instance.arrPlayerNetworkController[i] = null;
						}
					}
					UIGameManager.Instance.RefreshPlayerCountText();
					SurvivorLobbyManager.Instance.ShowBoard();
					NetworkGameManager.Instance.SpawnedCharacters.Remove(activePlayer);
					Player.network.playerPhoton.isKicked = true;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Debug.Log("Player Kicked");
				PlayerTempInventory item2 = new PlayerTempInventory
				{
					DeviceID = Player.network.playerPhoton.PlayerDeviceID,
					ArrInventory = Player.data.arrInventory.ToList()
				};
				NetworkGameManager.Instance.ListPlayerTempInventory.Add(item2);
				LobbyManager.Instance.KickPlayerDisconnected(Player);
			}
		});
	}

	public void WarnPlayer()
	{
		NetworkGameManager.Instance.ownPlayer.network.ShowBaloonChat(ChatType.CHAT_W_HURRY_UP, -1, -1, -1, -1, Player.network.GetIDX());
	}

	public void SelectButton(UIButton button)
	{
		button.Select();
	}

	public void ChangeAvatar(PlayerSkinData playerSkinData)
	{
		PlayerAvatar.ChangeHeadAvatarSprite(playerSkinData.GetHeadSkinAvatar());
		PlayerAvatar.ChangeBodyAvatarSprite(playerSkinData.GetBodySkinAvatar());
	}
}
