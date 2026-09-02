using System;
using TMPro;
using UnityEngine;
using _Modules.Player.BaseScripts;

namespace _Modules.UIInGame.Scripts;

public class LoadingInGameUIController : MonoBehaviour
{
	[Serializable]
	private class CharacterLoadingInGameData
	{
		[SerializeField]
		private CharacterRenderController _characterRenderController;

		[SerializeField]
		private TextMeshProUGUI _playerNameText;

		public TextMeshProUGUI PlayerNameText => _playerNameText;

		public void SetData(PlayerController playerController, string animationName1, string animationName2)
		{
			PlayAnimation(animationName1, animationName2);
			_characterRenderController.ChangeHeadSkin(playerController.characterRenderController.HeadLib.spriteLibraryAsset);
			_characterRenderController.ChangeWeaponSkin(playerController.characterRenderController.WeaponLib.spriteLibraryAsset);
			_characterRenderController.ChangeBodySkin(playerController.characterRenderController.BodyLib.spriteLibraryAsset);
			_characterRenderController.ChangeHandSkin(playerController.characterRenderController.Hand1Lib.spriteLibraryAsset);
			_characterRenderController.ChangeLegSkin(playerController.characterRenderController.Leg1Lib.spriteLibraryAsset);
			_characterRenderController.SetHeadMaterial(playerController.characterRenderController.GetHeadMaterial);
			_characterRenderController.SetUpperBodyMaterial(playerController.characterRenderController.GetUpperBodyMaterial);
			_characterRenderController.SetLowerBodyMaterial(playerController.characterRenderController.GetLowerBodyMaterial);
			SetPlayerName(playerController.network.GetPlayerName());
		}

		public void SetActive(bool setActive)
		{
			_characterRenderController.gameObject.SetActive(setActive);
			if ((bool)_playerNameText)
			{
				_playerNameText.gameObject.SetActive(setActive);
			}
		}

		private void SetPlayerName(string playerName)
		{
			if ((bool)_playerNameText)
			{
				_playerNameText.text = playerName;
			}
		}

		public void PlayAnimation(string animationName1, string animationName2)
		{
			float normalizedTime = UnityEngine.Random.Range(0f, 1f);
			if (!string.IsNullOrWhiteSpace(animationName1))
			{
				_characterRenderController.CharacterAnimator?.Play(animationName1, -1, normalizedTime);
			}
			if (!string.IsNullOrWhiteSpace(animationName2))
			{
				_characterRenderController.CharacterAnimator2?.Play(animationName2, -1, normalizedTime);
			}
		}

		public void SetAnimationSpeed(float animSpeed)
		{
			if (_characterRenderController.CharacterAnimator != null)
			{
				_characterRenderController.CharacterAnimator.speed = animSpeed;
			}
			if (_characterRenderController.CharacterAnimator != null)
			{
				_characterRenderController.CharacterAnimator2.speed = animSpeed;
			}
		}
	}

	[SerializeField]
	private string _animationUpperName = "IdleMelee180";

	[SerializeField]
	private string _animationLowerName = "";

	[SerializeField]
	private CharacterLoadingInGameData[] _characterRenderControllers;

	public void Init()
	{
		LoadingInGame loadingInGame = UIGameManager.Instance.loading;
		for (int i = 0; i < _characterRenderControllers.Length; i++)
		{
			_characterRenderControllers[i].SetActive(setActive: false);
			loadingInGame.playerNameList[i].text = "";
		}
		string startPos = GetStartPos();
		int num = 1;
		for (int j = 0; j < NetworkGameManager.Instance.arrPlayerController.Count; j++)
		{
			PlayerController playerController = NetworkGameManager.Instance.arrPlayerController[j];
			if (playerController.network.isLocalPlayer)
			{
				SetCharacterRender(playerController, 0);
				continue;
			}
			if (num >= 0 && num < startPos.Length)
			{
				int index = int.Parse(startPos.Substring(num, 1));
				SetCharacterRender(playerController, index);
			}
			num++;
		}
		void SetCharacterRender(PlayerController playerController2, int num2)
		{
			_characterRenderControllers[num2].SetActive(setActive: true);
			_characterRenderControllers[num2].SetData(playerController2, _animationUpperName, _animationLowerName);
			loadingInGame.playerNameList[num2].text = playerController2.network.GetPlayerName();
		}
	}

	public void TriggerAnimation(int life)
	{
		for (int i = 0; i < _characterRenderControllers.Length; i++)
		{
			_characterRenderControllers[i]?.PlayAnimation(_animationUpperName, _animationLowerName);
			if (life <= 0)
			{
				_characterRenderControllers[i]?.SetAnimationSpeed(0f);
			}
			else
			{
				_characterRenderControllers[i]?.SetAnimationSpeed(1f);
			}
		}
	}

	private string GetStartPos()
	{
		string result = "";
		if (NetworkGameManager.Instance.arrPlayerController.Count == 1)
		{
			result = "0";
		}
		else if (NetworkGameManager.Instance.arrPlayerController.Count == 2)
		{
			result = "03";
		}
		else if (NetworkGameManager.Instance.arrPlayerController.Count == 3)
		{
			result = "024";
		}
		else if (NetworkGameManager.Instance.arrPlayerController.Count == 4)
		{
			result = "0234";
		}
		else if (NetworkGameManager.Instance.arrPlayerController.Count == 5)
		{
			result = "01245";
		}
		else if (NetworkGameManager.Instance.arrPlayerController.Count == 6)
		{
			result = "012345";
		}
		return result;
	}
}
