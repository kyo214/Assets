using System;
using Steamworks;
using Steamworks.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Modules.Steam.Scripts;

public class SteamFriendContentUI : MonoBehaviour
{
	[SerializeField]
	private UnityEngine.UI.Image _friendImage;

	[SerializeField]
	private TextMeshProUGUI _friendNameText;

	[SerializeField]
	private Button _inviteButton;

	[SerializeField]
	private TextMeshProUGUI _inGameStatusText;

	[SerializeField]
	private TextMeshProUGUI _onlineStatusText;

	[SerializeField]
	private TextMeshProUGUI _inviteText;

	private Friend _friend;

	private Texture2D _avatarTexture;

	private Sprite _avatarSprite;

	public void Init(Friend friend, Action inviteAction)
	{
		_friend = friend;
		_friendNameText.text = _friend.Name;
		_inviteButton.onClick.RemoveAllListeners();
		_inviteButton.onClick.AddListener(() =>
		{
			inviteAction();
		});
		SetStatusText(_friend.IsPlayingThisGame);
		SetupProfileImage();
		if ((bool)NetworkGameManager.Instance && NetworkGameManager.Instance.arrPlayerController.Count + NetworkGameManager.Instance.arrPlayerDisconnected.Count < 4)
		{
			_inviteButton.interactable = true;
			_inviteButton.image.color = new UnityEngine.Color(1f, 1f, 1f, 1f);
			_inviteText.color = new UnityEngine.Color(1f, 1f, 1f, 1f);
		}
		else
		{
			_inviteButton.image.color = new UnityEngine.Color(1f, 1f, 1f, 0.3f);
			_inviteButton.interactable = false;
			_inviteText.color = new UnityEngine.Color(1f, 1f, 1f, 0.3f);
		}
	}

	private void SetStatusText(bool inGame)
	{
		if (inGame)
		{
			_inGameStatusText.gameObject.SetActive(value: true);
			_onlineStatusText.gameObject.SetActive(value: false);
		}
		else
		{
			_inGameStatusText.gameObject.SetActive(value: false);
			_onlineStatusText.gameObject.SetActive(value: true);
		}
	}

	private async void SetupProfileImage()
	{
		Steamworks.Data.Image? image = await SteamFriends.GetSmallAvatarAsync(_friend.Id);
		if (image.HasValue)
		{
			_avatarTexture = new Texture2D((int)image.Value.Width, (int)image.Value.Height);
			_avatarTexture.filterMode = FilterMode.Trilinear;
			for (int i = 0; i < image.Value.Width; i++)
			{
				for (int j = 0; j < image.Value.Height; j++)
				{
					Steamworks.Data.Color pixel = image.Value.GetPixel(i, j);
					_avatarTexture.SetPixel(i, (int)image.Value.Height - j, new UnityEngine.Color((float)(int)pixel.r / 255f, (float)(int)pixel.g / 255f, (float)(int)pixel.b / 255f, (float)(int)pixel.a / 255f));
				}
			}
			_avatarTexture.Apply();
			_avatarSprite = Sprite.Create(_avatarTexture, new Rect(0f, 0f, _avatarTexture.width, _avatarTexture.height), new Vector2(0.5f, 0.5f), 100f);
			_friendImage.sprite = _avatarSprite;
			_friendImage.gameObject.SetActive(value: true);
		}
		else
		{
			_friendImage.gameObject.SetActive(value: false);
		}
	}

	private void OnDestroy()
	{
	}
}
