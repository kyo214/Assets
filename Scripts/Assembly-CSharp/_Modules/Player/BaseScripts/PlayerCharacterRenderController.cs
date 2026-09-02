using UnityEngine;
using UnityEngine.U2D.Animation;

namespace _Modules.Player.BaseScripts;

public class PlayerCharacterRenderController : CharacterRenderController
{
	[SerializeField]
	private PlayerController _playerController;

	[SerializeField]
	private Transform _playerParticleTransform;

	public Transform PlayerParticleTransform => _playerParticleTransform ?? (_playerParticleTransform = _playerController.transform);

	public override void ShowCharacter()
	{
		base.ShowCharacter();
		UIGameManager.Instance.ArrPlayerInfo[_playerController.network.GetIDX()].gameObject.SetActive(value: true);
		_playerController.playerCollider.SetActive(value: true);
	}

	public override void HideCharacter()
	{
		base.HideCharacter();
		UIGameManager.Instance.ArrPlayerInfo[_playerController.network.GetIDX()].gameObject.SetActive(value: false);
		_playerController.playerCollider.SetActive(value: false);
	}

	public override void ChangeHandSkin(SpriteLibraryAsset spriteLibraryAsset)
	{
		if ((bool)_hand1Lib)
		{
			_hand1Lib.spriteLibraryAsset = spriteLibraryAsset;
		}
		if ((bool)_hand2Lib)
		{
			_hand2Lib.spriteLibraryAsset = spriteLibraryAsset;
		}
		if ((bool)_hand2BLib)
		{
			_hand2BLib.spriteLibraryAsset = spriteLibraryAsset;
		}
	}

	public override void ChangeLegSkin(SpriteLibraryAsset spriteLibraryAsset)
	{
		if ((bool)_leg1Lib)
		{
			_leg1Lib.spriteLibraryAsset = spriteLibraryAsset;
		}
		if ((bool)_leg2Lib)
		{
			_leg2Lib.spriteLibraryAsset = spriteLibraryAsset;
		}
	}
}
