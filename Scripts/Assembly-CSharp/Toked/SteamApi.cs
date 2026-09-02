using System;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using TMPro;
using UnityEngine;
using _Modules.Generator;

namespace Toked;

public static class SteamApi
{
	private static TMP_InputField _inputField;

	public static bool Initialize()
	{
		try
		{
			SteamClient.Init(2595010u);
		}
		catch (Exception message)
		{
			Debug.Log(message);
			if (!SteamManager.Instance.runWithoutSteamClient)
			{
				Debug.Log("Steam Manager is not running, Exit Playmode");
				Application.Quit();
			}
			return false;
		}
		return true;
	}

	public static string GetSteamId(bool returnDefaultValue = true)
	{
		if (!SteamManager.Initialized)
		{
			if (!returnDefaultValue)
			{
				return "";
			}
			return RandomUniqueIdGenerator.GenerateID();
		}
		return SteamClient.SteamId.ToString();
	}

	public static string GetAccountId(bool returnDefaultValue = true)
	{
		if (!SteamManager.Initialized)
		{
			if (!returnDefaultValue)
			{
				return "";
			}
			return RandomUniqueIdGenerator.GenerateID();
		}
		return SteamClient.SteamId.AccountId.ToString();
	}

	public static string GetLocalUsername()
	{
		string result = "";
		if (SteamManager.Initialized)
		{
			result = SteamClient.Name;
		}
		return result;
	}

	private static async Task<Image?> GetAvatar(SteamId steamId)
	{
		try
		{
			return await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId);
		}
		catch (Exception message)
		{
			Debug.Log(message);
			return null;
		}
	}

	private static Texture2D Covert(this Image image)
	{
		Texture2D texture2D = new Texture2D((int)image.Width, (int)image.Height, TextureFormat.ARGB32, mipChain: false)
		{
			filterMode = FilterMode.Trilinear
		};
		for (int i = 0; i < image.Width; i++)
		{
			for (int j = 0; j < image.Height; j++)
			{
				Steamworks.Data.Color pixel = image.GetPixel(i, j);
				texture2D.SetPixel(i, (int)image.Height - j, new UnityEngine.Color((float)(int)pixel.r / 255f, (float)(int)pixel.g / 255f, (float)(int)pixel.b / 255f, (float)(int)pixel.a / 255f));
			}
		}
		texture2D.Apply();
		return texture2D;
	}

	public static async Task<Texture2D> GetAvatarTexture(SteamId steamId)
	{
		if (!SteamManager.Initialized)
		{
			return null;
		}
		Task<Image?> avatar = GetAvatar(steamId);
		await Task.WhenAll<Image?>(avatar);
		Image? result = avatar.Result;
		return result.HasValue ? result.GetValueOrDefault().Covert() : null;
	}

	public static void OpenFloatingKeyboard(TMP_InputField inputField, int maxChars = 100)
	{
		if (SteamManager.Initialized)
		{
			_inputField = inputField;
			SteamUtils.ShowGamepadTextInput(GamepadTextInputMode.Normal, GamepadTextInputLineMode.SingleLine, "", maxChars);
			SteamUtils.OnGamepadTextInputDismissed += SteamUtils_OnGamepadTextInputDismissed;
			Debug.Log("<color=#acd550>[SteamApi]</color> Open Floating Keyboard");
		}
	}

	private static void SteamUtils_OnGamepadTextInputDismissed(bool obj)
	{
		string enteredGamepadText = SteamUtils.GetEnteredGamepadText();
		_inputField.text = enteredGamepadText;
		SteamUtils.OnGamepadTextInputDismissed -= SteamUtils_OnGamepadTextInputDismissed;
	}

	public static void OpenWebOverlay(string url)
	{
		if (SteamManager.Initialized)
		{
			SteamFriends.OpenWebOverlay(url);
		}
	}
}
