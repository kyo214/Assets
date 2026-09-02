using System;
using Dissonance.Audio.Playback;
using Dissonance.Datastructures;
using JetBrains.Annotations;
using UnityEngine;

namespace Dissonance;

internal class PlaybackPool
{
	private readonly Pool<IVoicePlaybackInternal> _pool;

	[NotNull]
	private readonly IPriorityManager _priority;

	[NotNull]
	private readonly IVolumeProvider _volume;

	private GameObject _prefab;

	private Transform _parent;

	public PlaybackPool([NotNull] IPriorityManager priority, [NotNull] IVolumeProvider volume)
	{
		_priority = priority ?? throw new ArgumentNullException("priority");
		_volume = volume ?? throw new ArgumentNullException("volume");
		_pool = new Pool<IVoicePlaybackInternal>(10, CreatePlayback);
	}

	public void Start([NotNull] GameObject playbackPrefab, [NotNull] Transform transform)
	{
		_prefab = playbackPrefab ?? throw new ArgumentNullException("playbackPrefab");
		_parent = transform ?? throw new ArgumentNullException("transform");
	}

	[NotNull]
	private IVoicePlaybackInternal CreatePlayback()
	{
		_prefab.gameObject.SetActive(value: false);
		GameObject gameObject = UnityEngine.Object.Instantiate(_prefab.gameObject);
		gameObject.transform.parent = _parent;
		IVoicePlaybackInternal component = gameObject.GetComponent<IVoicePlaybackInternal>();
		component.Setup(_priority, _volume);
		return component;
	}

	[NotNull]
	public IVoicePlaybackInternal Get([NotNull] string playerId)
	{
		if (playerId == null)
		{
			throw new ArgumentNullException("playerId");
		}
		IVoicePlaybackInternal voicePlaybackInternal = _pool.Get();
		((MonoBehaviour)voicePlaybackInternal).gameObject.name = "Player " + playerId + " voice comms";
		voicePlaybackInternal.PlayerName = playerId;
		return voicePlaybackInternal;
	}

	public void Put([NotNull] IVoicePlayback playback)
	{
		if (playback == null)
		{
			throw new ArgumentNullException("playback");
		}
		GameObject gameObject = ((MonoBehaviour)playback).gameObject;
		gameObject.SetActive(value: false);
		gameObject.name = "Spare voice comms";
		IVoicePlaybackInternal voicePlaybackInternal = (IVoicePlaybackInternal)playback;
		voicePlaybackInternal.PlayerName = null;
		if (!_pool.Put(voicePlaybackInternal))
		{
			UnityEngine.Object.Destroy(gameObject);
		}
	}
}
