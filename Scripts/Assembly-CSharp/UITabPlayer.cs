using Dissonance;
using I2.Loc;
using TMPro;
using Toked.Skill.UI;
using UnityEngine;
using UnityEngine.UI;
using _Modules.CharacterSkin.Scripts;
using _Modules.UIInGame.Scripts;

public class UITabPlayer : MonoBehaviour
{
	[SerializeField]
	private int _idx;

	[SerializeField]
	private Sprite _spriteSoundOn;

	[SerializeField]
	private Sprite _spriteSoundOff;

	[SerializeField]
	private Image _imgMuteMic;

	[SerializeField]
	private Toggle _checkBoxReady;

	[SerializeField]
	private GameObject _labelReady;

	[SerializeField]
	private GameObject _labelUnready;

	[SerializeField]
	private TextMeshProUGUI _labelDisconnected;

	[SerializeField]
	private UISkillStatusController _skillStatusController;

	[SerializeField]
	private UIPerkStatusController _perkStatusController;

	[SerializeField]
	private CharacterAvatarUIController _playerAvatarReady;

	private PlayerController _playerController;

	private bool _initPlayer;

	private bool _isInitialized;

	public void RefreshPlayerAvatar()
	{
		PlayerSkinData playerSkinData = GetPlayerTarget()?.data.PlayerSkinData;
		ChangePlayerAvatar(playerSkinData?.GetHeadSkinAvatar(), playerSkinData?.GetBodySkinAvatar());
	}

	public void ChangePlayerAvatar(Sprite headSprite, Sprite bodySprite)
	{
		if ((bool)headSprite)
		{
			_playerAvatarReady?.ChangeHeadAvatarSprite(headSprite);
		}
		if ((bool)bodySprite)
		{
			_playerAvatarReady?.ChangeBodyAvatarSprite(bodySprite);
		}
	}

	public void SetCheckBox(bool isOn)
	{
		if ((bool)_checkBoxReady)
		{
			_checkBoxReady.isOn = isOn;
		}
	}

	public void SetReadyUI()
	{
		_labelReady?.SetActive(value: true);
		_labelUnready?.SetActive(value: false);
		_labelDisconnected?.gameObject.SetActive(value: false);
	}

	public void SetUnreadyUI()
	{
		_labelReady?.SetActive(value: false);
		_labelUnready?.SetActive(value: true);
		_labelDisconnected?.gameObject.SetActive(value: false);
	}

	public void SetDisconnectedUI()
	{
		SetCheckBox(isOn: false);
		_labelReady?.SetActive(value: false);
		_labelUnready?.SetActive(value: false);
		_labelDisconnected?.gameObject.SetActive(value: true);
	}

	public void SetReconnectedUI()
	{
		SetCheckBox(isOn: false);
		_labelReady?.SetActive(value: false);
		_labelUnready?.SetActive(value: true);
		_labelDisconnected?.gameObject.SetActive(value: false);
	}

	public void SetDisconnectedUI(string timeCounter)
	{
		if ((bool)_labelDisconnected)
		{
			_labelDisconnected.text = LocalizationManager.GetTranslation("Menu/Disconnected").ToUpper() + " (" + timeCounter + ")";
		}
	}

	public PlayerController GetPlayerTarget()
	{
		return _playerController ?? (_playerController = NetworkGameManager.Instance.GetPlayer(_idx));
	}

	public void SetSkillPerksUI(PlayerController player)
	{
		_playerController = player;
		SetSkillUIInfo(player);
		SetPerksUIInfo(player);
		_initPlayer = true;
	}

	public void SetSkillUIInfo(PlayerController player = null)
	{
		_skillStatusController?.Init(player ?? GetPlayerTarget());
	}

	public void SetPerksUIInfo(PlayerController player = null)
	{
		_perkStatusController?.Init(player ?? GetPlayerTarget());
	}

	public void HideUI()
	{
		_perkStatusController?.HideUI();
		_skillStatusController?.HideUI();
	}

	private void FixedUpdate()
	{
		if (!NetworkGameManager.Instance || !VoiceChatGlobalController.Instance || !VoiceChatGlobalController.Instance || !VoiceChatGlobalController.Instance.VoiceComms.IsNetworkInitialized || _isInitialized || !NetworkGameManager.Instance)
		{
			return;
		}
		PlayerController playerTarget = GetPlayerTarget();
		if (!playerTarget)
		{
			return;
		}
		if (playerTarget.network.isLocalPlayer)
		{
			_imgMuteMic.gameObject.SetActive(value: false);
		}
		else
		{
			foreach (VoicePlayerState player in VoiceChatGlobalController.Instance.VoiceComms.Players)
			{
				if (player.Name.Contains(playerTarget.network.playerPhoton.voiceChatName) && player != null)
				{
					_imgMuteMic.sprite = _spriteSoundOn;
				}
			}
		}
		_isInitialized = true;
	}

	public void MuteClick()
	{
		PlayerController player = NetworkGameManager.Instance.GetPlayer(_idx);
		VoicePlayerState voicePlayerState = null;
		foreach (VoicePlayerState player2 in VoiceChatGlobalController.Instance.VoiceComms.Players)
		{
			if (player2.Name.Contains(player.network.playerPhoton.voiceChatName))
			{
				voicePlayerState = player2;
				if (voicePlayerState != null)
				{
					voicePlayerState.IsLocallyMuted = !voicePlayerState.IsLocallyMuted;
					SetUIMute(voicePlayerState);
				}
			}
		}
	}

	public void SetUIMute(VoicePlayerState playerVoiceState)
	{
		if (playerVoiceState.IsLocallyMuted)
		{
			_imgMuteMic.sprite = _spriteSoundOff;
		}
		else
		{
			_imgMuteMic.sprite = _spriteSoundOn;
		}
	}
}
