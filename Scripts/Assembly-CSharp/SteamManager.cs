using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using Toked;
using UnityEngine;
using _Modules.Steam.Scripts;

[DisallowMultipleComponent]
public class SteamManager : MonoBehaviour
{
	public static SteamManager Instance;

	public static bool Initialized;

	public static Lobby ActiveLobby;

	public static bool JoinedLobby;

	public const uint APP_RELEASE_ID = 1953230u;

	public const uint APP_FRIENDPASS_ID = 2595010u;

	public SteamLeaderBoard SteamLeaderBoard;

	public bool runWithoutSteamClient;

	[SerializeField]
	private Texture2D _testAvatarTexture;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(this);
			return;
		}
		Instance = this;
		InitSteam();
	}

	public void InitSteam()
	{
		Initialized = SteamApi.Initialize();
		if (Initialized)
		{
			SteamLeaderBoard?.Init();
			GetProfilePictureAsync();
			Debug.Log("Steam Manager Initialized");
		}
	}

	private async Task GetProfilePictureAsync()
	{
		_testAvatarTexture = await SteamApi.GetAvatarTexture(SteamClient.SteamId);
	}

	public Texture2D GetSteamProfilePicture()
	{
		if (_testAvatarTexture == null)
		{
			GetProfilePictureAsync();
		}
		return _testAvatarTexture;
	}

	private void OnApplicationQuit()
	{
		SteamRichPresence.ClearRichPresence();
	}
}
