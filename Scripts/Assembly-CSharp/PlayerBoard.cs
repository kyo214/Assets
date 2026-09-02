using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Modules.CharacterSkin.Scripts;
using _Modules.UIInGame.Scripts;

public class PlayerBoard : MonoBehaviour
{
	[SerializeField]
	private List<UITabPlayer> _uiTabPlayers = new List<UITabPlayer>();

	[SerializeField]
	private List<CharacterAvatarUIController> playerAvatarPlayerboard = new List<CharacterAvatarUIController>();

	public List<GameObject> boardPlayerList = new List<GameObject>();

	public List<TextMeshProUGUI> playerNameList = new List<TextMeshProUGUI>();

	public List<TextMeshProUGUI> Hp = new List<TextMeshProUGUI>();

	public List<Image> Weapon0 = new List<Image>();

	public List<Image> Weapon1 = new List<Image>();

	public List<InventoryUID> inventoryItem = new List<InventoryUID>();

	public GameObject ObjectWaiting;

	public static PlayerBoard Instance { get; private set; }

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

	private void Start()
	{
		if (NetworkGameManager.Instance.arrPlayerController.Count >= 2 && ObjectWaiting != null && ObjectWaiting.activeSelf)
		{
			ObjectWaiting.SetActive(value: false);
		}
	}

	public void ChangeAvatarPlayerBoard(int index, PlayerSkinData playerSkinData)
	{
		CharacterAvatarUIController characterAvatarUIController = playerAvatarPlayerboard[index];
		if ((bool)characterAvatarUIController)
		{
			characterAvatarUIController.ChangeHeadAvatarSprite(playerSkinData.GetHeadSkinAvatar());
			characterAvatarUIController.ChangeBodyAvatarSprite(playerSkinData.GetBodySkinAvatar());
		}
	}

	public void SetPlayerSkill(PlayerController playerController)
	{
		_uiTabPlayers[playerController.network.GetIDX()]?.SetSkillPerksUI(playerController);
	}
}
