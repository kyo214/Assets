using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkTonic.MasterAudio;

public static class AudioLoaderOptimizer
{
	private static readonly Dictionary<string, List<GameObject>> PlayingGameObjectsByClipName = new Dictionary<string, List<GameObject>>(StringComparer.OrdinalIgnoreCase);

	public static void AddNonPreloadedPlayingClip(AudioClip clip, GameObject maHolderGameObject)
	{
		if (clip == null)
		{
			return;
		}
		string key = clip.CachedName();
		if (!PlayingGameObjectsByClipName.ContainsKey(key))
		{
			PlayingGameObjectsByClipName.Add(key, new List<GameObject> { maHolderGameObject });
			return;
		}
		List<GameObject> list = PlayingGameObjectsByClipName[key];
		if (!list.Contains(maHolderGameObject))
		{
			list.Add(maHolderGameObject);
		}
	}

	public static void RemoveNonPreloadedPlayingClip(AudioClip clip, GameObject maHolderGameObject)
	{
		if (clip == null)
		{
			return;
		}
		string key = clip.CachedName();
		if (PlayingGameObjectsByClipName.ContainsKey(key))
		{
			List<GameObject> list = PlayingGameObjectsByClipName[key];
			list.Remove(maHolderGameObject);
			if (list.Count == 0)
			{
				PlayingGameObjectsByClipName.Remove(key);
			}
		}
	}

	public static bool IsAnyOfNonPreloadedClipPlaying(AudioClip clip)
	{
		if (clip == null)
		{
			return false;
		}
		return PlayingGameObjectsByClipName.ContainsKey(clip.CachedName());
	}
}
