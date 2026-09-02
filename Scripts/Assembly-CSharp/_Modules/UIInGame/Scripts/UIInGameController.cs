using System.Collections;
using Toked.Crafting.CraftingUI;
using Toked.Skill.UI;
using UnityEngine;
using _Modules.CharacterSkin.Scripts;

namespace _Modules.UIInGame.Scripts;

public class UIInGameController : MonoBehaviour
{
	[SerializeField]
	private GameObject _playerStatus;

	[SerializeField]
	private GameObject _inventoryStatus;

	[SerializeField]
	private GameObject _readyStatus;

	public GameObject MissionLobby;

	public CraftingMaterialsUIController craftingMaterialsUIController;

	[SerializeField]
	private CharacterAvatarUIController _characterAvatarUIController;

	[SerializeField]
	private UISkillStatusController _skillStatusController;

	[SerializeField]
	private UIPerkStatusController _perkStatusController;

	private bool _initialized;

	public CharacterAvatarUIController CharacterAvatarUIController => _characterAvatarUIController;

	private void Start()
	{
		StartCoroutine(InitEventCoroutine());
	}

	private IEnumerator InitEventCoroutine()
	{
		yield return new WaitUntil(() => (bool)NetworkGameManager.Instance && (bool)NetworkGameManager.Instance.ownPlayer);
		InitEvent();
	}

	private void OnDestroy()
	{
		RemoveEvent();
	}

	private void InitEvent()
	{
		SetSkillStatusUI();
		SetPerkStatusUI();
		craftingMaterialsUIController?.Init();
	}

	public void RemoveEvent()
	{
	}

	public void SetPlayerStatusUI(bool setActive)
	{
		_playerStatus?.SetActive(setActive);
	}

	public void SetInventoryStatusUI(bool setActive)
	{
		_inventoryStatus?.SetActive(setActive);
	}

	public void SetReadyStatusUI(bool setActive)
	{
		_readyStatus?.SetActive(setActive);
	}

	public void ChangeCharacterAvatarUI(PlayerSkinData playerSkinData)
	{
		_characterAvatarUIController.ChangeHeadAvatarSprite(playerSkinData.GetHeadSkinAvatar());
		_characterAvatarUIController.ChangeBodyAvatarSprite(playerSkinData.GetBodySkinAvatar());
	}

	public void SetCraftingMaterialsUI(bool show)
	{
		if ((bool)craftingMaterialsUIController)
		{
			craftingMaterialsUIController.RefreshUI();
			craftingMaterialsUIController.gameObject.SetActive(show);
		}
	}

	public void SetSkillStatusUI()
	{
		PlayerController playerController = NetworkGameManager.Instance?.ownPlayer;
		if ((bool)playerController)
		{
			_skillStatusController?.Init(playerController);
		}
	}

	public void SetPerkStatusUI()
	{
		_perkStatusController?.Init(NetworkGameManager.Instance?.ownPlayer);
	}

	private void OnChangeSkillLearn(string id)
	{
		SetSkillStatusUI();
	}

	private void OnPerkChangedAction(string perkId)
	{
		SetPerkStatusUI();
	}
}
