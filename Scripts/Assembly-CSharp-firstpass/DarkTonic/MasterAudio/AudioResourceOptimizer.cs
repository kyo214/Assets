using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarkTonic.MasterAudio;

public static class AudioResourceOptimizer
{
	private static readonly Dictionary<string, List<AudioSource>> AudioResourceTargetsByName = new Dictionary<string, List<AudioSource>>(StringComparer.OrdinalIgnoreCase);

	private static readonly Dictionary<string, AudioClip> AudioClipsByName = new Dictionary<string, AudioClip>(StringComparer.OrdinalIgnoreCase);

	private static readonly Dictionary<string, List<AudioClip>> PlaylistClipsByPlaylistName = new Dictionary<string, List<AudioClip>>(5, StringComparer.OrdinalIgnoreCase);

	private static string _supportedLanguageFolder = string.Empty;

	public static void ClearAudioClips()
	{
		AudioClipsByName.Clear();
		AudioResourceTargetsByName.Clear();
	}

	public static string GetLocalizedDynamicSoundGroupFileName(SystemLanguage localLanguage, bool useLocalization, string resourceFileName)
	{
		if (!useLocalization)
		{
			return resourceFileName;
		}
		if (MasterAudio.Instance != null)
		{
			return GetLocalizedFileName(useLocalization, resourceFileName);
		}
		return localLanguage.ToString() + "/" + resourceFileName;
	}

	public static string GetLocalizedFileName(bool useLocalization, string resourceFileName)
	{
		if (!useLocalization)
		{
			return resourceFileName;
		}
		return SupportedLanguageFolder() + "/" + resourceFileName;
	}

	public static void AddTargetForClip(string clipName, AudioSource source)
	{
		if (!AudioResourceTargetsByName.ContainsKey(clipName))
		{
			AudioResourceTargetsByName.Add(clipName, new List<AudioSource> { source });
			return;
		}
		List<AudioSource> list = AudioResourceTargetsByName[clipName];
		AudioClip audioClip = null;
		for (int i = 0; i < list.Count; i++)
		{
			AudioClip clip = list[i].clip;
			if (!(clip == null))
			{
				audioClip = clip;
				break;
			}
		}
		if (audioClip != null)
		{
			source.clip = audioClip;
		}
		list.Add(source);
	}

	private static string SupportedLanguageFolder()
	{
		if (!string.IsNullOrEmpty(_supportedLanguageFolder))
		{
			return _supportedLanguageFolder;
		}
		SystemLanguage item = Application.systemLanguage;
		if (MasterAudio.Instance != null)
		{
			switch (MasterAudio.Instance.langMode)
			{
			case MasterAudio.LanguageMode.SpecificLanguage:
				item = MasterAudio.Instance.testLanguage;
				break;
			case MasterAudio.LanguageMode.DynamicallySet:
				item = MasterAudio.DynamicLanguage;
				break;
			}
		}
		if (MasterAudio.Instance.supportedLanguages.Contains(item))
		{
			_supportedLanguageFolder = item.ToString();
		}
		else
		{
			_supportedLanguageFolder = MasterAudio.Instance.defaultLanguage.ToString();
		}
		return _supportedLanguageFolder;
	}

	public static void ClearSupportLanguageFolder()
	{
		_supportedLanguageFolder = string.Empty;
	}

	private static void FinishRecordingPlaylistClip(string controllerName, AudioClip resAudioClip)
	{
		List<AudioClip> list;
		if (!PlaylistClipsByPlaylistName.ContainsKey(controllerName))
		{
			list = new List<AudioClip>(5);
			PlaylistClipsByPlaylistName.Add(controllerName, list);
		}
		else
		{
			list = PlaylistClipsByPlaylistName[controllerName];
		}
		list.Add(resAudioClip);
	}

	public static IEnumerator PopulateResourceSongToPlaylistControllerAsync(MusicSetting songSetting, string songResourceName, string playlistName, PlaylistController controller, PlaylistController.AudioPlayType playType)
	{
		ResourceRequest asyncRes = Resources.LoadAsync(songResourceName, typeof(AudioClip));
		while (!asyncRes.isDone)
		{
			yield return MasterAudio.EndOfFrameDelay;
		}
		AudioClip audioClip = asyncRes.asset as AudioClip;
		if (audioClip == null)
		{
			MasterAudio.LogWarning("Resource file '" + songResourceName + "' could not be located from Playlist '" + playlistName + "'.");
			yield break;
		}
		if (!AudioUtil.AudioClipWillPreload(audioClip))
		{
			MasterAudio.LogWarning("Audio Clip for Resource file '" + songResourceName + "' from Playlist '" + playlistName + "' has 'Preload Audio Data' turned off, which can cause audio glitches. Resource files should always Preload Audio Data. Please turn it on.");
		}
		audioClip.name = songResourceName;
		FinishRecordingPlaylistClip(controller.ControllerName, audioClip);
		controller.FinishLoadingNewSong(songSetting, audioClip, playType);
	}

	public static IEnumerator PopulateSourcesWithResourceClipAsync(string clipName, SoundGroupVariation variation, Action successAction, Action failureAction)
	{
		bool isWarmingCall = MasterAudio.IsWarming;
		if (AudioClipsByName.ContainsKey(clipName))
		{
			successAction?.Invoke();
			if (isWarmingCall)
			{
				DTMonoHelper.SetActive(variation.GameObj, isActive: false);
			}
			yield break;
		}
		ResourceRequest asyncRes = Resources.LoadAsync(clipName, typeof(AudioClip));
		while (!asyncRes.isDone)
		{
			yield return MasterAudio.EndOfFrameDelay;
		}
		AudioClip audioClip = asyncRes.asset as AudioClip;
		if (audioClip == null)
		{
			MasterAudio.LogError("Resource file '" + clipName + "' could not be located.");
			failureAction?.Invoke();
			if (isWarmingCall)
			{
				DTMonoHelper.SetActive(variation.GameObj, isActive: false);
			}
			yield break;
		}
		if (!AudioResourceTargetsByName.ContainsKey(clipName))
		{
			MasterAudio.LogError("No Audio Sources found to add Resource file '" + clipName + "'.");
			failureAction?.Invoke();
			if (isWarmingCall)
			{
				DTMonoHelper.SetActive(variation.GameObj, isActive: false);
			}
			yield break;
		}
		if (!AudioUtil.AudioClipWillPreload(audioClip))
		{
			MasterAudio.LogWarning("Audio Clip for Resource file '" + clipName + "' of Sound Group '" + variation.ParentGroup.GameObjectName + "' has 'Preload Audio Data' turned off, which can cause audio glitches. Resource files should always Preload Audio Data. Please turn it on.");
		}
		List<AudioSource> list = AudioResourceTargetsByName[clipName];
		for (int i = 0; i < list.Count; i++)
		{
			list[i].clip = audioClip;
		}
		if (!AudioClipsByName.ContainsKey(clipName))
		{
			AudioClipsByName.Add(clipName, audioClip);
		}
		successAction?.Invoke();
		if (isWarmingCall)
		{
			DTMonoHelper.SetActive(variation.GameObj, isActive: false);
		}
	}

	public static void UnloadPlaylistSongIfUnused(string controllerName, AudioClip clipToRemove)
	{
		if (clipToRemove == null || !PlaylistClipsByPlaylistName.ContainsKey(controllerName))
		{
			return;
		}
		List<AudioClip> list = PlaylistClipsByPlaylistName[controllerName];
		if (list.Contains(clipToRemove))
		{
			list.Remove(clipToRemove);
			if (!list.Contains(clipToRemove))
			{
				Resources.UnloadAsset(clipToRemove);
			}
		}
	}

	public static void DeleteAudioSourceFromList(string clipName, AudioSource source)
	{
		if (!AudioResourceTargetsByName.ContainsKey(clipName))
		{
			MasterAudio.LogError("No Audio Sources found for Resource file '" + clipName + "'.");
			return;
		}
		List<AudioSource> list = AudioResourceTargetsByName[clipName];
		list.Remove(source);
		if (list.Count == 0)
		{
			AudioResourceTargetsByName.Remove(clipName);
		}
	}

	public static void UnloadClipIfUnused(string clipName)
	{
		if (!AudioClipsByName.ContainsKey(clipName))
		{
			return;
		}
		List<AudioSource> list = new List<AudioSource>();
		if (AudioResourceTargetsByName.ContainsKey(clipName))
		{
			list = AudioResourceTargetsByName[clipName];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].GetComponent<SoundGroupVariation>().IsPlaying)
				{
					return;
				}
			}
		}
		AudioClip assetToUnload = AudioClipsByName[clipName];
		for (int j = 0; j < list.Count; j++)
		{
			list[j].clip = null;
		}
		AudioClipsByName.Remove(clipName);
		Resources.UnloadAsset(assetToUnload);
	}
}
