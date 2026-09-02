using Cysharp.Threading.Tasks;
using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.Audio;

namespace Toked;

internal class AudioManager : MonoBehaviour
{
	public string prevPlaylistName;

	public PlaylistController playlistBGM;

	public PlaylistController playlistWhisper;

	public PlaylistController playlistAmbience;

	[SerializeField]
	private AudioMixer audioMixer;

	public bool BGMFixed;

	public static AudioManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public static void ChangeVolumeMaster(float value)
	{
		if (value <= 0f)
		{
			value = 0.0001f;
		}
		value = Mathf.Log10(value) * 20f;
		Instance.audioMixer.SetFloat("VolMaster", value);
	}

	public static string GetPlaylistName()
	{
		return Instance.playlistBGM.PlaylistName;
	}

	public static bool isPlayingBGM(string playlistName)
	{
		return Instance.playlistBGM.IsSongPlaying(playlistName);
	}

	public static void PlayBGM(string playlistName, string filename, float fadingTime = 0f, bool isLooping = true, bool savePlaylistName = false)
	{
		Debug.Log(filename);
		if (!Instance.BGMFixed)
		{
			if (savePlaylistName && (bool)Instance.playlistBGM.CurrentPlaylistClip)
			{
				Instance.prevPlaylistName = Instance.playlistBGM.CurrentPlaylistClip.name;
			}
			Instance.playlistBGM.StartPlaylist(playlistName, filename);
			Instance.playlistBGM.FadeToVolume((float)GlobalSaveData.instance.optionData.volMusic / 100f, fadingTime);
			Instance.playlistBGM.loopPlaylist = isLooping;
		}
	}

	public static void SetBGMFixed(bool value)
	{
		Instance.BGMFixed = value;
	}

	public static void BGMSetLoop(bool isLooping)
	{
		Instance.playlistBGM.loopPlaylist = isLooping;
	}

	public static void ChangeLowPass(float value)
	{
		Instance.audioMixer.SetFloat("LowPass", value);
	}

	public static void StopBGM(float fadeTime = -1f, bool isPlayPrevBGM = false)
	{
		if (Instance.BGMFixed)
		{
			return;
		}
		if (fadeTime == -1f)
		{
			Instance.playlistBGM.StopPlaylist();
		}
		else
		{
			Instance.playlistBGM.FadeToVolume(0f, fadeTime);
		}
		if (fadeTime == -1f)
		{
			Instance.playlistBGM.StartPlaylist("BGM");
		}
		else if (isPlayPrevBGM && Instance.prevPlaylistName != "")
		{
			UniTaskUtil.DelayedCall(Instance, fadeTime, () =>
			{
				Instance.playlistBGM.StartPlaylist("BGM", Instance.prevPlaylistName);
			}).Forget();
		}
		else
		{
			UniTaskUtil.DelayedCall(Instance, fadeTime, () =>
			{
				Instance.playlistBGM.StartPlaylist("BGM");
			}).Forget();
		}
	}

	public static void ChangeVolumeBGM(float value)
	{
		if (value <= 0f)
		{
			value = 0.0001f;
		}
		value = Mathf.Log10(value) * 20f;
		Instance.audioMixer.SetFloat("VolMusic", value);
		Instance.playlistBGM.PlaylistVolume = (float)GlobalSaveData.instance.optionData.volMusic / 100f;
	}

	public static void InitPlaylistWhisper()
	{
		Instance.playlistWhisper.ChangePlaylist("Whisper");
	}

	public static void PlayWhisper(string filename)
	{
		Instance.playlistWhisper.StartPlaylist("Whisper", filename);
	}

	public static void ChangeVolumeWhisper(float vol, float fadeTime = 0.5f)
	{
		Instance.playlistWhisper.FadeToVolume(vol, fadeTime);
	}

	public static void PlayAmbient(string ambientName, float fadeTime = 0f)
	{
		Instance.playlistAmbience.StartPlaylist("Ambience", ambientName);
		Instance.playlistAmbience.FadeToVolume(1f, fadeTime);
	}

	public static void PauseAmbient()
	{
		Instance.playlistAmbience.PausePlaylist();
	}

	public static void ResumeAmbient()
	{
		Instance.playlistAmbience.UnpausePlaylist();
	}

	public static void ChangeVolumeAmbient(float value)
	{
		if (value <= 0f)
		{
			value = 0.0001f;
		}
		value = Mathf.Log10(value) * 20f;
		Instance.audioMixer.SetFloat("VolAmbient", value);
		Instance.playlistAmbience.PlaylistVolume = (float)GlobalSaveData.instance.optionData.volAmbient / 100f;
	}

	public static void DisableAmbient()
	{
		Instance.playlistAmbience.FadeToVolume(0f, 1f);
	}

	public static void EnableAmbient()
	{
		Instance.playlistAmbience.FadeToVolume((float)GlobalSaveData.instance.optionData.volAmbient / 100f, 1f);
	}

	public static void PlaySFX(string filename)
	{
		MasterAudio.PlaySound(filename);
	}

	public static void PlaySFXVol(string filename, float newVolume = 1f)
	{
		MasterAudio.PlaySound(filename, newVolume);
	}

	public static void PlaySFX(string filename, string varName)
	{
		MasterAudio.PlaySound(filename, 1f, null, 0f, varName);
	}

	public static void StopSFX(string filename)
	{
		MasterAudio.StopAllOfSound(filename);
	}

	public static void PlaySFXTransform(string filename, Transform transform, bool isLocalPlayerTrigger, float vol = 1f, float delay = 0f)
	{
		MasterAudio.PlaySound3DFollowTransform(filename, transform, vol, null, delay);
	}

	public static void StopSFXTransform(Transform transform)
	{
		MasterAudio.StopAllSoundsOfTransform(transform);
	}

	public static void ChangeVolumeSFX(float value)
	{
		if (value <= 0f)
		{
			value = 0.0001f;
		}
		value = Mathf.Log10(value) * 20f;
		Instance.audioMixer.SetFloat("VolSFX", value);
		Instance.audioMixer.SetFloat("VolUI", value);
	}

	public static void ChangeVolumeVoice(float value)
	{
		if (value <= 0f)
		{
			value = 0.0001f;
		}
		value = Mathf.Log10(value) * 20f;
		Instance.audioMixer.SetFloat("VolVoice", value);
	}
}
